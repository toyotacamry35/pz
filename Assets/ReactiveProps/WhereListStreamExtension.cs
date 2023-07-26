using System;
using System.Collections.Generic;
using System.Linq;
using ReactivePropsNs;

namespace Assets.ReactiveProps
{
    public static class WhereListStreamExtension
    {
        public static IListStream<T> Where<T>(
            this IListStream<T> stream,
            ICollection<IDisposable> externalD,
            IStream<Predicate<T>> predicateStream)
        {
            var localD = externalD.CreateInnerD();

            var output = new ListStream<T>();
            localD.Add(output);

            var cache = new List<T>();
            localD.Add(new DisposeAgent(cache.Clear));

            var predicateValue = default(Predicate<T>);
            var predicateReady = false;

            predicateStream.Subscribe(
                externalD,
                func =>
                {
                    predicateValue = func;
                    predicateReady = true;

                    output.Clear();
                    foreach (var item in cache.Where(t => predicateValue(t)))
                        output.Add(item);
                },
                () => externalD.DisposeInnerD(localD));

            stream.InsertStream.Subscribe(
                externalD,
                insertEvent =>
                {
                    cache[insertEvent.Index] = insertEvent.Item;
                    if (predicateReady && predicateValue(insertEvent.Item))
                        output.Add(insertEvent.Item);
                },
                () => externalD.DisposeInnerD(localD));
            stream.ChangeStream.Action(
                externalD,
                changeEvent =>
                {
                    cache[changeEvent.Index] = changeEvent.NewItem;
                    if (predicateReady)
                    {
                        var cacheCount = cache.Count;
                        var outputIndex = 0;
                        var outputMax = cacheCount;
                        var outputCount = output.Count;
                        for (var i = 0; i < cacheCount; i++)
                            if (predicateValue(cache[i]))
                                output[outputIndex++] = cache[i];
                            else if (outputCount > --outputMax)
                                output.RemoveAt(--outputCount);
                    }
                });
            stream.RemoveStream.Action(
                externalD,
                removeEvent =>
                {
                    cache.RemoveAt(removeEvent.Index);
                    if (predicateReady)
                        output.Remove(removeEvent.Item);
                });


            return output;
        }

        public static ListStream<T> Where<T>(
            this IListStream<T> stream,
            ICollection<IDisposable> externalD,
            Predicate<T> predicateValue)
        {
            var localD = externalD.CreateInnerD();

            var produced = new List<int>();
            localD.Add(new DisposeAgent(produced.Clear));

            var output = new ListStream<T>();
            localD.Add(output);

            stream.InsertStream.Subscribe(
                localD,
                insertEvent =>
                {
                    var index = insertEvent.Index;
                    var item = insertEvent.Item;

                    if (predicateValue(item))
                    {
                        var nearest = output.Count;
                        for (var i = produced.Count - 1; i >= index; i--)
                            if (produced[i] > 0)
                            {
                                nearest = produced[i];
                                produced[i]++;
                            }

                        produced.Insert(index, nearest);
                        output.Insert(nearest, item);
                    }
                    else
                    {
                        produced.Insert(index, -1);
                    }
                },
                () => externalD.DisposeInnerD(localD)
            );
            stream.RemoveStream.Action(
                localD,
                removeEvent =>
                {
                    var removedIndex = removeEvent.Index;
                    var index = produced[removedIndex];
                    produced.RemoveAt(removedIndex);
                    if (index >= 0)
                    {
                        for (var i = produced.Count - 1; i >= removedIndex; i--)
                            if (produced[i] > 0)
                                produced[i]--;
                        output.RemoveAt(index);
                    }
                }
            );

            stream.ChangeStream.Action(
                localD,
                changeEvent =>
                {
                    var changedIndex = changeEvent.Index;
                    var item = changeEvent.NewItem;

                    var index = produced[changedIndex];
                    if (predicateValue(item))
                    {
                        if (index < 0)
                        {
                            var nearest = output.Count;
                            for (var i = produced.Count - 1; i > changedIndex; i--)
                                if (produced[i] > 0)
                                {
                                    nearest = produced[i];
                                    produced[i]++;
                                }

                            produced[changedIndex] = nearest;
                            output.Insert(nearest, item);
                        }
                        else
                            output[index] = item;
                    }
                    else
                    {
                        if (index >= 0)
                        {
                            produced[changedIndex] = -1;
                            for (var i = produced.Count - 1; i > changedIndex; i--)
                                if (produced[i] > 0)
                                    produced[i]--;
                            output.RemoveAt(index);
                        }
                    }
                }
            );

            return output;
        }
    }
}