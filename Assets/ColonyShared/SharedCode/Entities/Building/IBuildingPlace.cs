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
    public interface IBuildingPlace: IEntity, IEntityObject, IWorldObject, IBuildTimer, IHasBuildPlace, IBuildCollection, IVisual, IHasSimpleMovementSync, IDatabasedMapedEntity, IHasOwner
    {
        // - Fields -------------------------------------------------------------------------------
        [ReplicationLevel(ReplicationLevel.Always)]
        BuildState State { get; set; }

        [ReplicationLevel(ReplicationLevel.Always)]
        bool RemoveIfEmpty { get; set; }

        [ReplicationLevel(ReplicationLevel.Always)]
        IDeltaDictionary<Guid, IPositionedBuildingElement> Elements { get; }

        [ReplicationLevel(ReplicationLevel.Always)]
        IDeltaDictionary<Guid, IPositionedAttachment> Attachments { get; }

        // - Setters ------------------------------------------------------------------------------
        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> StateSet(BuildState value);

        // - Place manipulation -------------------------------------------------------------------
        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> StartPlace(bool instant);

        //TODO building, remove this method
        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> CancelPlace();

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> OnProgressPlace();
    }
}
