using System.Threading.Tasks;
using ColonyShared.SharedCode.Reactions;
using ResourceSystem.Reactions;
using SharedCode.EntitySystem;

namespace ColonyShared.SharedCode.Entities.Reactions
{
    public interface IHasReactionsOwner
    {
        [ReplicationLevel(ReplicationLevel.Master)]
        IReactionsOwner ReactionsOwner { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IReactionsOwner : IDeltaObject
    {
        [RuntimeData(SkipField = true), ReplicationLevel(ReplicationLevel.Master)]
        IReactionHandlersStack ReactionHandlersStack { get; }

        [ReplicationLevel(ReplicationLevel.Master)]
        Task InvokeReaction(ReactionDef reaction, ArgTuple[] args);
    }
}