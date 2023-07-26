using System;
using System.Text;
using System.Threading.Tasks;
using Core.Cheats;
using GeneratedCode.Custom.Config;
using GeneratedCode.DeltaObjects;
using GeneratorAnnotations;
using ProtoBuf;
using ResourceSystem.Aspects.Rewards;
using SharedCode.Aspects.Sessions;
using SharedCode.Cloud;
using SharedCode.Data;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace SharedCode.Entities.Cloud
{
    [ProtoContract]
    public class FindRealmRequestResult
    {
        [ProtoMember(1)]
        public bool Booked { get; set; }
        [ProtoMember(2)]
        public OuterRef<IEntity> Realm { get; set; }
    }


    [ProtoContract]
    public class RealmStatus
    {
        [ProtoMember(1)]
        public int CurrentCCU { get; set; }
        [ProtoMember(2)]
        public int CurrentMaxCCU { get; set; }
        [ProtoMember(3)]
        public bool AllowsToJoin { get; set; }
        [ProtoMember(4)]
        public bool IsDead { get; set; }
        [ProtoMember(5)]
        public string RealmDef { get; set; }
        [ProtoMember(6)]
        public string RealmId { get; set; }
        [ProtoMember(7)]
        public int AttachedAccountsCount { get; set; }

        public override string ToString()
        {
            var str = new System.IO.StringWriter();
            HttpHelperUtility.Serializer.Serialize(str, this);
            return str.ToString();
        }
    }


    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.Client | CloudNodeType.Server)]
    public interface ILoginServiceEntity : IEntity
    {
        [ReplicationLevel(ReplicationLevel.Master)]
        IDeltaDictionary<Guid, int> AccountEntityRequests { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<FindRealmRequestResult> FindRealm(RealmRulesQueryDef query, Guid accountId, Guid currentRealm);

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task<Guid> GetUserRealm(Guid userId);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        RealmRulesConfigDef RealmRulesConfigDef { get; set; }
        
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task<bool> AttachMapToRealm(Guid mapId, MapDef mapDef, Guid realmId);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task<LoginResult> Login(Platform platform, string version, string userId, string code);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task Logout();

        [ReplicationLevel(ReplicationLevel.Server)]
        [CheatRpc(AccountType.TechnicalSupport)]
        [Cheat]
        Task<RealmStatus> GetRealmStatus();

        [ReplicationLevel(ReplicationLevel.Server)]
        [CheatRpc(AccountType.TechnicalSupport)]
        [Cheat]
        Task<ELogoutResult> Kick(Guid id);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [CheatRpc(AccountType.TechnicalSupport)]
        [Cheat]
        ValueTask<bool> SetMaxCCU(int ccu);

        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [CheatRpc(AccountType.TechnicalSupport)]
        [Cheat]
           ValueTask<int> GetCCU();

        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [CheatRpc(AccountType.TechnicalSupport)]
        [Cheat]
        ValueTask<int> GetMaxCCU();
        
        [Cheat] [CheatRpc(AccountType.TechnicalSupport)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.Server)]
        ValueTask<AccountList> GetAllAccountsOnline();

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task<bool> IsMapDead(Guid map, Guid realmId); 
        
        [ReplicationLevel(ReplicationLevel.Server)]
        [CheatRpc(AccountType.TechnicalSupport)]
        [Cheat]
        ValueTask<bool> DeleteAllCharacters(Guid accountId);

        [ReplicationLevel(ReplicationLevel.Server)]
        [CheatRpc(AccountType.TechnicalSupport)]
        [Cheat]
        ValueTask<CharacterList> GetAllCharactersForAccount(Guid accountId);
        [ReplicationLevel(ReplicationLevel.Server)]
        [CheatRpc(AccountType.TechnicalSupport)]
        [Cheat]
        ValueTask<int> GetAccountExperience(Guid accountId);
        [ReplicationLevel(ReplicationLevel.Server)]
        [CheatRpc(AccountType.TechnicalSupport)]
        [Cheat]
        ValueTask<int> AddAccountExperience(Guid accountId, int expToGive);
        ValueTask<bool> RequestUpdateAccountData(Guid accId);
        ValueTask<bool> NotifyPlatformOfJoining(Guid accId, Guid realmId);
        ValueTask<bool> NotifyPlatformOfLeaving(Guid accId, Guid realmId);
        ValueTask<bool> NotifyPlatformOfRealmGiveUp(Guid accId, Guid realmId);

        [ReplicationLevel(ReplicationLevel.Server)]
        ValueTask<bool> GiveUpRealmOnDeath(Guid accId);
        [ReplicationLevel(ReplicationLevel.Server), EntityMethodCallType(EntityMethodCallType.Immutable)] 
        Task<Guid> GetAccountIdByUserId(Guid userId);

        //#note: not LockfreeLocal!, 'cos:
        // not Local 'cos AccEntty is needed inside. & it lives ONLY at Login node, where master-copy of LoginServiceEntty resides. So we need this method to be executed on master-sopy of LoginServiceEntty.
        // not Lockfree, 'cos it calls inside RequestAccount/ReleaseAccount, which are using delta-objects inside.
        [ReplicationLevel(ReplicationLevel.Server)]
        ValueTask<AccountStatsData> GetAccountDataForAccStats(Guid accountId);
        [ReplicationLevel(ReplicationLevel.Server)] Task<bool> AssignAccountToMap(Guid accId, Guid mapId);
        [ReplicationLevel(ReplicationLevel.Server)] Task<bool> GrantAccountReward(Guid characterId, RewardDef reward);

    }

    [ProtoContract]
    public struct AccountList
    {
        [ProtoMember(1)]
        public AccountData[] Accounts { get; set; }

        public override string ToString()
        {
            return Accounts != null ? string.Join<AccountData>(Environment.NewLine, Accounts) : string.Empty;
        }
    }

    [ProtoContract]
    public struct CharacterList
    {
        [ProtoMember(1)]
        public CharacterData[] Characters { get; set; }

        public override string ToString()
        {
            return Characters != null ? string.Join<CharacterData>(Environment.NewLine, Characters) : string.Empty;
        }
    }

    [ProtoContract]
    public class LoginResult
    {
        [ProtoMember(1)]  public Guid UserId { get; set; }
        [ProtoMember(2)]  public AccountData AccountData { get; set; }
        [ProtoMember(3)]  public Guid CharacterId { get; set; }
        [ProtoMember(4)]  public ELoginResult Result { get; set; }
        [ProtoMember(5)] public string PlatformApiToken { get; set; }
    }

    public enum ELoginResult
    {
        None,
        Success,
        ErrorBanned,
        ErrorServerIsFull,
        ErrorUserIdConfirmedIsEmpty,
        ErrorCreatingAccount,
        ErrorCreatingCharacter,
        ErrorAccountNotFound,
        ErrorCharacterNotFound,
        ErrorWorldNode,
        ErrorSceneNotFound,
        ErrorAnonymousDisabled,
        ErrorUnknown
    }

    public enum ELogoutResult
    {
        None,
        Success,
        ErrorUnknown
    }
}
