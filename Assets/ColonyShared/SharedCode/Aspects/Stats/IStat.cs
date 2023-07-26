using Assets.Src.Aspects.Impl.Stats;
using MongoDB.Bson.Serialization.Attributes;
using SharedCode.Entities;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using Src.Aspects.Impl.Stats.Proxy;
using System.Threading.Tasks;

namespace Src.Aspects.Impl.Stats
{
    public interface IStat
    {
        [BsonIgnore, ReplicationLevel(ReplicationLevel.ClientBroadcast)] float LimitMinCache { get; set; }
        [BsonIgnore, ReplicationLevel(ReplicationLevel.ClientBroadcast)] float LimitMaxCache { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] StatType StatType { get; set; }

        [ReplicationLevel(ReplicationLevel.Master)] ValueTask Initialize(StatDef statDef, bool resetState);
        [ReplicationLevel(ReplicationLevel.Master)] ValueTask<bool> RecalculateCaches(bool calcersOnly);
        [ReplicationLevel(ReplicationLevel.ClientBroadcast), EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] ValueTask<float> GetValue();        
    }
}
