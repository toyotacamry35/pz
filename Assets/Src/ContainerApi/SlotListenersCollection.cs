using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Shared;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.Inventory;
using Assets.Tools;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Serializers;
using Src.Aspects.Impl.Stats.Proxy;
using Uins;
using UnityEngine;

namespace Assets.Src.ContainerApis
{
    public class SlotListenersCollection
    {
        private const string StackPropertyName = nameof(ISlotItemClientBroadcast.Stack);
        private const string IdPropertyName = nameof(IItem.Id);

        private const int MaxAmmoContainersCount = 2;

        private ConcurrentDictionary<int, SlotListener> _slotListeners = new ConcurrentDictionary<int, SlotListener>();
        private static int _idCounter = 1;


        //=== Props ===========================================================

        public int CollectionId { get; }

        public bool WatchForSubitems { get; }

        public bool IsClientFull { get; private set; }


        //=== Ctor ============================================================

        public SlotListenersCollection(bool watchForSubitems, int parentCollectionId = 0)
        {
            CollectionId = parentCollectionId * 1000 + _idCounter++;
            WatchForSubitems = watchForSubitems;
        }


        //=== Public ==========================================================

        /// <param name="doEventCall">False - для коллекций зарядов</param>
        public async Task SubscribeOnItems(IDeltaDictionary<int, ISlotItemClientFull> items, bool doEventCall = true)
        {
            await SubscribeOnItems(items.ToDeltaDictionary<ISlotItemClientBroadcast>(), doEventCall, true);
        }

        public async Task SubscribeOnItems(IDeltaDictionary<int, ISlotItemClientBroadcast> items, bool doEventCall = true, bool isClientFull = false)
        {
            //UI.CallerLog($"[{CollectionId}] itemsCount={items?.Count}"); //DEBUG
            IsClientFull = isClientFull;
            if (items.AssertIfNull(nameof(items)))
                return;

            items.OnItemAddedOrUpdated += OnItemAddedOrUpdated;
            items.OnItemRemoved += OnItemRemoved;

            foreach (var kvp in items)
                await OnItemAddedOrUpdatedInternal(kvp.Key, kvp.Value, false);

            if (doEventCall && _slotListeners.Count > 0)
            {
                foreach (var kvp in _slotListeners)
                    kvp.Value.FireOnSlotItemChanged();
            }
        }

        public void UnsubscribeFromItems(IDeltaDictionary<int, ISlotItemClientFull> items)
        {
            UnsubscribeFromItems(items.ToDeltaDictionary<ISlotItemClientBroadcast>());
        }

        public void UnsubscribeFromItems(IDeltaDictionary<int, ISlotItemClientBroadcast> items)
        {
            //UI.CallerLog($"[{CollectionId}]  itemsCount={items?.Count}"); //DEBUG
            if (items.AssertIfNull(nameof(items)))
                return;

            items.OnItemAddedOrUpdated -= OnItemAddedOrUpdated;
            items.OnItemRemoved -= OnItemRemoved;

            foreach (var kvp in items)
                OnItemRemovedInternal(kvp.Key, kvp.Value);
        }

        public void OnSubscribeRequest(int slotIndex, SlotItemChangedDelegate onItemChanged)
        {
            var slotListener = GetSlotListener(slotIndex, true, WatchForSubitems, IsClientFull);
            if (slotListener.AssertIfNull(nameof(slotListener)))
                return;

            slotListener.ItemChanged += onItemChanged;
            if (!slotListener.IsEmpty) //сообщить новому подписчику непустой стейт
                UnityQueueHelper.RunInUnityThreadNoWait(() => onItemChanged(slotIndex, slotListener.SelfSlotItem, slotListener.StackDelta));
        }

        public void OnUnsubscribeRequest(int slotIndex, SlotItemChangedDelegate onItemChanged)
        {
            var slotListener = GetSlotListener(slotIndex, false, false, IsClientFull);
            if (slotListener != null)
                slotListener.ItemChanged -= onItemChanged;
        }

        public void OnStatsSubscribeRequest(int slotIndex, FrequentStatsChangedDelegate onFrequentStatsChanged)
        {
            var slotListener = GetSlotListener(slotIndex, true, WatchForSubitems, IsClientFull);
            if (slotListener.AssertIfNull(nameof(slotListener)))
                return;

            slotListener.FrequentStatsChanged += onFrequentStatsChanged;
            var hasValueStats = slotListener.GetHasValueStats();
            if (hasValueStats.Count > 0)
                UnityQueueHelper.RunInUnityThreadNoWait(() => onFrequentStatsChanged(slotIndex, hasValueStats));
        }

        public void OnStatsUnsubscribeRequest(int slotIndex, FrequentStatsChangedDelegate onFrequentStatsChanged)
        {
            var slotListener = GetSlotListener(slotIndex, false, false, IsClientFull);
            if (slotListener != null)
                slotListener.FrequentStatsChanged -= onFrequentStatsChanged;
        }

        public override string ToString()
        {
            return $"[{nameof(SlotListenersCollection)}[{CollectionId}]: {nameof(WatchForSubitems)}{WatchForSubitems.AsSign()} " +
                   $"{nameof(_slotListeners)}={_slotListeners.Count}]";
        }


        //=== Private =========================================================

        private SlotListener GetSlotListener(int slotIndex, bool createIfNotExists, bool watchForSubitems, bool isClientFull)
        {
            SlotListener slotListener;
            lock (_slotListeners)
            {
                if (!_slotListeners.TryGetValue(slotIndex, out slotListener))
                {
                    if (createIfNotExists)
                    {
                        slotListener = new SlotListener(slotIndex, watchForSubitems, CollectionId, isClientFull);
                        var tryAddResult = _slotListeners.TryAdd(slotIndex, slotListener);
                        if (!tryAddResult)
                            UI.Logger.IfError()?.Message($"new SlotListener(idx={slotIndex}): TryAdd fail - {this}").Write();
                    }
                }
            }

            return slotListener;
        }

        private async Task OnItemAddedOrUpdated(DeltaDictionaryChangedEventArgs<int, ISlotItemClientBroadcast> eventArgs)
        {
            var itemIndex = eventArgs.Key;
            var slotItemClientBroadcast = eventArgs.Value;
            await OnItemAddedOrUpdatedInternal(itemIndex, slotItemClientBroadcast);
        }

        private async Task OnItemAddedOrUpdatedInternal(int itemIndex, ISlotItemClientBroadcast slotItemClientBroadcast, bool doEventCall = true)
        {
            //UI.CallerLog($"{nameof(itemIndex)}={itemIndex} -- {this}"); //DEBUG
            var slotListener = GetSlotListener(itemIndex, true, WatchForSubitems, IsClientFull);
            if (!slotListener.AssertIfNull(nameof(slotListener)))
                await slotListener.SubscribeAndSetSelfSlotItem(slotItemClientBroadcast, doEventCall);
        }

        private Task OnItemRemoved(DeltaDictionaryChangedEventArgs<int, ISlotItemClientBroadcast> eventArgs)
        {
            var itemIndex = eventArgs.Key;
            var slotItemClientBroadcast = eventArgs.OldValue;
            OnItemRemovedInternal(itemIndex, slotItemClientBroadcast);
            return Task.CompletedTask;
        }

        private void OnItemRemovedInternal(int itemIndex, ISlotItemClientBroadcast slotItemClientBroadcast)
        {
            //UI.CallerLog($"{nameof(itemIndex)}={itemIndex} -- {this}"); //DEBUG
            SlotListener slotListener;
            lock (_slotListeners)
                if (!_slotListeners.TryGetValue(itemIndex, out slotListener))
                    return;

            slotListener.UnsubscribeAndResetSelfSlotItem(slotItemClientBroadcast);
        }


        //=== Class ===================================================================================================

        private class SlotListener
        {
            public event SlotItemChangedDelegate ItemChanged;
            public event FrequentStatsChangedDelegate FrequentStatsChanged;

            private SlotListenersCollection _ammoSlotListenersCollection;
            private int _parentCollectionId;

            private ConcurrentDictionary<StatResource, TimeStatListener> _timeStatListeners =
                new ConcurrentDictionary<StatResource, TimeStatListener>();

            private ConcurrentDictionary<StatResource, AccumStatListener> _accumStatListeners =
                new ConcurrentDictionary<StatResource, AccumStatListener>();

            private ConcurrentDictionary<StatResource, ValueStatListener> _valueStatListeners =
                new ConcurrentDictionary<StatResource, ValueStatListener>();

            //=== Props =======================================================

            public bool WatchForSubitems { get; }

            public SlotItem SelfSlotItem { get; }

            public int StackDelta { get; set; }

            public int SlotIndex { get; }

            public bool IsSubscribed { get; private set; }

            public bool IsSubscribedOnSubitems { get; private set; }

            public bool IsEmpty => SelfSlotItem.Count == 0;

            public bool IsClientFull { get; }


            //=== Ctor ========================================================

            public SlotListener(int slotIndex, bool watchForSubitems, int parentCollectionId, bool isClientFull)
            {
                SlotIndex = slotIndex;
                SelfSlotItem = new SlotItem();
                IsClientFull = isClientFull;
                WatchForSubitems = watchForSubitems;
                _parentCollectionId = parentCollectionId;
                if (WatchForSubitems)
                {
                    _ammoSlotListenersCollection =
                        new SlotListenersCollection(false, parentCollectionId); //у сабайтемов нет собств. сабайтемов
                    for (int i = 0; i < MaxAmmoContainersCount; i++) //Подписываемся один раз на события собственных контейнер-листенеров
                        _ammoSlotListenersCollection.OnSubscribeRequest(i, OnAmmoContainerChanged);
                }
            }


            //=== Public ======================================================

            public override string ToString()
            {
                return $"{nameof(SlotListener)}[slc={_parentCollectionId}, idx={SlotIndex}]{(IsSubscribed ? "" : " UNSUBSCRIBED")}: " +
                       $"eventSubscrs={(ItemChanged == null ? 0 : ItemChanged.GetInvocationList().Length)}, " +
                       $"{nameof(WatchForSubitems)}{WatchForSubitems.AsSign()}, {nameof(IsSubscribedOnSubitems)}{IsSubscribedOnSubitems.AsSign()} " +
                       $"{(IsEmpty ? " EMPTY\n" : " ")}{SelfSlotItem}";
            }

            public async Task SubscribeAndSetSelfSlotItem(ISlotItemClientBroadcast slotItemCb, bool doEventCall = true)
            {
                //UI.CallerLog($"item={slotItemCb?.Item?.ItemResource}, count={slotItemCb?.Stack}, " +
                //             $"{nameof(doEventCall)}{doEventCall.AsSign()}");
                SelfSlotItem.Reset();
                _timeStatListeners.Values.ForEach(tsl => tsl.Clear());
                _accumStatListeners.Values.ForEach(asl => asl.Clear());
                _valueStatListeners.Values.ForEach(asl => asl.Clear());

                if (!slotItemCb.AssertIfNull(nameof(slotItemCb)) &&
                    !slotItemCb.Item.AssertIfNull($"{nameof(slotItemCb)}.{nameof(slotItemCb.Item)}"))
                    //были белые квадратики из-за null здесь
                {
                    var itemCb = slotItemCb.Item;
                    if (!itemCb.AssertIfNull(nameof(itemCb)))
                    {
                        SelfSlotItem.GeneralStats = (await itemCb.Stats.GetSnapshot(StatType.General))
                            .Select(v => new StatModifier(v.Stat, v.Value))
                            .ToArray();
                        SelfSlotItem.SpecificStats = (await itemCb.Stats.GetSnapshot(StatType.Specific))
                            .Select(v => new StatModifier(v.Stat, v.Value))
                            .ToArray();

                        if (!IsSubscribed)
                        {
                            itemCb.SubscribePropertyChanged(IdPropertyName, OnChangeIdTask);
                        }

                        SelfSlotItem.ItemGuid = itemCb.Id;
                        SelfSlotItem.ItemResource = itemCb.ItemResource;

                        if (IsClientFull)
                        {
                            var slotItemClientFull = slotItemCb.To<ISlotItemClientFull>();
                            if (slotItemClientFull != null)
                            {
                                var stats = slotItemClientFull.Item.Stats;
                                //UI.CallerLog($"timeStats={stats?.TimeStats.Count}, accumStats={stats?.AccumulatedStats.Count}\n{this}"); //DEBUG
                                if (!stats.AssertIfNull("slotItemClientFull"))
                                {
                                    if (stats.TimeStats != null)
                                        foreach (var timeStatKvp in stats.TimeStats)
                                        {
                                            var timeStatListener = GetStatListener(timeStatKvp.Key, _timeStatListeners);
                                            if (timeStatListener != null)
                                            {
                                                timeStatListener.InitialSet(timeStatKvp.Value);
                                                timeStatListener.SubscribeIfNeed(timeStatKvp.Value);
                                            }
                                            else
                                            {
                                                UI.Logger.IfError()?.Message($"{nameof(timeStatListener)} is null! For {SelfSlotItem}").Write();
                                            }
                                        }

                                    if (stats.AccumulatedStats != null)
                                        foreach (var accumStatKvp in stats.AccumulatedStats)
                                        {
                                            var accumStatListener = GetStatListener(accumStatKvp.Key, _accumStatListeners);
                                            if (accumStatListener != null)
                                            {
                                                //await не дает сделать метод синхронным
                                                accumStatListener.SetAccumValue(await accumStatKvp.Value.GetValue());
                                                accumStatListener.SubscribeIfNeed(accumStatKvp.Value);
                                            }
                                            else
                                            {
                                                UI.Logger.IfError()?.Message($"{nameof(accumStatListener)} is null! For {SelfSlotItem}").Write();
                                            }
                                        }

                                    if (stats.ValueStats != null)
                                        foreach (var valueStatKvp in stats.ValueStats)
                                        {
                                            var valueStatListener = GetStatListener(valueStatKvp.Key, _valueStatListeners);
                                            if (valueStatListener != null)
                                            {
                                                //await не дает сделать метод синхронным
                                                valueStatListener.SetAccumValue(await valueStatKvp.Value.GetValue());
                                                valueStatListener.SubscribeIfNeed(valueStatKvp.Value);
                                            }
                                            else
                                            {
                                                UI.Logger.IfError()?.Message($"{nameof(valueStatListener)} is null! For {SelfSlotItem}").Write();
                                            }
                                        }
                                }
                            }
                            else
                            {
                                UI.Logger.Error(
                                    $"Subscribe. Can't raise {slotItemCb.GetType().Name} to {nameof(ISlotItemClientFull)} for entity " +
                                    $"{EntitiesRepository.GetTypeById(slotItemCb.ParentTypeId)}.{slotItemCb.ParentEntityId}");
                            }
                        }

                        if (WatchForSubitems)
                        {
                            var ammoContainer = itemCb.AmmoContainer;
                            if (ammoContainer != null && ammoContainer.Items != null)
                            {
                                if (!IsSubscribedOnSubitems)
                                {
                                    IsSubscribedOnSubitems = true;
                                    await _ammoSlotListenersCollection.SubscribeOnItems(ammoContainer.Items, false, IsClientFull);
                                }

                                foreach (var kvp in _ammoSlotListenersCollection._slotListeners)
                                    SelfSlotItem.SetInnerContainer(kvp.Key, kvp.Value.SelfSlotItem);
                            }
                        }
                    }

                    if (!IsSubscribed)
                        slotItemCb.SubscribePropertyChanged(StackPropertyName, OnChangeStackTask);

                    StackDelta = slotItemCb.Stack;
                    SelfSlotItem.Count = (uint) StackDelta;
                    IsSubscribed = true;
                }

                if (doEventCall)
                    FireOnSlotItemChanged();
            }

            public void UnsubscribeAndResetSelfSlotItem(ISlotItemClientBroadcast slotItemCb)
            {
                if (!IsSubscribed)
                    UI.Logger.IfWarn()?.Message($"Attempt to unsubscribe not subscribed item: {SelfSlotItem}").Write();

                slotItemCb.UnsubscribePropertyChanged(StackPropertyName, OnChangeStackTask);
                var itemCb = slotItemCb.Item;
                if (!itemCb.AssertIfNull(nameof(itemCb)))
                {
                    itemCb.UnsubscribePropertyChanged(IdPropertyName, OnChangeIdTask);

                    if (IsClientFull)
                    {
                        var slotItemClientFull = slotItemCb.TryTo<ISlotItemClientFull>();
                        if (slotItemClientFull != null)
                        {
                            var stats = slotItemClientFull.Item.Stats;
                            if (stats != null)
                            {
                                //UI.CallerLog($"{this} [{SlotIndex}] --- StatUnsubscribes ---"); //DEBUG
                                if (stats.TimeStats != null)
                                    foreach (var timeStatKvp in stats.TimeStats)
                                    {
                                        var timeStatListener = GetStatListener(timeStatKvp.Key, _timeStatListeners, false);
                                        if (timeStatListener != null)
                                        {
                                            timeStatListener.UnsubscribeIfNeed(timeStatKvp.Value);
                                        }
                                        else
                                        {
                                            UI.Logger.IfError()?.Message($"{nameof(timeStatListener)} is null! For {SelfSlotItem}").Write();
                                        }
                                    }

                                if (stats.AccumulatedStats != null)
                                    foreach (var accumStatKvp in stats.AccumulatedStats)
                                    {
                                        var accumStatListener = GetStatListener(accumStatKvp.Key, _accumStatListeners, false);
                                        if (accumStatListener != null)
                                        {
                                            accumStatListener.UnsubscribeIfNeed(accumStatKvp.Value);
                                        }
                                        else
                                        {
                                            UI.Logger.IfError()?.Message($"{nameof(accumStatListener)} is null! For {SelfSlotItem}").Write();
                                        }
                                    }

                                if (stats.ValueStats != null)
                                    foreach (var valueStatKvp in stats.ValueStats)
                                    {
                                        var valueStatListener = GetStatListener(valueStatKvp.Key, _valueStatListeners, false);
                                        if (valueStatListener != null)
                                        {
                                            valueStatListener.UnsubscribeIfNeed(valueStatKvp.Value);
                                        }
                                        else
                                        {
                                            UI.Logger.IfError()?.Message($"{nameof(valueStatListener)} is null! For {SelfSlotItem}").Write();
                                        }
                                    }
                            }
                        }
                        else
                        {
                            UI.Logger.Error(
                                $"Unsubscribe. Can't raise {slotItemCb.GetType().Name} to {nameof(ISlotItemClientFull)} for entity " +
                                $"{EntitiesRepository.GetTypeById(slotItemCb.ParentTypeId)}.{slotItemCb.ParentEntityId}");
                        }
                    }

                    if (WatchForSubitems && IsSubscribedOnSubitems)
                    {
                        var itemClientBroadcast = itemCb.To<IItemClientBroadcast>();
                        if (!itemClientBroadcast.AssertIfNull(nameof(itemClientBroadcast)) &&
                            !itemClientBroadcast.AmmoContainer.AssertIfNull(nameof(itemClientBroadcast.AmmoContainer)) &&
                            !itemClientBroadcast.AmmoContainer.Items.AssertIfNull(nameof(itemClientBroadcast.AmmoContainer.Items)))
                        {
                            IsSubscribedOnSubitems = false;
                            UnsubscribeFromSubitems(itemClientBroadcast.AmmoContainer.Items);
                        }
                    }
                }

                IsSubscribed = false;
                StackDelta = -(int) SelfSlotItem.Count;

                SelfSlotItem.Reset();
                _timeStatListeners.Values.ForEach(tsl => tsl.Clear());
                _accumStatListeners.Values.ForEach(asl => asl.Clear());
                _valueStatListeners.Values.ForEach(asl => asl.Clear());
                if (StackDelta < 0)
                    FireOnSlotItemChanged();
            }

            public List<KeyValuePair<StatResource, float>> GetHasValueStats()
            {
                var list = new List<KeyValuePair<StatResource, float>>();
                list.AddRange(_timeStatListeners.Where(kvp => kvp.Value.HasValue)
                    .Select(kvp => new KeyValuePair<StatResource, float>(kvp.Key, kvp.Value.Value)));
                list.AddRange(_accumStatListeners.Where(kvp => kvp.Value.HasValue)
                    .Select(kvp => new KeyValuePair<StatResource, float>(kvp.Key, kvp.Value.Value)));
                list.AddRange(_valueStatListeners.Where(kvp => kvp.Value.HasValue)
                    .Select(kvp => new KeyValuePair<StatResource, float>(kvp.Key, kvp.Value.Value)));
                return list;
            }


            //=== Private =========================================================

            private T GetStatListener<T>(StatResource statResource, ConcurrentDictionary<StatResource, T> listeners,
                bool createIfNotExists = true) where T : AccumStatListener, new()
            {
                T statListener;
                lock (listeners)
                {
                    if (!listeners.TryGetValue(statResource, out statListener))
                    {
                        if (createIfNotExists)
                        {
                            statListener = new T();
                            statListener.Init(statResource, this);
                            listeners.TryAdd(statResource, statListener);
                        }
                    }
                }

                return statListener;
            }

            private Task OnChangeStackTask(EntityEventArgs args)
            {
                int newVal = (int) args.NewValue;
                StackDelta = newVal - (int) SelfSlotItem.Count;
                SelfSlotItem.Count = (uint) newVal;
                FireOnSlotItemChanged();
                return Task.CompletedTask;
            }

            private Task OnChangeIdTask(EntityEventArgs args)
            {
                SelfSlotItem.ItemGuid = (Guid) args.NewValue;
                StackDelta = 0; //TODO: или Stack?
                FireOnSlotItemChanged();
                return Task.CompletedTask;
            }

            public void FireOnSlotItemChanged()
            {
                //UI.CallerLog($"{nameof(StackDelta)}={StackDelta} {this}"); //DEBUG
                if (ItemChanged == null)
                    return;

                UnityQueueHelper.RunInUnityThreadNoWait(() =>
                {
                    if (ItemChanged != null)
                    {
                        var invocationList = ItemChanged.GetInvocationList();
                        for (int i = 0; i < invocationList.Length; i++)
                        {
                            try
                            {
                                SlotItemChangedDelegate slotItemChangedDelegate = (SlotItemChangedDelegate) invocationList[i];
                                slotItemChangedDelegate(SlotIndex, SelfSlotItem, StackDelta);
                            }
                            catch (Exception e)
                            {
                                UI.Logger.IfError()?.Message($"UI Exception: {e.Message}\n{e.StackTrace}").Write();
                            }
                        }
                    }
                });

                //Конвенция обновления: при получении ItemChanged подписчики должны сбрасывать FrequentStats
                var hasValueStats = GetHasValueStats();
                if (hasValueStats.Count > 0)
                    FireOnFrequentStatsChanged(hasValueStats);
            }

            public void FireOnFrequentStatsChanged(List<KeyValuePair<StatResource, float>> stats)
            {
//                UI.CallerLog($"stats={stats.Select(kvp => $"[{kvp.Key.DebugName}]={kvp.Value}").ItemsToString()}\n" +
//                             $"{(FrequentStatsChanged == null ? "FrequentStatsChanged==null " : "")}{this} [{SlotIndex}]");
                if (FrequentStatsChanged == null)
                    return;

                UnityQueueHelper.RunInUnityThreadNoWait(() => { FrequentStatsChanged.Invoke(SlotIndex, stats); });
            }

            private void UnsubscribeFromSubitems(IDeltaDictionary<int, ISlotItemClientBroadcast> items)
            {
                _ammoSlotListenersCollection.UnsubscribeFromItems(items);
            }

            private void OnAmmoContainerChanged(int ammoContainerIndex, SlotItem ammoContainerSlotItem, int stackDelta)
            {
                SelfSlotItem.SetInnerContainer(ammoContainerIndex, ammoContainerSlotItem);
                StackDelta = 0;
                AsyncUtils.RunAsyncTask(FireOnSlotItemChanged);
            }


            //=========================================================================================================

            private class AccumStatListener
            {
                public StatResource StatResource { get; private set; }

                protected SlotListener ParentSlotListener { get; private set; }

                public float Value { get; protected set; }

                public bool HasValue { get; protected set; }

                public bool IsSubscribed { get; protected set; }


                //=== Public ==================================================

                public void SetAccumValue(float accumValue)
                {
                    Value = accumValue;
                    HasValue = true;
                }

                public virtual void Clear()
                {
                    HasValue = false;
                    Value = 0;
                }

                public void Init(StatResource statResource, SlotListener slotListener)
                {
                    StatResource = statResource;
                    ParentSlotListener = slotListener;
                }

                public void SubscribeIfNeed(IAccumulatedStatClientFull accumStatClientFull)
                {
                    if (IsSubscribed ||
                        accumStatClientFull.AssertIfNull(nameof(accumStatClientFull)))
                        return;

                    IsSubscribed = true;
                    accumStatClientFull.SubscribePropertyChanged(nameof(IAccumulatedStatClientFull.ValueCache), OnStatChanged);
                }

                public void UnsubscribeIfNeed(IAccumulatedStatClientFull accumStatClientFull)
                {
                    if (!IsSubscribed ||
                        accumStatClientFull.AssertIfNull(nameof(accumStatClientFull)))
                        return;

                    IsSubscribed = false;
                    accumStatClientFull.UnsubscribePropertyChanged(nameof(IAccumulatedStatClientFull.ValueCache), OnStatChanged);
                }

                public override string ToString()
                {
                    return $"<{GetType()}> [{StatResource.DebugName}] {nameof(Value)}={Value}";
                }

                protected virtual Task OnStatChanged(EntityEventArgs args)
                {
                    HasValue = true;
                    var val = Value = (float) args.NewValue;
//                    UI.CallerLog($"---------------- Value={Value} --- {this}"); //DEBUG
                    ParentSlotListener.FireOnFrequentStatsChanged(
                        Enumerable.Repeat(new KeyValuePair<StatResource, float>(StatResource, val), 1).ToList());
                    return Task.CompletedTask;
                }
            }

            private class ValueStatListener : AccumStatListener
            {
                public void SubscribeIfNeed(IValueStatClientFull valueStatClientFull)
                {
                    if (IsSubscribed ||
                        valueStatClientFull.AssertIfNull(nameof(valueStatClientFull)))
                        return;

                    IsSubscribed = true;
                    valueStatClientFull.SubscribePropertyChanged(nameof(IValueStatClientFull.Value), OnStatChanged);
                }

                public void UnsubscribeIfNeed(IValueStatClientFull valueStatClientFull)
                {
                    if (!IsSubscribed ||
                        valueStatClientFull.AssertIfNull(nameof(valueStatClientFull)))
                        return;

                    IsSubscribed = false;
                    valueStatClientFull.UnsubscribePropertyChanged(nameof(IValueStatClientFull.Value), OnStatChanged);
                }
            }

            //=========================================================================================================

            private class TimeStatListener : AccumStatListener
            {
                protected TimeStatState State { get; private set; }

                public float MinLimit { get; private set; }

                public float MaxLimit { get; private set; }


                //=== Public ==================================================

                public void InitialSet(ITimeStatClientFull timeStatClientFull)
                {
                    State = timeStatClientFull.State;
                    MinLimit = timeStatClientFull.LimitMinCache;
                    MaxLimit = timeStatClientFull.LimitMaxCache;
                    CalcValue();
                }


                protected override Task OnStatChanged(EntityEventArgs args)
                {
                    HasValue = true;
                    State = (TimeStatState) args.NewValue;
                    CalcValue();
//                    UI.CallerLog($"---------------- Value={Value} --- {this}"); //DEBUG
                    ParentSlotListener.FireOnFrequentStatsChanged(
                        Enumerable.Repeat(new KeyValuePair<StatResource, float>(StatResource, Value), 1).ToList());
                    return Task.CompletedTask;
                }

                private Task OnTimeStatLimitMinCacheChanged(EntityEventArgs args)
                {
                    HasValue = true;
//                    UI.CallerLog($"newValue={args.NewValue} --- {this}"); //DEBUG
                    var oldVal = Value;
                    MinLimit = (float) args.NewValue;
                    if (Mathf.Approximately(CalcValue(), oldVal))
                        return Task.CompletedTask;

                    ParentSlotListener.FireOnFrequentStatsChanged(
                        Enumerable.Repeat(new KeyValuePair<StatResource, float>(StatResource, Value), 1).ToList());
                    return Task.CompletedTask;
                }

                private Task OnTimeStatLimitMaxCacheChanged(EntityEventArgs args)
                {
                    HasValue = true;
//                    UI.CallerLog($"newValue={args.NewValue} --- {this}"); //DEBUG
                    var oldVal = Value;
                    MaxLimit = (float) args.NewValue;
                    if (Mathf.Approximately(CalcValue(), oldVal))
                        return Task.CompletedTask;

                    ParentSlotListener.FireOnFrequentStatsChanged(
                        Enumerable.Repeat(new KeyValuePair<StatResource, float>(StatResource, Value), 1).ToList());
                    return Task.CompletedTask;
                }

                public override void Clear()
                {
                    base.Clear();
                    MinLimit = MaxLimit = 0;
                    State = new TimeStatState();
                }

                public float CalcValue()
                {
                    HasValue = true;
                    return Value = GetValue(State, MinLimit, MaxLimit);
                }

                public static float GetValue(TimeStatState state, float limitMinCache, float limitMaxCache)
                {
                    long passedMs = SyncTime.Now - state.LastBreakPointTime;
                    var passedSeconds = passedMs / 1000f;
                    var val = state.LastBreakPointValue + state.ChangeRateCache * passedSeconds;
                    val = Math.Min(Math.Max(val, limitMinCache), limitMaxCache);
                    return val;
                }

                public void SubscribeIfNeed(ITimeStatClientFull timeStatClientFull)
                {
                    if (IsSubscribed ||
                        timeStatClientFull.AssertIfNull(nameof(timeStatClientFull)))
                        return;

                    IsSubscribed = true;
                    timeStatClientFull.SubscribePropertyChanged(nameof(ITimeStatClientFull.State), OnStatChanged);
                    timeStatClientFull.SubscribePropertyChanged(nameof(ITimeStatClientFull.LimitMinCache), OnTimeStatLimitMinCacheChanged);
                    timeStatClientFull.SubscribePropertyChanged(nameof(ITimeStatClientFull.LimitMaxCache), OnTimeStatLimitMaxCacheChanged);
                }

                public void UnsubscribeIfNeed(ITimeStatClientFull timeStatClientFull)
                {
                    if (!IsSubscribed ||
                        timeStatClientFull.AssertIfNull(nameof(timeStatClientFull)))
                        return;

                    IsSubscribed = false;
                    timeStatClientFull.UnsubscribePropertyChanged(nameof(ITimeStatClientFull.State), OnStatChanged);
                    timeStatClientFull.UnsubscribePropertyChanged(nameof(ITimeStatClientFull.LimitMinCache),
                        OnTimeStatLimitMinCacheChanged);
                    timeStatClientFull.UnsubscribePropertyChanged(nameof(ITimeStatClientFull.LimitMaxCache),
                        OnTimeStatLimitMaxCacheChanged);
                }
            }
        }
    }
}