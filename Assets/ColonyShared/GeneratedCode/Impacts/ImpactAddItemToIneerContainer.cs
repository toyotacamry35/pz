using System;
using ColonyShared.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Transactions;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using SharedCode.Wizardry;
using System.Linq;
using System.Threading.Tasks;
using SharedCode.Entities;
using System.Collections.Generic;
using SharedCode.Aspects.Item.Templates;

namespace Src.Impacts
{
    public class ImpactAddItemToIneerContainer : IImpactBinding<ImpactAddItemToIneerContainerDef>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactAddItemToIneerContainerDef indef)
        {
            var def = (ImpactAddItemToIneerContainerDef)indef;
            var spellCastWithSlotItem = (SpellCastWithSlotItem)cast.CastData;
            var targetRef = cast.Caster;
            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast, repo);
            if (!targetRef.IsValid)
                targetRef = cast.Caster;

            using (var ec = await repo.Get(targetRef.TypeId, targetRef.Guid))
            {
                var character = ec.Get<IWorldCharacterServer>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.Server);

                ISlotItemServer item = null;
                if (spellCastWithSlotItem.IsInventory)
                    character.Inventory.Items.TryGetValue(spellCastWithSlotItem.SlotId, out item);
                else
                    character.Doll.Items.TryGetValue(spellCastWithSlotItem.SlotId, out item);

                var resource = item.Item.ItemResource as ItemResource;
                if (resource == null)
                    return;

                if (Array.IndexOf(resource.ApplicableInnerItems, def.Item.Target) != -1)
                {
                    PropertyAddress innerContainerAddress = EntityPropertyResolver.GetPropertyAddress(item.Item.AmmoContainer);

                    if (item.Item.AmmoContainer.Items.Any())
                    {
                        var itemSlot = item.Item.AmmoContainer.Items[0];
                        if (itemSlot.Item.ItemResource != def.Item.Target)
                        {
                            var itemTransaction = new ItemRemoveBatchManagementTransaction(new List<RemoveItemBatchElement>() { new RemoveItemBatchElement(innerContainerAddress, 0, itemSlot.Stack, itemSlot.Item.Id) }, false, repo);
                            var itemRemoveResult = await itemTransaction.ExecuteTransaction();
                        }
                    }

                    var itemsInInnerContainer = item.Item.AmmoContainer.Items.Count > 0 ? item.Item.AmmoContainer.Items[0].Stack : 0;
                    var itemsToAdd = Math.Min(Math.Max(0, (resource.WeaponDef.Target?.MaxInnerStack ?? 0) - itemsInInnerContainer), def.Count);

                    var itemAddTransaction = new ItemAddBatchManagementTransaction(new List<ItemResourcePack>() {new ItemResourcePack(def.Item.Target, (uint)itemsToAdd) }, innerContainerAddress, false, repo);
                    //return (await itemAddTransaction.ExecuteTransaction()).IsSuccess;
                    await itemAddTransaction.ExecuteTransaction();
                }
            }

 //           return false;
        }
    }
}
