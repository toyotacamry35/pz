using ProtoBuf;
using SharedCode.EntitySystem;
using System;

namespace Assets.ColonyShared.SharedCode.Aspects.Item
{
    [Flags]
    public enum ContainerItemOperationResult
    {
        None,
        Success,

        ErrorSrcNotFound,
        ErrorDstNotFound,
        ErrorSrcCantAdd,
        ErrorDstCantAdd,
        ErrorSrcCantRemove,
        ErrorDstCantRemove,
        ErrorSrcIncorrectSlotId,
        ErrorDstIncorrectSlotId,
        ErrorSrcSlotLockedByOtherTransaction,
        ErrorDstSlotLockedByOtherTransaction,

        ErrorDstIsFull,
        ErrorSrcIsFull,
        ErrorDstStackIsFull,
        ErrorSrcAndDstIsSame,

        ErrorSrcClientMismatch,
        ErrorChangeTransactionNotFound,
        ErrorUnknown
    }

    public static class ContainerItemOperationResultExtensions
    {
        public static bool IsSuccess(this ContainerItemOperationResult check)
        {
            return check == ContainerItemOperationResult.Success;
        }
    }

    [ProtoContract]
    public class ContainerItemOperation
    {
        [ProtoMember(1)]
        public int ItemsCount = 0;
        [ProtoMember(2)]
        public ContainerItemOperationResult Result = ContainerItemOperationResult.None;

        public bool IsSuccess => Result.IsSuccess();

        [ProtoIgnore]
        public static readonly ContainerItemOperation Error = new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorUnknown };

        [ProtoIgnore]
        public static readonly ContainerItemOperation None = new ContainerItemOperation() { Result = ContainerItemOperationResult.None };

        public override string ToString()
        {
            return $"ItemsCount = {ItemsCount}; Result = {Result}";
        }
    }
}
