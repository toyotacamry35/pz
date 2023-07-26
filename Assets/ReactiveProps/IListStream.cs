namespace ReactivePropsNs
{
    public interface IListStream<T>
    {
        IStream<ChangeEvent<T>> ChangeStream { get; }
        IStream<InsertEvent<T>> InsertStream { get; }
        IStream<RemoveEvent<T>> RemoveStream { get; }
        IStream<int> CountStream { get; }
    }

    public struct InsertEvent<T>
    {
        public readonly int Index;
        public readonly T Item;

        public InsertEvent(int index, T item)
        {
            Index = index;
            Item = item;
        }

        public override string ToString()
        {
            return $"{GetType().NiceName()} [{Index}]={Item}";
        }
    }

    public struct RemoveEvent<T>
    {
        public readonly int Index;
        public readonly T Item;

        public RemoveEvent(int index, T item)
        {
            Index = index;
            Item = item;
        }

        public override string ToString()
        {
            return $"{GetType().NiceName()} [{Index}]={Item}";
        }
    }

    public struct ChangeEvent<T>
    {
        public readonly int Index;
        public readonly T OldItem;
        public readonly T NewItem;

        public ChangeEvent(int index, T oldItem, T newItem)
        {
            Index = index;
            OldItem = oldItem;
            NewItem = newItem;
        }

        public override string ToString()
        {
            return $"{GetType().NiceName()} [{Index}] {OldItem} -> {NewItem}";
        }
    }
}