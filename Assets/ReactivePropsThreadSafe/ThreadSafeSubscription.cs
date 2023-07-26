namespace ReactivePropsNs.ThreadSafe
{
    public static class Subscription<T>
    {
        public static readonly ISubscription<T> Empty = new EmptySubscriptionSingleton();

        private class EmptySubscriptionSingleton : ISubscription<T>
        {
            public bool IsDisposed => true;
            public void Dispose() { }
            public void OnCompleted() { }
            public void OnNext(T value) { }
        }
    }
}