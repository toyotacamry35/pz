using System;
using System.Collections.Generic;

namespace ReactivePropsNs
{
    public static class HashSetStreamExtensions
    {
        public static IHashSetStream<T> Where<T>(
            this IHashSetStream<T> stream,
            ICollection<IDisposable> disposables,
            Func<T, bool> predicate)
        {
            var output = new HashSetStream<T>();

            void OnCompletedAction() => output.Dispose();

            stream.AddStream.Subscribe(
                disposables,
                value =>
                {
                    if (predicate(value))
                        output.Add(value);
                },
                OnCompletedAction);
            stream.RemoveStream.Subscribe(disposables, value => output.Remove(value), OnCompletedAction);

            return output;
        }

        public static IHashSetStream<T> Where<T, TPredicateParam>(
            this IHashSetStream<T> stream,
            ICollection<IDisposable> disposables,
            IStream<TPredicateParam> predicateParam,
            Func<T, TPredicateParam, bool> predicate)
        {
            var cachedHash = new HashSet<T>();
            var predicateParamValue = default(TPredicateParam);
            var predicateParamHasValue = false;

            var output = new HashSetStream<T>();

            void OnCompletedAction()
            {
                cachedHash.Clear();
                output.Dispose();
            }

            disposables.Add(
                predicateParam.Subscribe(
                    disposables,
                    param =>
                    {
                        predicateParamValue = param;
                        predicateParamHasValue = true;

                        foreach (var value in cachedHash)
                            if (predicate(value, predicateParamValue))
                                output.Add(value);
                            else
                                output.Remove(value);
                    },
                    OnCompletedAction
                ));

            disposables.Add(
                stream.AddStream.Subscribe(
                    disposables,
                    value =>
                    {
                        if (cachedHash.Add(value))
                            if (predicateParamHasValue && predicate(value, predicateParamValue))
                                output.Add(value);
                    },
                    OnCompletedAction));

            disposables.Add(
                stream.RemoveStream.Subscribe(
                    disposables,
                    value =>
                    {
                        if (cachedHash.Remove(value))
                            output.Remove(value);
                    },
                    OnCompletedAction));

            return output;
        }

        public static HashSetStream<TOutKey> FuncMutable<TKey, TOutKey>(
            this IHashSetStream<TKey> source,
            ICollection<IDisposable> externalD,
            IStream<Func<TKey, ICollection<IDisposable>, (bool result, TOutKey)>> factoryStream)
        {
            var localD = externalD.CreateInnerD();
            var output = new HashSetStream<TOutKey>();
            localD.Add(output);
            
            var produced = new Dictionary<TKey, (bool result, IDisposable, TOutKey)>();
            localD.Add(new DisposeAgent(produced.Clear));

            var currentD = localD.CreateInnerD();

            factoryStream.Subscribe(
                localD,
                factory =>
                {
                    localD.DisposeInnerD(currentD);
                    currentD = localD.CreateInnerD();
                    produced.Clear();

                    source.AddStream.Action(currentD, Create);
                    source.RemoveStream.Subscribe(currentD, Destroy, output.Clear);

                    void Create(TKey inKey)
                    {
                        var innerD = currentD.CreateInnerD();
                        var (result, key) = factory.Invoke(inKey, innerD);
                        produced[inKey] = (result, innerD, key);
                        if (result)
                            output.Add(key);
                    }

                    void Destroy(TKey inKey)
                    {
                        if (produced.TryGetValue(inKey, out var tuple))
                        {
                            var (result, innerD, key) = tuple;
                            if (result) 
                                output.Remove(key);
                            currentD.DisposeInnerD(innerD);
                        }
                    }
                },
                () => externalD.DisposeInnerD(localD));

            return output;
        }

        public static HashSetStream<TOutKey> FuncMutable<TKey, TOutKey>(
            this IHashSetStream<TKey> source,
            ICollection<IDisposable> externalD,
            IStream<Func<TKey, ICollection<IDisposable>, TOutKey>> factoryStream)
        {
            var generalFactory = factoryStream
                .Func<
                    Func<TKey, ICollection<IDisposable>, TOutKey>,
                    Func<TKey, ICollection<IDisposable>, (bool result, TOutKey)>
                >(
                    externalD,
                    func =>
                    {
                        return (tKey, localD) =>
                        {
                            var tOutKey = func.Invoke(tKey, localD);
                            return (true, tOutKey);
                        };
                    }
                );
            return FuncMutable(source, externalD, generalFactory);
        }
    }
}