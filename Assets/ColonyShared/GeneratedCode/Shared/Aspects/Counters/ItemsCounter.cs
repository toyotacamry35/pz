using Assets.Src.Aspects.Impl.Factions.Template;
using ColonyShared.SharedCode.Aspects.Counters.Template;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratorAnnotations;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratedCode.DeltaObjects
{
    [GenerateDeltaObjectCode]
    public interface IItemsCounter : IQuestCounter
    {
        [ReplicationLevel(ReplicationLevel.Server)] Task SetCount(int count);
    }
    
    public partial class ItemsCounter
    {
        private ItemsCounterDef Def => (ItemsCounterDef)CounterDef;
        private bool _counterCompleted = false;
        private PropertyChangedDelegate _stackPropertyChangedFn;

        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        protected override void constructor()
        {
            base.constructor();
            _stackPropertyChangedFn = Stack_PropertyChanged;
        }
        
        public Task OnInitImpl(QuestDef questDef, QuestCounterDef counterDef, IEntitiesRepository repository)
        {
            QuestDef = questDef;
            CounterDef = counterDef;
            _counterCompleted = false;
            return Task.CompletedTask;
        }

        public async Task OnDatabaseLoadImpl(IEntitiesRepository repository)
        {
            await Subscribe(repository, true);
        }

        public Task SetCountImpl(int count)
        {
            Count = CountForClient = count;
            return Task.CompletedTask;
        }

        private void SubscribeContainer(IDeltaDictionary<int, ISlotItemServer> items, SourceType containerFlag, bool subscribe)
        {
            if (Def.SourceType.CheckFlag(containerFlag))
                foreach (var item in items)
                    if (Def.AllTargets.Contains(item.Value.Item.ItemResource))
                        if (subscribe)
                            item.Value.SubscribePropertyChanged(nameof(item.Value.Stack), _stackPropertyChangedFn);
                        else
                            item.Value.UnsubscribePropertyChanged(nameof(item.Value.Stack), _stackPropertyChangedFn);

            SubscribeToInnerContainer(items, subscribe);
        }

        private void SubscribeToInnerContainer(IDeltaDictionary<int, ISlotItemServer> items, bool subscribe)
        {
            if (Def.SourceType.CheckFlag(SourceType.ItemInventory))
                foreach (var item in GetItemsThatCanContainTargetSubitems(items))
                {
                    SubscribeToInnerContainer(item.Value, subscribe);
                }
        }

        private void SubscribeToInnerContainer(ISlotItemServer item, bool subscribe)
        {
            if (subscribe)
            {
                item.Item.AmmoContainer.Items.OnItemAddedOrUpdated += Items_OnItemAddedOrUpdated;
                item.Item.AmmoContainer.Items.OnItemRemoved += Items_OnItemRemoved;
            }
            else
            {
                item.Item.AmmoContainer.Items.OnItemAddedOrUpdated -= Items_OnItemAddedOrUpdated;
                item.Item.AmmoContainer.Items.OnItemRemoved -= Items_OnItemRemoved;
            }

            foreach (var subitem in item.Item.AmmoContainer.Items.Where(v => Def.AllTargets.Contains(v.Value.Item.ItemResource)))
                if (subscribe)
                    subitem.Value.SubscribePropertyChanged(nameof(subitem.Value.Stack), Stack_PropertyChanged);
                else
                    subitem.Value.UnsubscribePropertyChanged(nameof(subitem.Value.Stack), Stack_PropertyChanged);
        }

        private async Task Items_OnItemAddedOrUpdated(DeltaDictionaryChangedEventArgs<int, ISlotItemServer> args)
        {
            if (_counterCompleted)
                return;

            if (parentEntity == null)
                return;

            using (var wrapper = await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
            {
                using (var sender = await EntitiesRepository.Get(args.Sender.ParentTypeId, args.Sender.ParentEntityId))
                {
                    if (Def.Source.Target == args.Value.Item.ItemResource || Def.AllTargets.Contains(args.Value.Item.ItemResource))
                    {
                        args.Value.SubscribePropertyChanged(nameof(args.Value.Stack), _stackPropertyChangedFn);
                        if (IsTargetSubitems(args.Value))
                            SubscribeToInnerContainer(args.Value, true);

                        await CheckItemsCount(wrapper);
                    }
                }
            }
        }

        private async Task Items_OnItemRemoved(DeltaDictionaryChangedEventArgs<int, ISlotItemServer> args)
        {
            if (_counterCompleted)
                return;

            if (parentEntity == null)
                return;

            using (var wrapper = await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
            {
                using (var sender = await EntitiesRepository.Get(args.Sender.ParentTypeId, args.Sender.ParentEntityId))
                {
                    if (Def.Source.Target == args.Value.Item.ItemResource || Def.AllTargets.Contains(args.Value.Item.ItemResource))
                    {
                        args.OldValue.UnsubscribePropertyChanged(nameof(args.OldValue.Stack), _stackPropertyChangedFn);
                        if (IsTargetSubitems(args.OldValue))
                            SubscribeToInnerContainer(args.OldValue, false);

                        await CheckItemsCount(wrapper);
                    }
                }
            }
        }

        private async Task Stack_PropertyChanged(EntityEventArgs args)
        {
            if (_counterCompleted)
                return;

            using (var wrapper = await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
            {
                using (var sender = await EntitiesRepository.Get(args.Sender.ParentTypeId, args.Sender.ParentEntityId))
                {
                    await CheckItemsCount(wrapper);
                }
            }
        }

        private async Task CheckItemsCount(IEntitiesContainer entityWrapper)
        {
            if (_counterCompleted)
                return;

            if (entityWrapper == null)
                return;

            int itemCount = 0;


            if ( (Def.SourceType.CheckFlag(SourceType.PlayerInventory) && Def.HaveTargets) || (Def.SourceType.CheckFlag(SourceType.ItemInventory) && Def.HaveSource) )
            {
                var entity = entityWrapper.Get<IHasInventoryServer>(parentEntity.TypeId, parentEntity.Id, ReplicationLevel.Server);
                if (entity != null)
                    itemCount += CalcEntityContainItems(entity.Inventory.Items, SourceType.PlayerInventory);
            }

            if ((Def.SourceType.CheckFlag(SourceType.PlayerDoll) && Def.HaveTargets) || (Def.SourceType.CheckFlag(SourceType.ItemInventory) && Def.HaveSource))
            {
                var entity = entityWrapper.Get<IHasDollServer>(parentEntity.TypeId, parentEntity.Id, ReplicationLevel.Server);
                if (entity != null)
                    itemCount += CalcEntityContainItems(entity.Doll.Items, SourceType.PlayerDoll);
            }
            if (SourceType.PerksEverywhere.CheckFlag(Def.SourceType))
            {
                var entity = entityWrapper.Get<IHasPerksServer>(parentEntity.TypeId, parentEntity.Id, ReplicationLevel.Server);
                if (entity != null)
                {

                    if(Def.SourceType.CheckFlag(SourceType.PerksTemporary))
                        itemCount += CalcEntityContainItems(entity.TemporaryPerks.Items, SourceType.PerksTemporary);
                    if (Def.SourceType.CheckFlag(SourceType.PerksPermanent))
                        itemCount += CalcEntityContainItems(entity.PermanentPerks.Items, SourceType.PerksPermanent);
                    if (Def.SourceType.CheckFlag(SourceType.PerksSaved))
                        itemCount += CalcEntityContainItems(entity.SavedPerks.Items, SourceType.PerksSaved);
                }
            }

            if (itemCount != Count || Def.Less)
            {
                if (Def.Less)
                    if (itemCount < Def.Count)
                        await SetCount(Def.Count);
                    else
                        return;
                else
                    await SetCount(Math.Min(itemCount, Def.Count));

              
                if (!PreventOnComplete && Count >= Def.Count)
                {
                    _counterCompleted = true;
                    await Subscribe(EntitiesRepository, false);
                    await OnOnCounterChangedInvoke(this);
                    await OnOnCounterCompletedInvoke(QuestDef, this);
                    return;
                }
                await OnOnCounterChangedInvoke(this);
            }
        }

        private int CalcEntityContainItems(IDeltaDictionary<int, ISlotItemServer> items, SourceType containerFlag)
        {
            int result = 0;
            if (Def.SourceType.CheckFlag(containerFlag))
                result += items
                    .Where(v => Def.AllTargets.Contains(v.Value.Item.ItemResource))
                    .Sum(v => v.Value.Stack);

            if (Def.SourceType.CheckFlag(SourceType.ItemInventory))
                result += GetItemsThatCanContainTargetSubitems(items)
                    .Sum(v => v.Value.Stack * v.Value.Item.AmmoContainer?.Items
                        .Where(a => Def.AllTargets.Contains(a.Value.Item.ItemResource))
                        .Sum(a => a.Value.Stack) ?? 0);

            return result;
        }

        private IEnumerable<KeyValuePair<int, ISlotItemServer>> GetItemsThatCanContainTargetSubitems(IDeltaDictionary<int, ISlotItemServer> items)
        {
            return items.Where(v => IsTargetSubitems(v.Value));
        }

        private bool IsTargetSubitems(ISlotItemServer item)
        {
            return Def.HaveSource ? (item.Item.ItemResource == Def.Source) : true;
        }

        public async Task OnDestroyImpl(IEntitiesRepository repository)
        {
            if( ! _counterCompleted)
                await Subscribe(repository, false);

        }
        

        private async Task Subscribe(IEntitiesRepository repository, bool subscribe)
        {
            using (var wrapper = await repository.Get(parentEntity.TypeId, parentEntity.Id))
            {
                if (Def.SourceType.CheckFlag(SourceType.PlayerInventory) || (Def.SourceType.CheckFlag(SourceType.ItemInventory) && Def.HaveSource))
                {
                    var entity = wrapper.Get<IHasInventoryServer>(parentEntity.TypeId, parentEntity.Id, ReplicationLevel.Server);
                    if (entity == null)
                        return;

                    if (subscribe)
                    {
                        entity.Inventory.Items.OnItemAddedOrUpdated += Items_OnItemAddedOrUpdated;
                        entity.Inventory.Items.OnItemRemoved += Items_OnItemRemoved;
                    }
                    else
                    {
                        entity.Inventory.Items.OnItemAddedOrUpdated -= Items_OnItemAddedOrUpdated;
                        entity.Inventory.Items.OnItemRemoved -= Items_OnItemRemoved;
                    }

                    SubscribeContainer(entity.Inventory.Items, SourceType.PlayerInventory, subscribe);
                }

                if (Def.SourceType.CheckFlag(SourceType.PlayerDoll) || (Def.SourceType.CheckFlag(SourceType.ItemInventory) && Def.HaveSource))
                {
                    var entity = wrapper.Get<IHasDollServer>(parentEntity.TypeId, parentEntity.Id, ReplicationLevel.Server);
                    if (entity == null)
                        return;

                    if (subscribe)
                    {
                        entity.Doll.Items.OnItemAddedOrUpdated += Items_OnItemAddedOrUpdated;
                        entity.Doll.Items.OnItemRemoved += Items_OnItemRemoved;
                    }
                    else
                    {
                        entity.Doll.Items.OnItemAddedOrUpdated -= Items_OnItemAddedOrUpdated;
                        entity.Doll.Items.OnItemRemoved -= Items_OnItemRemoved;
                    }

                    SubscribeContainer(entity.Doll.Items, SourceType.PlayerDoll, subscribe);
                }

                if (SourceType.PerksEverywhere.CheckFlag(Def.SourceType))
                {
                    var entity = wrapper.Get<IHasPerksServer>(parentEntity.TypeId, parentEntity.Id, ReplicationLevel.Server);
                    if (entity == null)
                        return;

                    if (subscribe)
                        PerksSubscribe(entity);
                    else
                        PerksUnsubscribe(entity);
                }


                if (subscribe)
                    await CheckItemsCount(wrapper);
            }
        }
        private void PerksSubscribe(IHasPerksServer entity)
        {
            if (Def.SourceType.CheckFlag(SourceType.PerksTemporary))
            {
                entity.TemporaryPerks.Items.OnItemAddedOrUpdated += Items_OnItemAddedOrUpdated;
                entity.TemporaryPerks.Items.OnItemRemoved += Items_OnItemRemoved;
            }
            if (Def.SourceType.CheckFlag(SourceType.PerksPermanent))
            {
                entity.PermanentPerks.Items.OnItemAddedOrUpdated += Items_OnItemAddedOrUpdated;
                entity.PermanentPerks.Items.OnItemRemoved += Items_OnItemRemoved;
            }
            if (Def.SourceType.CheckFlag(SourceType.PerksSaved))
            {
                entity.SavedPerks.Items.OnItemAddedOrUpdated += Items_OnItemAddedOrUpdated;
                entity.SavedPerks.Items.OnItemRemoved += Items_OnItemRemoved;
            }
        }
        private void PerksUnsubscribe(IHasPerksServer entity)
        {
            if (Def.SourceType.CheckFlag(SourceType.PerksTemporary))
            {
                entity.TemporaryPerks.Items.OnItemAddedOrUpdated -= Items_OnItemAddedOrUpdated;
                entity.TemporaryPerks.Items.OnItemRemoved -= Items_OnItemRemoved;
            }
            if (Def.SourceType.CheckFlag(SourceType.PerksPermanent))
            {
                entity.PermanentPerks.Items.OnItemAddedOrUpdated -= Items_OnItemAddedOrUpdated;
                entity.PermanentPerks.Items.OnItemRemoved -= Items_OnItemRemoved;
            }
            if (Def.SourceType.CheckFlag(SourceType.PerksSaved))
            {
                entity.SavedPerks.Items.OnItemAddedOrUpdated -= Items_OnItemAddedOrUpdated;
                entity.SavedPerks.Items.OnItemRemoved -= Items_OnItemRemoved;
            }
        }

        public Task PreventOnCompleteEventImpl()
        {
            PreventOnComplete = true;
            return Task.CompletedTask;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("ItemCounter: Count = {0}, Def = {1}, Items = ", Count, Def.____GetDebugShortName());
            foreach (var target in Def.AllTargets)
                sb.AppendFormat("{0}; ", target.____GetDebugShortName());
            return sb.ToString();
        }
    }
}
