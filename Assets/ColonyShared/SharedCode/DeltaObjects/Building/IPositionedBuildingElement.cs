using SharedCode.EntitySystem;
using SharedCode.Utils;

namespace SharedCode.DeltaObjects.Building
{
    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IPositionedBuildingElement: IDeltaObject, IPositionedBuild
    {
        [ReplicationLevel(ReplicationLevel.Always)]
        Vector3Int Block { get; set; }

        [ReplicationLevel(ReplicationLevel.Always)]
        BuildingElementType Type { get; set; }

        [ReplicationLevel(ReplicationLevel.Always)]
        BuildingElementFace Face { get; set; }

        [ReplicationLevel(ReplicationLevel.Always)]
        BuildingElementSide Side { get; set; }
    }
}
