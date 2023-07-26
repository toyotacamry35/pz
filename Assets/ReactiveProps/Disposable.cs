using System;

namespace ReactivePropsNs
{
    public static class Disposable
    {
        public static readonly IDisposable Empty = EmptyDisposable.Singleton;

        public static IDisposable Create(Action disposeAction)
        {
            return new AnonymousDisposable(disposeAction);
        }

//        public static IDisposable CreateWithState<TState>(TState state, Action<TState> disposeAction)
//        {
//            return new AnonymousDisposable<TState>(state, disposeAction);
//        }


        //=== Subclasses ==============================================================================================

        private class EmptyDisposable : IDisposable
        {
            public static EmptyDisposable Singleton = new EmptyDisposable();

            private EmptyDisposable()
            {
            }

            public void Dispose()
            {
            }
        }

        private class AnonymousDisposable : IDisposable
        {
            bool _isDisposed = false;
            readonly Action _dispose;

            public AnonymousDisposable(Action dispose)
            {
                _dispose = dispose;
            }

            public void Dispose()
            {
                if (!_isDisposed)
                {
                    _isDisposed = true;
                    _dispose();
                }
            }
        }

//        class AnonymousDisposable<T> : IDisposable
//        {
//            bool isDisposed = false;
//            readonly T state;
//            readonly Action<T> dispose;
//
//            public AnonymousDisposable(T state, Action<T> dispose)
//            {
//                this.state = state;
//                this.dispose = dispose;
//            }
//
//            public void Dispose()
//            {
//                if (!isDisposed)
//                {
//                    isDisposed = true;
//                    dispose(state);
//                }
//            }
//        }
    }
}