using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;
using GeneratorAnnotations;
using SharedCode.Entities;
using SharedCode.Entities.Service;

namespace SharedCode.AI
{
    [GenerateDeltaObjectCode]
    public interface IPoiEntity : IEntity, IEntityObject, IHasSimpleMovementSync, IHasAutoAddToWorldSpace, IHasWorldSpaced, IIsDummyLegionary
    {
    }
}