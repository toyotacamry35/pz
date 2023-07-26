using System.Threading.Tasks;
using SharedCode.EntitySystem;

namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface IHasAnimationDoerOwner
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IAnimationDoerOwner AnimationDoerOwner { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IAnimationDoerOwner : IDeltaObject
    {
        /// Присутствует (не null) только на client'ах
        [RuntimeData(SkipField = true), ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IAnimationDoer AnimationDoer { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        Task SetAnimationDoer(IAnimationDoer doer);
        
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        Task UnsetAnimationDoer(IAnimationDoer doer);
    }
}