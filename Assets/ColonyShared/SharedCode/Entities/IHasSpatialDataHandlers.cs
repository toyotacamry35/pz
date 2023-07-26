using SharedCode.EntitySystem;

namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface IHasSpatialDataHandlers
    {
        [ReplicationLevel(ReplicationLevel.Master)]
        ISpatialDataHandlers SpatialDatahandlers { get; set; }

        [RuntimeData(SkipField = true)]
        bool QuerySpatialData { get; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface ISpatialDataHandlers : IDeltaObject
    {
    }
}