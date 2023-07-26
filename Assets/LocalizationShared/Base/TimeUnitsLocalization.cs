using System;
using System.Text;

namespace L10n
{
    public static class TimeUnitsLocalization
    {
        public static string GetLocalizedStringFromMinutes(TimeUnitsDef timeUnitsDef, float minutes)
        {
            return GetDhmsLocalizedStringFromSeconds(timeUnitsDef, minutes * 60, false);
        }

        /// <summary>
        /// Возвращает локализованную строку вида: X дн. Y ч. Z мин.[ S сек.]
        /// </summary>
        public static string GetDhmsLocalizedStringFromSeconds(TimeUnitsDef timeUnitsDef, float fromSeconds, bool useSeconds = true, string divider = " ")
        {
            if (timeUnitsDef.AssertIfNull(nameof(timeUnitsDef)))
                return "";

            var timeSpan = new TimeSpan(0, 0, (int) fromSeconds);

            var sb = new StringBuilder();
            var isStarted = false;

            if (timeSpan.Days > 0)
            {
                isStarted = true;
                sb.Append(timeUnitsDef.Days.GetText(timeSpan.Days));
            }

            if (timeSpan.Hours > 0 || isStarted)
            {
                if (isStarted)
                    sb.Append(divider);

                isStarted = true;
                sb.Append(timeUnitsDef.Hours.GetText(timeSpan.Hours));
            }

            if (timeSpan.Minutes > 0 || isStarted || !useSeconds)
            {
                if (isStarted)
                    sb.Append(divider);

                isStarted = true;
                sb.Append(timeUnitsDef.Minutes.GetText(timeSpan.Minutes));
            }

            if (useSeconds)
            {
                if (isStarted)
                    sb.Append(divider);

                sb.Append(timeUnitsDef.Seconds.GetText(timeSpan.Seconds));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Возвращает локализованную строку вида: X дн.[ Y ч.]
        /// </summary>
        public static string GetDaysHoursLocalizedStringFromSeconds(TimeUnitsDef timeUnitsDef, float fromSeconds, bool useHours = false)
        {
            if (timeUnitsDef.AssertIfNull(nameof(timeUnitsDef)))
                return "";

            var timeSpan = new TimeSpan(0, 0, (int) fromSeconds);

            return useHours && timeSpan.Hours > 0
                ? $"{(timeUnitsDef.Days.GetText(timeSpan.Days))} {(timeUnitsDef.Hours.GetText(timeSpan.Hours))}"
                : timeUnitsDef.Days.GetText(timeSpan.Days);
        }

        /// <summary>
        /// Возвращает локализованную строку вида: X дн. или Y ч. или Z мин. или S сек.
        /// </summary>
        public static string GetDhmsMajorUnitOnlyFromSeconds(TimeUnitsDef timeUnitsDef, float fromSeconds)
        {
            if (timeUnitsDef.AssertIfNull(nameof(timeUnitsDef)))
                return "";

            var timeSpan = new TimeSpan(0, 0, (int) fromSeconds);

            if (timeSpan.Days > 0)
                return timeUnitsDef.Days.GetText(timeSpan.Days);

            if (timeSpan.Hours > 0)
                return timeUnitsDef.Hours.GetText(timeSpan.Hours);

            if (timeSpan.Minutes > 0)
                return timeUnitsDef.Minutes.GetText(timeSpan.Minutes);

            return timeUnitsDef.Seconds.GetText(timeSpan.Seconds);
        }
    }
}