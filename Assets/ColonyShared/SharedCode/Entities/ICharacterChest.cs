using ColonyShared.SharedCode.Entities;
using GeneratorAnnotations;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.MapSystem;

namespace SharedCode.Entities
{
    [GenerateDeltaObjectCode]
    [DatabaseSaveType(DatabaseSaveType.Explicit)]
    public interface ICharacterChest : IEntity, IMountable, IHasInventory, IHasOwner, IHasOpenMechanics, IHasContainerApi, IDatabasedMapedEntity, IHasStatsEngine, IHasHealth, IOpenable, IHasPersistentIncomingDamageMultiplier
    {
    }
}
