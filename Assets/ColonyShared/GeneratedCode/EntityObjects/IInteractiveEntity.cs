using Assets.ColonyShared.SharedCode.Aspects.Item;
using Assets.ColonyShared.SharedCode.Wizardry;
using GeneratorAnnotations;
using SharedCode.AI;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;
using System.Threading.Tasks;

namespace Assets.ColonyShared.SharedCode.Entities
{
    [GenerateDeltaObjectCode]
    public interface IInteractiveEntity : IEntity, IInteractiveEntityInterface, IHasLifespan, IMountable
    {
    }

    public interface IInteractiveEntityInterface : IEntityObject, IWorldObject, IHasLootable, 
        IHasComputableStateMachine, IHasDestroyable, IHasMortal, IHasSimpleMovementSync, IHasSpawnedObject, IIsDummyLegionary, IHasBuffs, IHasLinksEngine
    {
    }

    [GenerateDeltaObjectCode]
    public interface ICorpseInteractiveEntity : IEntity, IInteractiveEntityInterface, IHasOwner, IHasDoll, IHasInventory, IHasOpenMechanics, IHasLifespan, IHasContainerApi, IOpenable
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        Task<ContainerItemOperation> MoveAllItems(PropertyAddress source, PropertyAddress destination);
    }
}
