using System;
using System.Linq;
using System.Threading.Tasks;
using ColonyShared.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Transactions;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using SharedCode.Wizardry;

namespace Src.Impacts
{
    public class ImpactMoveItems : IImpactBinding<ImpactMoveItemsDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactMoveItemsDef def)
        {
            OuterRef<IEntity> fromRef = cast.Caster;
            if (def.From.Entity.Target != null)
                fromRef = await def.From.Entity.Target.GetOuterRef(cast, repo);

            if (!fromRef.IsValid)
                fromRef = cast.Caster;

            OuterRef<IEntity> toRef = cast.Caster;
            if (def.To.Entity.Target != null)
                toRef = await def.To.Entity.Target.GetOuterRef(cast, repo);

            if (!toRef.IsValid)
                toRef = cast.Caster;

            if (fromRef != null && toRef != null)
            {
                var fromSlotId = (def.From.Slot != default) ? def.From.Slot.Target.SlotId : def.From.SlotId;
                var toSlotId = (def.To.Slot != default) ? def.To.Slot.Target.SlotId : def.To.SlotId;

                PropertyAddress toAddress = await GetPropertyAddress(toRef, def.To.Container, -1, repo);
                PropertyAddress fromAddress = await GetPropertyAddress(fromRef, def.From.Container, fromSlotId, repo);

                if (toAddress != null && fromAddress != null)
                {
                    if (fromSlotId >= 0)
                    {
                        var transaction = new ItemMoveManagementTransaction(fromAddress, fromSlotId, toAddress, toSlotId, def.From.Count, Guid.Empty, false, repo);
                        var result = await transaction.ExecuteTransaction();
                        //return result.IsSuccess;
                    }
                    else
                    {
                        var transaction = new ItemMoveAllManagementTransaction(fromAddress, toAddress, false, false, repo);
                        var result = await transaction.ExecuteTransaction();
                        //return result.IsSuccess;
                    }
                }
            }
        }

        public static async Task<PropertyAddress> GetPropertyAddress(OuterRef<IEntity> entityRef, ContainerType containerType, int slot, IEntitiesRepository repo)
        {
            using (var fromWrapper = await repo.Get(entityRef))
            {
                switch (containerType)
                {
                    case ContainerType.Doll:
                        {
                            var entity = fromWrapper.Get<IHasDollServer>(entityRef.TypeId, entityRef.Guid, ReplicationLevel.Server);
                            if (slot < 0 || entity.Doll.Items.TryGetValue(slot, out var _))
                                return EntityPropertyResolver.GetPropertyAddress(entity.Doll);

                            break;
                        }
                    case ContainerType.Inventory:
                        {
                            var entity = fromWrapper.Get<IHasInventoryServer>(entityRef.TypeId, entityRef.Guid, ReplicationLevel.Server);
                            if (slot < 0 || entity.Inventory.Items.TryGetValue(slot, out var _))
                                return EntityPropertyResolver.GetPropertyAddress(entity.Inventory);

                            break;
                        }
                    default:
                        break;
                }
            }

            return default;
        }
    }
}
