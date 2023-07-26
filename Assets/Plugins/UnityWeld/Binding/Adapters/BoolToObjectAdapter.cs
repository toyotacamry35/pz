namespace UnityWeld.Binding.Adapters
{
    [Adapter(typeof(bool), typeof(object))]
    public class BoolToObjectAdapter : IAdapter
    {
        public object Convert(object valueIn, AdapterOptions options)
        {
            return valueIn;
        }
    }
}