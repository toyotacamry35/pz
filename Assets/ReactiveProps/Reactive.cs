using System;
using System.Collections.Generic;

namespace ReactivePropsNs
{
    public static class Reactive
    {
        public static IStream<T> FromEvent<T>(ICollection<IDisposable> disposibles, Action<T> action)
        {
            var proxy = PooledStreamProxy<T>.Create();
            action += proxy.OnNext;
            disposibles.Add(new DisposeAgent(() =>
            {
                action -= proxy.OnNext;
                proxy.Dispose();
            }));
            return proxy;
        }
    }
}