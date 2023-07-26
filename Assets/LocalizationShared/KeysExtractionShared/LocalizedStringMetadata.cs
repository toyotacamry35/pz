namespace L10n.KeysExtractionNs
{
    public class LocalizedStringMetadata
    {
        public string JdbRelPath;

        /// <summary>
        /// В зависимости от условий создания это будет:
        ///     1) фантик с ключом в поле Key
        ///     2) контейнер с данными перевода и с текстом в поле Key
        /// </summary>
        public LocalizedString LocalizedString;
        public string HierPath;
        public bool IsNewKey;

        public LocalizedStringMetadata(string jdbRelPath, LocalizedString localizedString, string hierPath, bool isNewKey)
        {
            JdbRelPath = jdbRelPath;
            LocalizedString = localizedString;
            HierPath = hierPath;
            IsNewKey = isNewKey;
        }

        public static string GetComment(string jdbRelPath, string propsHierPath)
        {
            return $"{jdbRelPath}, {propsHierPath}";
        }

        public override string ToString()
        {
            return $"jdb='{JdbRelPath}', '{HierPath}' {LocalizedString}";
        }
    }
}