using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using Assets.ColonyShared.SharedCode.Player;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.InputActions;
using Core.Environment.Logging.Extension;
using GeneratedCode.EntitySystem;
using JetBrains.Annotations;
using NLog;
using SharedCode.Aspects.Item.Templates;
using SharedCode.DeltaObjects;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Serializers;

namespace GeneratedCode.DeltaObjects
{
    [UsedImplicitly]
    public partial class InputActionHandlers : IHookOnReplicationLevelChanged, IHookOnDestroy
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
 
        private readonly Dictionary<int, UsedItemInfo> _itemsInUsedSlot = new Dictionary<int, UsedItemInfo>();
        private readonly Lazy<(InputActionLayersStack stack, CancellationTokenSource cts)> _stackHolder = new Lazy<(InputActionLayersStack,CancellationTokenSource)>(() => (new InputActionLayersStack(), new CancellationTokenSource()));

        public IInputActionLayersStack LayersStack => _stackHolder.Value.stack;

        public IInputActionBindingsSource BindingsSource => _stackHolder.Value.stack;

        private string Mark => parentEntity.IsMaster() ? "Master" : "Slave";
        
        public Task OnDestroy()
        {
            if (_stackHolder.IsValueCreated)
            {
                _stackHolder.Value.cts.Cancel();
                _stackHolder.Value.cts.Dispose();
                _stackHolder.Value.stack.Dispose();
            }
            return Task.CompletedTask;
        }
        
        public void OnReplicationLevelChanged(long oldReplicationMask, long newReplicationMask)
        {
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"OnReplicationLevelChanged({(ReplicationLevel)oldReplicationMask}, {(ReplicationLevel)newReplicationMask})").Write();
            if (!IsProperReplicationLevel(oldReplicationMask) && IsProperReplicationLevel(newReplicationMask))
            {
                var stack = _stackHolder.Value.stack;
                AsyncUtils.RunAsyncTask(() => InitializeHandlersStack(stack, newReplicationMask==(long)ReplicationLevel.Master), EntitiesRepository);
            }
            else
            if (IsProperReplicationLevel(oldReplicationMask) && !IsProperReplicationLevel(newReplicationMask))
            {
                if (_stackHolder.IsValueCreated && !_stackHolder.Value.cts.IsCancellationRequested)
                {
                    var ct = _stackHolder.Value.cts.Token;
                    var stack = _stackHolder.Value.stack;
                    AsyncUtils.RunAsyncTask(() => DeinitializeHandlersStack(stack, ct), EntitiesRepository);
                }
            }
        }

        private static bool IsProperReplicationLevel(long level)
        {
            return (level == (long) ReplicationLevel.ClientFull || level == (long) ReplicationLevel.Master);
        }
        
        private async Task InitializeHandlersStack(InputActionLayersStack stack, bool isMaster)
        {
            using (await parentEntity.GetThis())
            {
                stack.EntityId = ParentEntityId;
                stack.IsMaster = isMaster;
                
                var ownerDef = (parentEntity as IEntityObject)?.Def as IHasInputActionHandlersDef;
                if(ownerDef == null)
                    Logger.IfWarn()?.Message($"Def {(parentEntity as IEntityObject)?.Def.____GetDebugShortName()} of entity {parentEntity.Id} is not {nameof(IHasInputActionHandlersDef)}").Write();

                if (ownerDef?.InputActionHandlers != null)
                    foreach (var kv in ownerDef.InputActionHandlers)
                        stack.PushLayer(this, kv.Key, kv.Value);

                if (parentEntity is IHasDoll ownerHasDoll)
                {
                    for (int i = 0; i < ownerHasDoll.Doll.UsedSlots.Count; ++i)
                    {
                        var slotId = ownerHasDoll.Doll.UsedSlots[i];
                        var slotIdx = ItemHelper.TmpPlug_GetIndexBySlotResourceIdFull(slotId);
                        if (ownerHasDoll.Doll.Items.TryGetValue(slotIdx, out var slotItem))
                            if (slotItem.Item != null)
                                lock (_itemsInUsedSlot)
                                    ActivateItemInternal(slotItem.Item.Id, slotItem.Item.ItemResource, slotIdx);
                    }
                    ownerHasDoll.Doll.UsedSlots.OnItemAdded += OnUsedSlotAdded;
                    ownerHasDoll.Doll.UsedSlots.OnItemRemoved += OnUsedSlotRemoved;
                    ownerHasDoll.Doll.Items.OnItemAddedOrUpdated += OnItemChanged;
                    ownerHasDoll.Doll.Items.OnItemRemoved += OnItemChanged;
                }
            }
        }

        private async Task DeinitializeHandlersStack(InputActionLayersStack stack, CancellationToken ct)
        {
            if (!ct.IsCancellationRequested)
                using (await parentEntity.GetThis())
                {
                    if (parentEntity is IHasDoll ownerHasDoll)
                    {
                        ownerHasDoll.Doll.UsedSlots.OnItemAdded -= OnUsedSlotAdded;
                        ownerHasDoll.Doll.UsedSlots.OnItemRemoved -= OnUsedSlotRemoved;
                        ownerHasDoll.Doll.Items.OnItemAddedOrUpdated -= OnItemChanged;
                        ownerHasDoll.Doll.Items.OnItemRemoved -= OnItemChanged;
                        for (int i = 0; !ct.IsCancellationRequested && i < ownerHasDoll.Doll.UsedSlots.Count; ++i)
                        {
                            var slotId = ownerHasDoll.Doll.UsedSlots[i];
                            var slotIdx = ItemHelper.TmpPlug_GetIndexBySlotResourceIdFull(slotId);
                            if (ownerHasDoll.Doll.Items.TryGetValue(slotIdx, out var slotItem))
                                if (slotItem.Item != null)
                                    lock (_itemsInUsedSlot)
                                        DeactivateItemInternal(slotItem.Item.Id, slotItem.Item.ItemResource, slotIdx);
                        }
                    }

                    var ownerDef = (parentEntity as IEntityObject)?.Def as IHasInputActionHandlersDef;
                    if (ownerDef?.InputActionHandlers != null)
                        foreach (var kv in ownerDef.InputActionHandlers)
                            stack.DeleteLayer(this, kv.Key);
                }
        }

//        private async Task InitializeActionsStatesOverride()
//        {
//            if(_ownerMortal != null)
//                using (await _ownerEntity.GetThis())
//                {
//                    _ownerMortal.DieEvent += OnDie;
//                }
//        }

//        private async Task DeinitializeActionsStatesOverride()
//        {
//            if(_ownerMortal != null)
//                using (await _ownerEntity.GetThis())
//                {
//                    _ownerMortal.DieEvent -= OnDie;
//                }
//        }
        
        private async Task OnUsedSlotAdded(DeltaListChangedEventArgs<ResourceIDFull> args)
        {
            var slotIdx = ItemHelper.TmpPlug_GetIndexBySlotResourceIdFull(args.Value);
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{Mark} | Used slot added | Slot:{slotIdx} [{args.Value}]").Write();
            using(await parentEntity.GetThis())
            {
                if ((parentEntity as IHasDoll).Doll.Items.TryGetValue(slotIdx, out var slotItem))
                {
                    if (slotItem.Item != null)
                        lock(_itemsInUsedSlot)
                            if(!_itemsInUsedSlot.ContainsKey(slotIdx))
                                ActivateItemInternal(slotItem.Item.Id, slotItem.Item.ItemResource, slotIdx);
                }
                else
                    Logger.IfError()?.Message($"{Mark} | Item correspond to used slot {args.Value} was not found").Write();
            }            
        }

        private async Task OnUsedSlotRemoved(DeltaListChangedEventArgs<ResourceIDFull> args)
        {
            var slotIdx = ItemHelper.TmpPlug_GetIndexBySlotResourceIdFull(args.Value);
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{Mark} | Used slot removed | Slot:{slotIdx} [{args.Value}]").Write();
            using (await parentEntity.GetThis())
            {
                lock(_itemsInUsedSlot)
                    if (_itemsInUsedSlot.TryGetValue(slotIdx, out var itemInfo))
                        DeactivateItemInternal(itemInfo.ItemId, itemInfo.ItemDef, slotIdx);
            }
        }

        // При смене предмета в слоте, сначала приходит event об удалении предмета (в котором newItem==null), а потом отдельный event о добавлении предмета (в котром oldItem==null). 
        private async Task OnItemChanged(DeltaDictionaryChangedEventArgs<int, ISlotItem> args)
        {
            var oldItem = args.OldValue;
            var newItem = args.Value;
            var slotIdx = args.Key;

            if (oldItem?.Item?.Id == newItem?.Item?.Id)
                return;
            
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{Mark} | Slot item changed | SlotIdx:{slotIdx} OldItem:{oldItem?.Item?.ItemResource.____GetDebugAddress()}({oldItem?.Item?.Id}) NewItem:{newItem?.Item?.ItemResource.____GetDebugAddress()}({newItem?.Item?.Id})").Write();
            using (await parentEntity.GetThis())
            {
                if (oldItem?.Item != null)
                {
                    lock (_itemsInUsedSlot)
                        if (_itemsInUsedSlot.TryGetValue(args.Key, out var itemInfo))
                        {
                            if (oldItem.Item.Id != itemInfo.ItemId) throw new Exception($"{Mark} | Old item Id mismatch | oldItem.Item.Id:{oldItem.Item.Id} itemInfo.ItemId:{itemInfo.ItemId} slotIdx:{slotIdx}");
                            if (oldItem.Item.ItemResource != itemInfo.ItemDef) throw new Exception($"{Mark} | Old item Def mismatch | oldItem.Item.Id:{oldItem.Item.ItemResource.____GetDebugAddress()} itemInfo.ItemId:{itemInfo.ItemDef.____GetDebugAddress()} slotIdx:{slotIdx}");
                            DeactivateItemInternal(oldItem.Item.Id, oldItem.Item.ItemResource, slotIdx);
                        }
                }
                if (newItem?.Item != null && (parentEntity as IHasDoll).Doll.UsedSlots.Any(x => ItemHelper.TmpPlug_GetIndexBySlotResourceIdFull(x) == slotIdx))
                {
                    lock (_itemsInUsedSlot)
                    {
                        if (_itemsInUsedSlot.TryGetValue(slotIdx, out var itemInfo))
                            DeactivateItemInternal(itemInfo.ItemId, itemInfo.ItemDef, slotIdx);
                        ActivateItemInternal(newItem.Item.Id, newItem.Item.ItemResource, slotIdx);
                    }
                }
            }
        }

        private void ActivateItemInternal(Guid itemId, BaseItemResource itemDef, int slotIdx)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{Mark} | Activate item | Item:{itemDef.____GetDebugAddress()} Item.Id:{itemId} Slot:{slotIdx}").Write();

            ItemResource itemResourceDef = itemDef as ItemResource;
            if (itemResourceDef?.InputActionHandlers != null)
                foreach (var pair in itemResourceDef.InputActionHandlers)
                    LayersStack.PushLayer(itemId, pair.Key, pair.Value);
            _itemsInUsedSlot.Add(slotIdx, new UsedItemInfo { ItemId = itemId, ItemDef = itemDef});
        }

        private void DeactivateItemInternal(Guid itemId, BaseItemResource itemDef, int slotIdx)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{Mark} | Deactivate item | Item:{itemDef.____GetDebugAddress()} Item.Id:{itemId} Slot:{slotIdx}").Write();

            ItemResource itemResourceDef = itemDef as ItemResource;
            if (itemResourceDef?.InputActionHandlers != null)
                foreach (var pair in itemResourceDef.InputActionHandlers)
                    LayersStack.DeleteLayer(itemId, pair.Key);
            _itemsInUsedSlot.Remove(slotIdx);
        }

//        private Task OnDie(Guid arg1, int arg2)
//        {
//            //_inputActionsStatesOverride?.Clear;
//            return Task.CompletedTask;
//        }

        private struct UsedItemInfo
        {
            public Guid ItemId;
            public BaseItemResource ItemDef;
        }
    }
}