using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using Core.Environment.Logging.Extension;

namespace ReactivePropsNs
{
    public class DictionaryStream<TKey, TValue> : IDictionaryStream<TKey, TValue>, IDictionary<TKey, TValue>, IIsDisposed
    {
        private const bool CREATE_LOG_WARNING = false;

        private static readonly bool IsDebug = false;
        private readonly string _name;

        private StreamProxy<DctAddEvent<TKey, TValue>> _addProxy;
        private StreamProxy<DctRemoveEvent<TKey, TValue>> _removeProxy;
        private StreamProxy<DctChangeEvent<TKey, TValue>> _changeProxy;

        /// <summary>
        /// Конвенция использования: во время рассылки событий через _addProxy и _removeProxy данное свойство содержит неактуальную информацию.
        /// Для получения актуальной информации следует пользоваться свойством Count
        /// </summary>
        private ReactiveProperty<int> _countRp;

        private Dictionary<TKey, TValue> _dictionary;
        private readonly bool _reactOnChangesOnly;

        private string _stackTrace = null;
        private Func<string, string> _createLog;

        //=== Props ===========================================================

        public static IDictionaryStream<TKey, TValue> EmptyDictionaryStream { get; } = new DictionaryStream<TKey, TValue>();

        public bool AllowToFinalizeWithoutDispose { get; set; }

        public bool IsDisposed { get; private set; }

        public int Count => IsDisposed ? 0 : _dictionary.Count;

        public bool IsReadOnly => false;

        public ICollection<TKey> Keys => IsDisposed ? CollectionExtensions<TKey>.GetEmptyCollection() : _dictionary.Keys;
        public ICollection<TValue> Values => IsDisposed ? CollectionExtensions<TValue>.GetEmptyCollection() : _dictionary.Values;

        public IStream<DctAddEvent<TKey, TValue>> AddStream => _addProxy;
        public IStream<DctRemoveEvent<TKey, TValue>> RemoveStream => _removeProxy;
        public IStream<DctChangeEvent<TKey, TValue>> ChangeStream => _changeProxy;
        public IStream<int> CountStream => _countRp;


        //=== Ctor ============================================================

        public DictionaryStream(bool reactOnChangesOnly = true, Func<string, string> createLog = null)
        {
            if (CREATE_LOG_WARNING)
            {
                #pragma warning disable CS0162 // Unreachable code detected
                _stackTrace = Tools.StackTraceLastString();
                ReactiveLogs.Logger.IfWarn()?.Message($"Не создан формирователь лога для {GetType().NiceName()} по адресу: \n {_stackTrace}").Write();
                #pragma warning restore CS0162 // Unreachable code detected
            }
            if (createLog == null)
            {
                _createLog = prefix => $"{prefix}{GetType().NiceName()}{{Count:{_dictionary.Count}}}{(_stackTrace != null ? $" ({_stackTrace})" : "")}";
            }
            else
                _createLog = createLog;
            _reactOnChangesOnly = reactOnChangesOnly;
            _name = $"{GetType().NiceName()} [{UniqueId.Id++}] {(IsDebug ? new StackTrace().ToString() : "")}";
            _dictionary = new Dictionary<TKey, TValue>();
            _addProxy = PooledStreamProxy<DctAddEvent<TKey, TValue>>.Create(OnNewSubscription, prefix => $"{prefix}{GetType().NiceName()}.AddStream\n{DeepLog(prefix + '\t')}");
            _removeProxy = PooledStreamProxy<DctRemoveEvent<TKey, TValue>>.Create(createLog: prefix => $"{prefix}{GetType().NiceName()}.RemoveStream\n{DeepLog(prefix + '\t')}");
            _changeProxy = PooledStreamProxy<DctChangeEvent<TKey, TValue>>.Create(createLog: prefix => $"{prefix}{GetType().NiceName()}.ChangeStream\n{DeepLog(prefix + '\t')}");
            _countRp = PooledReactiveProperty<int>.Create(prefix => $"{prefix}{GetType().NiceName()}.CountStream{{Value = {_countRp.Value}}}\n{DeepLog(prefix + '\t')}").InitialValue(0);
        }

        ~DictionaryStream()
        {
            if (AllowToFinalizeWithoutDispose || IsDisposed)
                return;
            //ReactiveLogs.Logger.IfWarn()?.Message($"Non disposed {GetType().NiceName()} was finalized -- {this}").Write();
        }

        private void OnNewSubscription(ISubscription<DctAddEvent<TKey, TValue>> agent)
        {
            foreach (var item in _dictionary)
                agent.OnNext(new DctAddEvent<TKey, TValue>(item.Key, item.Value));
        }


        //=== Public ==========================================================

        public override string ToString()
        {
            return IsDisposed ? $"{_name} DISPOSED" : _name;
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            DisposeInternal();
            GC.SuppressFinalize(this);
        }

        private void DisposeInternal()
        {
            IsDisposed = true;
            _addProxy?.Dispose();
            _removeProxy?.Dispose();
            _changeProxy?.Dispose();
            _countRp?.Dispose();
            _dictionary = null;

        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (IsDisposed)
                return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();

            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            Add(keyValuePair.Key, keyValuePair.Value);
        }

        public void Add(TKey key, TValue value)
        {
            if (IsDisposed)
                return;

            InnerAdd(key, value);
        }

        /// <summary> Ужасно далеко от оптимальности. Но и сам этот метод нужен, как говорится, только для совместимости. </summary>
        public void Clear()
        {
            if (IsDisposed)
                return;

            while (_dictionary.Count > 0)
                Remove(_dictionary.Keys.First());
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (IsDisposed)
                return false;

            return ((ICollection<KeyValuePair<TKey, TValue>>) _dictionary).Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            if (IsDisposed)
                return false;

            return _dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (IsDisposed)
                return;

            ((ICollection<KeyValuePair<TKey, TValue>>) _dictionary).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (IsDisposed)
                return false;

            if (!((ICollection<KeyValuePair<TKey, TValue>>) _dictionary).Contains(item))
                return false;

            Remove(item.Key);
            return true;
        }

        public bool Remove(TKey key)
        {
            if (IsDisposed)
                return false;

            if (!_dictionary.TryGetValue(key, out var val))
                return false;

            _dictionary.Remove(key);
            _removeProxy.OnNext(new DctRemoveEvent<TKey, TValue>(key, val));
            UpdateCountStream();
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (IsDisposed)
            {
                value = default;
                return false;
            }

            return _dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get
            {
                if (IsDisposed)
                    throw new KeyNotFoundException();

                return _dictionary[key];
            }
            set
            {
                if (IsDisposed)
                    return;

                if (_dictionary.TryGetValue(key, out var oldValue))
                {
                    if (!_reactOnChangesOnly || !Equals(oldValue, value))
                    {
                        _dictionary[key] = value;
                        _changeProxy.OnNext(new DctChangeEvent<TKey, TValue>(key, oldValue, value));
                    }
                }
                else
                {
                    InnerAdd(key, value);
                }
            }
        }


        //=== Private =========================================================

        private void OnNewSubscriber(ISubscription<DctAddEvent<TKey, TValue>> agent)
        {
            if (IsDisposed)
                return;

            foreach (var kvp in _dictionary)
                agent.OnNext(new DctAddEvent<TKey, TValue>(kvp.Key, kvp.Value));
        }

        private void InnerAdd(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            _addProxy.OnNext(new DctAddEvent<TKey, TValue>(key, value));
            UpdateCountStream();
        }

        private void UpdateCountStream()
        {
            _countRp.Value = _dictionary.Count;
        }

        /// <summary> Метод медленный и нагружающий систему лишней информацией. Не в отладке не трогать. </summary>
        public string DeepLog(string prefix) => _createLog(prefix);
        private string DeepLogDisposed(string prefix) => $"{prefix}{GetType().NiceName()} DISPOSED";
    }


    //=== Class =======================================================================================================

    public class CollectionExtensions<T>
    {
        private static readonly List<T> EmptyList = new List<T>();

        public static ICollection<T> GetEmptyCollection()
        {
            return EmptyList;
        }
    }
}