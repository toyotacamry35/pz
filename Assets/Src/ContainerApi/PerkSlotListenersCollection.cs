using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedCode.Aspects.Item.Templates;
using SharedCode.EntitySystem.Delta;

namespace Assets.Src.ContainerApis
{
    public class PerkSlotListenersCollection
    {
        private Dictionary<int, PerkSlotListener> _listeners = new Dictionary<int, PerkSlotListener>();

        private int _collectionId;
        private static int _idCounter;


        //=== Ctor ============================================================

        public PerkSlotListenersCollection()
        {
            _collectionId = _idCounter++;
        }


        //=== Public ==========================================================

        public async Task SubscribeOnCollection(IDeltaDictionary<int, ItemTypeResource> perkSlots)
        {
            if (perkSlots.AssertIfNull(nameof(perkSlots)))
                return;

            perkSlots.OnItemAddedOrUpdated += OnItemAddedOrUpdated;
            perkSlots.OnItemRemoved += OnItemRemoved;

            foreach (var kvp in perkSlots)
                OnItemAddedOrUpdatedInternal(kvp.Key, kvp.Value, false);

            if (_listeners.Count > 0)
            {
                UnityQueueHelper.RunInUnityThreadNoWait(() =>
                {
                    var keys = _listeners.Keys.ToList();
                    foreach (var listenerKey in keys)
                        if (_listeners.ContainsKey(listenerKey))
                            _listeners[listenerKey].FireEvent();
                });
            }
        }

        public async Task UnsubscribeFromCollection(IDeltaDictionary<int, ItemTypeResource> perkSlots)
        {
            if (perkSlots.AssertIfNull(nameof(perkSlots)))
                return;

            perkSlots.OnItemAddedOrUpdated -= OnItemAddedOrUpdated;
            perkSlots.OnItemRemoved -= OnItemRemoved;

            foreach (var kvp in perkSlots)
                OnItemRemovedInternal(kvp.Key);
        }

        public override string ToString()
        {
            lock (_listeners)
                return $"[{nameof(PerkSlotListenersCollection)}[id={_collectionId}]: {nameof(_listeners)}={_listeners.Count}]";
        }

        public void OnSubscribeRequest(int slotIndex, PerkSlotsBaseFullApi.PerkSlotTypeChangedDelegate onPerkTypeChanged)
        {
            PerkSlotListener listener;
            lock (_listeners)
            {
                if (!_listeners.TryGetValue(slotIndex, out listener))
                {
                    listener = new PerkSlotListener(slotIndex, _collectionId);
                    _listeners.Add(slotIndex, listener);
                }
            }

            listener.OnPerkSlotTypeChanged += onPerkTypeChanged;
            if (!listener.IsEmpty) //сообщить новому подписчику непустой стейт
                UnityQueueHelper.RunInUnityThreadNoWait(() => onPerkTypeChanged(slotIndex, listener.PerkSlotType));
        }

        public void OnUnsubscribeRequest(int slotIndex, PerkSlotsBaseFullApi.PerkSlotTypeChangedDelegate onPerkTypeChanged)
        {
            lock (_listeners)
            {
                PerkSlotListener listener;
                if (_listeners.TryGetValue(slotIndex, out listener))
                    listener.OnPerkSlotTypeChanged -= onPerkTypeChanged;
            }
        }


        //=== Private =========================================================

        private Task OnItemAddedOrUpdated(DeltaDictionaryChangedEventArgs<int, ItemTypeResource> eventArgs)
        {
            var itemIndex = eventArgs.Key;
            var perkSlotType = eventArgs.Value;
            OnItemAddedOrUpdatedInternal(itemIndex, perkSlotType);
            return Task.CompletedTask;
        }

        private void OnItemAddedOrUpdatedInternal(int itemIndex, ItemTypeResource perkSlotType, bool doEventCall = true)
        {
            PerkSlotListener listener;
            lock (_listeners)
            {
                if (!_listeners.TryGetValue(itemIndex, out listener))
                {
                    listener = new PerkSlotListener(itemIndex, _collectionId);
                    _listeners.Add(itemIndex, listener);
                }
            }

            listener.SetPerkSlotType(perkSlotType, doEventCall);
        }

        private Task OnItemRemoved(DeltaDictionaryChangedEventArgs<int, ItemTypeResource> eventArgs)
        {
            var itemIndex = eventArgs.Key;
            OnItemRemovedInternal(itemIndex);
            return Task.CompletedTask;
        }

        private void OnItemRemovedInternal(int itemIndex)
        {
            PerkSlotListener listener;
            lock (_listeners)
                if (!_listeners.TryGetValue(itemIndex, out listener))
                    return;

            listener.SetPerkSlotType(null);
        }

        //=== Class ===================================================================================================

        public class PerkSlotListener
        {
            public event PerkSlotsBaseFullApi.PerkSlotTypeChangedDelegate OnPerkSlotTypeChanged;

            private int _parentCollectionId;


            //=== Props =======================================================

            public int SlotIndex { get; }

            public ItemTypeResource PerkSlotType { get; private set; }

            public bool IsEmpty => PerkSlotType == null;


            //=== Ctor ========================================================

            public PerkSlotListener(int slotIndex, int parentCollectionId)
            {
                SlotIndex = slotIndex;
                _parentCollectionId = parentCollectionId;
            }


            //=== Public ======================================================

            public override string ToString()
            {
                return $"{nameof(PerkSlotListener)}[slc={_parentCollectionId}, idx={SlotIndex}]: " +
                       $"{(IsEmpty ? " EMPTY" : PerkSlotType.ToString())}";
            }

            public void SetPerkSlotType(ItemTypeResource perkSlotType, bool doEventCall = true)
            {
                if (perkSlotType == PerkSlotType)
                    return;

                PerkSlotType = perkSlotType;

                if (doEventCall)
                    UnityQueueHelper.RunInUnityThreadNoWait(FireEvent);
            }

            public void FireEvent()
            {
                OnPerkSlotTypeChanged?.Invoke(SlotIndex, PerkSlotType);
            }
        }
    }
}