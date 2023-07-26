namespace UnityWeld.Binding.Adapters
{
    [Adapter(typeof(string), typeof(object))]
    public class StringToObjectAdapter : IAdapter
    {
        public object Convert(object valueIn, AdapterOptions options)
        {
            return valueIn;
        }
    }
}