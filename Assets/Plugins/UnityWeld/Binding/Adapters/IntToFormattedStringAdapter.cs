namespace UnityWeld.Binding.Adapters
{
    [Adapter(typeof(int), typeof(string), typeof(IntToFormattedStringAdapterOptions))]
    public class IntToFormattedStringAdapter : IAdapter
    {
        public object Convert(object valueIn, AdapterOptions options)
        {
            var format = ((IntToFormattedStringAdapterOptions) options).Format;
            return ((int) valueIn).ToString(format);
        }
    }
}