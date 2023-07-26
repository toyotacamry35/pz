using GeneratorAnnotations;
using SharedCode.EntitySystem;
using SharedCode.MapSystem;

namespace SharedCode.Entities
{
    [GenerateDeltaObjectCode]
    public interface IWorldBox : IEntity, IMountable, IHasInventory, IHasOpenMechanics, IHasOwner, IHasContainerApi, IHasLimitedLifetime, IOpenable
    {
    }
}