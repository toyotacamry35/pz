namespace UnityWeld.Binding.Adapters
{
    [Adapter(typeof(int), typeof(object))]
    public class IntToObjectAdapter : IAdapter
    {
        public object Convert(object valueIn, AdapterOptions options)
        {
            return valueIn;
        }
    }
}