using GeneratorAnnotations;
using SharedCode.EntitySystem;

namespace Assets.ColonyShared.SharedCode.Entities.Service
{
    // Starts on Unity servers by "UnityNodeProto.jdb"
    [GenerateDeltaObjectCode]
    [EntityService]
    public interface INumenServiceEntity : IEntity
    {
    }
}