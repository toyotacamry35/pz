using GeneratorAnnotations;
using SharedCode.EntitySystem;
using SharedCode.MapSystem;

namespace SharedCode.Entities
{
    [GenerateDeltaObjectCode]
    [DatabaseSaveType(DatabaseSaveType.Explicit)]
    public interface IBankCell : IEntity, IHasOwner, IHasInventory, IHasOpenMechanics, IHasContainerApi, IOpenable
    {
    }
}
