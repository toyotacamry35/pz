using GeneratorAnnotations;
using SharedCode.Cloud;
using SharedCode.EntitySystem;
using System.Threading.Tasks;

namespace SharedCode.Entities
{
    [EntityService(replicateToNodeType: CloudNodeType.Server | CloudNodeType.Client)]
    [GenerateDeltaObjectCode]
    public interface ITimeSyncerEntity : IEntity
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        long LastServerTime { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task<bool> UpdateTime();

        [EntityMethodCallType(EntityMethodCallType.Immutable)]//NOT local immutable
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task<long> Ping();
    }
}
