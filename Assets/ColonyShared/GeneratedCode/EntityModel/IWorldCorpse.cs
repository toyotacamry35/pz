using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;

namespace SharedCode.Entities
{
    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IWorldCorpse : IEntity, IEntityObjectWithCustomUnityInstantiation, IWorldObject, IHasInventory, IHasDoll, IHasOpenMechanics, IHasOwner, IHasContainerApi, IHasLimitedLifetime, IHasSimpleMovementSync, IOpenable
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        Task<ContainerItemOperation> MoveAllItems(PropertyAddress source, PropertyAddress destination);
        
        [ReplicationLevel(ReplicationLevel.Always)]
        bool VisibleOnlyForOwner { get; set; }
    }
}