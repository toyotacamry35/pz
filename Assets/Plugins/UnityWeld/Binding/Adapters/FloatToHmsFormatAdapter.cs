using System;
using UnityEngine;

namespace UnityWeld.Binding.Adapters
{
    /// <summary>
    /// Adapter that converts a float to a string H:m:s
    /// </summary>
    [Adapter(typeof(float), typeof(string), typeof(FloatToHmsFormatAdapterOptions))]
    public class FloatToHmsFormatAdapter : IAdapter
    {
        public object Convert(object valueIn, AdapterOptions options)
        {
            var format = ((FloatToHmsFormatAdapterOptions)options).Format;
            var intSeconds = Mathf.Max(Mathf.CeilToInt((float)valueIn), 0);
            return new TimeSpan(0, 0, 0, intSeconds).ToString(format);
        }
    }
}