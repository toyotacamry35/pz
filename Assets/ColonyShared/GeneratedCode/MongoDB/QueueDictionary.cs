using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace SharedCode.Utils
{
    /// <summary>
    /// Represents a dictionary that tracks the order that items were added.
    /// </summary>
    /// <typeparam name="TKey">The type of the dictionary keys.</typeparam>
    /// <typeparam name="TValue">The type of the dictionary values.</typeparam>
    /// <remarks>
    /// This dictionary makes it possible to get the index of a key and a key based on an index.
    /// It can be costly to find the index of a key because it must be searched for linearly.
    /// It can be costly to insert a key/value pair because other key's indexes must be adjusted.
    /// It can be costly to remove a key/value pair because other keys' indexes must be adjusted.
    /// </remarks>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(QueueDictionaryDebugView<,>))]
    public sealed class QueueDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {
        private readonly Dictionary<TKey, int> _dictionary;
        private readonly IndexQueue<TKey> _keys;
        private readonly IndexQueue<TValue> _values;
        private int _version;
        private int _counter;

        /// <summary>
        /// Initializes a new instance of an OrderedDictionary.
        /// </summary>
        public QueueDictionary()
            : this(0, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of an OrderedDictionary.
        /// </summary>
        /// <param name="capacity">The initial capacity of the dictionary.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">The capacity is less than zero.</exception>
        public QueueDictionary(int capacity)
            : this(capacity, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of an OrderedDictionary.
        /// </summary>
        /// <param name="comparer">The equality comparer to use to compare keys.</param>
        public QueueDictionary(IEqualityComparer<TKey> comparer)
            : this(0, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of an OrderedDictionary.
        /// </summary>
        /// <param name="capacity">The initial capacity of the dictionary.</param>
        /// <param name="comparer">The equality comparer to use to compare keys.</param>
        public QueueDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, int>(capacity, comparer ?? EqualityComparer<TKey>.Default);
            _keys = new IndexQueue<TKey>(capacity);
            _values = new IndexQueue<TValue>(capacity);
        }

        /// <summary>
        /// Gets the equality comparer used to compare keys.
        /// </summary>
        public IEqualityComparer<TKey> Comparer => _dictionary.Comparer;

        /// <summary>
        /// Adds the given key/value pair to the dictionary.
        /// </summary>
        /// <param name="key">The key to add to the dictionary.</param>
        /// <param name="value">The value to associated with the key.</param>
        /// <exception cref="System.ArgumentException">The given key already exists in the dictionary.</exception>
        /// <exception cref="System.ArgumentNullException">The key is null.</exception>
        public void Enqueue(TKey key, TValue value)
        {
            int newIndex = _values.Count + _counter;
            if(newIndex == int.MaxValue)
            {
                for (int i = 0; i < _keys.Count; ++i)
                {
                    _dictionary[_keys[i]] = i;
                    _counter = 0;
                }
            }
            _dictionary.Add(key, _values.Count + _counter);
            _keys.Enqueue(key);
            _values.Enqueue(value);
            ++_version;
        }

        //
        // Summary:
        //     Removes and returns the object at the beginning of the System.Collections.Generic.Queue`1.
        //
        // Returns:
        //     The object that is removed from the beginning of the System.Collections.Generic.Queue`1.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The System.Collections.Generic.Queue`1 is empty.
        public KeyValuePair<TKey, TValue> Dequeue()
        {
            var key = _keys.Dequeue();
            var value = _values.Dequeue();
            _dictionary.Remove(key);
            ++_version;
            ++_counter;
            return new KeyValuePair<TKey, TValue>(key, value);
        }

        public bool TryDequeue(out KeyValuePair<TKey, TValue> result)
        {
            if (_dictionary.Count == 0)
            {
                result = default(KeyValuePair<TKey, TValue>);
                return false;
            }

            result = Dequeue();
            return true;
        }

        public KeyValuePair<TKey, TValue> Peek()
        {
            var key = _keys.Peek();
            var value = _values.Peek();
            return new KeyValuePair<TKey, TValue>(key, value);
        }

        /// <summary>
        /// Determines whether the given key exists in the dictionary.
        /// </summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>True if the key exists in the dictionary; otherwise, false.</returns>
        /// <exception cref="System.ArgumentNullException">The key is null.</exception>
        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

        /// <summary>
        /// Gets the key at the given index.
        /// </summary>
        /// <param name="index">The index of the key to get.</param>
        /// <returns>The key at the given index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The index is negative -or- larger than the number of keys.</exception>
        public TKey GetKey(int index) => _keys[index];

        /// <summary>
        /// Gets the index of the given key.
        /// </summary>
        /// <param name="key">The key to get the index of.</param>
        /// <returns>The index of the key in the dictionary -or- -1 if the key is not found.</returns>
        /// <remarks>The operation runs in O(n).</remarks>
        public int IndexOf(TKey key)
        {
            int index;
            if (_dictionary.TryGetValue(key, out index))
            {
                return index - _counter;
            }
            return -1;
        }

        /// <summary>
        /// Gets the keys in the dictionary in the order they were added.
        /// </summary>
        public ReadOnlyIndexQueueCollection<TKey> Keys => new ReadOnlyIndexQueueCollection<TKey>(_keys);

        /// <summary>
        /// Tries to get the value associated with the given key. If the key is not found,
        /// default(TValue) value is stored in the value.
        /// </summary>
        /// <param name="key">The key to get the value for.</param>
        /// <param name="value">The value used to hold the results.</param>
        /// <returns>True if the key was found; otherwise, false.</returns>
        /// <exception cref="System.ArgumentNullException">The key is null.</exception>
        public bool TryGetValue(TKey key, out TValue value)
        {
            int index;
            if (_dictionary.TryGetValue(key, out index))
            {
                value = _values[index - _counter];
                return true;
            }
            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Gets the values in the dictionary.
        /// </summary>
        public ReadOnlyIndexQueueCollection<TValue> Values => new ReadOnlyIndexQueueCollection<TValue>(_values);

        /// <summary>
        /// Gets or sets the value at the given index.
        /// </summary>
        /// <param name="index">The index of the value to get.</param>
        /// <returns>The value at the given index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The index is negative -or- beyond the length of the dictionary.</exception>
        public TValue this[int index]
        {
            get { return _values[index]; }
            set { _values[index] = value; }
        }

        /// <summary>
        /// Gets or sets the value associated with the given key.
        /// </summary>
        /// <param name="key">The key to get the associated value by or to associate with the value.</param>
        /// <returns>The value associated with the given key.</returns>
        /// <exception cref="System.ArgumentNullException">The key is null.</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">The key is not in the dictionary.</exception>
        public TValue this[TKey key]
        {
            get
            {
                return _values[_dictionary[key] - _counter];
            }
            set
            {
                int index;
                if (_dictionary.TryGetValue(key, out index))
                {
                    _values[index - _counter] = value;
                }
                else
                {
                    Enqueue(key, value);
                }
            }
        }

        /// <summary>
        /// Removes all key/value pairs from the dictionary.
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
            _keys.Clear();
            _values.Clear();
            ++_version;
            _counter = 0;
        }

        /// <summary>
        /// Gets the number of key/value pairs in the dictionary.
        /// </summary>
        public int Count => _dictionary.Count;

        /// <summary>
        /// Gets the key/value pairs in the dictionary in the order they were added.
        /// </summary>
        /// <returns>An enumerator over the key/value pairs in the dictionary.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            int startVersion = _version;
            for (int index = 0; index != _keys.Count; ++index)
            {
                var key = _keys[index];
                var value = _values[index];
                yield return new KeyValuePair<TKey, TValue>(key, value);
                if (_version != startVersion)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Wraps the keys and values in an QueuerDictionary.
        /// </summary>
        [DebuggerTypeProxy(typeof(ReadOnlyIndexQueueCollectionDebugView<,>))]
        [DebuggerDisplay("Count = {Count}")]
        public sealed class ReadOnlyIndexQueueCollection<TValue> : ICollection<TValue>
        {
            private readonly IndexQueue<TValue> _values;

            /// <summary>
            /// Initializes a new instance of a ReadOnlyIndexQueueCollection.
            /// </summary>
            /// <param name="values">The OrderedDictionary whose keys to wrap.</param>
            /// <exception cref="System.ArgumentNullException">The dictionary is null.</exception>
            internal ReadOnlyIndexQueueCollection(IndexQueue<TValue> values)
            {
                _values = values;
            }

            /// <summary>
            /// Copies the values from the OrderedDictionary to the given array, starting at the given index.
            /// </summary>
            /// <param name="array">The array to copy the values to.</param>
            /// <param name="arrayIndex">The index into the array to start copying the values.</param>
            /// <exception cref="System.ArgumentNullException">The array is null.</exception>
            /// <exception cref="System.ArgumentOutOfRangeException">The arrayIndex is negative.</exception>
            /// <exception cref="System.ArgumentException">The array, starting at the given index, is not large enough to contain all the values.</exception>
            public void CopyTo(TValue[] array, int arrayIndex)
            {
                _values.CopyTo(array, arrayIndex);
            }

            /// <summary>
            /// Gets the number of values in the OrderedDictionary.
            /// </summary>
            public int Count => _values.Count;

            /// <summary>
            /// Gets an enumerator over the values in the OrderedDictionary.
            /// </summary>
            /// <returns>The enumerator.</returns>
            public IEnumerator<TValue> GetEnumerator() => _values.GetEnumerator();

            [EditorBrowsable(EditorBrowsableState.Never)]
            bool ICollection<TValue>.Contains(TValue item) => _values.Contains(item);

            [EditorBrowsable(EditorBrowsableState.Never)]
            void ICollection<TValue>.Add(TValue item) { throw new NotSupportedException("An attempt was made to edit a read-only list."); }

            [EditorBrowsable(EditorBrowsableState.Never)]
            void ICollection<TValue>.Clear() { throw new NotSupportedException("An attempt was made to edit a read-only list."); }

            [EditorBrowsable(EditorBrowsableState.Never)]
            bool ICollection<TValue>.IsReadOnly => true;

            [EditorBrowsable(EditorBrowsableState.Never)]
            bool ICollection<TValue>.Remove(TValue item) { throw new NotSupportedException("An attempt was made to edit a read-only list."); }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }

    internal sealed class QueueDictionaryDebugView<TKey, TValue>
    {
        private readonly QueueDictionary<TKey, TValue> _dictionary;

        public QueueDictionaryDebugView(QueueDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<TKey, TValue>[] Items => _dictionary.ToArray();
    }

    internal sealed class ReadOnlyIndexQueueCollectionDebugView<TKey, TValue>
    {
        private readonly ICollection<TValue> _collection;

        public ReadOnlyIndexQueueCollectionDebugView(ICollection<TValue> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            _collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public TValue[] Items
        {
            get
            {
                TValue[] items = new TValue[_collection.Count];
                _collection.CopyTo(items, 0);
                return items;
            }
        }
    }
}
