using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using MongoDB.Bson.Serialization.Attributes;
using SharedCode.EntitySystem;

namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface IHasSpecificStats
    {
        [RuntimeData(SkipField = true), ReplicationLevel(ReplicationLevel.ClientFull), BsonIgnore] ItemSpecificStats SpecificStats { get; }
    }
}
