using GeneratorAnnotations;
using System.Threading.Tasks;
using SharedCode.EntitySystem;
using SharedCode.Cloud;
using SharedCode.Entities.Telemetry;

namespace SharedCode.Entities.Service
{
    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.Client | CloudNodeType.Server)]
    public interface ITelemetryServiceEntity : IEntity
    {
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        Task<TelemetryEventResult> IndexEvent(TelemetryEvent telemetryEvent);
    }
}