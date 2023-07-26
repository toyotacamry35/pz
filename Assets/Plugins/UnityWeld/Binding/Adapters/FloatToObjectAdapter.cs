namespace UnityWeld.Binding.Adapters
{
    [Adapter(typeof(float), typeof(object))]
    public class FloatToObjectAdapter : IAdapter
    {
        public object Convert(object valueIn, AdapterOptions options)
        {
            return valueIn;
        }
    }
}