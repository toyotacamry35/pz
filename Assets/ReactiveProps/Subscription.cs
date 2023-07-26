using System;

namespace ReactivePropsNs
{
    public static class Subscription<T>
    {
        public static readonly ISubscription<T> Empty = EmptySubscriptionSingleton.Singleton;

        private class EmptySubscriptionSingleton : ISubscription<T>
        {
            public static readonly EmptySubscriptionSingleton Singleton = new EmptySubscriptionSingleton();

            private EmptySubscriptionSingleton() { }

            public bool IsDisposed => true;

            public void Dispose() { }

            public void OnCompleted() { }

            public void OnNext(T value) { }

            public void SetRequestLogHandler(Func<string, string> handler) { }
        }
    }
}