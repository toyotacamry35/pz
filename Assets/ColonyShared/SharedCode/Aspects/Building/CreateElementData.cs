using ProtoBuf;
using SharedCode.DeltaObjects.Building;
using SharedCode.Utils;
using System;
using SharedCode.Serializers.Protobuf;

namespace SharedCode.Aspects.Building
{
    [ProtoContract]
    [ProtoInclude(20, typeof(CreateBuildingElementData))]
    [ProtoInclude(30, typeof(CreateFenceElementData))]
    [ProtoInclude(40, typeof(CreateAttachmentData))]
    public class CreateBuildElementData
    {
        [ProtoMember(1)]
        public Vector3 Position { get; set; }
        [ProtoMember(2)]
        public Quaternion Rotation { get; set; }
        [ProtoMember(3)]
        public float Health { get; set; }
        [ProtoMember(4)]
        public int Visual { get; set; }
        [ProtoMember(5)]
        public int Interaction { get; set; }
    }

    [ProtoContract]
    [ProtoInclude(50, typeof(CreateBuildingPlaceAndBuildingElementData))]
    public class CreateBuildingElementData : CreateBuildElementData
    {
        [ProtoMember(6)]
        public Vector3Int Block { get; set; }
        [ProtoMember(7)]
        public BuildingElementType Type { get; set; }
        [ProtoMember(8)]
        public BuildingElementFace Face { get; set; }
        [ProtoMember(9)]
        public BuildingElementSide Side { get; set; }
    }

    [ProtoContract]
    public class CreateBuildingPlaceAndBuildingElementData : CreateBuildingElementData
    {
        [ProtoMember(10)]
        public Vector3 PlacePosition { get; set; }
        [ProtoMember(11)]
        public Quaternion PlaceRotation { get; set; }
    }

    [ProtoContract]
    public class CreateFenceElementData : CreateBuildElementData
    {
    }

    [ProtoContract]
    public class CreateAttachmentData : CreateBuildElementData
    {
        [ProtoMember(6)]
        public Guid ParentElementId { get; set; }
    }
}
