//#define ENABLE_REACTIVE_STACKTRACE
//#define ENABLE_REACTIVE_FINALIZER_CHECK
using System;
using System.Collections.Generic;
using ReactiveProps;

namespace ReactivePropsNs
{
    public class ReactiveProperty<T> : StreamProxyBase<T>, IReactiveProperty<T>
    {
        private T _value;
        private bool _hasValue;
        private bool _reactOnChangesOnly;

        public ReactiveProperty(bool reactOnChangesOnly = true, Func<string, string> createLog = null) => Setup(reactOnChangesOnly, createLog);

        public ReactiveProperty(Func<string, string> createLog) => Setup(true, createLog);

        public bool HasValue => _hasValue;
        
        public T Value
        {
            get => _value;
            set
            {
                if (IsDisposed)
                    return;
                if (!HasValue || !_reactOnChangesOnly || !EqualityComparer<T>.Default.Equals(_value, value))
                {
                    _value = value;
                    _hasValue = true;
                    OnNext(value);
                }
            }
        }

        public ReactiveProperty<T> InitialValue(T value)
        {
            if (_hasValue) throw new Exception("ReactiveProperty already has an initial value");
            Value = value;
            return this;
        }

        protected void Setup(bool reactOnChangesOnly = true, Func<string, string> createLog = null)
        {
            base.Setup(createLog);
            _reactOnChangesOnly = reactOnChangesOnly;
            _hasValue = default;
            _value = default;
        }

        protected override void OnNewSubscription(ISubscription<T> listener)
        {
            if (HasValue)
                listener.OnNext(_value);
        }

        /// <summary> Функция медленная, но вызывается только на отладке, так что и пофигу </summary>
        protected override string Log(string prefix) => prefix + $"{ClassName} {(HasValue ? "Value:" + Value : "HasValue:false")}{CreationStackTrace}";
    }

    
    /// <summary>
    /// 
    /// </summary>
    public class PooledReactiveProperty<T> : ReactiveProperty<T>, IPooledObject
    {
        private static readonly Pool<PooledReactiveProperty<T>> _Pool = Pool.Create(() => new PooledReactiveProperty<T>(), PoolSizes.GetPoolMaxCapacityForType<PooledReactiveProperty<T>>(1000));

        public static PooledReactiveProperty<T> Create(bool reactOnChangesOnly = true, Func<string, string> createLog = null)
        {
            var prop = _Pool.Acquire();
            prop.Setup(reactOnChangesOnly, createLog);
            return prop;
        }
        
        public static PooledReactiveProperty<T> Create(Func<string, string> createLog) 
        {
            return Create(true, createLog);
        }

        public new PooledReactiveProperty<T> InitialValue(T value)
        {
            base.InitialValue(value);
            return this;
        }
        
        private void Release(PooledReactiveProperty<T> prop)
        {
            _Pool.Release(prop);
        }

        protected override void DisposeInternal()
        {
            base.DisposeInternal();
            Release(this); // FIXME: Ugly! Но политика работы со временем жизни в реактивностях не позволяет сделать иначе 
        }

        private PooledReactiveProperty() {}
        
        bool IPooledObject.Released { get; set; }
    }
}