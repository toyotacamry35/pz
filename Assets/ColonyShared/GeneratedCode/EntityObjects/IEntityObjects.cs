using SharedCode.Cloud;
using SharedCode.EntitySystem;
using GeneratorAnnotations;

namespace SharedCode.Entities.GameObjectEntities
{
    // Service entity
    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.None, addedByDefaultToNodeType: CloudNodeType.Client)]
    public interface IEntityObjects : IEntity
    {
    }
}