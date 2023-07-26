using Assets.ColonyShared.SharedCode.Aspects.Item;
using ResourcesSystem.Loader;
using ColonyHelpers;
using ColonyShared.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Transactions;
using SharedCode.Aspects.Item.Templates;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Src.Impacts
{
    public class ImpactEquipItems : IImpactBinding<ImpactEquipItemsDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactEquipItemsDef def)
        {
            var targetRef = cast.Caster;

            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast, repo);

            if (!targetRef.IsValid)
                targetRef = cast.Caster;

            var equipped = false;
            var dollSlotsList = GameResourcesHolder.Instance.LoadResource<SlotsListDef>("/UtilPrefabs/Slots/CharacterDollSlots");
            using (var wrapper = await repo.Get(targetRef.TypeId, targetRef.Guid))
            {
                var hasDoll = wrapper?.Get<IHasDollClientBroadcast>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.ClientBroadcast);
                var hasInventory = wrapper?.Get<IHasInventoryClientFull>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.ClientFull);

                var fromAddress = EntityPropertyResolver.GetPropertyAddress(hasInventory.Inventory);
                var toAddress = EntityPropertyResolver.GetPropertyAddress(hasDoll.Doll);

                var slotItems = hasInventory.Inventory.Items.Where(v => def.Items.Any(x => x.Target == v.Value.Item.ItemResource)).Shuffle();
                foreach (var slotItem in slotItems)
                {
                    Guid fromItemGuid = slotItem.Value.Item.Id;
                    var allowerSlotIds = dollSlotsList.Slots
                        .Where(v => v.Target.AcceptsItems
                            .Any(x => x == slotItem.Value.Item.ItemResource.ItemType.Target))
                        .Select(v => v.Target.SlotId);

                    foreach (var slotId in allowerSlotIds)
                    {
                        if (hasDoll.Doll.Items.ContainsKey(slotId))
                            continue;

                        var itemTransaction = new ItemMoveManagementTransaction(fromAddress, slotItem.Key, toAddress, slotId, def.Count, fromItemGuid, false, repo);
                        ContainerItemOperation res = await itemTransaction.ExecuteTransaction();
                        equipped |= res.IsSuccess;
                        if (res.IsSuccess)
                            break;
                    }
                }
            }
            //
            // return equipped;
        }
    }
}