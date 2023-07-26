using GeneratorAnnotations;
using SharedCode.Cloud;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using System;
using System.Threading.Tasks;

namespace SharedCode.Entities.Service
{
    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.Server)]
    public interface IBakenCoordinatorServiceEntity : IEntity
    {
        [ReplicationLevel(ReplicationLevel.Master)]
        IDeltaDictionary<Guid, Guid> CharacterByRepoId { get; }
        [ReplicationLevel(ReplicationLevel.Master)]
        IDeltaDictionary<Guid, Guid> CharacterByBakenId { get; }

        [ReplicationLevel(ReplicationLevel.Server)] ValueTask OnUserConnected(Guid charId, Guid repoId);
        [ReplicationLevel(ReplicationLevel.Master)] ValueTask OnUserDisconnected(Guid repoId);

        [ReplicationLevel(ReplicationLevel.Master)] ValueTask CheckForLoad(Guid charId);
        [ReplicationLevel(ReplicationLevel.Master)] ValueTask CheckForUnload(Guid charId);


        [ReplicationLevel(ReplicationLevel.Server)][EntityMethodCallType(EntityMethodCallType.Mutable)]
        ValueTask RegisterBaken(Guid characterId, Guid masterRepoId, OuterRef<IEntity> bakenRef);

        [ReplicationLevel(ReplicationLevel.Server)][EntityMethodCallType(EntityMethodCallType.Mutable)]
        ValueTask UnregisterBaken(OuterRef<IEntity> bakenRef);

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Mutable)]
        ValueTask BakenIsDestroyed(Guid characterId, OuterRef<IEntity> bakenRef);

        [ReplicationLevel(ReplicationLevel.Server)][EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task SetCharacterLoaded(Guid characterId, bool loaded);

        [ReplicationLevel(ReplicationLevel.Server)][EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task<bool> ActivateBaken(Guid characterId, OuterRef<IEntity> bakenRef);

        [ReplicationLevel(ReplicationLevel.Server)][EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task<OuterRef<IEntity>> GetActiveBaken(Guid characterId);

    }

}
