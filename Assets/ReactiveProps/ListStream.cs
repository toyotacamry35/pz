using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ReactivePropsNs
{
    public class ListStream<T> : IListStream<T>, IList<T>, IIsDisposed
    {
        private static readonly bool IsDebug = false;
        private readonly string _name;

        private StreamProxy<InsertEvent<T>> _insertProxy;
        private StreamProxy<RemoveEvent<T>> _removeProxy;
        private StreamProxy<ChangeEvent<T>> _changeProxy;

        /// <summary>
        /// Конвенция использования: во время рассылки событий через _insertProxy и _removeProxy данное свойство содержит неактуальную информацию.
        /// Для получения актуальной информации следует пользоваться свойством Count
        /// </summary>
        private ReactiveProperty<int> _countRp = PooledReactiveProperty<int>.Create();

        private List<T> _list;
        private readonly bool _reactOnChangesOnly;

        //=== Props ===========================================================

        public bool IsDisposed { get; private set; }

        public IStream<int> CountStream => _countRp;
        public IStream<InsertEvent<T>> InsertStream => _insertProxy;
        public IStream<RemoveEvent<T>> RemoveStream => _removeProxy;
        public IStream<ChangeEvent<T>> ChangeStream => _changeProxy;

        public int Count => _list?.Count ?? 0;

        bool ICollection<T>.IsReadOnly => false;


        //=== Ctor ============================================================

        public ListStream(List<T> list = null, bool reactOnChangesOnly = true)
        {
            _reactOnChangesOnly = reactOnChangesOnly;
            _name = $"{GetType().NiceName()} [{UniqueId.Id++}] {(IsDebug ? new StackTrace().ToString() : "")}";
            _list = list ?? new List<T>();
            _countRp.Value = 0;
            _insertProxy = PooledStreamProxy<InsertEvent<T>>.Create(OnNewSubscriber);
            _removeProxy = PooledStreamProxy<RemoveEvent<T>>.Create();
            _changeProxy = PooledStreamProxy<ChangeEvent<T>>.Create();
        }

        ~ListStream()
        {
            if (IsDisposed)
                return;

            //ReactiveLogs.Logger.IfWarn()?.Message($"Non disposed {GetType().NiceName()} was finalized -- {this}").Write();
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
            _insertProxy?.Dispose();
            _removeProxy?.Dispose();
            _changeProxy?.Dispose();
            _countRp?.Dispose();
            _list = null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (IsDisposed)
                return Enumerable.Empty<T>().GetEnumerator();

            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            if (IsDisposed)
                return;
            Insert(_list.Count, item);
        }

        public void Clear()
        {
            if (IsDisposed)
                return;

            while (_list.Count > 0)
                RemoveAt(_list.Count - 1);
        }

        public bool Contains(T item)
        {
            if (IsDisposed)
                return false;

            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (IsDisposed)
                return;

            _list.CopyTo(array, arrayIndex);
        }

        public int IndexOf(T item)
        {
            if (IsDisposed)
                return -1;

            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (IsDisposed)
                return;

            _list.Insert(index, item);
            _insertProxy.OnNext(new InsertEvent<T>(index, item));
            UpdateCountStream();
        }

        public bool Remove(T item)
        {
            if (IsDisposed)
                return false;

            var idx = IndexOf(item);
            if (idx < 0)
                return false;

            RemoveAt(idx);
            return true;
        }

        public void RemoveAt(int index)
        {
            if (IsDisposed)
                return;

            var item = _list[index];
            _list.RemoveAt(index);
            _removeProxy.OnNext(new RemoveEvent<T>(index, item));
            UpdateCountStream();
        }

        public T this[int index]
        {
            get => IsDisposed ? default : _list[index];
            set
            {
                if (IsDisposed)
                    return;

                var oldVal = _list[index];
                if (_reactOnChangesOnly && Equals(oldVal, value)) //отбрасываем повторы
                    return;

                _list[index] = value;
                _changeProxy.OnNext(new ChangeEvent<T>(index, oldVal, value));
            }
        }


        //=== Private =========================================================

        private void OnNewSubscriber(ISubscription<InsertEvent<T>> agent)
        {
            if (IsDisposed)
                return;

            for (int i = 0; i < _list.Count; i++)
                agent.OnNext(new InsertEvent<T>(i, _list[i]));
        }

        private void UpdateCountStream()
        {
            _countRp.Value = _list.Count;
        }
    }
}