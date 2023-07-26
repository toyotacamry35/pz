using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using SharedCode.EntitySystem;
using Src.Aspects.Impl.Stats.Proxy;
using GeneratorAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Src.Aspects.Impl.Stats
{
    [GenerateDeltaObjectCode]
    public interface IProceduralStat : IStat, IDeltaObject
    {
        [BsonIgnore, ReplicationLevel(ReplicationLevel.ClientBroadcast)] float ValueCache { get; set; }
    }
}
