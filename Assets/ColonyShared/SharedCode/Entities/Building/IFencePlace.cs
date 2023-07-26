using SharedCode.DeltaObjects.Building;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using System.Threading.Tasks;
using System;
using SharedCode.MovementSync;
using GeneratorAnnotations;
using SharedCode.MapSystem;

namespace SharedCode.Entities.Building
{
    [DatabaseSaveType(DatabaseSaveType.Explicit)]
    [GenerateDeltaObjectCode]
    public interface IFencePlace : IEntity, IWorldObject, IEntityObject, IHasBuildPlace, IBuildCollection, IHasSimpleMovementSync, IDatabasedMapedEntity
    {
        // - Fields -------------------------------------------------------------------------------
        [ReplicationLevel(ReplicationLevel.Always)]
        IDeltaDictionary<Guid, IPositionedFenceElement> Elements { get; }

        [ReplicationLevel(ReplicationLevel.Always)]
        IDeltaDictionary<Guid, IPositionedAttachment> Attachments { get; }

        [ReplicationLevel(ReplicationLevel.Always)]
        BuildState State { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> StateSet(BuildState value);
    }
}
