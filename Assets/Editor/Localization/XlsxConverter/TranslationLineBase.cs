namespace L10n.XlsConverterNs
{
    public abstract class TranslationLineBase
    {
        public const string WarningBegin = "Warning";
        public const string ErrorBegin = "Error";

        public int Version;
        public string Key;
        public bool IsPlural;

        public string Errors { get; protected set; }

        public bool HasErrors => Errors != null && Errors.Contains(ErrorBegin);

        public void AddErrors(string comment)
        {
            Errors = string.IsNullOrEmpty(Errors) ? comment : $"{Errors}\n{comment}";
        }
    }
}