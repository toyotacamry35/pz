using SharedCode.Aspects.Building;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Utils;

namespace SharedCode.DeltaObjects.Building
{
    public interface IPositionedBuild : IBuildTimer, IStats, IInteraction, IVisual, IHasId
    {
        [ReplicationLevel(ReplicationLevel.Always)]
        BuildState State { get; set; }

        [ReplicationLevel(ReplicationLevel.Always)]
        OuterRef<IEntity> Owner { get; set; }

        [ReplicationLevel(ReplicationLevel.Always)]
        BuildRecipeDef RecipeDef { get; set; }

        [ReplicationLevel(ReplicationLevel.Always)]
        Vector3 Position { get; set; }

        [ReplicationLevel(ReplicationLevel.Always)]
        Quaternion Rotation { get; set; }

        [ReplicationLevel(ReplicationLevel.Always)]
        int Depth { get; set; }

        [RuntimeData, ReplicationLevel(ReplicationLevel.Master)]
        IPositionHistory PositionHistory { get; set; }
    }
}
