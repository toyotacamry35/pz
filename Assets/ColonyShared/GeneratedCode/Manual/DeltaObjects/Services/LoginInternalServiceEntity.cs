using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.Custom.Config;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Newtonsoft.Json;
using NLog;
using ResourceSystem.Aspects.Rewards;
using SharedCode.Config;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.Serializers;

namespace GeneratedCode.DeltaObjects
{
    public partial class LoginInternalServiceEntity : IHasLoadFromJObject
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<Guid, AccountData> _accountsByUserId = new ConcurrentDictionary<Guid, AccountData>();
        private readonly ConcurrentDictionary<Guid, CharacterData> _charactersByUserId = new ConcurrentDictionary<Guid, CharacterData>();
        private readonly ConcurrentDictionary<Guid, Guid> _usersByCharacterId = new ConcurrentDictionary<Guid, Guid>();

        private ServerServicesConfigDef _webServicesConfig;
        public Task Load(CloudSharedDataConfig sharedConfig, CustomConfig config, IEntitiesRepository entitiesRepository)
        {
            _webServicesConfig = sharedConfig.WebServicesConfig.Target;
            return Task.CompletedTask;
        }

        public Task<Guid> GetUserIdByAccountIdImpl(Guid accountId)
        {
            var accounts = _accountsByUserId.FirstOrDefault(v => v.Value.AccountId == accountId);
            return Task.FromResult(accounts.Key);
        }
        public Task<Guid> GetUserIdByCharacterIdImpl(Guid characterId)
        {
            var accounts = _charactersByUserId.FirstOrDefault(v => v.Value.CharacterId == characterId);
            return Task.FromResult(accounts.Key);
        }
        public Task<Guid> GetUserIdByAccountNameImpl(string accountName)
        {
            var accounts = _accountsByUserId.FirstOrDefault(v => v.Value.AccountName == accountName);
            return Task.FromResult(accounts.Key);
        }

        public Task<AccountData> GetAccountDataByUserIdImpl(Guid userId)
        {
            if (!_accountsByUserId.TryGetValue(userId, out var result))
                Logger.IfError()?.Message("Not found account by user repo Id {0}", userId).Write();
            else
                Logger.IfInfo()?.Message("Returning account {0} for user repo Id {1}", result, userId).Write();

            return Task.FromResult(result);
        }

        public Task<AccountData> GetAccountDataByAccountNameImpl(string accountName)
        {
            var accounts = _accountsByUserId.FirstOrDefault(v => v.Value.AccountName == accountName);
            return Task.FromResult(accounts.Value);
        }

        public Task<CharacterData> GetCharacterDataByUserIdImpl(Guid userId)
        {
            if (!_charactersByUserId.TryGetValue(userId, out var result))
                Logger.IfError()?.Message("Not found user repo id {0}", userId).Write();
            else
                Logger.IfInfo()?.Message("Returning character {0} by user repo Id {1}", result, userId).Write();

            return Task.FromResult(result);
        }

        public ValueTask<bool> AddCharacterImpl(Guid userId, CharacterData data)
        {
            _usersByCharacterId[data.CharacterId] = userId;
            _charactersByUserId[userId] = data;
            Logger.IfInfo()?.Message("Add character user repo Id {0} charId {1}", userId, data.CharacterId).Write();
            return new ValueTask<bool>(true);
        }
        public ValueTask<bool> UpdateAccountDataImpl(Guid userId, string[] newPacks)
        {
            if (_charactersByUserId.TryGetValue(userId, out var data))
                data.Packs = newPacks.ToList();

            return new ValueTask<bool>(true);
        }
        public ValueTask<bool> UpdateCharacterImpl(Guid userId, Guid newCharId)
        {
            if (_charactersByUserId.TryGetValue(userId, out var data))
                data.CharacterId = newCharId;

            return new ValueTask<bool>(true);
        }
        public ValueTask<bool> RemoveCharacterImpl(Guid userId)
        {
            CharacterData characterEntityId;
            if(_charactersByUserId.TryRemove(userId, out characterEntityId))
                _usersByCharacterId.TryRemove(characterEntityId.CharacterId, out var _);
            Logger.IfInfo()?.Message("Removed character userId {0} charId {1}", userId, characterEntityId).Write();
            return new ValueTask<bool>(true);
        }

        public Task AddClientConnectionImpl(List<Guid> userIds, string host, int port, Guid repositoryId)
        {
            foreach (var userId in userIds)
            {
                var userIdCopy = userId;
                AsyncUtils.RunAsyncTask(async () =>
                {
                    for (int i = 0; i < 100; i++)
                    {
                        bool found = true;
                        using (var wrapper = await EntitiesRepository.Get<IClientCommunicationEntityServer>(userIdCopy))
                        {
                            var clientCommunicationEntity = wrapper?.Get<IClientCommunicationEntityServer>(userIdCopy);
                            if (clientCommunicationEntity == null)
                            {
                                if ((i + 1) % 20 == 0)
                                    Logger.IfWarn()?.Message("After {0} attemps still can't get client communication entity to AddUser", i).Write();
                                else if (i == 99)
                                    Logger.IfError()?.Message("Can't get client communication entity").Write();
                                found = false;
                            }
                            else
                            {
                                var result = await clientCommunicationEntity.AddConection(host, port, repositoryId);
                                if (!result)
                                {
                                    found = false;
                                    Logger.IfError()?.Message("Error on adding connection to user repo id {repo_id}", userIdCopy).Write();
                                }
                            }

                        }
                        if (!found)
                        {
                            await Task.Delay(1000);
                        }
                        else
                            break;
                    }
                });
            }

            return Task.CompletedTask;
        }

        public Task RemoveClientConnectionImpl(List<Guid> userIds, Guid repositoryId)
        {
            foreach (var userId in userIds)
            {
                var userIdCopy = userId;
                AsyncUtils.RunAsyncTask(async () =>
                {
                    using (var wrapper = await EntitiesRepository.Get<IClientCommunicationEntityServer>(userIdCopy))
                    {
                        var clientCommunicationEntity = wrapper?.Get<IClientCommunicationEntityServer>(userIdCopy);
                        if (clientCommunicationEntity == null)
                        {
                            Logger.IfError()?.Message("RemoveClientConnection IClientCommunicationEntity {0} not found", userIdCopy).Write();
                            return;
                        }

                        var result = await clientCommunicationEntity.RemoveConection(repositoryId);
                        if (!result)
                            Logger.IfError()?.Message("Error on removing connection to user repo id {repo_id}", userIdCopy).Write();
                    }
                });
            }

            return Task.CompletedTask;
        }

        public ValueTask<bool> AddAccountImpl(Guid userId, AccountData data)
        {
            if (_accountsByUserId.TryAdd(userId, data))
            {
                Logger.IfInfo()?.Message("Added account user repo Id {repo_id} account id {user_id}", userId, data.AccountId).Write();
                return new ValueTask<bool>(true);
            }

            Logger.IfError()?.Message("Cant add account user repo Id {repo_id} account id {user_id}", userId, data.AccountId).Write();
            return new ValueTask<bool>(false);
        }
        public ValueTask<bool> RemoveAccountImpl(Guid userId)
        {
            if (_accountsByUserId.TryRemove(userId, out var accountEntityId))
            {
                Logger.IfInfo()?.Message("Removed character user repo id {repo_id} account id {user_id}", userId, accountEntityId).Write();
                return new ValueTask<bool>(true);
            }

            Logger.IfError()?.Message("User repo Id {repo_id} not found", userId).Write();
            return new ValueTask<bool>(false);
        }


        public class GetPremiumStatusResponse
        {
            public string status { get; set; }
            public Data data { get; set; }

            public class Data
            {
                public IList<Status> statuses { get; set; } = Array.Empty<Status>();

                public class Status
                {
                    public string status_id { get; set; }
                    public string name { get; set; }
                    public DateTimeOffset expires_at { get; set; }
                }
            }
        }
    
        private static async Task<GetPremiumStatusResponse.Data> RequestPremium(string serverAddress, Guid userId)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var resp = await client.GetAsync($"{serverAddress}?user_token={userId}");
                    var data = await resp.Content.ReadAsStringAsync();
                    if (!resp.IsSuccessStatusCode)
                    {
                        Logger.IfError()?.Message("Cant get premium status for user {0}, Code {1}", userId, resp.StatusCode).Write();
                        return new GetPremiumStatusResponse.Data();
                    }

                    var result = JsonConvert.DeserializeObject<GetPremiumStatusResponse>(data);
                    if (result.status != "ok")
                    {
                        Logger.IfError()?.Message("Cant get premium status for user {0}, Status {1}", userId, result.status).Write();
                        return new GetPremiumStatusResponse.Data();
                    }

                    return result.data;
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message(e, "Cant get premium status for user {0}", userId).Write();
                    return new GetPremiumStatusResponse.Data();
                }
            }
        }
     
        public ValueTask<AccountData[]> ListAllKnownAccountsImpl()
        {
            return new ValueTask<AccountData[]>(_accountsByUserId.Values.ToArray());
        }
    }
}
