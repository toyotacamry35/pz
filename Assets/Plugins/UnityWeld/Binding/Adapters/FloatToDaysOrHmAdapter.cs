using System;
using UnityEngine;

namespace UnityWeld.Binding.Adapters
{
    /// <summary>
    /// Adapter that converts a float to a string like: 6h 3m, 24 day(s)
    /// </summary>
    [Adapter(typeof(float), typeof(string))]
    public class FloatToDaysOrHmAdapter : IAdapter
    {
        private const int MinutesInDay = 24 * 60;

        public object Convert(object valueIn, AdapterOptions options)
        {
            var minutes = Mathf.CeilToInt((float) valueIn / 60);
            var timeSpan = new TimeSpan(0, 0, minutes, 0);
            if (minutes < MinutesInDay)
                return timeSpan.ToString(@"h\h\ m\m"); //6h 3m

            return $"{timeSpan.TotalDays:f0} day(s)";
        }
    }
}
