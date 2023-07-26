using System;

namespace ReactivePropsNs
{
    public class ReactivePropertyConnector<T> : IStream<T>
    {
        private bool _useDefauld;

        public ReactivePropertyConnector(bool reactOnChangesOnly = true, bool useDefauld = true) {
            _output = new ReactiveProperty<T>(reactOnChangesOnly);
            _useDefauld = useDefauld;
        }

        private readonly ReactiveProperty<T> _output = new ReactiveProperty<T>();
        private IDisposable _lastConnection;
        private ActionBasedListener<T> _listener;

        public void Connect(IStream<T> stream)
        {
            Disconnect();
            if (stream == null)
                return;
            _listener = new ActionBasedListener<T>(t => _output.Value = t, Disconnect);
            _lastConnection = stream.Subscribe(_listener);
        }

        public IDisposable Subscribe(IListener<T> listener)
        {
            return _output.Subscribe(listener);
        }

        public void Dispose()
        {
            _output.Dispose();
            _lastConnection?.Dispose();
            _listener?.Dispose();
        }

        public bool IsDisposed => _output.IsDisposed;

        public string DeepLog(string prefix)
        {
            return _output.DeepLog(prefix);
        }

        public void Disconnect()
        {
            _lastConnection?.Dispose();
            _lastConnection = null;
            _listener?.Dispose();
            _listener = null;
            if (_useDefauld)
                _output.Value = default;
        }
    }
}