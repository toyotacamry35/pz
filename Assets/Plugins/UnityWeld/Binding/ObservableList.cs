using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityWeld.Binding
{
    public class ObservableList<T> : IList<T>, IList, INotifyCollectionChanged
    {
        /// <summary>
        /// Inner (non-obsevable) list.
        /// </summary>
        private readonly List<T> _innerList = new List<T>();

        /// <summary>
        /// Event raised when the collection has been changed.
        /// </summary>
        public event EventHandler<NotifyCollectionChangedEventArgs> CollectionChanged;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ObservableList()
        {
        }

        /// <summary>
        /// Create from existing items.
        /// </summary>
        public ObservableList(IEnumerable<T> items)
        {
            _innerList.AddRange(items);
        }

        public int IndexOf(T item)
        {
            return _innerList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _innerList.Insert(index, item);

            CollectionChanged?.Invoke(this, NotifyCollectionChangedEventArgs.ItemAdded(item, index));
        }

        public void RemoveAt(int index)
        {
            var item = _innerList[index];

            _innerList.RemoveAt(index);

            CollectionChanged?.Invoke(this, NotifyCollectionChangedEventArgs.ItemRemoved(item, index));
        }

        public T this[int index]
        {
            get { return _innerList[index]; }
            set
            {
                var oldValue = _innerList[index];
                _innerList[index] = value;
                CollectionChanged?.Invoke(this, NotifyCollectionChangedEventArgs.ItemRemoved(oldValue, index));
                CollectionChanged?.Invoke(this, NotifyCollectionChangedEventArgs.ItemAdded(value, index));
            }
        }

        public void Add(T item)
        {
            var newIndex = _innerList.Count;

            _innerList.Add(item);

            CollectionChanged?.Invoke(this, NotifyCollectionChangedEventArgs.ItemAdded(item, newIndex));
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach(var item in items)
                Add(item);
        }

        public void Clear()
        {
            var oldItems = _innerList.Cast<object>().ToArray();

            _innerList.Clear();

            CollectionChanged?.Invoke(this, NotifyCollectionChangedEventArgs.Reset(oldItems));
        }

        public bool Contains(T item)
        {
            return _innerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _innerList.CopyTo(array, arrayIndex);
        }

        public int Count => _innerList.Count;

        public bool IsReadOnly => false;

        public bool IsFixedSize => true;

        public object SyncRoot => this;

        public bool IsSynchronized => false;

        object IList.this[int index]
        {
            get { return _innerList[index]; }
            set
            {
                var oldValue = _innerList[index];
                _innerList[index] = (T) value;
                CollectionChanged?.Invoke(this, NotifyCollectionChangedEventArgs.ItemRemoved(oldValue, index));
                CollectionChanged?.Invoke(this, NotifyCollectionChangedEventArgs.ItemAdded(value, index));
            }
        }

        public bool Remove(T item)
        {
            var index = _innerList.IndexOf(item);
            var result = _innerList.Remove(item);

            if (result)
                CollectionChanged?.Invoke(this, NotifyCollectionChangedEventArgs.ItemRemoved(item, index));

            return result;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        public int Add(object item)
        {
            var newIndex = _innerList.Count;

            _innerList.Add((T) item);

            CollectionChanged?.Invoke(this, NotifyCollectionChangedEventArgs.ItemAdded(item, newIndex));

            return _innerList.Count - 1;
        }

        public bool Contains(object item)
        {
            return _innerList.Contains((T) item);
        }

        public int IndexOf(object item)
        {
            return _innerList.IndexOf((T) item);
        }

        public void Insert(int index, object item)
        {
            _innerList.Insert(index, (T) item);

            CollectionChanged?.Invoke(this, NotifyCollectionChangedEventArgs.ItemAdded(item, index));
        }

        public void Remove(object item)
        {
            var index = _innerList.IndexOf((T) item);
            var result = _innerList.Remove((T) item);

            if (result)
                CollectionChanged?.Invoke(this, NotifyCollectionChangedEventArgs.ItemRemoved(item, index));
        }

        public void CopyTo(Array array, int index)
        {
            _innerList.CopyTo((T[]) array, index);
        }
    }

    public static class LinqExts
    {
        /// <summary>
        /// Convert an IEnumerable into an observable list.
        /// </summary>
        public static ObservableList<T> ToObservableList<T>(this IEnumerable<T> source)
        {
            return new ObservableList<T>(source);
        }

        /// <summary>
        /// Convert a variable length argument list of items to an ObservableList.
        /// </summary>
        public static ObservableList<T> ObservableListFromItems<T>(params T[] items)
        {
            return new ObservableList<T>(items);
        }
    }
}