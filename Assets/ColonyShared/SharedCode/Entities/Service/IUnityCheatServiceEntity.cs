using System;
using System.Threading.Tasks;
using GeneratorAnnotations;
using SharedCode.Cloud;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Utils;

namespace Assets.ColonyShared.SharedCode.Entities.Service
{
    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.Server)]
    public interface IUnityCheatServiceEntity : IEntity
    {
        // Emulation GC on server
        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task MainUnityThreadOnServerSleep(bool isOn, float sleepTime, float delayBeforeSleep, float repeatTime);

        /// <param name="enabledStatus">Activete|Inactivete</param>
        /// <param name="dump">SaveToFile if Inactivete</param>
        /// <param name="serverOnly">Don't trigger event on the clients replicas</param>
        /// <returns></returns>
        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task SetCurveLoggerState(bool enabledStatus, bool dump, string loggerName, Guid dumpId);

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task<Transform> GetClosestPlayerSpawnPointTransform(Vector3 pos);
    }
}
