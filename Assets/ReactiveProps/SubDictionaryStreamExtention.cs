using System;
using System.Collections.Generic;

namespace ReactivePropsNs
{
    public static class SubDictionaryStreamExtention
    {
        public static DictionaryStream<TKey, TValue> SubDictionaryStream<T, TKey, TValue>(this IStream<T> source, ICollection<IDisposable> disposables,
            Func<T, IDictionaryStream<TKey, TValue>> dictStreamFunc, bool reactOnChangesOnly = false)
        {
            var innerD = new DisposableComposite();
            disposables.Add(innerD);
            var outDictStream = new DictionaryStream<TKey, TValue>(reactOnChangesOnly);
            innerD.Add(outDictStream);
            var tempoD = new DisposableComposite();

            IDictionaryStream<TKey, TValue> connected = default;

            void Connect(IDictionaryStream<TKey, TValue> dict)
            {
                if (connected == dict)
                    return;

                if (connected != null)
                    Disconnect();

                if (dict == null)
                    return;

                connected = dict;
                tempoD.Add(dict.AddStream.Subscribe(tempoD, insertEvent => outDictStream.Add(insertEvent.Key, insertEvent.Value), Disconnect));
                tempoD.Add(dict.RemoveStream.Subscribe(tempoD, removeEvent => outDictStream.Remove(removeEvent.Key), Disconnect));
                tempoD.Add(dict.ChangeStream.Subscribe(tempoD, changeEvent => outDictStream[changeEvent.Key] = changeEvent.NewValue, Disconnect));
            }

            void Disconnect()
            {
                if (connected != null)
                {
                    tempoD.Clear();
                    outDictStream.Clear();
                    connected = null;
                }
            }

            innerD.Add(source.Subscribe(innerD,
                tValue =>
                {
                    if (tValue != null)
                        Connect(dictStreamFunc(tValue));
                    else
                        Disconnect();
                },
                () =>
                {
                    Disconnect();
                    innerD.Dispose();
                }
            ));

            return outDictStream;
        }
    }
}