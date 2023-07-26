using System;
using System.Collections.Generic;
using ColonyShared.SharedCode.Utils;

namespace ReactivePropsNs.ThreadSafe
{
    public static class FuncListener
    {
        public static IStream<(T1,T2)> Zip<T1, T2>(this IStream<T1> stream1, IDisposableCollection disposables, IStream<T2> stream2)
        {
            return ZipFunc(stream1, disposables, stream2, (a, b) => (a, b));
        }

        public static IStream<U> ZipFunc<T1, T2, U>(this IStream<T1> stream1, IDisposableCollection disposables, IStream<T2> stream2, Func<T1, T2, U> func, bool useDefaultValue = false)
        {
            return ZipFunc(stream1, disposables, stream2, func, useDefaultValue, null, null);
        }

        public static IStream<U> ZipFunc<T1, T2, U>(this IStream<T1> stream1, IDisposableCollection disposables, IStream<T2> stream2, Func<T1, T2, U> func, bool useDefaultValue, IEqualityComparer<T1> comparer1, IEqualityComparer<T2> comparer2)
        {
            comparer1 = comparer1 ?? EqualityComparer<T1>.Default;
            comparer2 = comparer2 ?? EqualityComparer<T2>.Default;
            var comparerOut = EqualityComparerFactory.FalseComparer<U>();

            T1 value1 = default; T2 value2 = default;
            bool has1 = useDefaultValue, has2 = useDefaultValue;
            var @lock = SpinLockExt.Create();

            //var output = useDefaultValue ? new ReactiveProperty<U>(default, comparerOut) : new ReactiveProperty<U>(comparerOut);
            var output = new StreamProxy<U>();
            disposables.Add(output);
            void Dispose() => output.Dispose();

            stream1.Subscribe(disposables,
                v1 =>
                {
                    T2 v2;
                    bool send;
                    bool locked = false;
                    try
                    {
                        @lock.EnterWithTimeout(ref locked);
                        has1 = true;
                        v2 = value2;
                        send = has1 && has2 && !comparer1.Equals(value1, v1);
                        value1 = v1;
                    }
                    finally
                    {
                        if (locked) @lock.Exit();
                    }
                    if (send)
                        //output.Value = func(v1, v2);
                        output.OnNext(func(v1, v2));
                }, Dispose);

            stream2.Subscribe(disposables,
                v2 =>
                {
                    T1 v1;
                    bool send;
                    bool locked = false;
                    try
                    {
                        @lock.EnterWithTimeout(ref locked);
                        has2 = true;
                        v1 = value1;
                        send = has1 && has2 && !comparer2.Equals(value2, v2);
                        value2 = v2;
                    }
                    finally
                    {
                        if (locked) @lock.Exit();
                    }
                    if (send)
                        //output.Value = func(v1, v2);
                        output.OnNext(func(v1, v2));

                }, Dispose);

            return output;
        }
        

        public static IStream<U> ZipFunc<T, U>(this IStream<T> stream1, IDisposableCollection disposables, IStream<T> stream2, Func<T, T, U> func, bool useDefaultValue, IEqualityComparer<T> comparer)
        {
            return ZipFunc(stream1, disposables, stream2, func, useDefaultValue, comparer, comparer);
        }

        
            public static IStream<T> Where<T>(this IStream<T> stream, IDisposableCollection disposables, Func<T, bool> predicate)
        {
            var proxy = new StreamProxy<T>();
            disposables.Add(proxy);
            stream.Subscribe(disposables, value =>
                {
                    if (predicate(value))
                        proxy.OnNext(value);
                },
                () => { proxy.Dispose(); });
            return proxy;
        }

        public static IStream<(T1, T2)> Where<T1, T2>(this IStream<(T1, T2)> stream, IDisposableCollection disposables, Func<T1, T2, bool> predicate)
        {
            var proxy = new StreamProxy<(T1, T2)>();
            disposables.Add(proxy);
            stream.Subscribe(disposables, value =>
                {
                    if (predicate(value.Item1, value.Item2))
                        proxy.OnNext(value);
                },
                () => { proxy.Dispose(); });
            return proxy;
        }

        public static IStream<((T1, T2), T3)> Where<T1, T2, T3>(this IStream<((T1, T2), T3)> stream, IDisposableCollection disposables,
            Func<T1, T2, T3, bool> predicate)
        {
            var proxy = new StreamProxy<((T1, T2), T3)>();
            disposables.Add(proxy);
            stream.Subscribe(disposables, value =>
                {
                    if (predicate(value.Item1.Item1, value.Item1.Item2, value.Item2))
                        proxy.OnNext(value);
                },
                () => { proxy.Dispose(); });
            return proxy;
        }

        public static IStream<(((T1, T2), T3), T4)> Where<T1, T2, T3, T4>(this IStream<(((T1, T2), T3), T4)> stream, IDisposableCollection disposables,
            Func<T1, T2, T3, T4, bool> predicate)
        {
            var proxy = new StreamProxy<(((T1, T2), T3), T4)>();
            disposables.Add(proxy);
            stream.Subscribe(disposables, value =>
                {
                    if (predicate(value.Item1.Item1.Item1, value.Item1.Item1.Item2, value.Item1.Item2, value.Item2))
                        proxy.OnNext(value);
                },
                () => { proxy.Dispose(); });
            return proxy;
        }

        public static IStream<((((T1, T2), T3), T4), T5)> Where<T1, T2, T3, T4, T5>(this IStream<((((T1, T2), T3), T4), T5)> stream,
            IDisposableCollection disposables, Func<T1, T2, T3, T4, T5, bool> predicate)
        {
            var proxy = new StreamProxy<((((T1, T2), T3), T4), T5)>();
            disposables.Add(proxy);
            stream.Subscribe(disposables, value =>
                {
                    if (predicate(value.Item1.Item1.Item1.Item1, value.Item1.Item1.Item1.Item2, value.Item1.Item1.Item2, value.Item1.Item2, value.Item2))
                        proxy.OnNext(value);
                },
                () => { proxy.Dispose(); });
            return proxy;
        }
        
        public static IStream<U> Func<T, U>(this IStream<T> stream, IDisposableCollection disposables, Func<T, U> func, IEqualityComparer<U> equalityComparer = null)
        {
            var internalReactive = new ReactiveProperty<U>(equalityComparer);
            disposables.Add(internalReactive);
            disposables.Add(stream.Subscribe(disposables, next => internalReactive.Value = func(next), () => internalReactive.Dispose()));
            return internalReactive;
        }

        public static IStream<U> Func<T1, T2, U>(this IStream<(T1, T2)> stream, IDisposableCollection disposables, Func<T1, T2, U> func, IEqualityComparer<U> equalityComparer = null)
        {
            var internalReactive = new ReactiveProperty<U>(equalityComparer);
            disposables.Add(internalReactive);
            stream.Subscribe(disposables, val => internalReactive.Value = func(val.Item1, val.Item2), () => internalReactive.Dispose());
            return internalReactive;
        }
        
        public static IStream<T> Merge<T>(this IStream<T> stream1, IDisposableCollection disposables, IStream<T> stream2)
        {
            var output = new StreamProxy<T>();
            bool disposed1 = false, disposed2 = false;
            disposables.Add(output);
            stream1.Subscribe(disposables, t => output.OnNext(t), () =>
            {
                disposed1 = true;
                if(disposed1 && disposed2)
                    output.Dispose();
            });
            stream2.Subscribe(disposables, t => output.OnNext(t), () =>
            {
                disposed2 = true;
                if(disposed1 && disposed2)
                    output.Dispose();
            });
            return output;
        }

        public static IStream<T> First<T>(this IStream<T> stream, IDisposableCollection disposables)
        {
            var output = new StreamProxy<T>();
            disposables.Add(output);
            stream.Subscribe(disposables, x =>
            {
                output.OnNext(x);
                output.Dispose();
            }, () => output.Dispose());
            return output;
        }
    }
}