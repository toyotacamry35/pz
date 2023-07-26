using System;

namespace SharedCode.Utils
{
    public static class UnixTimeHelper
    {
        private static readonly DateTime UnixStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnix(this DateTime date)
        {
            return (long)(date - UnixStart).TotalMilliseconds;
        }

        public static DateTime DateTimeFromUnix(long milliseconds)
        {
            return UnixStart + TimeSpanFromUnix(milliseconds);
        }

        public static TimeSpan TimeSpanFromUnix(long milliseconds)
        {
            return TimeSpan.FromMilliseconds(milliseconds);
        }
    }
}
