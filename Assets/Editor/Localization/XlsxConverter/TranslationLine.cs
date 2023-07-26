namespace L10n.XlsConverterNs
{
    public class TranslationLine : TranslationLineBase
    {
        public string OrgText;
        public string DstOldText;
        public string DstNewText;
        public string Comments;

        public override string ToString()
        {
            return $"({nameof(TranslationLine)}: [{Key}] v{Version}, {(IsPlural ? "plural, " : "")} {nameof(OrgText)}='{OrgText}'" +
                   $"{(string.IsNullOrEmpty(DstOldText) ? "" : $", {nameof(DstOldText)}='{DstOldText}'")}" +
                   $"{(string.IsNullOrEmpty(Errors) ? "" : $", {nameof(Errors)}='{Errors}'")})";
        }
    }
}