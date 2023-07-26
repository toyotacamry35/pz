using System;
using System.Linq;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ReactivePropsNs.ThreadSafe
{
    public interface IDisposableCollection : ICollection<IDisposable>, IDisposable
    {}
    
    public class DisposableComposite : IDisposableCollection
    {
        private ConcurrentDictionary<IDisposable, bool> _disposables = new ConcurrentDictionary<IDisposable, bool>();


        //=== Props ===========================================================

        public int Count => _disposables.Count;
        public bool IsReadOnly => false;


        //=== Public ==========================================================

        public IEnumerator<IDisposable> GetEnumerator()
        {
            return _disposables.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _disposables.GetEnumerator();
        }

        public void Add(IDisposable item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (_disposables == null) throw new ObjectDisposedException($"{this}");
            if (item == null)
                throw new ArgumentException($"Something wrong! {GetType().NiceName()}.Add(<null>)");
            _disposables.TryAdd(item, true);
        }

        public void Clear()
        {
            var disposables = Interlocked.Exchange(ref _disposables, new ConcurrentDictionary<IDisposable, bool>());
            if (disposables != null)
                foreach (var pair in disposables)
                    pair.Key.Dispose();
        }

        public bool Contains(IDisposable item)
        {
            throw new NotSupportedException($"{GetType()}.{nameof(Contains)} is incorrect use case.");
        }

        public void CopyTo(IDisposable[] array, int arrayIndex)
        {
            throw new NotSupportedException($"{GetType()}.{nameof(CopyTo)} is incorrect use case.");
        }

        public bool Remove(IDisposable item)
        {
            if (_disposables == null) throw new ObjectDisposedException($"{this}");
            if (item == null)
                throw new ArgumentException($"Something wrong! {GetType().NiceName()}.Add(<null>)");
            return _disposables.TryRemove(item, out var outTrue);
        }

        public void Dispose()
        {
            var disposables = Interlocked.Exchange(ref _disposables, null);
            if (disposables != null)
                foreach(var pair in disposables)
                    pair.Key.Dispose();
        }
    }
}