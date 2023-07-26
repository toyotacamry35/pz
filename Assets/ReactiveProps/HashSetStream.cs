using System;
using System.Collections;
using System.Collections.Generic;
using Core.Environment.Logging.Extension;

namespace ReactivePropsNs
{
    public class HashSetStream<T> : IHashSetStream<T>, IIsDisposed
    {
        private const bool CREATE_LOG_WARNING = false;

        private HashSet<T> _internalStorage = new HashSet<T>();
        private StreamProxy<T> _addStream;
        private StreamProxy<T> _removeStream;
        private ReactiveProperty<int> _countStream;

        private string _stackTrace = null;
        private Func<string, string> _createLog;

        

        public HashSetStream(bool reactOnChangesOnly = true, Func<string, string> createLog = null)
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
                _createLog = prefix => $"{prefix}{GetType().NiceName()}{{Count:{_internalStorage.Count}}}{(_stackTrace != null ? $" ({_stackTrace})" : "")}";
            }
            else
                _createLog = createLog;

            _addStream = PooledStreamProxy<T>.Create(OnNewListener, prefix => $"{prefix}{GetType().NiceName()}.AddStream\n{DeepLog(prefix + '\t')}");
            _removeStream = PooledStreamProxy<T>.Create(createLog:prefix => $"{prefix}{GetType().NiceName()}.RemoveStream\n{DeepLog(prefix + '\t')}");
            _countStream = PooledReactiveProperty<int>.Create(prefix => $"{prefix}{GetType().NiceName()}.CountStream{{Value = {_countStream.Value}}}\n{DeepLog(prefix + '\t')}").InitialValue(0);
        }

        public IStream<T> AddStream => _addStream;
        public IStream<T> RemoveStream => _removeStream;
        public IStream<int> CountStream => _countStream;

        private void OnNewListener(ISubscription<T> newSubsciption)
        {
            foreach (var item in _internalStorage)
                _addStream.OnNext(item);
        }
        
        public int Count => _internalStorage.Count;
        public bool IsReadOnly => false;

        void ICollection<T>.Add(T item) => Add(item);
        public bool Add(T item)
        {
            if (IsDisposed)
                return false;
            if (_internalStorage.Add(item))
            {
                _addStream.OnNext(item);
                _countStream.Value = _internalStorage.Count;
                return true;
            }
            return false;
        }

        public bool Remove(T item)
        {
            if (IsDisposed)
                return false;
            if (_internalStorage.Remove(item))
            {
                _removeStream.OnNext(item);
                _countStream.Value = _internalStorage.Count;
                return true;
            }
            return false;
        }

        public void Clear()
        {
            if (IsDisposed)
                return;
            var enumerator = _internalStorage.GetEnumerator();
            while(enumerator.MoveNext()) {
                Remove(enumerator.Current);
                enumerator = _internalStorage.GetEnumerator();
            }
        }

        public bool Contains(T item) => _internalStorage.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => _internalStorage.CopyTo(array, arrayIndex);
        public IEnumerator<T> GetEnumerator() => _internalStorage.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _internalStorage.GetEnumerator();

        /// <summary> Метод медленный и нагружающий систему лишней информацией. Не в отладке не трогать. </summary>
        public string DeepLog(string prefix) => _createLog(prefix);

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;
            IsDisposed = true;
            _addStream.Dispose();
            _removeStream.Dispose();
            _countStream.Dispose();
            _internalStorage.Clear();
            _createLog = DeepLogDisposed;
        }
        private string DeepLogDisposed(string prefix) => $"{prefix}{GetType().NiceName()} DISPOSED";
    }
}
