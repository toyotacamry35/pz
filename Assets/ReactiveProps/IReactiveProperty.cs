namespace ReactivePropsNs
{
    public interface IReactiveProperty<out T> : IStream<T>
    {
        bool HasValue { get; }
        T Value { get; }
    }
}