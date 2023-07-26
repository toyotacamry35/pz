using SharedCode.EntitySystem;

namespace SharedCode.DeltaObjects.Building
{
    public interface IStats
    {
        [ReplicationLevel(ReplicationLevel.Always)]
        float Health { get; set; }
    }
}