using System;
using System.Threading.Tasks;
using GeneratorAnnotations;
using SharedCode.Cloud;
using SharedCode.EntitySystem;

namespace Assets.ColonyShared.SharedCode.Entities.Service
{
    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.Server | CloudNodeType.Client)]
    public interface IInGameTimeServiceEntity : IEntity
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]  DateTime ServerStartTime         { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]  DateTime ServerStartupIngameTime { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]  TimeSpan IngameTimeDayDuration   { get; set; }

        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]  Task<DateTime> GetCurrentIngameTime();
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]  Task<bool> SetCurrentIngameTime(DateTime time);


        [ReplicationLevel(ReplicationLevel.Server)] Task<bool> SetTimeFromRealm(DateTime serverStartTime);
    }

}
