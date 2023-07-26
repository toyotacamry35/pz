using UnityEngine;

namespace UnityWeld.Binding.Adapters
{
    /// <summary>
    /// Adapter for converting from an float to a int
    /// </summary>
    [Adapter(typeof(float), typeof(int))]
    public class FloatToIntAdapter : IAdapter
    {
        public object Convert(object valueIn, AdapterOptions options)
        {
            return Mathf.RoundToInt((float) valueIn);
        }
    }
}