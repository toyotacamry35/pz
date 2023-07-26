using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneratorAnnotations;
using ProtoBuf;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.EntitySystem.Delta;

namespace SharedCode.Entities.Core
{
    [GenerateDeltaObjectCode]
    public interface IEntityMethodsCallsChain: IDeltaObject, IHasId
    {
        [ReplicationLevel(ReplicationLevel.Master)]
        IChainContext ChainContext { get; }

        [ReplicationLevel(ReplicationLevel.Master)]
        IDeltaList<ChainBlockBase> Chain { get; }

        [ReplicationLevel(ReplicationLevel.Master)]
        int CurrentChainIndex { get; }

        [ReplicationLevel(ReplicationLevel.Master)]
        long NextTimeToCall { get; }

        [ReplicationLevel(ReplicationLevel.Master)]
        IDeltaList<Guid> ForksIds { get; }

        [ReplicationLevel(ReplicationLevel.Master)]
        Guid ForkCreatorId { get; }

        [ReplicationLevel(ReplicationLevel.Master)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task<ChainBlockBase> GetCurrentChainBlock();

        [ReplicationLevel(ReplicationLevel.Master)]
        Task<NextEntityToCallResult> TryGetNextEntityToCall();

        [ReplicationLevel(ReplicationLevel.Master)]
        Task<IEntityMethodsCallsChain> CreateFork(int index);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task ForkFinished(Guid forkId);

        [ReplicationLevel(ReplicationLevel.Master)]
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        Task<string> GetDescription();

        [ReplicationLevel(ReplicationLevel.Master)]
        Task SetNextTimeToCall(long nextTimeToCall);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task IncrementCurrentChainIndex();

        [ReplicationLevel(ReplicationLevel.Master)]
        Task DecrementCurrentChainIndex();
    }

    [ProtoContract]
    public struct NextEntityToCallResult
    {
        [ProtoMember(1)]
        public bool Result;

        [ProtoMember(2)]
        public int TypeID;

        [ProtoMember(3)]
        public Guid EntityId;
    }
}
