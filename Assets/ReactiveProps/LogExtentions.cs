using System;
using System.Collections.Generic;
using Core.Environment.Logging.Extension;
using NLog;

namespace ReactivePropsNs
{
    public static class LogExtentions
    {
        public static IStream<T> Log<T>(
            this IStream<T> source,
            ICollection<IDisposable> disposables,
            string prefix,
            Func<T, string> toString = null,
            bool isInfo = false,
            Logger logger = null)
        {
            if (logger == null) 
                logger = ReactiveLogs.Logger;

            var output = PooledReactiveProperty<T>.Create(reactOnChangesOnly: false);
            disposables.Add(output);
            var subscribtion = source.Subscribe(
                disposables,
                value =>
                {
                    IfLevel(logger, isInfo)?.Message($"{prefix} {(toString != null ? toString(value) : value?.ToString())}\n<_{source}_>").Write();
                    output.Value = value;
                },
                () =>
                {
                    IfLevel(logger, isInfo)?.Message($"{prefix} OnCompleted()").Write();
                });
            disposables.Add(subscribtion);
            return output;
        }

        public static IStream<ValueTuple<T1, T2>> Log<T1, T2>(
            this IStream<ValueTuple<T1, T2>> source,
            ICollection<IDisposable> disposables,
            string prefix,
            Func<T1, T2, string> toString = null,
            bool isInfo = false,
            Logger logger = null)
        {
            return source.Log(disposables, prefix, val => toString != null ? toString(val.Item1, val.Item2) : null, isInfo, logger);
        }


        public static IListStream<T> Log<T>(
            this IListStream<T> list,
            ICollection<IDisposable> disposables,
            string prefix,
            Func<T, string> toString = null,
            bool isInfo = false,
            Logger logger = null)
        {
            if (logger == null) 
                logger = ReactiveLogs.Logger;

            var outList = new ListStream<T>();
            disposables.Add(outList);

            string EventToString(int index, T item, T oldItem = default)
            {
                return $"[{index}]={ItemToString(item)}{(Equals(oldItem, default) ? "" : $" <-- {ItemToString(oldItem)}")}\n<_{list}_>";
            }

            string ItemToString(T item)
            {
                return toString != null ? toString(item) : item?.ToString();
            }

            string OnCompleteToString()
            {
                return $"OnCompleted()\n<_{list}_>";
            }

            disposables.Add(
                list.InsertStream.Subscribe(
                    disposables,
                    ie =>
                    {
                        IfLevel(logger, isInfo)?.Message($"{prefix} {nameof(list.InsertStream)} {EventToString(ie.Index, ie.Item)}").Write();
                        outList.Insert(ie.Index, ie.Item);
                    },
                    () =>
                    {
                        IfLevel(logger, isInfo)?.Message($"{prefix} {list.InsertStream} {OnCompleteToString()}").Write();
                        outList.Dispose();
                    }));
            disposables.Add(
                list.RemoveStream.Subscribe(
                    disposables,
                    re =>
                    {
                        IfLevel(logger, isInfo)?.Message($"{prefix} {nameof(list.RemoveStream)} {EventToString(re.Index, re.Item)}").Write();
                        outList.RemoveAt(re.Index);
                    },
                    () =>
                    {
                        IfLevel(logger, isInfo)?.Message($"{prefix} {list.RemoveStream} {OnCompleteToString()}").Write();
                        outList.Dispose();
                    }));
            disposables.Add(
                list.ChangeStream.Subscribe(
                    disposables,
                    ce =>
                    {
                        IfLevel(logger, isInfo)?.Message($"{prefix} {nameof(list.ChangeStream)} {EventToString(ce.Index, ce.NewItem, ce.OldItem)}").Write();
                        outList[ce.Index] = ce.NewItem;
                    },
                    () =>
                    {
                        outList.Dispose();
                        IfLevel(logger, isInfo)?.Message($"{prefix} {list.ChangeStream} {OnCompleteToString()}").Write();
                    }));
            return outList;
        }

        public static IDictionaryStream<TKey, TValue> Log<TKey, TValue>(
            this IDictionaryStream<TKey, TValue> dict,
            ICollection<IDisposable> disposables,
            string prefix,
            Func<TValue, string> toString = null,
            bool isInfo = false,
            Logger logger = null)
        {
            if (logger == null) 
                logger = ReactiveLogs.Logger;

            var outDict = new DictionaryStream<TKey, TValue>();
            disposables.Add(outDict);

            string EventToString(TKey key, TValue item, TValue oldItem = default)
            {
                return $"[{key}]={ItemToString(item)}{(Equals(oldItem, default) ? "" : $" <-- {ItemToString(oldItem)}")}\n<_{dict}_>";
            }

            string ItemToString(TValue item)
            {
                return toString != null ? toString(item) : item?.ToString();
            }

            string OnCompleteToString()
            {
                return $"OnCompleted()\n<_{dict}_>";
            }

            disposables.Add(
                dict.AddStream.Subscribe(
                    disposables,
                    ae =>
                    {
                        IfLevel(logger, isInfo)?.Message($"{prefix} {nameof(dict.AddStream)} {EventToString(ae.Key, ae.Value)}").Write();
                        outDict.Add(ae.Key, ae.Value);
                    },
                    () =>
                    {
                        outDict.Dispose();
                        IfLevel(logger, isInfo)?.Message($"{prefix} {dict.AddStream} {OnCompleteToString()}").Write();
                    }));
            disposables.Add(
                dict.RemoveStream.Subscribe(
                    disposables,
                    re =>
                    {
                        IfLevel(logger, isInfo)?.Message($"{prefix} {nameof(dict.RemoveStream)} {EventToString(re.Key, re.Value)}").Write();
                        outDict.Remove(re.Key);
                    },
                    () =>
                    {
                        outDict.Dispose();
                        IfLevel(logger, isInfo)?.Message($"{prefix} {dict.RemoveStream} {OnCompleteToString()}").Write();
                    }));
            disposables.Add(
                dict.ChangeStream.Subscribe(
                    disposables,
                    ce =>
                    {
                        IfLevel(logger, isInfo)?.Message($"{prefix} {nameof(dict.ChangeStream)} {EventToString(ce.Key, ce.NewValue, ce.OldValue)}").Write();
                        outDict[ce.Key] = ce.NewValue;
                    },
                    () =>
                    {
                        outDict.Dispose();
                        IfLevel(logger, isInfo)?.Message($"{prefix} {dict.ChangeStream} {OnCompleteToString()}").Write();
                    }));
            return outDict;
        }

        private static Level? IfLevel(Logger logger, bool isInfoNotDebug)
        {
            return isInfoNotDebug ? logger.IfInfo() : logger.IfDebug();
        }
    }
}