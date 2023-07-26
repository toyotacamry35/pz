using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;

namespace Assets.Src.Tools
{
    public static class DictionaryGetOrCreateExtension
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public static TV GetOrCreate<TK, TV>(this ConcurrentDictionary<TK, TV> dictionary, TK key) where TV : new()
        {
            return dictionary.GetOrAdd(key, (k)=> new TV());
        }
        public static TV GetOrCreate<TK, TV>(this ConcurrentDictionary<TK, TV> dictionary, TK key, Func<TV> create)
        {
            return dictionary.GetOrAdd(key, (k) => create());
        }

        public static TV GetOrCreate<TK, TV>(this IDictionary<TK, TV> dictionary, TK key) where TV : new()
        {
            TV value;
            if (dictionary.TryGetValue(key, out value))
                return value;
            value = dictionary[key] = new TV();
            return value;
        }

        public static bool GetOrCreate<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, out TV value, int maxCount) where TV : new()
        {
            if (dictionary.TryGetValue(key, out value))
                return true;

            if (dictionary.Count >= maxCount)
            {
                Logger.IfError()?.Message("MaxCount reached. Can't add item.").Write();
                return false;
            }

            value = dictionary[key] = new TV();
            return true;
        }

        //        public static TV GetOrCreate<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, Func<TK, TV> ctor)
        //        {
        //            TV value;
        //            if (dictionary.TryGetValue(key, out value))
        //                return value;
        //            value = dictionary[key] = ctor(key);
        //            return value;
        //        }

        public static TV2 GetOrCreate<TK, TV, TK2, TV2>(this IDictionary<TK, TV> dictionary, TK2 key, Func<TK2, TV2> ctor) where TK2 : TK where TV2 : TV
        {
            TV basevalue;
            if (dictionary.TryGetValue(key, out basevalue))
                return (TV2)basevalue;
            TV2 value = ctor(key);
            dictionary[key] = value;
            return value;
        }

        public static void GetOrCreateAndChange<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, Func<TV, TV> newValueFactory) where TV : new()
        {
            TV value;
            if (!dictionary.TryGetValue(key, out value))
                value = dictionary[key] = new TV();
            dictionary[key] = newValueFactory(value);
        }

        public static TV GetOrDefault<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, TV @default = default(TV))
        {
            return dictionary.TryGetValue(key, out var value) ? value : @default;
        }
        
        public static TV GetOrDefault<TK, TV>(this IReadOnlyDictionary<TK, TV> dictionary, TK key, TV @default = default(TV))
        {
            return dictionary.TryGetValue(key, out var value) ? value : @default;
        }
    }


    public static class QueueTryDequeueInLock
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public static bool TryDequeueInLock<T>(this Queue<T> queue, out T val)
        {
            val = default(T);

            if (queue == null || queue.Count == 0)
                return false;

            try
            {
                lock (queue)
                    val = queue.Dequeue();
            }
            catch (InvalidOperationException)
            {
                if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("Queue is empty.").Write();;
                return false;
            }

            return true;
        }
    }

    public static class ListTryDequeueInLock
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public static bool TryDequeueInLock<T>(this List<T> list, out T val)
        {
            val = default(T);

            if (list == null || list.Count == 0)
                return false;

            try
            {
                lock (list)
                {
                    val = list[0];
                    list.RemoveAt(0);
                }
            }
            catch (IndexOutOfRangeException)
            {
                if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("List is empty.").Write();;
                return false;
            }

            return true;
        }
    }

}
