using System.Collections.Generic;
using System.Threading;

namespace ReactivePropsNs.ThreadSafe
{
    public sealed class ReactiveProperty<T> : StreamProxyBase<T>
    {
        private SpinLock _lock = SpinLockExt.Create();
        private readonly IEqualityComparer<T> _comparer;
        private T _value;
        private bool _hasValue;

        public ReactiveProperty()
        {
            _comparer = EqualityComparer<T>.Default;
        }

        public ReactiveProperty(IEqualityComparer<T> comparer)
        {
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }
        
        public ReactiveProperty(T initialValue, IEqualityComparer<T> comparer = null)
        {
            _comparer = comparer ?? EqualityComparer<T>.Default;
            _value = initialValue;
            _hasValue = true;
        }

        public T Value
        {
            set
            {
                StreamProxyBase<T> outStream = null;
                bool locked = false;
                try
                {
                    _lock.EnterWithTimeout(ref locked);
                    if (!IsDisposed && (!_hasValue || !_comparer.Equals(_value, value)))
                    {
                        _value = value;
                        _hasValue = true;
                        outStream = this;
                    }
                } finally { if (locked) _lock.Exit(); }
                outStream?.OnNext(value);
            }
        }
        
        protected override void OnNewListener(ISubscription<T> listener)
        {
            T value = default;
            bool locked = false;
            bool send = false;
            try
            {
                _lock.EnterWithTimeout(ref locked);
                if (!IsDisposed && _hasValue)
                {
                    send = true;
                    value = _value;
                }
            } finally { if (locked) _lock.Exit(); }
            if (send)
                listener.OnNext(value);
        }
        
        protected override void OnDispose()
        {
            _value = default;
            _hasValue = false;
        }
    }
}