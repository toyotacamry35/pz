using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using L10n.Loaders;
using L10n.Plural;
using NLog;

namespace L10n
{
    public class TextCatalog : ICatalog
    {
        private static readonly Logger Logger = LogManager.GetLogger("UI");

        private TextCatalog _fallbackTextCatalog;
        private Regex _keyReplaceRegex;

        /// <summary>
        /// Флаг наличия изменений, для удобства обработки при импорте переводов
        /// </summary>
        [NonSerialized]
        public bool IsChanged;


        //=== Props ===========================================================

        public bool HasDevTextCatalog => _fallbackTextCatalog != null;

        public CultureInfo CultureInfo { get; protected set; }

        public CultureData CultureData { get; protected set; }

        public Dictionary<string, TranslationDataExt> Translations { get; protected set; } = new Dictionary<string, TranslationDataExt>();

        private int _numPlurals;

        public int NumPlurals
        {
            get
            {
                if (_numPlurals == 0)
                    _numPlurals = PluralRule?.NumPlurals ?? -1;
                return _numPlurals;
            }
        }

        private IPluralRule _pluralRule;

        public IPluralRule PluralRule
        {
            get => _pluralRule;
            set
            {
                if (value == null)
                    Logger.IfError()?.Message($"{nameof(PluralRule)} is null -- {this}").Write();
                _pluralRule = value;
            }
        }


        //=== Ctors ===========================================================

        public TextCatalog(CultureData cultureData, ILoader loader)
        {
            if (cultureData.Code.AssertIfNull(nameof(cultureData.Code)) ||
                loader.AssertIfNull(nameof(loader)))
                return;

            CultureData = cultureData;
            CultureInfo = new CultureInfo(CultureData.Code);
            _keyReplaceRegex = new Regex(LocalizedString.KeyReferencesRegexString, RegexOptions.Compiled);

            try
            {
                Load(loader);
            }
            catch (FileNotFoundException exception)
            {
                Logger.IfError()?.Message($"Translation file loading fail: {exception.Message}\n{exception.StackTrace}").Write();
            }

            Logger.IfDebug()?.Message($"TextCatalog created: {this}").Write();
        }

        public void SetFallbackTextCatalog(TextCatalog fallbackTextCatalog)
        {
            _fallbackTextCatalog = fallbackTextCatalog;
        }


        //=== Public ==========================================================

        public void Load(ILoader loader)
        {
            if (loader.AssertIfNull(nameof(loader)))
                return;

            loader.Load(this);
        }

        /// <summary>
        /// Запрос из UI 1: Передаем ключ и все возможные аргументы. Все что нужно, будет учтено
        /// </summary>
        public string GetStringAuto(string key, long pluralNum, object[] args)
        {
            var translationData = GetTranslationData(key);
            if (translationData == null) //у себя не нашли
            {
                if (HasDevTextCatalog) //передаем задачу, если есть кому
                    return _fallbackTextCatalog.GetStringAuto(key, pluralNum, args);

                Logger.IfWarn()?.Message($"{nameof(GetStringAuto)}() {nameof(TranslationData)}  not found for key='{key}'").Write();
                return key; //иначе просто возвращаем key
            }

            if (translationData.IsPlural)
                return GetPluralStringOrDefault(translationData, key, pluralNum, args);

            return GetSimpleStringOrDefault(translationData, key, args);
        }

        /// <summary>
        /// Запрос из UI 2: Переданы частные данные, используем словарь только как источник PluralRule,  в остальном руководствуемся присланными данными
        /// </summary>
        public string GetStringFromCustomData(TranslationData translationData, long pluralNum, object[] args)
        {
            if (translationData.AssertIfNull(nameof(translationData)) ||
                string.IsNullOrEmpty(translationData.Text))
            {
                Logger.IfWarn()?.Message($"{nameof(GetStringFromCustomData)}() {nameof(translationData)} is empty").Write();
                return null;
            }

            if (HasDevTextCatalog) //Определенно следует обрабатывать в контенксте dev-словаря
                return _fallbackTextCatalog.GetStringFromCustomData(translationData, pluralNum, args);

            if (translationData.IsPlural)
                GetPluralStringOrDefault(translationData, "NoKey", pluralNum, args);

            return GetSimpleStringOrDefault(translationData, translationData.Text, args != null ? args : null);
        }

        public override string ToString()
        {
            return $"({nameof(TextCatalog)} '{CultureData.Folder}': {CultureInfo.Name}, {Translations.Count} translations" +
                   $"{(_pluralRule == null ? ", no pluralRule" : "")})";
        }


        //=== Private =========================================================

        private string GetSimpleStringOrDefault(TranslationData translationData, string key, params object[] args)
        {
            var text = key;
            if (translationData?.Text != null)
            {
                text = translationData.Text;
                if (HasKeyReference(text))
                    text = ReplaceKeyReference(text);
            }

            return args != null ? string.Format(CultureInfo, text, args) : text;
        }

        /// <summary>
        /// Есть ли в text ссылки на ключи
        /// </summary>
        private bool HasKeyReference(string text)
        {
            return text.LastIndexOf(LocalizedString.KeyBegin, StringComparison.Ordinal) > 0;
        }

        /// <summary>
        /// Возвращает строку с подстановкой в text вместо ссылок на ключи '#...' их значений из словаря.
        /// Поддерживаются множественные вхождения ключей
        /// </summary>
        private string ReplaceKeyReference(string text, long pluralNum = 0)
        {
            return _keyReplaceRegex.Replace(text, match => GetStringAuto(match.ToString(), pluralNum, null));
        }

        private string GetPluralStringOrDefault(TranslationData translationData, string keyText, long pluralNum, params object[] args)
        {
            var pluralIndex = GetPluralIndex(pluralNum);
            string pluralString;
            string[] forms = translationData.GetForms_SelfOnly();
            if (forms == null || forms.Length <= pluralIndex)
            {
                Logger.IfWarn()?.Message($"{nameof(translationData.GetForms_SelfOnly)}({pluralIndex}) not found for message id='{keyText}'").Write();
                pluralString = keyText; //по сути сообщаем имя кривого ключа
            }
            else
            {
                pluralString = forms[pluralIndex];
                if (HasKeyReference(pluralString))
                    pluralString = ReplaceKeyReference(pluralString);
            }

            return string.Format(CultureInfo, pluralString, GetArgsList(pluralNum, args));
        }

        /// <summary>
        /// Возвращает TranslationData из своего словаря
        /// </summary>
        private TranslationData GetTranslationData(string keyText)
        {
            if (string.IsNullOrEmpty(keyText))
                return null;

            Translations.TryGetValue(keyText, out var translationData);
            return translationData;
        }

        private object[] GetArgsList(long num, params object[] args)
        {
            var allArgs = new List<object>() {num};
            if (args != null)
                for (int i = 0; i < args.Length; i++)
                    allArgs.Add(args[i]);

            return allArgs.ToArray();
        }

        private int GetPluralIndex(long pluralNum)
        {
            var pluralIndex = PluralRule.Evaluate(pluralNum);
            if (pluralIndex < 0 || pluralIndex >= PluralRule.NumPlurals)
            {
                Logger.IfError()?.Message($"Calculated plural form index ({pluralIndex}) is out of range (0~{PluralRule.NumPlurals - 1}) -- {this}").Write();
                pluralIndex = 0;
            }

            return pluralIndex;
        }
    }
}