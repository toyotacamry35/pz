using System;

namespace ReactivePropsNs.Touchables
{
    /// <summary>
    /// Класс является обёрткой, которую можно синхронно вернуть и синхронно приказхать ей застрелиться,
    /// Но в которую информация кто именно должен застрелиться будет помещена позже
    /// </summary>

    public class SuspendedDisposable : IIsDisposed
    {
        private bool _disposed;
        private object _disposeLock = new object();
        private IDisposable _wrappedDisposable;

        public bool IsDisposed { get { lock (_disposeLock) return _disposed; } }

        public void SetDisposable(IDisposable disposable)
        {
            lock(_disposeLock)
            {
                if (_wrappedDisposable != null)
                    throw new Exception($"{GetType().Name}.{nameof(SetDisposable)}({disposable}) // Wrapped disposable is already defined: {_wrappedDisposable}");
                if (_disposed)
                {
                    disposable.Dispose();
                }
                else
                {
                    _wrappedDisposable = disposable;
                }
            }
        }

        public void Dispose()
        {
            lock (_disposeLock)
            {
                if (_disposed)
                    return;
                _disposed = true;
                if (_wrappedDisposable != null)
                    _wrappedDisposable.Dispose();
            }
        }
    }
}
