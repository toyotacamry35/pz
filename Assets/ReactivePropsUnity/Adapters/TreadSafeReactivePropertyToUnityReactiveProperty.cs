using System;
using System.Collections.Generic;

namespace ReactivePropsNs
{
    public static class TreadSafeReactivePropertyToUnityReactiveProperty
    {
        public static IStream<T> ThreadSafeToStream<T>(this ThreadSafe.IStream<T> source, ICollection<IDisposable> disposibles)
        {
            var output = new StreamProxy<T>(createLog: prefix => $"{prefix}{source.GetType().NiceName()}.{nameof(ThreadSafeToStream)}<{typeof(T).NiceName()}>({disposibles})");
            ThreadSafe.Reactive.Subscribe(source, disposibles, t => { output.OnNext(t); }, () => { output.Dispose(); disposibles.Remove(output); });
            return output;
        }

        public static IStream<T> ThreadSafeToUnityStream<T>(this ThreadSafe.IStream<T> source, ICollection<IDisposable> disposibles)
        {
            var output = new UnityThreadStream<T>(createLog: prefix => $"{prefix}{source.GetType().NiceName()}.{nameof(ThreadSafeToUnityStream)}<{typeof(T).NiceName()}>({disposibles})");
            ThreadSafe.Reactive.Subscribe(source, disposibles, t => { output.OnNext(t); }, () => { output.Dispose(); disposibles.Remove(output); });
            return output;
        }

        public static IStream<T> ThreadSafeToReactive<T>(this ThreadSafe.IStream<T> source, ICollection<IDisposable> disposibles)
        {
            var output = new ReactiveProperty<T>(createLog: prefix => $"{prefix}{source.GetType().NiceName()}.{nameof(ThreadSafeToStream)}<{typeof(T).NiceName()}>({disposibles})");
            ThreadSafe.Reactive.Subscribe(source, disposibles, t => { output.Value = t; }, () => { output.Dispose(); disposibles.Remove(output); });
            return output;
        }

        public static IStream<T> ThreadSafeToUnityReactive<T>(this ThreadSafe.IStream<T> source, ICollection<IDisposable> disposibles)
        {
            var landing = new UnityThreadStream<T>(createLog: prefix => $"{prefix}{source.GetType().NiceName()}.{nameof(ThreadSafeToUnityStream)}<{typeof(T).NiceName()}>({disposibles})");
            var output = new ReactiveProperty<T>(createLog: prefix => $"{prefix}{source.GetType().NiceName()}.{nameof(ThreadSafeToStream)}<{typeof(T).NiceName()}>({disposibles})");
            landing.Action(disposibles, t => output.Value = t);
            ThreadSafe.Reactive.Subscribe(source, disposibles, t => { landing.OnNext(t); }, () => { output.Dispose(); disposibles.Remove(output); landing.Dispose(); disposibles.Remove(landing); });
            return output;
        }
    }
}