using GeneratedCode.MapSystem;
using GeneratorAnnotations;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.MapSystem;
using System.Threading.Tasks;
using ColonyShared.SharedCode.Entities;

namespace SharedCode.Entities
{
    [DatabaseSaveType(DatabaseSaveType.Explicit)]
    [GenerateDeltaObjectCode]
    public interface IWorldBaken : IEntity, IMountable, IHasOwner, IDatabasedMapedEntity, IHasStatsEngine, IHasHealth, IScenicEntity, IHasPersistentIncomingDamageMultiplier
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] long ReadyTimeUTC0InMilliseconds { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] Task SetCooldown();
               
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] Task<float> GetVerticalSpawnPointDistance();
    }
}