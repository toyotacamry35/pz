namespace UnityWeld.Binding.Adapters
{
    /// <summary>
    /// Adapter for converting from an float to a int
    /// </summary>
    [Adapter(typeof(int), typeof(float))]
    public class IntToFloatAdapter : IAdapter
    {
        public object Convert(object valueIn, AdapterOptions options)
        {
            return (float) (int) valueIn;
        }
    }
}