using ColonyShared.ManualDefsForSpells;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using System.Threading.Tasks;

namespace Assets.Src.Aspects.Doings
{
    public static class BotActions2Support
    {
        public static async Task<PropertyAddress> GetPropertyAddress(OuterRef<IEntity> entityRef, ContainerType containerType, IEntitiesRepository repo)
        {
            using (var fromWrapper = await repo.Get(entityRef))
            {
                switch (containerType)
                {
                    case ContainerType.Doll:
                        {
                            var entity = fromWrapper.Get<IHasDollClientFull>(entityRef.TypeId, entityRef.Guid, ReplicationLevel.ClientFull);
                            return EntityPropertyResolver.GetPropertyAddress(entity.Doll);
                        }
                    case ContainerType.Inventory:
                        {
                            var entity = fromWrapper.Get<IHasInventoryClientFull>(entityRef.TypeId, entityRef.Guid, ReplicationLevel.ClientFull);
                            return EntityPropertyResolver.GetPropertyAddress(entity.Inventory);
                        }
                    default:
                        break;
                }
            }

            return default;
        }
    }
}
