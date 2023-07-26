using System;
using Core.Environment.Logging.Extension;
using NLog;

namespace L10n
{
    public struct LocalizedString : IEquatable<LocalizedString>
    {
        private static readonly Logger Logger = LogManager.GetLogger("Default");

        public static readonly LocalizedString Empty = new LocalizedString();
        
        public const string KeyBegin = "#";

        /// <summary>
        /// Поиск в строке ключей вида ##Abc_234 (т.е. комбинация букв, цифр и '_' после двойной решетки).
        /// При этом ключ не должен начинать строку (защита от дурака)
        /// </summary>
        public const string KeyReferencesRegexString = @"(?!^)(##\w+)";

        /// <summary>
        /// Поиск в строке вхождений аргументов string.Format вида {0} (т.е. чисел, обрамленных фигурными скобками)
        /// </summary>
        public const string ArgReferencesRegexString = @"\{\d+\}";


        //=== Props ===========================================================

        public string Key { get; set; }

        public TranslationData TranslationData { get; set; }

        public bool IsValid => Key != null;


        //=== Ctor ============================================================

        public LocalizedString(string key, TranslationData translationData = null)
        {
            Key = key;
            TranslationData = translationData;
        }


        //=== Public ==========================================================

        public static bool IsKey(LocalizedString localizedString)
        {
            return localizedString.Key?.StartsWith(KeyBegin) ?? false;
        }

        public override string ToString()
        {
            return $"(LS: Key='{Key}' {TranslationData})";
        }

        public string GetString(ICatalog catalog, int pluralNum, params object[] args)
        {
            if (catalog.AssertIfNull(nameof(catalog)))
                return "";

            if (IsKey(this) && TranslationData != null)
                Logger.IfWarn()?.Message($"key='{Key}' is key but {nameof(TranslationData)} isn't null").Write();

            if (Key != null && Key.StartsWith("!")) //DEBUG
                return "!Err" + Key;

            if (string.IsNullOrWhiteSpace(Key))
                return Key ?? "";

            return IsKey(this)
                ? catalog.GetStringAuto(Key, pluralNum, args)
                : catalog.GetStringFromCustomData(CreateTranslationDataWithText(), pluralNum, args);
        }

        public TranslationData CreateTranslationDataWithText()
        {
            var data = TranslationData != null ? TranslationData.Clone() : new TranslationData();
            if (!IsKey(this)) //Предполагается, что это всегда выполняется, но для пояснения логики и сюда добавлено
                data.Text = Key;
            return data;
        }

        public bool Equals(LocalizedString other)
        {
            return Key == other.Key && Equals(TranslationData, other.TranslationData);
        }

        public override bool Equals(object obj)
        {
            return obj is LocalizedString other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Key != null ? Key.GetHashCode() : 0) * 397) ^ (TranslationData != null ? TranslationData.GetHashCode() : 0);
            }
        }
    }
}