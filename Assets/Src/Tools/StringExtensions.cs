namespace Assets.Src.Tools
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhitespace(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (char.IsWhiteSpace(str[i]) == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
