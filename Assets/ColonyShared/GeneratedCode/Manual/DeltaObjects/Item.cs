using System;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.Aspects.Item.Templates;
using SharedCode.CustomData;
using SharedCode.DeltaObjects;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage;
using ColonyShared.SharedCode.Entities;
using Core.Environment.Logging.Extension;
using GeneratedCode.EntitySystem;

namespace GeneratedCode.DeltaObjects
{
    public partial class Item : IHookOnInit, IHookOnDatabaseLoad
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public Task OnInit()
        {
            Health.ZeroHealthEvent += Health_ZeroHealthEvent;
            return Task.CompletedTask;
        }

        public Task OnDatabaseLoad()
        {
            Health.ZeroHealthEvent += Health_ZeroHealthEvent;
            return Task.CompletedTask;
        }

        private async Task Health_ZeroHealthEvent(System.Guid arg1, int arg2)
        {
            if (!parentEntity.IsMaster())
                throw new System.InvalidOperationException("Should not be used on replica");
            
            //Logger.IfInfo()?.Message($"Health_ZeroHealthEvent on ({ItemResource.ItemName})").Write();
            if (await Health.GetMaxHealthAbsolute() <= 0)
                return;

            var entity = parentEntity;
            var container = this.GetParentItemsContainer();
            var currentItems = container.Items.Where(v => v.Value.Item.Id == Id);

            if (currentItems.Any())
            {
                var currentSlot = currentItems.First();
                var stack = currentSlot.Value.Stack;

                ItemPackDef[] newItems = (ItemResource as ItemResource).Durability.Target?.ItemsOnZeroDurability;
                if (newItems != null)
                {
                    using (var worldCharacterWrapper = await entity.EntitiesRepository.Get<IWorldCharacterServer>(entity.Id))
                    {
                        var worldCharacter = worldCharacterWrapper.Get<IWorldCharacterServer>(entity.Id);
                        PropertyAddress itemAddress = null;
                        if (!EntityPropertyResolver.TryGetPropertyAddress((IDeltaObject) container, out itemAddress))
                        {
                            Logger.IfError()?.Message("container itemAddress not found").Write();
                            return;
                        }
                        PropertyAddress inventoryAddress = EntityPropertyResolver.GetPropertyAddress(worldCharacter.Inventory);

                        if ((await worldCharacter.RemoveItem(itemAddress, currentSlot.Key, stack, Id)).IsSuccess)
                        {
                            var itemsList = new List<ItemResourcePack>();
                            foreach (var items in newItems)
                                itemsList.Add(new ItemResourcePack(items.Item, (uint)(items.Count * stack)));

                            if (!(await worldCharacter.AddItems(itemsList, itemAddress)).IsSuccess)
                                await worldCharacter.AddItems(itemsList, inventoryAddress);
                        }
                    }
                }
                else
                {
                    using (var worldCharacterWrapper = await entity.EntitiesRepository.Get<IWorldCharacterClientFull>(entity.Id))
                    {
                        var dollContainer = container as ICharacterDoll;

                        var worldCharacter = worldCharacterWrapper.Get<IWorldCharacterClientFull>(entity.Id);

                        if (dollContainer != null)
                        {
                            foreach (var usedSlot in dollContainer.UsedSlots.ToArray())
                            {
                                int index = ItemHelper.TmpPlug_GetIndexBySlotResourceIdFull(usedSlot);
                                if (index == currentSlot.Key)
                                    await dollContainer.RemoveUsedSlot(usedSlot);
                            }
                        }
                        
                        PropertyAddress dollAddress = null;
                        if (!EntityPropertyResolver.TryGetPropertyAddress(dollContainer, out dollAddress))
                        {
                            Logger.IfError()?.Message("dollAddress not found").Write();
                            return;
                        }
                        PropertyAddress inventoryAddress = EntityPropertyResolver.GetPropertyAddress(worldCharacter.Inventory);

                        var result = await worldCharacter.MoveItem(dollAddress, currentSlot.Key, inventoryAddress, -1, stack, Id);
                    }
                }
            }
        }
        
        public ValueTask<DamageResult> ReceiveDamageInternalImpl(Damage damage, Guid aggressorId, int aggressorTypeId)
        {
            return new ValueTask<DamageResult>(DamageResult.None);
        }

        public async ValueTask<bool> ChangeHealthInternalImpl(float deltaValue)
        {
            if (!parentEntity.IsMaster())
                throw new System.InvalidOperationException("Should not be used on replica");
            
            using (var cnt = await parentEntity.EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
            {
                var hasFaction = cnt.Get<IHasFaction>(parentEntity.TypeId, parentEntity.Id); 
                return hasFaction != null && hasFaction.Faction.UnbreakableItems;
            }
        }
        
    }

    public static class ItemExtensions
    {
        public static IItemsContainer GetParentItemsContainer(this Item item)
        {
            return ((IItem)item).GetParentItemsContainer();
        }

        public static IItemsContainer GetParentItemsContainer(this IItem item)
        {
            return (IItemsContainer)((IDeltaObjectExt)item)?.GetParentObject()?.GetParentObject()?.GetParentObject();
        }
    }
}
