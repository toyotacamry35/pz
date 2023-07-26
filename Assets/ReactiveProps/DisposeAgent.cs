using System;

namespace ReactivePropsNs
{
    public class DisposeAgent : IDisposable
    {
        private Action _onDispose;

        public DisposeAgent(Action onDispose)
        {
            _onDispose = onDispose;
        }

        public void Dispose()
        {
            _onDispose?.Invoke();
            _onDispose = null;
        }
    }
}