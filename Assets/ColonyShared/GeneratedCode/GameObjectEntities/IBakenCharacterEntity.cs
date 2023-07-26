using GeneratorAnnotations;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using System;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedCode.Entities
{
    [DatabaseSaveType(DatabaseSaveType.Explicit)]
    [GenerateDeltaObjectCode]
    public interface IBakenCharacterEntity : IEntity
    {
        [ReplicationLevel(ReplicationLevel.Server)]
        Guid CharacterId { get; set; }

        [BsonIgnore]
        [ReplicationLevel(ReplicationLevel.Server)]
        bool CharacterLoaded { get; set; }

        [BsonIgnore]
        [ReplicationLevel(ReplicationLevel.Master)]
        bool Logined { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IDeltaDictionary<OuterRef<IEntity>, bool> RegisteredBakens { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        OuterRef<IEntity> ActiveBaken { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        ValueTask<bool> BakenCanBeActivated(OuterRef<IEntity> bakenRef);

        [ReplicationLevel(ReplicationLevel.Server)]
        ValueTask<bool> ActivateBaken(OuterRef<IEntity> bakenRef);

        [ReplicationLevel(ReplicationLevel.Server)]
        ValueTask RegisterBaken(OuterRef<IEntity> bakenRef, bool loaded);

        [ReplicationLevel(ReplicationLevel.Server)]
        ValueTask BakenIsDestroyed(OuterRef<IEntity> bakenRef);

        [ReplicationLevel(ReplicationLevel.Server)]
        ValueTask SetCharacterLoaded(bool loaded);

        [ReplicationLevel(ReplicationLevel.Server)]
        ValueTask SetLogin(bool logined);


        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        ValueTask<bool> CanBeUnloaded();
    }
}
