using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utilities
{
    [System.Serializable]
    public class SerializedDictionary<K, V> : IEnumerable<KeyValuePair<K,V>>
    {
        private Dictionary<K, int> _lookup;
        [SerializeField]
        List<K> _keys = new List<K>();
        [SerializeField]
        List<V> _values = new List<V>();
        
        private Dictionary<K, int> Lookup
        {
            get
            {
                if (_lookup == null)
                {
                    _lookup = new Dictionary<K, int>();
                    for (int i = 0; i < _keys.Count; i++)
                        _lookup.Add(_keys[i], i);
                }
                return _lookup;
            }
        }

        public bool Has(K key)
        {
            return Lookup.ContainsKey(key);
        }

        public V Get(K key)
        {
            if (Has(key))
                return _values[Lookup[key]];

            return default(V);
        }

        public void Set(K key, V value)
        {
            if (Has(key))
            {
                _values[Lookup[key]] = value;
            }
            else
            {
                _keys.Add(key);
                _values.Add(value);
                Lookup.Add(key, _keys.Count - 1);
            }
        }

        public V Remove(K key)
        {
            if (Has(key))
            {
                var keyIndex = Lookup[key];
                Lookup.Remove(key);
                _keys.RemoveAt(keyIndex);
                var val = _values[keyIndex];
                _values.RemoveAt(keyIndex);
                return val;
            }
            return default(V);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            for (int i = 0; i < _keys.Count; i++)
            {
                var key = _keys[i];
                var val = _values[i];
                yield return new KeyValuePair<K, V>(key, val);
            }
        }
    }

}

