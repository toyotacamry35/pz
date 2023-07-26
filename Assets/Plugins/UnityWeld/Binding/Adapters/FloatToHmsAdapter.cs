using System;
using UnityEngine;

namespace UnityWeld.Binding.Adapters
{
    /// <summary>
    /// Adapter that converts a float to a string like: "5s", "2m 5s", "1h 2m 5s"
    /// </summary>
    [Adapter(typeof(float), typeof(string))]
    public class FloatToHmsAdapter : IAdapter
    {
        public object Convert(object valueIn, AdapterOptions options)
        {
            var intSeconds = Mathf.CeilToInt((float) valueIn);
            if (intSeconds < 60)
                return $"{intSeconds}s";

            if (intSeconds < 60 * 60)
                return new TimeSpan(0, 0, 0, intSeconds).ToString(@"m\m\ s\s");

            return new TimeSpan(0, 0, 0, intSeconds).ToString(@"h\h\ m\m\ s\s");
        }
    }
}