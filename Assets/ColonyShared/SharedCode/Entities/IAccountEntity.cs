using System;
using System.Threading.Tasks;
using Assets.ResourceSystem.Account;
using GeneratorAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;
using ResourceSystem.Aspects.Misc;
using SharedCode.Aspects.Sessions;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace SharedCode.Entities
{
    [GenerateDeltaObjectCode]
    [DatabaseSaveType(DatabaseSaveType.Explicit, DatabaseServiceType.MetaData)]
    public interface IAccountEntity : IEntity
    {
        // --- Master: ------------------------
        [RuntimeData]
        [BsonIgnore]
        [ReplicationLevel(ReplicationLevel.Master)]  Guid CurrentUserId { get; set; }

        // --- Server: ------------------------
        [ReplicationLevel(ReplicationLevel.Server)]  ValueTask SetCurrentUserId(Guid userId);
        [ReplicationLevel(ReplicationLevel.Server)]  ValueTask<Guid> GetCurrentUserId();
        [ReplicationLevel(ReplicationLevel.Server)] ValueTask<bool> ClearAndConsumeOldRealmRewards();
        [ReplicationLevel(ReplicationLevel.ClientFull)] ValueTask<bool> ConsumeRewards();

        // --- Cl Full: ------------------------

        // Весь заслуженный опыт == Experience + UnconsumedExperience:
        [ReplicationLevel(ReplicationLevel.ClientFull)] int Experience { get; set; }
        // Несконсьюмленная часть заслуженного опыта:
        [ReplicationLevel(ReplicationLevel.ClientFull)] int UnconsumedExperience { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)]  ISessionResult LastSessionResult { get; set; } // Для интерфейса
        [BsonIgnore]
        [ReplicationLevel(ReplicationLevel.ClientFull)]  IDeltaDictionary<RealmRulesQueryDef, RealmRulesQueryState> AvailableRealmQueries { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)]  IDeltaList<IAccountCharacter> Characters { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)]  ValueTask<DeleteCharacterResultType> DeleteAccountCharacter(Guid characterId);
        [ReplicationLevel(ReplicationLevel.ClientFull)]  ICharRealmData CharRealmData { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)]  ValueTask<CreateNewCharacterResult> CreateNewCharacter(string name, Guid accountId);


        // --- Cl Broadcast: ------------------------
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] string AccountId { get; set; }


        // Data for account progress window: --------------------
        [ReplicationLevel(ReplicationLevel.ClientFull)] GenderDef Gender { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task SetGender(GenderDef val);
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task TryConsumeUnconsumedExp(int val);
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task<int> CalcAccLevel();

        [ReplicationLevel(ReplicationLevel.Server)] ValueTask<int> AddExperience(int deltaVal);
    }

    [ProtoContract]
    public struct RealmRulesQueryState
    {
        [ProtoMember(1)]
        public bool Available;

        public RealmRulesQueryState(bool available)
        {
            Available = available;
        }
    }

    [ProtoContract]
    public struct AccountAchievementData
    {
        [ProtoMember(1)]
        public AccountLevelAchievementDef Def;

        [ProtoMember(2)]
        public bool Consumed;
    }


    [ProtoContract]
    public class CharacterDataToCreate
    {
        [ProtoMember(1)]
        public string CharacterName { get; set; }
    }

    [ProtoContract]
    public class CreateNewCharacterResult
    {
        [ProtoMember(1)]
        public CreateNewCharacterResultType Result { get; set; }

        [ProtoMember(2)]
        public Guid CharacterId { get; set; }
    }

    public enum CreateNewCharacterResultType
    {
        None,
        Success,
        ErrorLimitReached,
        ErrorUnknown
    }

    public enum DeleteCharacterResultType
    {
        None,
        Success,
        ErrorCharacterNotFound,
        ErrorUnknown
    }
}