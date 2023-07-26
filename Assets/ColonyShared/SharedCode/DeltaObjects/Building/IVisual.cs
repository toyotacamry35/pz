using SharedCode.EntitySystem;

namespace SharedCode.DeltaObjects.Building
{
    public interface IVisual
    {
        [ReplicationLevel(ReplicationLevel.Always)]
        int Visual { get; set; }
    }
}