using System;

namespace ReactivePropsNs.ThreadSafe
{
    public class Stream<T>
    {
        public static readonly IStream<T> Empty = new NullStream();
        
        private class NullStream : IStream<T>
        {
            IDisposable IStream<T>.Subscribe(IListener<T> listener) => Subscription<T>.Empty;
            public void Dispose() {}
            public bool IsDisposed => true;
        }
    } 
}