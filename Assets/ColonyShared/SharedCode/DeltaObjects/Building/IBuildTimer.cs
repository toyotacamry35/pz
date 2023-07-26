using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;

namespace SharedCode.DeltaObjects.Building
{
    public interface IBuildTimer
    {
        [ReplicationLevel(ReplicationLevel.Server)]
        ChainCancellationToken BuildToken { get; set; }

        [ReplicationLevel(ReplicationLevel.Always)]
        long BuildTimestamp { get; set; }

        [ReplicationLevel(ReplicationLevel.Always)]
        float BuildTime { get; set; }
    }
}
