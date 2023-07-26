using System;

namespace UnityWeld.Binding.Adapters
{
    /// <summary>
    /// Adapter that converts a float to a string
    /// </summary>
    [Adapter(typeof(float), typeof(string), typeof(FloatToStringWithLimitAdapterOptions))]
    public class FloatToStringWithLimitAdapter : IAdapter
    {
        public object Convert(object valueIn, AdapterOptions options)
        {
            var limitAdapterOptions = (FloatToStringWithLimitAdapterOptions) options;
            var val = (float) valueIn;
            if (Math.Abs(val) > limitAdapterOptions.AbsMax)
                return val > 0 ? $">{limitAdapterOptions.AbsMax}" : $"<-{limitAdapterOptions.AbsMax}";

            return val.ToString(limitAdapterOptions.Format);
        }
    }
}