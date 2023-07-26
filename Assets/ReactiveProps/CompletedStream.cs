using System;

namespace ReactivePropsNs
{
    public class CompletedStream<T> : IStream<T>
    {
        public static readonly IStream<T> Empty = new CompletedStream<T>();
        
        public void Dispose()
        {}

        public bool IsDisposed => true;
        
        public IDisposable Subscribe(IListener<T> listener)
        {
            listener.OnCompleted();
            return Subscription<T>.Empty;
        }

        public string DeepLog(string prefix)
        {
            return typeof(CompletedStream<T>).NiceName();
        }
    }
}