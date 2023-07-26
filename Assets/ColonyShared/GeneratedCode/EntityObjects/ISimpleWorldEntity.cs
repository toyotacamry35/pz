using GeneratorAnnotations;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;

namespace SharedCode.Entities.GameObjectEntities
{
    [GenerateDeltaObjectCode]
    public interface ISimpleWorldEntity : IWorldObject, IEntity, IHasSimpleMovementSync
    {
    }
}