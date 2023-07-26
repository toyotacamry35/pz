using System;
using System.Threading.Tasks;
using GeneratorAnnotations;
using SharedCode.EntitySystem;

namespace SharedCode.MovementSync
{
    [GenerateDeltaObjectCode]
    public interface IVisibilityEntity : IEntity
    {
        [ReplicationLevel(ReplicationLevel.Server)]
        Guid WorldSpace { get; set; }

        [ReplicationLevel(ReplicationLevel.Master)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task<bool> Update();

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task<bool> ForceUnsubscribeAll(Guid user);
    }
}