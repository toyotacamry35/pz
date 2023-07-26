using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratorAnnotations;
using ProtoBuf;
using ResourceSystem.Aspects.Rewards;
using SharedCode.Cloud;
using SharedCode.EntitySystem;

namespace SharedCode.Entities.Service
{
    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.Server)]
    public interface ILoginInternalServiceEntity : IEntity
    {
        [ReplicationLevel(ReplicationLevel.Server), EntityMethodCallType(EntityMethodCallType.Immutable)] Task<Guid> GetUserIdByAccountId(Guid accountId);
        [ReplicationLevel(ReplicationLevel.Server), EntityMethodCallType(EntityMethodCallType.Immutable)] Task<Guid> GetUserIdByCharacterId(Guid characterId);
        [ReplicationLevel(ReplicationLevel.Server), EntityMethodCallType(EntityMethodCallType.Immutable)] Task<Guid> GetUserIdByAccountName(string accountName);
        [ReplicationLevel(ReplicationLevel.Server), EntityMethodCallType(EntityMethodCallType.Immutable)] Task<AccountData> GetAccountDataByUserId(Guid userId);
        [ReplicationLevel(ReplicationLevel.Server), EntityMethodCallType(EntityMethodCallType.Immutable)] Task<AccountData> GetAccountDataByAccountName(string accountName);  
        [ReplicationLevel(ReplicationLevel.Server), EntityMethodCallType(EntityMethodCallType.Immutable)] Task<CharacterData> GetCharacterDataByUserId(Guid userId);

        [ReplicationLevel(ReplicationLevel.Server), EntityMethodCallType(EntityMethodCallType.Immutable)] ValueTask<bool> AddAccount(Guid userId, AccountData data);
        [ReplicationLevel(ReplicationLevel.Server), EntityMethodCallType(EntityMethodCallType.Immutable)] ValueTask<bool> RemoveAccount(Guid userId);
        [ReplicationLevel(ReplicationLevel.Server), EntityMethodCallType(EntityMethodCallType.Immutable)] ValueTask<bool> AddCharacter(Guid userId, CharacterData data);
        [ReplicationLevel(ReplicationLevel.Server), EntityMethodCallType(EntityMethodCallType.Immutable)] ValueTask<bool> UpdateCharacter(Guid userId, Guid newCharId);
        [ReplicationLevel(ReplicationLevel.Server), EntityMethodCallType(EntityMethodCallType.Immutable)] ValueTask<bool> UpdateAccountData(Guid userId, string[] newPacks);
        [ReplicationLevel(ReplicationLevel.Server), EntityMethodCallType(EntityMethodCallType.Immutable)] ValueTask<bool> RemoveCharacter(Guid userId);
        [ReplicationLevel(ReplicationLevel.Server), EntityMethodCallType(EntityMethodCallType.Lockfree)] Task AddClientConnection(List<Guid> userIds, string host, int port, Guid repositoryId);
        [ReplicationLevel(ReplicationLevel.Server), EntityMethodCallType(EntityMethodCallType.Immutable)] Task RemoveClientConnection(List<Guid> userIds, Guid repositoryId);

        [ReplicationLevel(ReplicationLevel.Server), EntityMethodCallType(EntityMethodCallType.Immutable)] ValueTask<AccountData[]> ListAllKnownAccounts();
    }

    [ProtoContract]
    public class AccountData
    {
        [ProtoMember(1)]
        public Guid AccountId;
        [ProtoMember(2)]
        public string AccountName;

        public override string ToString()
        {
            return $"{AccountId}: {(string.IsNullOrWhiteSpace(AccountName) ? "empty" : AccountName)}";
        }
    }

    [ProtoContract]
    public class CharacterData
    {
        object _lock = new object();
        Guid characterId;
        [ProtoMember(1)]
        public Guid CharacterId { get { lock (_lock) return characterId; } set { lock (_lock) characterId = value; } }
        [ProtoMember(2)]
        public string CharacterName;
        List<string> _packs;
        [ProtoMember(3)]
        public List<string> Packs { get { lock (_lock) return _packs; } set { lock (_lock) _packs = value; } }

        public override string ToString()
        {
            var packStr = Packs != null ? string.Join(", ", Packs) : "<null>";
            return $"{CharacterId}: {(string.IsNullOrWhiteSpace(CharacterName) ? "empty" : CharacterName)} packs: {packStr}";
        }
    }
}
