using System;

namespace ReactivePropsNs
{
    public class StreamConnector<T> : IReactiveProperty<T>
    {
        private readonly ReactiveProperty<T> _outProxy = new ReactiveProperty<T>();
        private DisposableComposite _localD;

        public void Connect(IStream<T> stream)
        {
            Disconnect();

            _localD = new DisposableComposite();
            var listener = PooledActionBasedListener<T>.Create(obj => _outProxy.Value = obj, Disconnect);
            _localD.Add(listener);
            _localD.Add(stream.Subscribe(listener));
        }

        public IDisposable Subscribe(IListener<T> listener)
        {
            return _outProxy.Subscribe(listener);
        }

        public void Dispose()
        {
            Disconnect();
            _outProxy.Dispose();
        }

        public bool IsDisposed => _outProxy.IsDisposed;

        public string DeepLog(string prefix)
        {
            return _outProxy.DeepLog(prefix);
        }

        public void Disconnect()
        {
            _localD?.Dispose();
            _localD = null;
        }

        public bool HasValue => _outProxy.HasValue;
        public T Value => _outProxy.Value;
    }
}