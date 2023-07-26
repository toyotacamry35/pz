using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Environment.Logging.Extension;
using NLog;

namespace L10n
{
    public class TranslationData
    {
        public const string PluralDataOpeningBracket = "[[";
        public const string PluralDataClosingBracket = "]]";
        public const char PluralDataDelimiter = '|';


        private static Regex _keyReferencesRegex;
        private static Regex _argReferencesRegex;

        public string Text { get; set; }

        /// <summary>
        /// Казалось бы, избыточное свойство. Однако вычисляемость при использовании подстановок становится нетривиальной (плюс аллокации, лишние ресурсы).
        /// Поэтому: 1) при первичном заведении LocalizedString выставляем ручками
        ///          2) перевычисляем на этапе добавки в dev-словарь во время извлечения ключей
        /// и в рантайме пользуемся
        /// </summary>
        public bool IsPlural { get; set; }

        /// <summary>
        /// Защита от удаления, даже если не используется
        /// </summary>
        public bool IsProtected { get; set; }

        public string Comments { get; set; }

        private static readonly Logger Logger = LogManager.GetLogger("UI");


        //=== Ctor ============================================================

        public TranslationData()
        {
            if (_keyReferencesRegex == null)
                _keyReferencesRegex = new Regex(LocalizedString.KeyReferencesRegexString, RegexOptions.Compiled);

            if (_argReferencesRegex == null)
                _argReferencesRegex = new Regex(LocalizedString.ArgReferencesRegexString, RegexOptions.Compiled);
        }


        //=== Public ==========================================================

        public string[] GetKeyReferencesFromText()
        {
            if (Text == null)
            {
                Logger.IfError()?.Message($"{nameof(GetKeyReferencesFromText)}: {nameof(Text)} is null -- {this}").Write();
                return new string[0];
            }

            return _keyReferencesRegex.Matches(Text)
                .Cast<Match>()
                .Select(match => match.Value.Replace(LocalizedString.KeyBegin + LocalizedString.KeyBegin, LocalizedString.KeyBegin))
                .ToArray();
        }

        /// <summary>
        /// Возвращает число ссылок на аргументы вида {0}.
        /// Из плюрального блока [[...]] возвращается число аргументов в одной форме (в каждой форме должно быть одинаковое число аргументов).
        /// При обнаружении каких либо несоответствий возвращает -1
        /// </summary>
        /// <returns></returns>
        public int GetArgReferencesCountFromText()
        {
            if (Text == null)
            {
                Logger.IfError()?.Message($"{nameof(GetArgReferencesCountFromText)}: {nameof(Text)} is null -- {this}").Write();
                return -1;
            }

            var argsCount = -1;
            var forms = GetForms_SelfOnly();
            for (int i = 0; i < forms.Length; i++)
            {
                var count = _argReferencesRegex.Matches(forms[i]).Cast<Match>().Count();
                if (argsCount < 0)
                {
                    argsCount = count;
                }
                else
                {
                    if (argsCount != count)
                    {
                        Logger.IfError()?.Message($"{nameof(GetArgReferencesCountFromText)}: Argument counts in forms doesn't match -- {this}").Write();
                        return -1;
                    }
                }
            }

            return argsCount;
        }

        /// <summary>
        /// Возвращает массив форм (plural) текста, если они есть. Например, Text 'Начало [[минута|минуты|минут]] окончание' вернет:
        /// массив из 3 строк: 'Начало минута окончание', 'Начало минуты окончание', 'Начало минут окончание'.
        /// На IsPlural не смотрит. Ссылки на ключи в тексте не смотрит (т.е. если в тексте ссылка на плюральный ключ, то вернет все равно только свои формы)
        /// </summary>
        public string[] GetForms_SelfOnly()
        {
            //специально не регэкспами
            var openingBracketIndex = Text.IndexOf(PluralDataOpeningBracket, StringComparison.OrdinalIgnoreCase);
            if (openingBracketIndex < 0)
                return new[] {Text};

            var closingBracketIndex = Text.LastIndexOf(PluralDataClosingBracket, StringComparison.OrdinalIgnoreCase);
            //Условие: в Text д.б. PluralDataOpeningBracket и идущий после него PluralDataClosingBracket
            if (closingBracketIndex < 0 || openingBracketIndex > closingBracketIndex)
                return new[] {Text};

            var beginPart = Text.Substring(0, openingBracketIndex);
            var endingPart = Text.Substring(closingBracketIndex + PluralDataClosingBracket.Length);
            var formsPart = Text.Substring(
                openingBracketIndex + PluralDataOpeningBracket.Length,
                Text.Length - (PluralDataOpeningBracket.Length + PluralDataClosingBracket.Length + beginPart.Length + endingPart.Length));
            return formsPart.Split(PluralDataDelimiter).Select(form => beginPart + form + endingPart).ToArray();
        }

        public TranslationDataExt ToTranslationDataExt(bool isPlural)
        {
            return new TranslationDataExt()
            {
                Text = Text,
                IsPlural = isPlural,
                Comments = Comments,
                Version = 1,
                TextHashcode = GetTextHashCode(),
            };
        }

        private readonly int TextMaxLength = 30;
        private readonly string TrimmingEnd = "~'";

        public override string ToString()
        {
            var pluralInfo = IsPlural ? "plural, " : "";
            var protectedInfo = IsProtected ? "protected, " : "";
            var textInfo = $"'{Text}'";
            if (textInfo.Length > TextMaxLength + TrimmingEnd.Length)
                textInfo = textInfo.Substring(0, TextMaxLength) + TrimmingEnd;
            return $"(TD: {pluralInfo}{protectedInfo}{textInfo})";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TranslationData))
                return false;

            return Equals((TranslationData) obj);
        }

        protected bool Equals(TranslationData other)
        {
            if (other == null)
                return false;

            return Comments == other.Comments &&
                   Text == other.Text;
        }

        public TranslationData Clone()
        {
            return (TranslationData) MemberwiseClone();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = GetTextHashCode();
                hashCode = (hashCode * 397) ^ (Comments != null ? StringComparer.InvariantCulture.GetHashCode(Comments) : 0);
                return hashCode;
            }
        }

        public int GetTextHashCode()
        {
            unchecked
            {
                var hashCode = -1;
                if (!string.IsNullOrEmpty(Text))
                    hashCode = (hashCode * 397) ^ StringComparer.InvariantCulture.GetHashCode(Text);
                return hashCode;
            }
        }
    }
}