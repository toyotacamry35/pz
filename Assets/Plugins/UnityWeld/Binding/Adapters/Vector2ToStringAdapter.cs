using UnityEngine;

namespace UnityWeld.Binding.Adapters
{
    /// <summary>
    /// Adapter for converting from an Vector2 to a string.
    /// </summary>
    [Adapter(typeof(Vector2), typeof(string))]
    public class Vector2ToStringAdapter : IAdapter
    {
        public object Convert(object valueIn, AdapterOptions options)
        {
            return ((Vector2) valueIn).ToString();
        }
    }
}