namespace ReactivePropsNs
{
    public interface IDictionaryStream<TKey, TValue>
    {
        IStream<DctAddEvent<TKey, TValue>> AddStream { get; }
        IStream<DctRemoveEvent<TKey, TValue>> RemoveStream { get; }
        IStream<DctChangeEvent<TKey, TValue>> ChangeStream { get; }
        IStream<int> CountStream { get; }
    }

    public struct DctAddEvent<TKey, TValue>
    {
        public readonly TKey Key;
        public readonly TValue Value;

        public DctAddEvent(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    public struct DctRemoveEvent<TKey, TValue>
    {
        public readonly TKey Key;
        public readonly TValue Value;

        public DctRemoveEvent(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    public struct DctChangeEvent<TKey, TValue>
    {
        public readonly TKey Key;
        public readonly TValue OldValue;
        public readonly TValue NewValue;

        public DctChangeEvent(TKey key, TValue oldValue, TValue newValue)
        {
            Key = key;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}