using System;
using System.Collections.Generic;

namespace ReactivePropsNs.ThreadSafe
{
    public static class Reactive
    {
        public static IDisposable Subscribe<T>(this IStream<T> stream, ICollection<IDisposable> disposables, Action<T> onNextAction, Action onCompletedAction)
        {
            var listener = new ActionBasedListener<T>(onNextAction, onCompletedAction);
            disposables.Add(listener);
            var disposable = stream.Subscribe(listener);
            disposables.Add(disposable);
            return listener;
        }
        
        public static IDisposable Subscribe<T>(this IStream<T> stream, ICollection<IDisposable> disposables,
            IListener<T> listener)
        {
            var disposable = stream.Subscribe(listener);
            disposables.Add(disposable);
            return listener;
        }
        
        public static IDisposable Subscribe<T1, T2>(this IStream<ValueTuple<T1, T2>> stream, ICollection<IDisposable> disposables,
            Action<T1, T2> onNextAction, Action onCompletedAction)
        {
            return stream.Subscribe(disposables, val => onNextAction(val.Item1, val.Item2), onCompletedAction);
        }

        public static IDisposable Action<T1, T2, T3>(this IStream<ValueTuple<ValueTuple<T1, T2>, T3>> stream, ICollection<IDisposable> disposables,
            Action<T1, T2, T3> onNextAction, Action onCompletedAction)
        {
            return stream.Subscribe(disposables, val => onNextAction(val.Item1.Item1, val.Item1.Item2, val.Item2), onCompletedAction);
        }

        public static IDisposable Action<T1, T2, T3, T4>(this IStream<ValueTuple<ValueTuple<ValueTuple<T1, T2>, T3>, T4>> stream,
            ICollection<IDisposable> disposables, Action<T1, T2, T3, T4> onNextAction, Action onCompletedAction)
        {
            return stream.Subscribe(
                disposables, val => onNextAction(val.Item1.Item1.Item1, val.Item1.Item1.Item2, val.Item1.Item2, val.Item2), onCompletedAction);
        }
        
        public static IDisposable Action<T>(this IStream<T> stream, ICollection<IDisposable> disposables, Action<T> onNextAction)
        {
            return Subscribe(stream, disposables, onNextAction, null);
        }

        public static IDisposable Action<T1, T2>(this IStream<ValueTuple<T1, T2>> stream, ICollection<IDisposable> disposables, Action<T1, T2> action)
        {
            return stream.Action(disposables, val => action(val.Item1, val.Item2));
        }

        public static IDisposable Action<T1, T2, T3>(this IStream<ValueTuple<ValueTuple<T1, T2>, T3>> stream, ICollection<IDisposable> disposables,
            Action<T1, T2, T3> action)
        {
            return stream.Action(disposables, val => action(val.Item1.Item1, val.Item1.Item2, val.Item2));
        }

        public static IDisposable Action<T1, T2, T3, T4>(this IStream<ValueTuple<ValueTuple<ValueTuple<T1, T2>, T3>, T4>> stream,
            ICollection<IDisposable> disposables, Action<T1, T2, T3, T4> action)
        {
            return stream.Action(disposables, val => action(val.Item1.Item1.Item1, val.Item1.Item1.Item2, val.Item1.Item2, val.Item2));
        }
    }
}