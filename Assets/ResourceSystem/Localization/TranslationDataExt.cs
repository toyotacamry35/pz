namespace L10n
{
    public class TranslationDataExt : TranslationData
    {
        //=== Props ===========================================================

        public int Version { get; set; }

        public long TextHashcode { get; set; }


        //=== Public ==========================================================

        public void ChangeText(string newText, bool upVersion)
        {
            Text = newText;
            TextHashcode = GetTextHashCode();
            if (upVersion)
                Version++;
        }

        public void HashRecalc()
        {
            TextHashcode = GetTextHashCode();
        }
    }
}