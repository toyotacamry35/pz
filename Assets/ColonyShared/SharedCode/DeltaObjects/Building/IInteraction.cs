using SharedCode.EntitySystem;

namespace SharedCode.DeltaObjects.Building
{
    public interface IInteraction
    {
        [ReplicationLevel(ReplicationLevel.Always)]
        int Interaction { get; set; }
    }
}
