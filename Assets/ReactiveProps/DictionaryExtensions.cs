using System;
using System.Collections.Generic;
using Core.Environment.Logging.Extension;

namespace ReactivePropsNs
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Преобразование типа элементов: из DictionaryStream(TKey, TVal) в DictionaryStream(TKey, UVal) через func
        /// </summary>
        public static DictionaryStream<TKey, UVal> Func<TKey, TVal, UVal>(
            this DictionaryStream<TKey, TVal> dct,
            ICollection<IDisposable> disposables,
            Func<TVal, UVal> func)
        {
            var innerD = new DisposableComposite();
            if (dct == null)
            {
                var completedStream = new DictionaryStream<TKey, UVal>();
                completedStream.Dispose();
                return completedStream;
            }

            var outStrm = new DictionaryStream<TKey, UVal>();
            innerD.Add(outStrm);

            innerD.Add(
                dct.AddStream.Subscribe(
                    innerD,
                    insertEvent => outStrm.Add(insertEvent.Key, func(insertEvent.Value)),
                    () => innerD.Clear()));
            innerD.Add(dct.RemoveStream.Subscribe(innerD, removeEvent => outStrm.Remove(removeEvent.Key), () => innerD.Clear()));
            innerD.Add(
                dct.ChangeStream.Subscribe(
                    innerD,
                    changeEvent => outStrm[changeEvent.Key] = func(changeEvent.NewValue),
                    () => innerD.Clear()));
            foreach (var disposable in innerD)
                disposables.Add(disposable);
            return outStrm;
        }

        /// <summary>
        /// Получение из интерфейса экземпляра ListStream. Внутренний метод
        /// </summary>
        public static DictionaryStream<TKey, TValue> ToDictionaryStream<TKey, TValue>(
            this IDictionaryStream<TKey, TValue> dictionary,
            ICollection<IDisposable> disposables,
            bool reactOnChangesOnly = true)
        {
            if (dictionary == null)
            {
                var completedStream = new DictionaryStream<TKey, TValue>();
                completedStream.Dispose();
                return completedStream;
            }

            DisposableComposite innerD = new DisposableComposite();
            var outDictionary = new DictionaryStream<TKey, TValue>(reactOnChangesOnly);
            innerD.Add(outDictionary);

            innerD.Add(
                dictionary.AddStream.Subscribe(innerD, addEvent => outDictionary.Add(addEvent.Key, addEvent.Value), () => innerD.Clear()));
            innerD.Add(
                dictionary.RemoveStream.Subscribe(innerD, removeEvent => outDictionary.Remove(removeEvent.Key), () => innerD.Clear()));
            innerD.Add(
                dictionary.ChangeStream.Subscribe(
                    innerD,
                    changeEvent => outDictionary[changeEvent.Key] = changeEvent.NewValue,
                    () => innerD.Clear()));

            foreach (var disposable in innerD)
                disposables.Add(disposable);

            return outDictionary;
        }

        public static ListStream<TValue> ToListStream<TKey, TValue>(
            this IDictionaryStream<TKey, TValue> dictionary,
            ICollection<IDisposable> externalD,
            bool reactOnChangesOnly = true)
        {
            return ToSortedListStream(dictionary, externalD, reactOnChangesOnly: reactOnChangesOnly);
        }

        public static ListStream<TValue> ToSortedListStream<TKey, TValue>(
            this IDictionaryStream<TKey, TValue> dictionary,
            ICollection<IDisposable> externalD,
            Func<TValue, int> sortFunc = null,
            bool reactOnChangesOnly = true)
        {
            if (dictionary == null)
            {
                var completedStream = new ListStream<TValue>();
                completedStream.Dispose();
                return completedStream;
            }

            var localD = externalD.CreateInnerD();
            var outListStream = new ListStream<TValue>(null, reactOnChangesOnly);
            localD.Add(outListStream);

            dictionary.AddStream.Action(
                localD,
                addEvent =>
                {
                    var item = addEvent.Value;
                    var index = GetIndexForItem(outListStream, item, sortFunc);
                    if (index >= 0)
                        outListStream.Insert(index, item);
                }
            );
            dictionary.RemoveStream.Action(
                localD,
                removeEvent => { outListStream.Remove(removeEvent.Value); }
            );
            dictionary.ChangeStream.Subscribe(
                localD,
                changeEvent =>
                {
                    outListStream.Remove(changeEvent.OldValue);
                    var newItem = changeEvent.NewValue;
                    var index = GetIndexForItem(outListStream, newItem, sortFunc);
                    if (index >= 0)
                        outListStream.Insert(index, newItem);
                },
                () => localD.Dispose()
            );

            return outListStream;
        }

        private static int GetIndexForItem<T>(IList<T> list, T item, Func<T, int> sortFunc)
        {
            if (list == null || item == null)
                return -1;

            //Нет функтора сортировки - ставим в конец
            if (sortFunc == null)
                return list.Count;

            var itemSortIndex = sortFunc(item);
            for (var i = list.Count - 1; i >= 0; i--)
                //по возрастанию с 0, если есть элементы с таким же индексом, то новый ставим после них
                if (itemSortIndex >= sortFunc(list[i]))
                    return i + 1;

            return 0;
        }

        /// <summary>
        /// Стрим изменений DictionaryStream, где в качестве параметра рассылается собственно dictionary
        /// </summary>
        [Obsolete]
        public static IStream<DictionaryStream<TKey, TValue>> DictionaryChanges<TKey, TValue>(
            this DictionaryStream<TKey, TValue> dictionary,
            ICollection<IDisposable> disposables)
        {
            DisposableComposite innerD = new DisposableComposite();
            if (dictionary == null)
            {
                return CompletedStream<DictionaryStream<TKey, TValue>>.Empty;
            }

            //при подписке передаем текущее состояние словаря
            var outStream = PooledStreamProxy<DictionaryStream<TKey, TValue>>.Create(subscription => subscription.OnNext(dictionary));
            innerD.Add(outStream);
            innerD.Add(dictionary.AddStream.Subscribe(innerD, insertEvent => outStream.OnNext(dictionary), () => innerD.Clear()));
            innerD.Add(dictionary.RemoveStream.Subscribe(innerD, removeEvent => outStream.OnNext(dictionary), () => innerD.Clear()));
            innerD.Add(dictionary.ChangeStream.Subscribe(innerD, changeEvent => outStream.OnNext(dictionary), () => innerD.Clear()));
            //Непосредственно при подписке outStream на dictionary.AddStream, из него придет череда ивентов, строящих словарь.
            //Мы их пропускаем - у нас есть словарь и этого достаточно
            foreach (var disposable in innerD)
                disposables.Add(disposable);
            return outStream;
        }

        /// <summary>
        /// Подписка на IDictionaryStream, в OnNext() рассылается DictionaryStream(TKey, TValue)-клон
        /// </summary>
        [Obsolete]
        public static IStream<DictionaryStream<TKey, TValue>> DictionaryChanges<TKey, TValue>(
            this IDictionaryStream<TKey, TValue> dictionary,
            ICollection<IDisposable> disposables)
        {
            DisposableComposite innerD = new DisposableComposite();
            if (dictionary == null)
            {
                return CompletedStream<DictionaryStream<TKey, TValue>>.Empty;
            }

            var innerDictionary = new DictionaryStream<TKey, TValue>();
            innerD.Add(innerDictionary);
            //при подписке новичку передаем текущее состояние словаря
            var outStream = PooledStreamProxy<DictionaryStream<TKey, TValue>>.Create(subscription => subscription.OnNext(innerDictionary));
            innerD.Add(outStream);

            innerD.Add(
                dictionary.AddStream.Subscribe(
                    innerD,
                    insertEvent =>
                    {
                        innerDictionary.Add(insertEvent.Key, insertEvent.Value);
                        outStream.OnNext(innerDictionary);
                    },
                    () => innerD.Clear()));
            innerD.Add(
                dictionary.RemoveStream.Subscribe(
                    innerD,
                    removeEvent =>
                    {
                        innerDictionary.Remove(removeEvent.Key);
                        outStream.OnNext(innerDictionary);
                    },
                    () => innerD.Clear()));
            innerD.Add(
                dictionary.ChangeStream.Subscribe(
                    innerD,
                    changeEvent =>
                    {
                        innerDictionary[changeEvent.Key] = changeEvent.NewValue;
                        outStream.OnNext(innerDictionary);
                    },
                    () => innerD.Clear()));
            //Собран innerList - клон dictionary
            //При подписке на outStream получаем OnNext(innerDictionary) и далее события при любых его изменениях
            foreach (var disposable in innerD)
                disposables.Add(disposable);
            return outStream;
        }

        public static IStream<TValue> KeyStream<TKey, TValue>(
            this IDictionaryStream<TKey, TValue> dictionary,
            ICollection<IDisposable> disposables,
            TKey key,
            TValue defaultValue = default,
            bool reactOnChangesOnly = true)
        {
            DisposableComposite innerD = new DisposableComposite();
            if (dictionary == null)
                throw new Exception("dictionary == null; Пытаемся подписаться на элемент в отсутствующем списке. Признак того, что инициализация реактивных свойств построена неверно.");

            var outStream = PooledReactiveProperty<TValue>.Create(reactOnChangesOnly).InitialValue(defaultValue);
            innerD.Add(outStream);

            innerD.Add(
                dictionary.AddStream.Subscribe(
                    innerD,
                    insertEvent =>
                    {
                        if (insertEvent.Key.Equals(key))
                            outStream.Value = insertEvent.Value;
                    },
                    () => innerD.Clear()));
            innerD.Add(
                dictionary.RemoveStream.Subscribe(
                    innerD,
                    removeEvent =>
                    {
                        if (removeEvent.Key.Equals(key))
                            outStream.Value = defaultValue;
                    },
                    () => innerD.Clear()));
            innerD.Add(
                dictionary.ChangeStream.Subscribe(
                    innerD,
                    changeEvent =>
                    {
                        if (changeEvent.Key.Equals(key))
                            outStream.Value = changeEvent.NewValue;
                    },
                    () => innerD.Clear()));

            foreach (var disposable in innerD)
                disposables.Add(disposable);

            return outStream;
        }

        public static DictionaryStream<TKey, TValue> Join<TKey, TValue>(
            this IDictionaryStream<TKey, TValue> dictionary,
            ICollection<IDisposable> disposables,
            IListStream<TKey> keysList,
            Func<TKey, TValue> defaultValue,
            JoinType joinType = JoinType.OuterFull,
            bool reactOnChangesOnly = true)
        {
            return dictionary.ToDictionaryStream(disposables, reactOnChangesOnly)
                .Join(disposables, keysList.ToListStream(disposables, reactOnChangesOnly), defaultValue, joinType, reactOnChangesOnly);
        }

        public static DictionaryStream<TKey, TValue> Join<TKey, TValue>(
            this DictionaryStream<TKey, TValue> dictionary,
            ICollection<IDisposable> disposables,
            IListStream<TKey> keysList,
            Func<TKey, TValue> defaultValue,
            JoinType joinType = JoinType.OuterFull,
            bool reactOnChangesOnly = true)
        {
            return dictionary.Join(
                disposables,
                keysList.ToListStream(disposables, reactOnChangesOnly),
                defaultValue,
                joinType,
                reactOnChangesOnly);
        }

        public static DictionaryStream<TKey, TValue> Join<TKey, TValue>(
            this IDictionaryStream<TKey, TValue> dictionary,
            ICollection<IDisposable> disposables,
            ListStream<TKey> keysList,
            Func<TKey, TValue> defaultValue,
            JoinType joinType = JoinType.OuterFull,
            bool reactOnChangesOnly = true)
        {
            return dictionary.ToDictionaryStream(disposables, reactOnChangesOnly)
                .Join(disposables, keysList, defaultValue, joinType, reactOnChangesOnly);
        }

        /// <summary>
        /// Аналог dictionary RIGHT JOIN keysList: в outDictionary из dictionary берем ключи, имеющиеся в keysList,
        /// а также добавляем дефолтные значения по недостающим ключам из keysList
        /// </summary>
        public static DictionaryStream<TKey, TValue> Join<TKey, TValue>(
            this DictionaryStream<TKey, TValue> dictionary,
            ICollection<IDisposable> disposables,
            ListStream<TKey> keysList,
            Func<TKey, TValue> defaultValue,
            JoinType joinType = JoinType.OuterFull,
            bool reactOnChangesOnly = true)
        {
            if (dictionary == null || keysList == null)
            {
                var completedStream = new DictionaryStream<TKey, TValue>();
                completedStream.Dispose();
                return completedStream;
            }

            DisposableComposite innerD = new DisposableComposite();
            var outDictionary = new DictionaryStream<TKey, TValue>(reactOnChangesOnly);

            //Подписки на изменения фильтр-листа
            void OnKeyListElementRemove(TKey removedKey)
            {
                if (Equals(removedKey, default(TKey)))
                {
                    ReactiveLogs.Logger.IfError()?.Message($"Unexpected empty key removed from {nameof(keysList)}").Write();
                    return;
                }

                switch (joinType)
                {
                    case JoinType.OuterRight:
                    case JoinType.Inner:
                        if (outDictionary.ContainsKey(removedKey))
                            outDictionary.Remove(removedKey);
                        break;

                    case JoinType.OuterFull:
                        if (!dictionary.ContainsKey(removedKey) && outDictionary.ContainsKey(removedKey))
                            outDictionary.Remove(removedKey);
                        break;
                }
            }

            void OnKeyListElementAdd(TKey addedKey)
            {
                if (Equals(addedKey, default(TKey)))
                {
                    ReactiveLogs.Logger.IfError()?.Message($"Unexpected empty key added to {nameof(keysList)}").Write();
                    return;
                }

                switch (joinType)
                {
                    case JoinType.OuterRight:
                    case JoinType.OuterFull:
                        if (outDictionary.ContainsKey(addedKey))
                            ReactiveLogs.Logger.IfError()?.Message($"Unexpected key double: {addedKey}").Write();
                        else
                            outDictionary.Add(addedKey, dictionary.ContainsKey(addedKey) ? dictionary[addedKey] : defaultValue(addedKey));
                        break;

                    case JoinType.Inner:
                        if (dictionary.ContainsKey(addedKey))
                        {
                            if (outDictionary.ContainsKey(addedKey))
                                ReactiveLogs.Logger.IfError()?.Message($"Unexpected key double: {addedKey}").Write();
                            else
                                outDictionary.Add(addedKey, dictionary[addedKey]);
                        }

                        break;
                }
            }

            innerD.Add(keysList.InsertStream.Subscribe(innerD, insertEvent => OnKeyListElementAdd(insertEvent.Item), () => innerD.Clear()));
            innerD.Add(
                keysList.RemoveStream.Subscribe(innerD, removeEvent => OnKeyListElementRemove(removeEvent.Item), () => innerD.Clear()));
            innerD.Add(
                keysList.ChangeStream.Subscribe(
                    innerD,
                    changeEvent =>
                    {
                        OnKeyListElementRemove(changeEvent.OldItem);
                        OnKeyListElementAdd(changeEvent.NewItem);
                    },
                    () => innerD.Clear()));

            //подписки на изменения словаря
            innerD.Add(
                dictionary.AddStream.Subscribe(
                    innerD,
                    addEvent =>
                    {
                        var addedKey = addEvent.Key;
                        switch (joinType)
                        {
                            case JoinType.OuterRight:
                                if (keysList.Contains(addedKey))
                                    outDictionary.Add(addedKey, addEvent.Value);
                                break;

                            case JoinType.OuterFull:
                                if (outDictionary.ContainsKey(addedKey))
                                    outDictionary[addedKey] = addEvent.Value;
                                else
                                    outDictionary.Add(addedKey, addEvent.Value);
                                break;

                            case JoinType.Inner:
                                if (keysList.Contains(addedKey))
                                {
                                    if (outDictionary.ContainsKey(addedKey))
                                        ReactiveLogs.Logger.IfError()?.Message($"Unexpected key double: {addedKey}").Write();
                                    else
                                        outDictionary.Add(addedKey, addEvent.Value);
                                }

                                break;
                        }
                    },
                    () => innerD.Clear()));

            innerD.Add(
                dictionary.RemoveStream.Subscribe(
                    innerD,
                    removeEvent =>
                    {
                        var removedKey = removeEvent.Key;
                        switch (joinType)
                        {
                            case JoinType.OuterRight:
                            case JoinType.OuterFull:
                                if (keysList.Contains(removedKey))
                                {
                                    outDictionary[removedKey] = defaultValue(removedKey);
                                }
                                else
                                {
                                    if (outDictionary.ContainsKey(removedKey))
                                        outDictionary.Remove(removedKey);
                                }

                                break;

                            case JoinType.Inner:
                                if (outDictionary.ContainsKey(removedKey))
                                    outDictionary.Remove(removedKey);
                                break;
                        }
                    },
                    () => innerD.Clear()));

            innerD.Add(
                dictionary.ChangeStream.Subscribe(
                    innerD,
                    changeEvent =>
                    {
                        var changedKey = changeEvent.Key;
                        switch (joinType)
                        {
                            case JoinType.OuterRight:
                            case JoinType.Inner:
                                if (keysList.Contains(changedKey))
                                    outDictionary[changedKey] = changeEvent.NewValue;
                                break;

                            case JoinType.OuterFull:
                                outDictionary[changedKey] = changeEvent.NewValue;
                                break;
                        }
                    },
                    () => innerD.Clear()));

            foreach (var disposable in innerD)
                disposables.Add(disposable);

            return outDictionary;
        }

        public static DictionaryStream<TOutKey, TOutValue> FuncMutable<TKey, TValue, TOutKey, TOutValue>(
            this IDictionaryStream<TKey, TValue> source,
            ICollection<IDisposable> externalD,
            IStream<Func<(TKey, TValue), ICollection<IDisposable>, (bool result, TOutKey, TOutValue)>> factoryStream)
        {
            var localD = externalD.CreateInnerD();
            var output = new DictionaryStream<TOutKey, TOutValue>();
            localD.Add(output);
            var produced = new Dictionary<TKey, (bool, IDisposable, TOutKey)>();
            localD.Add(new DisposeAgent(produced.Clear));

            var currentD = localD.CreateInnerD();

            factoryStream.Subscribe(
                localD,
                factory =>
                {
                    localD.DisposeInnerD(currentD);
                    currentD = localD.CreateInnerD();
                    produced.Clear();

                    source.AddStream.Action(
                        currentD,
                        e => Create(e.Key, e.Value));
                    source.RemoveStream.Action(
                        currentD,
                        e => Destroy(e.Key));
                    source.ChangeStream.Subscribe(
                        currentD,
                        e =>
                        {
                            Destroy(e.Key);
                            Create(e.Key, e.NewValue);
                        },
                        () => output.Clear());

                    void Create(TKey inKey, TValue inValue)
                    {
                        var innerD = currentD.CreateInnerD();
                        var (result, key, value) = factory.Invoke((inKey, inValue), innerD);
                        produced[inKey] = (result, innerD, key);
                        if (result)
                            output.Add(key, value);
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

        public static DictionaryStream<TOutKey, TOutValue> FuncMutable<TKey, TValue, TOutKey, TOutValue>(
            this IDictionaryStream<TKey, TValue> source,
            ICollection<IDisposable> externalD,
            IStream<Func<(TKey, TValue), ICollection<IDisposable>, (TOutKey, TOutValue)>> factoryStream)
        {
            var generalFactory = factoryStream
                .Func<
                    Func<(TKey, TValue), ICollection<IDisposable>, (TOutKey, TOutValue)>,
                    Func<(TKey, TValue), ICollection<IDisposable>, (bool result, TOutKey, TOutValue)>
                >(
                    externalD,
                    func =>
                    {
                        return (tuple, localD) =>
                        {
                            var (tOutKey, tOutValue) = func.Invoke(tuple, localD);
                            return (true, tOutKey, tOutValue);
                        };
                    }
                );
            return FuncMutable(source, externalD, generalFactory);
        }

        public static DictionaryStream<TOutKey, TOutValue> Transform<TKey, TValue, TOutKey, TOutValue>(
            this IDictionaryStream<TKey, TValue> source,
            ICollection<IDisposable> externalD,
            Func<(TKey, TValue), ICollection<IDisposable>, (bool result, TOutKey, TOutValue)> factory)
        {
            var localD = externalD.CreateInnerD();
            var output = new DictionaryStream<TOutKey, TOutValue>();
            localD.Add(output);
            var produced = new Dictionary<TKey, (bool, IDisposable, TOutKey)>();
            localD.Add(new DisposeAgent(produced.Clear));

            source.AddStream.Action(
                localD,
                e => Create(e.Key, e.Value));
            source.RemoveStream.Action(
                localD,
                e => Destroy(e.Key));
            source.ChangeStream.Subscribe(
                localD,
                e =>
                {
                    Destroy(e.Key);
                    Create(e.Key, e.NewValue);
                },
                () => externalD.DisposeInnerD(localD));

            void Create(TKey inKey, TValue inValue)
            {
                var innerD = localD.CreateInnerD();
                var (result, key, value) = factory.Invoke((inKey, inValue), innerD);
                produced[inKey] = (result, innerD, key);
                if (result)
                    output.Add(key, value);
            }

            void Destroy(TKey inKey)
            {
                if (produced.TryGetValue(inKey, out var tuple))
                {
                    var (result, innerD, key) = tuple;
                    if (result)
                        output.Remove(key);
                    localD.DisposeInnerD(innerD);
                }
            }

            return output;
        }
    }
}