using System;
using System.Collections.Generic;

namespace ReactivePropsNs
{
    public static class FuncStreamExtensions
    {
        /// <summary>
        /// Fires 1st time, when all streams got value, then fires on value changed at any of streams
        /// </summary>
        public static IStream<(T1, T2)> Zip<T1, T2>(
            this IStream<T1> input1,
            ICollection<IDisposable> disposables,
            IStream<T2> input2,
            bool reactOnChangesOnly = false)
        {
            var localD = disposables.CreateInnerD();
            var output = PooledReactiveProperty<(T1, T2)>.Create(reactOnChangesOnly);
            localD.Add(output);

            T1 input1Value = default;
            T2 input2Value = default;
            var input1HasValue = false;
            var input2HasValue = false;

            input1.Subscribe(
                localD,
                val =>
                {
                    if (input1HasValue && reactOnChangesOnly && EqualityComparer<T1>.Default.Equals(input1Value, val))
                        return;
                    input1Value = val;
                    input1HasValue = true;
                    if (input2HasValue)
                        output.Value = (input1Value, input2Value);
                },
                () => disposables.DisposeInnerD(localD));
            input2.Subscribe(
                localD,
                val =>
                {
                    if (input2HasValue && reactOnChangesOnly && EqualityComparer<T2>.Default.Equals(input2Value, val))
                        return;
                    input2Value = val;
                    input2HasValue = true;
                    if (input1HasValue)
                        output.Value = (input1Value, input2Value);
                },
                () => disposables.DisposeInnerD(localD));

            return output;
        }

        public static IStream<( T1 Value1, T2 Value2)> ZipAnyOrDefault<T1, T2>(
            this IStream<T1> stream1,
            ICollection<IDisposable> disposables,
            IStream<T2> stream2,
            bool reactOnChangesOnly = false,
            T1 default1 = default,
            T2 default2 = default)
        {
            var stream1Value = default1;
            var stream2Value = default2;
            var stream1HasValue = false;
            var stream2HasValue = false;

            var output = new ReactiveProperty<(T1 Value1, T2 Value2)>(reactOnChangesOnly);
            disposables.Add(output);

            stream1.Subscribe(
                disposables,
                val =>
                {
                    if (stream1HasValue && reactOnChangesOnly && EqualityComparer<T1>.Default.Equals(stream1Value, val))
                        return;
                    stream1HasValue = true;
                    stream1Value = val;
                    output.Value = (Value1: stream1Value, Value2: stream2Value);
                },
                () => { output.Dispose(); }
            );

            stream2.Subscribe(
                disposables,
                val =>
                {
                    if (stream2HasValue && reactOnChangesOnly && EqualityComparer<T2>.Default.Equals(stream2Value, val))
                        return;
                    stream2HasValue = true;
                    stream2Value = val;
                    output.Value = (Value1: stream1Value, Value2: stream2Value);
                },
                () => { output.Dispose(); }
            );

            return output;
        }

        public static IStream<( T1 Value1, T2 Value2)> ZipSecondOrDefault<T1, T2>(
            this IStream<T1> stream1,
            ICollection<IDisposable> disposables,
            IStream<T2> stream2,
            bool reactOnChangesOnly = false,
            T2 default2 = default)
        {
            var stream1Value = default(T1);
            var stream2Value = default2;
            var stream1HasValue = false;
            var stream2HasValue = false;

            var output = new ReactiveProperty<(T1 Value1, T2 Value2)>(reactOnChangesOnly);
            disposables.Add(output);

            stream1.Subscribe(
                disposables,
                val =>
                {
                    if (stream1HasValue && reactOnChangesOnly && EqualityComparer<T1>.Default.Equals(stream1Value, val))
                        return;
                    stream1HasValue = true;
                    stream1Value = val;
                    output.Value = (Value1: stream1Value, Value2: stream2Value);
                },
                () => { output.Dispose(); }
            );

            stream2.Subscribe(
                disposables,
                val =>
                {
                    if (stream2HasValue && reactOnChangesOnly && EqualityComparer<T2>.Default.Equals(stream2Value, val))
                        return;
                    stream2HasValue = true;
                    stream2Value = val;
                    if (stream1HasValue)
                        output.Value = (Value1: stream1Value, Value2: stream2Value);
                },
                () => { output.Dispose(); }
            );

            return output;
        }

        /// <summary>
        /// .Zip + .Func
        /// </summary>
        public static IStream<U> ZipFunc<T1, T2, U>(
            this IStream<T1> stream1,
            ICollection<IDisposable> disposables,
            IStream<T2> stream2,
            Func<T1, T2, U> func)
        {
            var input1 = PooledReactiveProperty<T1>.Create();
            var input2 = PooledReactiveProperty<T2>.Create();
            var output = PooledReactiveProperty<U>.Create();
            disposables.Add(input1);
            disposables.Add(input2);
            disposables.Add(output);

            void dispose()
            {
                input1.Dispose();
                input2.Dispose();
                output.Dispose();
            }

            stream1.Subscribe(
                disposables,
                val => input1.Value = val,
                dispose);

            stream2.Subscribe(
                disposables,
                val => input2.Value = val,
                dispose);

            input1.Action(
                disposables,
                val =>
                {
                    if (input2.HasValue)
                        output.Value = func(input1.Value, input2.Value);
                });
            input2.Action(
                disposables,
                val =>
                {
                    if (input1.HasValue)
                        output.Value = func(input1.Value, input2.Value);
                });

            return output;
        }

        public static IReactiveProperty<U> Func<T, U>(
            this IStream<T> stream,
            ICollection<IDisposable> disposables,
            Func<T, U> func,
            bool reactOnChangesOnly = true)
        {
            var internalReactive = PooledReactiveProperty<U>.Create(reactOnChangesOnly);
            disposables.Add(internalReactive);
            disposables.Add(
                stream.Subscribe(
                    disposables,
                    (T next) => internalReactive.Value = func(next),
                    () => internalReactive.Dispose()));
            return internalReactive;
        }

        public static IStream<U> Func<T1, T2, U>(
            this IStream<(T1, T2)> stream,
            ICollection<IDisposable> disposables,
            Func<T1, T2, U> func,
            bool reactOnChangesOnly = true)
        {
            var internalReactive = PooledReactiveProperty<U>.Create(reactOnChangesOnly);
            disposables.Add(internalReactive);

            stream.Subscribe(
                disposables,
                val => internalReactive.Value = func(val.Item1, val.Item2),
                () => internalReactive.Dispose());

            return internalReactive;
        }

        public static IStream<U> Func<T1, T2, T3, U>(
            this IStream<((T1, T2), T3)> stream,
            ICollection<IDisposable> disposables,
            Func<T1, T2, T3, U> func,
            bool reactOnChangesOnly = true)
        {
            ReactiveProperty<U> internalReactive = null;
            internalReactive = PooledReactiveProperty<U>.Create(
                reactOnChangesOnly,
                prefix =>
                    $"{prefix}{stream.GetType().NiceName()}.Func(func:{func}, reactOnChangesOnly:{reactOnChangesOnly}){(internalReactive.HasValue ? $"VALUE: {internalReactive.Value}" : " HAS NO VALUE")}\n{stream.DeepLog(prefix + "\t")}");
            disposables.Add(internalReactive);

            stream.Subscribe(
                disposables,
                val => internalReactive.Value = func(val.Item1.Item1, val.Item1.Item2, val.Item2),
                () => internalReactive.Dispose());

            return internalReactive;
        }

        /// <summary>
        /// Для (T1,T2,T3)
        /// </summary>
        public static IStream<U> Func<T1, T2, T3, U>(
            this IStream<(T1, T2, T3)> stream,
            ICollection<IDisposable> disposables,
            Func<T1, T2, T3, U> func,
            bool reactOnChangesOnly = true)
        {
            var internalReactive = PooledReactiveProperty<U>.Create(reactOnChangesOnly);
            disposables.Add(internalReactive);

            stream.Subscribe(
                disposables,
                val => internalReactive.Value = func(val.Item1, val.Item2, val.Item3),
                () => internalReactive.Dispose());

            return internalReactive;
        }

        public static IStream<U> Func<T1, T2, T3, T4, U>(
            this IStream<(((T1, T2), T3), T4)> stream,
            ICollection<IDisposable> disposables,
            Func<T1, T2, T3, T4, U> func,
            bool reactOnChangesOnly = true)
        {
            var internalReactive = PooledReactiveProperty<U>.Create(reactOnChangesOnly);
            disposables.Add(internalReactive);

            stream.Subscribe(
                disposables,
                val => internalReactive.Value = func(val.Item1.Item1.Item1, val.Item1.Item1.Item2, val.Item1.Item2, val.Item2),
                () => internalReactive.Dispose());

            return internalReactive;
        }

        /// <summary>
        /// Для (T1,T2,T3,T4)
        /// </summary>
        public static IStream<U> Func<T1, T2, T3, T4, U>(
            this IStream<(T1, T2, T3, T4)> stream,
            ICollection<IDisposable> disposables,
            Func<T1, T2, T3, T4, U> func,
            bool reactOnChangesOnly = true)
        {
            var internalReactive = PooledReactiveProperty<U>.Create(reactOnChangesOnly);
            disposables.Add(internalReactive);

            stream.Subscribe(
                disposables,
                val => internalReactive.Value = func(val.Item1, val.Item2, val.Item3, val.Item4),
                () => internalReactive.Dispose());

            return internalReactive;
        }

        public static IStream<U> Func<T1, T2, T3, T4, T5, U>(
            this IStream<((((T1, T2), T3), T4), T5)> stream,
            ICollection<IDisposable> disposables,
            Func<T1, T2, T3, T4, T5, U> func,
            bool reactOnChangesOnly = true)
        {
            var internalReactive = PooledReactiveProperty<U>.Create(reactOnChangesOnly);
            disposables.Add(internalReactive);

            stream.Subscribe(
                disposables,
                val => internalReactive.Value =
                    func(val.Item1.Item1.Item1.Item1, val.Item1.Item1.Item1.Item2, val.Item1.Item1.Item2, val.Item1.Item2, val.Item2),
                () => internalReactive.Dispose());

            return internalReactive;
        }

        /// <summary>
        /// Для (T1,T2,T3,T4,T5)
        /// </summary>
        public static IStream<U> Func<T1, T2, T3, T4, T5, U>(
            this IStream<(T1, T2, T3, T4, T5)> stream,
            ICollection<IDisposable> disposables,
            Func<T1, T2, T3, T4, T5, U> func,
            bool reactOnChangesOnly = true)
        {
            var internalReactive = PooledReactiveProperty<U>.Create(reactOnChangesOnly);
            disposables.Add(internalReactive);

            stream.Subscribe(
                disposables,
                val => internalReactive.Value = func(val.Item1, val.Item2, val.Item3, val.Item4, val.Item5),
                () => internalReactive.Dispose());

            return internalReactive;
        }

        public static IStream<(T prev, T current)> PrevAndCurrent<T>(
            this IStream<T> stream,
            ICollection<IDisposable> disposables,
            bool reactOnChangesOnly = true)
        {
            var curr = PooledReactiveProperty<T>.Create(reactOnChangesOnly);
            var prev = PooledReactiveProperty<T>.Create(reactOnChangesOnly);
            var output = PooledReactiveProperty<(T, T)>.Create(reactOnChangesOnly);

            stream.Subscribe(
                disposables,
                val =>
                {
                    if (curr.HasValue)
                        prev.Value = curr.Value;

                    curr.Value = val;

                    if (prev.HasValue)
                        output.Value = (prev.Value, curr.Value);
                },
                () =>
                {
                    curr.Dispose();
                    prev.Dispose();
                    output.Dispose();
                });


            return output;
        }

        public static IStream<(T prev, T current)> PrevAndCurrentWithDefault<T>(
            this IStream<T> stream,
            ICollection<IDisposable> disposables,
            T defaultValue = default,
            bool reactOnChangesOnly = true)
        {
            var output = new ReactiveProperty<(T, T)>(reactOnChangesOnly);

            T prev;
            var curr = defaultValue;

            stream.Subscribe(
                disposables,
                val =>
                {
                    prev = curr;
                    curr = val;
                    output.Value = (prev, curr);
                },
                () => { output.Dispose(); });


            return output;
        }

        public static IStream<T> WhereProp<T>(this IStream<T> stream, ICollection<IDisposable> disposables, Func<T, bool> predicate)
        {
            var proxy = new ReactiveProperty<T>(false);
            disposables.Add(proxy);
            stream.Subscribe(
                disposables,
                value =>
                {
                    if (predicate(value))
                        proxy.Value = value;
                },
                proxy.Dispose);
            return proxy;
        }

        public static IStream<T> Where<T>(this IStream<T> stream, ICollection<IDisposable> disposables, Func<T, bool> predicate)
        {
            var trace = Tools.CreateStackTrace(1, true);
            var proxy = PooledStreamProxy<T>.Create(
                createLog: prefix =>
                    $"{prefix}IStream<{typeof(T).NiceName()}>.Where({typeof(T).NiceName()} => bool ({Tools.StackFilePosition(trace)})) // input stream:{stream}\n{stream.DeepLog(prefix + "\t")}"
            );
            disposables.Add(proxy);
            stream.Subscribe(
                disposables,
                value =>
                {
                    if (predicate(value))
                        proxy.OnNext(value);
                },
                () => { proxy.Dispose(); });
            return proxy;
        }

        public static IStream<(T1, T2)> Where<T1, T2>(
            this IStream<(T1, T2)> stream,
            ICollection<IDisposable> disposables,
            Func<T1, T2, bool> predicate)
        {
            var trace = Tools.CreateStackTrace(1, true);
            var proxy = PooledStreamProxy<(T1, T2)>.Create(
                createLog: prefix =>
                    $"{prefix}IStream<({typeof(T1).NiceName()},{typeof(T2).NiceName()})>.Where(({typeof(T1).NiceName()},{typeof(T2).NiceName()}) => bool ({Tools.StackFilePosition(trace)})) // input stream:{stream}\n{stream.DeepLog(prefix + "\t")}"
            );
            disposables.Add(proxy);
            stream.Subscribe(
                disposables,
                value =>
                {
                    if (predicate(value.Item1, value.Item2))
                        proxy.OnNext(value);
                },
                () => { proxy.Dispose(); });
            return proxy;
        }

        public static IStream<((T1, T2), T3)> Where<T1, T2, T3>(
            this IStream<((T1, T2), T3)> stream,
            ICollection<IDisposable> disposables,
            Func<T1, T2, T3, bool> predicate)
        {
            var trace = Tools.CreateStackTrace(1, true);
            var proxy = PooledStreamProxy<((T1, T2), T3)>.Create(
                createLog: prefix =>
                    $"{prefix}IStream<(({typeof(T1).NiceName()},{typeof(T2).NiceName()}),{typeof(T3).NiceName()})>.Where(({typeof(T1).NiceName()},{typeof(T2).NiceName()},{typeof(T3).NiceName()}) => bool ({Tools.StackFilePosition(trace)})) // input stream:{stream}\n{stream.DeepLog(prefix + "\t")}"
            );
            disposables.Add(proxy);
            stream.Subscribe(
                disposables,
                value =>
                {
                    if (predicate(value.Item1.Item1, value.Item1.Item2, value.Item2))
                        proxy.OnNext(value);
                },
                () => { proxy.Dispose(); });
            return proxy;
        }

        public static IStream<(((T1, T2), T3), T4)> Where<T1, T2, T3, T4>(
            this IStream<(((T1, T2), T3), T4)> stream,
            ICollection<IDisposable> disposables,
            Func<T1, T2, T3, T4, bool> predicate)
        {
            var trace = Tools.CreateStackTrace(1, true);
            var proxy = PooledStreamProxy<(((T1, T2), T3), T4)>.Create(
                createLog: prefix =>
                    $"{prefix}IStream<((({typeof(T1).NiceName()},{typeof(T2).NiceName()}),{typeof(T3).NiceName()}),{typeof(T4).NiceName()})>.Where(({typeof(T1).NiceName()},{typeof(T2).NiceName()},{typeof(T3).NiceName()},{typeof(T4).NiceName()}) => bool ({Tools.StackFilePosition(trace)})) // input stream:{stream}\n{stream.DeepLog(prefix + "\t")}"
            );
            disposables.Add(proxy);
            stream.Subscribe(
                disposables,
                value =>
                {
                    if (predicate(value.Item1.Item1.Item1, value.Item1.Item1.Item2, value.Item1.Item2, value.Item2))
                        proxy.OnNext(value);
                },
                () => { proxy.Dispose(); });
            return proxy;
        }

        public static IStream<((((T1, T2), T3), T4), T5)> Where<T1, T2, T3, T4, T5>(
            this IStream<((((T1, T2), T3), T4), T5)> stream,
            ICollection<IDisposable> disposables,
            Func<T1, T2, T3, T4, T5, bool> predicate)
        {
            var trace = Tools.CreateStackTrace(1, true);
            var proxy = PooledStreamProxy<((((T1, T2), T3), T4), T5)>.Create(
                createLog: prefix =>
                    $"{prefix}IStream<(((({typeof(T1).NiceName()},{typeof(T2).NiceName()}),{typeof(T3).NiceName()}),{typeof(T4).NiceName()}),{typeof(T5).NiceName()})>.Where(({typeof(T1).NiceName()},{typeof(T2).NiceName()},{typeof(T3).NiceName()},{typeof(T4).NiceName()},{typeof(T5).NiceName()}) => bool ({Tools.StackFilePosition(trace)})) // input stream:{stream}\n{stream.DeepLog(prefix + "\t")}"
            );
            disposables.Add(proxy);
            stream.Subscribe(
                disposables,
                value =>
                {
                    if (predicate(
                        value.Item1.Item1.Item1.Item1,
                        value.Item1.Item1.Item1.Item2,
                        value.Item1.Item1.Item2,
                        value.Item1.Item2,
                        value.Item2))
                        proxy.OnNext(value);
                },
                () => { proxy.Dispose(); });
            return proxy;
        }



        public static IStream<T> Merge<T>(this IStream<T> stream1, ICollection<IDisposable> disposables, IStream<T> stream2)
        {
            var output = PooledStreamProxy<T>.Create();
            bool disposed1 = false, disposed2 = false;
            disposables.Add(output);
            stream1.Subscribe(
                disposables,
                t => output.OnNext(t),
                () =>
                {
                    disposed1 = true;
                    if (disposed1 && disposed2)
                        output.Dispose();
                });
            stream2.Subscribe(
                disposables,
                t => output.OnNext(t),
                () =>
                {
                    disposed2 = true;
                    if (disposed1 && disposed2)
                        output.Dispose();
                });
            return output;
        }

        public static IStream<T> First<T>(this IStream<T> stream, ICollection<IDisposable> disposables)
        {
            var output = PooledStreamProxy<T>.Create();
            disposables.Add(output);
            stream.Subscribe(
                disposables,
                x =>
                {
                    output.OnNext(x);
                    output.Dispose();
                },
                () => output.Dispose());
            return output;
        }

        public static IStream<T> Skip<T>(this IStream<T> stream, ICollection<IDisposable> disposables, int count)
        {
            var output = PooledStreamProxy<T>.Create();
            disposables.Add(output);
            stream.Subscribe(
                disposables,
                x =>
                {
                    if (count > 0)
                        --count;
                    else
                        output.OnNext(x);
                },
                () => output.Dispose());
            return output;
        }
    }
}