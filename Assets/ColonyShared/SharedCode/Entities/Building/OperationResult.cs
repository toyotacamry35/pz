using ProtoBuf;
using SharedCode.Aspects.Building;
using SharedCode.DeltaObjects.Building;
using System;

namespace SharedCode.Entities.Building
{
    public enum OperationType
    {
        None,
        Create,
        Damage,
        Interact,
        Remove,
        CheatDamage,
        CheatClaimResources,
        CheatDebug,
        WithRecipe,
    }

    public enum ErrorCode
    {
        None,
        Success,
        Error_UnknownError,
        Error_InvalidPlace,
        Error_InvalidElement,
        Error_InvalidElementDef,
        Error_InvalidCreateElementData,
        Error_InvalidOperationData,
        Error_InvalidPlaceId,
        Error_CantFindWorldSpaceEntity,
        Error_PlaceNotFound,
        Error_PlaceNotCreated,
        Error_PlaceNotStarted,
        Error_PlaceNotOwner,
        Error_PlacePositionNotAllowed,
        Error_FencePositionNotAllowed,
        Error_CreatePositionedBuild,
        Error_CheckResourcesNull,
        Error_CheckResourcesError,
        Error_CheckResourcesNotEnough,
        Error_ClaimResourcesNull,
        Error_ClaimResourcesError,
        Error_CheckElement,
        Error_StartElement,
        Error_OperateElement,
        Error_UnknownBuildType,
        Error_UnknownOperationType,
    }

    [ProtoContract]
    [ProtoInclude(10, typeof(DamageData))]
    [ProtoInclude(20, typeof(InteractData))]
    [ProtoInclude(30, typeof(RemoveData))]
    [ProtoInclude(40, typeof(CheatDamageData))]
    [ProtoInclude(50, typeof(CheatClaimResourcesData))]
    [ProtoInclude(60, typeof(CheatDebugData))]
    [ProtoInclude(70, typeof(OperationDataWithRecipe))]
    public class OperationData
    {
        [ProtoMember(1)]
        public OperationType Type { get; set; }
    }

    [ProtoContract]
    public class DamageData : OperationData
    {
        [ProtoMember(2)]
        public float Damage { get; set; }
        [ProtoMember(3)]
        public float DestructionPower { get;  set; }
    }

    [ProtoContract]
    public class InteractData : OperationData
    {
        [ProtoMember(2)]
        public int Interaction { get; set; }
    }

    [ProtoContract]
    public class RemoveData : OperationData
    {
    }

    [ProtoContract]
    public class CheatDamageData : OperationData
    {
        [ProtoMember(2)]
        public bool Enable { get; set; }

        [ProtoMember(3)]
        public float Value { get; set; }
    }

    [ProtoContract]
    public class CheatClaimResourcesData : OperationData
    {
        [ProtoMember(2)]
        public bool Enable { get; set; }

        [ProtoMember(3)]
        public bool Value { get; set; }
    }

    [ProtoContract]
    public class CheatDebugData : OperationData
    {
        [ProtoMember(2)]
        public bool Enable { get; set; }

        [ProtoMember(3)]
        public bool Verboose { get; set; }
    }

    [ProtoContract]
    public class OperationDataWithRecipe : OperationData
    {
        [ProtoMember(2)]
        public BuildRecipeDef Recipe { get; set; }
    }

    [ProtoContract]
    [ProtoInclude(10, typeof(OperationResultEx))]
    public class OperationResult
    {
        [ProtoMember(1)]
        public OperationType Type { get; set; }

        [ProtoMember(2)]
        public BuildType BuildType { get; set; }

        [ProtoMember(3)]
        public ErrorCode Result { get; set; }

        [ProtoMember(4)]
        public Guid ElementId { get; set; }
    }

    [ProtoContract]
    public class OperationResultEx : OperationResult
    {
        [ProtoMember(5)]
        public OperationData OperationData { get; set; }
    }
}
