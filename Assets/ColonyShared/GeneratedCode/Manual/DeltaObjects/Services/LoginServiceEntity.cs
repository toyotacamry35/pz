using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.Data;
using SharedCode.Entities;
using SharedCode.Entities.Cloud;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using Newtonsoft.Json;
using GeneratedCode.Custom.Config;
using GeneratedCode.Network.Statistic;
using SharedCode.Config;
using SharedCode.Refs;
using SharedCode.Aspects.Sessions;
using SharedCode.Serializers;
using System.Collections.Generic;
using System.Net.Http;
using GeneratedCode.Repositories;
using SharedCode.Repositories;
using System.Text;
using Assets.Src.ResourcesSystem.Base;
using System.Security.Cryptography;
using Core.Environment.Logging.Extension;
using ResourceSystem.Aspects.Rewards;
using ResourcesSystem.Loader;
using ColonyShared.SharedCode.Utils;

namespace GeneratedCode.DeltaObjects
{
    public partial class LoginServiceEntity : IHasLoadFromJObject, IHookOnStart, IHookOnInit
    {
        public static bool CallPlatform = false;
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly Logger UiLogger = LogManager.GetLogger("UI");

        //readonly ConcurrentDictionary<string, Guid> _userAccounts = new ConcurrentDictionary<string, Guid>();
        readonly ConcurrentDictionary<Guid /*userId*/, Guid /*accountId*/> _accountIdByUserId = new ConcurrentDictionary<Guid, Guid>();
      
        private int _maxCCU = 20;

        private bool _enableAnonymousLogin;

        private LoginNodeServiceEntityConfig _config;
        private ServerServicesConfigDef _webServicesConfig;

        public Task Load(CloudSharedDataConfig sharedConfig, CustomConfig config, IEntitiesRepository entitiesRepository)
        {
            _config = (LoginNodeServiceEntityConfig)config;
            _webServicesConfig = sharedConfig.WebServicesConfig.Target;
            _maxCCU = _config.MaxCCUOnRealm;
            _enableAnonymousLogin = _config.EnableAnonymousLogin;
            Statistics<CCUStatistics>.Instance.SetMaxCCU(_maxCCU);
            entitiesRepository.CloudRequirementsMet += EntitiesRepository_CloudRequirementsMet;
            return Task.CompletedTask;
        }
        object _reqLock = new object();
        bool _requirementsAreMet = false;
        Guid _readyRealm = Guid.Empty;
        private Task EntitiesRepository_CloudRequirementsMet()
        {
            lock (_reqLock)
            {
                _requirementsAreMet = true;
                if (_readyRealm != Guid.Empty)
                    return Post<RealmIsReadyResponse>(_webServicesConfig.APIEndpoint, _webServicesConfig.APIHostname, _webServicesConfig.APIToken,
                        "/api/game/realms/ready/", new RealmIsReadyRequest()
                        {
                            realmId = _readyRealm.ToString()
                        });
            }
            return Task.CompletedTask;
        }

        public Task OnInit()
        {
            RealmRulesConfigDef = _config.Rules.Target;

            return Task.CompletedTask;
        }

        public Task OnStart()
        {
            EntitiesRepository.UserDisconnected += EntitiesRepository_UserDisconnected;
            return Task.CompletedTask;
        }

        private async Task EntitiesRepository_UserDisconnected(Guid repoId)
        {
            Logger.IfInfo()?.Message("LoginServiceEntity User disconnected {user_repo_id}", repoId).Write();
            await logout(repoId);
        }
        async Task<int> GetOrCreateRealmsCollection()
        {
            if (EntitiesRepository.TryGetLockfree<IRealmsCollectionEntity>(_config.RealmsCollectionId, ReplicationLevel.Master) == null)
            {
                var ent = await EntitiesRepository.Load<IRealmsCollectionEntity>(_config.RealmsCollectionId);
                if (ent == null)
                    ent = await EntitiesRepository.Create<IRealmsCollectionEntity>(_config.RealmsCollectionId);
                else
                    using (var rcw = await EntitiesRepository.Get<IRealmsCollectionEntity>(_config.RealmsCollectionId))
                    {
                        var rc = rcw.Get<IRealmsCollectionEntity>(_config.RealmsCollectionId);
                        return rc.Realms.Count;
                    }
            }
            using (var rcw = await EntitiesRepository.Get<IRealmsCollectionEntity>(_config.RealmsCollectionId))
            {
                var rc = rcw.Get<IRealmsCollectionEntity>(_config.RealmsCollectionId);
                return rc.Realms.Count;
            }
        }
        async Task<List<KeyValuePair<Guid, RealmRulesDef>>> GetRealms(RealmRulesQueryDef query, Guid currentRealm)
        {
            await GetOrCreateRealmsCollection();
            List<KeyValuePair<Guid, RealmRulesDef>> realmsList;
            using (var realmsCollectionWrapper = await EntitiesRepository.Get<IRealmsCollectionEntity>(_config.RealmsCollectionId))
            {
                var realmsCollection = realmsCollectionWrapper.Get<IRealmsCollectionEntity>(_config.RealmsCollectionId);
                if (query == null && currentRealm == default)
                {
                    realmsList = realmsCollection.Realms.ToList();
                }   
                else
                {
                    realmsList = currentRealm == default ?
                        realmsCollection.Realms.Where(x => x.Value == query?.RealmRules).ToList() :
                        realmsCollection.Realms.Where(x => x.Key == currentRealm).ToList();

                }
                    
            }
            return realmsList;
        }
        class BookRealmRequest
        {
            [JsonProperty("userId")]
            public string accountId { get; set; }
            public string definition { get; set; }
        }
        class BookRealmResponse
        {
            public string realmId { get; set; }
        }
        async Task<bool> MeetsMinimumRequirementsToJoin(Guid accountEntityId, RealmRulesQueryDef query)
        {
            if (!await RequestAccount(accountEntityId, string.Empty))
                throw new InvalidOperationException($"Entity {accountEntityId} not found");

            try
            {
                using (var wrapper = await EntitiesRepository.Get<IAccountEntityServer>(accountEntityId))
                {
                    if (wrapper.TryGet<IAccountEntityServer>(accountEntityId, out var accountEntity))
                    {
                        return accountEntity.AvailableRealmQueries.Any(x => x.Key == query && x.Value.Available);
                    }
                    else
                        throw new InvalidOperationException($"Entity {accountEntityId} failed to load");
                }
            }
            finally
            {
                await ReleaseAccount(accountEntityId);
            }

        }
        public async Task<FindRealmRequestResult> FindRealmInternal(RealmRulesQueryDef query, Guid accountId, Guid currentRealm)
        {
            if (accountId != default && query != null)
            {
                bool canSelectThisQuery = await MeetsMinimumRequirementsToJoin(accountId, query);
                if (!canSelectThisQuery)
                    return new FindRealmRequestResult();
            }    

            var realmsList = await GetRealms(query, currentRealm);
            if (realmsList.Any())
            {
                List<Guid> realmsToLoad = new List<Guid>();
                foreach (var realmMeta in realmsList)
                {
                    Logger.IfInfo()?.Message($"Try attach {accountId} {query?.____GetDebugShortName()} to {realmMeta.Key} {realmMeta.Value.____GetDebugShortName()}").Write();
                    using (var realmW = await EntitiesRepository.Get<IRealmEntity>(realmMeta.Key))
                    {
                        if (!realmW.TryGet<IRealmEntityServer>(realmMeta.Key, out var realm))
                        {
                            realmsToLoad.Add(realmMeta.Key);
                            Logger.IfInfo()?.Message($"Realm {realmMeta.Key} {realmMeta.Value.____GetDebugShortName()} is not loaded").Write();
                            continue;
                        }


                        //if (realm.AttachedAccounts.Count > realmMeta.Value.MaxCCU)
                        //    continue;
                        if (await realm.TryAttach(accountId))
                        {
                            if (Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId != Guid.Empty)
                                await EntitiesRepository.SubscribeReplication(RealmEntity.StaticTypeId, realm.Id,
                            Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId, ReplicationLevel.Server);
                            Logger.IfInfo()?.Message($"FOUND REALM {realm.Id}").Write();
                            return new FindRealmRequestResult() { Realm = new OuterRef<IEntity>(realm.Id, RealmEntity.StaticTypeId) };

                        }
                    }
                }
                foreach (var realmToLoad in realmsToLoad)
                {
                    await GetOrCreateRealm(realmToLoad, null);
                    using (var realmW = await EntitiesRepository.Get<IRealmEntity>(realmToLoad))
                    {
                        if (!realmW.TryGet<IRealmEntityServer>(realmToLoad, out var realm))
                        {
                            Logger.IfError()?.Message($"Realm {realmToLoad} failed to load").Write();
                            continue;
                        }
                        //if (realm.AttachedAccounts.Count > realm.Def.MaxCCU)
                        //   continue;
                        if (await realm.TryAttach(accountId))
                        {
                            if (Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId != Guid.Empty)
                                await EntitiesRepository.SubscribeReplication(RealmEntity.StaticTypeId, realm.Id,
                        Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId, ReplicationLevel.Server);
                            Logger.IfInfo()?.Message($"FOUND REALM {realm.Id}").Write();
                            return new FindRealmRequestResult() { Realm = new OuterRef<IEntity>(realm.Id, RealmEntity.StaticTypeId) };
                        }
                    }
                }
            }
            else if (await GetOrCreateRealmsCollection() == 0)
            {
                var reId = currentRealm == default ? Guid.NewGuid() : currentRealm;
                await GetOrCreateRealm(reId, query.RealmRules.Target);

                using (var realmW = await EntitiesRepository.Get<IRealmEntity>(reId))
                {
                    var realm = realmW.Get<IRealmEntity>(reId);
                    
                    if (await realm.TryAttach(accountId))
                    {
                        if (Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId != Guid.Empty)
                            await EntitiesRepository.SubscribeReplication(RealmEntity.StaticTypeId, realm.Id,
                    Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId, ReplicationLevel.Server);
                        Logger.IfInfo()?.Message($"FOUND REALM {realm.Id}").Write();
                        return new FindRealmRequestResult() { Realm = new OuterRef<IEntity>(realm.Id, RealmEntity.StaticTypeId) };
                    }
                }
            }
            Logger.Error($"CAN'T CREATE NEW REALM {query?.____GetDebugShortName()}, ALREADY HAVE TOO MUCH");

            return default;
        }
        public async Task<FindRealmRequestResult> FindRealmImpl(RealmRulesQueryDef query, Guid accountId, Guid currentRealm)
        {
            if (accountId != default && query != null)
            {
                bool canSelectThisQuery = await MeetsMinimumRequirementsToJoin(accountId, query);
                if (!canSelectThisQuery)
                    return new FindRealmRequestResult();
            }
            if (accountId != default && query != null)
            {
                var br = await Post<BookRealmResponse>(
                    _webServicesConfig.APIEndpoint,
                    _webServicesConfig.APIHostname,
                    _webServicesConfig.APIToken,
                    "/api/game/users/realms/booked/",
                    new BookRealmRequest()
                {
                    accountId = accountId.ToString(),
                    definition = ((IResource)query).Address.Root
                });
                if (br != null)
                {
                    Guid.TryParse(br.realmId, out var targetRealm);
                    if (targetRealm != default)
                    {
                        var realmsListSearch = await GetRealms(null, default);
                        if (realmsListSearch.Any())
                        {
                            realmsListSearch = await GetRealms(null, targetRealm);
                            if (realmsListSearch.Any())
                                return await FindRealmInternal(query, accountId, targetRealm);
                        }
                        else
                            return await FindRealmInternal(query, accountId, targetRealm);
                    }   

                    return new FindRealmRequestResult() { Booked = true };
                }
            }

            return await FindRealmInternal(query, accountId, currentRealm);
        }
        async Task GetOrCreateRealm(Guid realmId, RealmRulesDef rules)
        {
            if (EntitiesRepository.TryGetLockfree<IRealmEntity>(realmId, ReplicationLevel.Master) != null)
                return;
            var re = await EntitiesRepository.Load<IRealmEntity>(realmId);
            if (re == null)
            {
                re = await EntitiesRepository.Create<IRealmEntity>(realmId, async (e) =>
                {
                    e.Def = rules;
                });

                Logger.IfInfo()?.Message($"Realm {realmId} {rules?.____GetDebugShortName()} is created").Write();
                await GetOrCreateRealmsCollection();
                using (var realmsCollectionWrapper = await EntitiesRepository.Get<IRealmsCollectionEntity>(_config.RealmsCollectionId))
                {
                    var realmsCollection = realmsCollectionWrapper.Get<IRealmsCollectionEntity>(_config.RealmsCollectionId);
                    await realmsCollection.AddRealm(realmId, rules);
                }
            }
            lock (_reqLock)
            {
                _readyRealm = realmId;
            }
            if (_requirementsAreMet)
                await Post<RealmIsReadyResponse>(
                    _webServicesConfig.APIEndpoint,
                    _webServicesConfig.APIHostname,
                    _webServicesConfig.APIToken,
                    "/api/game/realms/ready/",
            new RealmIsReadyRequest()
            {
                realmId = _readyRealm.ToString()
            });
            Logger.IfInfo()?.Message($"Realm {realmId} {rules?.____GetDebugShortName()} is loaded").Write();
        }
        public async Task<bool> AttachMapToRealmImpl(Guid mapId, MapDef mapDef, Guid realmId)
        {
            await GetOrCreateRealm(realmId, null);
            using (var realmW = await EntitiesRepository.Get<IRealmEntity>(realmId))
            {
                var realm = realmW.Get<IRealmEntity>(realmId);

                await realm.AddMap(mapId, new SharedCode.MapSystem.MapMeta() { MapDef = mapDef });
            }

            return true;
        }
        private async Task logout(Guid userId)
        {
            Logger.IfInfo()?.Message("Logging out user {user_id}", userId).Write();
            if (!_accountIdByUserId.TryRemove(userId, out var accountEntityId))
            {
                Logger.IfWarn()?.Message("LoginServiceEntity logout. Not found accountId by userId {user_id}", userId).Write();
                return;
            }
            var currentCCU = GetCurrentCCU();
            Statistics<CCUStatistics>.Instance.SetCurrentCCU(currentCCU);

            using (var wrapper = await EntitiesRepository.Get<IAccountEntityServer>(accountEntityId))
            {
                var accountEntity = wrapper?.Get<IAccountEntityServer>(accountEntityId);
                if (accountEntity == null)
                {
                    Logger.IfError()?.Message("Logout account entity {account_id} not found", accountEntityId).Write();
                    return;
                }

                var oldUserId = await accountEntity.GetCurrentUserId();
                if (oldUserId != userId)
                    Logger.IfError()?.Message("Old userId {0} and disconnected userId {1} mismatch. AccountEntityId {2}", oldUserId, userId, accountEntityId).Write();

                await accountEntity.SetCurrentUserId(Guid.Empty);
                await accountEntity.CharRealmData.LeaveCurrentRealm();
            }
            using (var wrapper = await EntitiesRepository.GetFirstService<IWorldCoordinatorNodeServiceEntity>())
            {
                var entityService = wrapper?.GetFirstService<IWorldCoordinatorNodeServiceEntity>();
                if (entityService != null)
                {
                    await entityService.RequestLogoutFromMap(userId, true);
                }
                else
                    Logger.IfError()?.Message($"{nameof(ILoginInternalServiceEntity)} not found for remove accountType user {GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId}").Write();
            }

            using (var wrapper = await EntitiesRepository.GetFirstService<ILoginInternalServiceEntity>())
            {
                var entityService = wrapper?.GetFirstService<ILoginInternalServiceEntityServer>();
                if (entityService != null)
                {
                    await entityService.RemoveAccount(userId);
                    await entityService.RemoveCharacter(userId);
                }
                else
                    Logger.IfError()?.Message($"{nameof(ILoginInternalServiceEntity)} not found for remove accountType user {GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId}").Write();
            }

            /*
            using (var wrapperBots = await EntitiesRepository.GetFirstService<IBotCoordinator>())
            {
                var botCoordReplica = wrapperBots?.GetFirstService<IBotCoordinatorServer>();
                if (botCoordReplica == null)
                {
                    Logger.IfError()?.Message("IBotCoordinator not found {0}", EntitiesRepository.Id).Write();
                }
                else
                    await botCoordReplica.DeactivateAllBots(accountEntityId);
            }*/


            using (var wrapper = await EntitiesRepository.GetFirstService<IAccountTypeServiceEntityServer>())
            {
                var accountTypeService = wrapper?.GetFirstService<IAccountTypeServiceEntityServer>();
                if (accountTypeService != null)
                    await accountTypeService.RemoveAccountType(GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId);
                else
                    Logger.IfError()?.Message("IAccountTypeServiceEntity not found for remove accountType user {0}", GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId).Write();
            }

            using (var wrapper = await EntitiesRepository.Get<IClientCommunicationEntityServer>(userId))
            {
                if (wrapper.TryGet<IClientCommunicationEntityServer>(userId, out var entity))
                {
                    var connectionsCopy = entity.Connections.Select(x => x.NodeId).ToList();
                    connectionsCopy.Add(this.EntitiesRepository.Id);
                    var repository = EntitiesRepository;
                    foreach (var repositoryId in connectionsCopy)
                    {
                        var repositoryIdCopy = repositoryId;
                        Logger.IfInfo()?.Message("Logout user {user_id} close connection {repo_id}", userId, repositoryIdCopy).Write();
                        _ = AsyncUtils.RunAsyncTask(async () =>
                        {
                            using (var wrapper2 = await repository.Get<IRepositoryCommunicationEntityServer>(repositoryIdCopy))
                            {
                                var repositoryEntity = wrapper2?.Get<IRepositoryCommunicationEntityServer>(repositoryIdCopy);
                                if (repositoryEntity != null)
                                {
                                    var result = await repositoryEntity.ForceCloseConnection(userId);
                                    if (result == false)
                                        Logger.IfError()?.Message("Logout user {user_id} error on close connection on node {1}, maybe already disconnected", userId, repositoryEntity.ToString()).Write();
                                }
                                else
                                    Logger.IfError()?.Message("Logout user {user_id} not found remote server RepositoryCommunicationEntity {1} for force disconnect", userId, repositoryIdCopy).Write();
                            }
                        }, repository);
                    }
                }
            }

            using (await this.GetThisWrite())
                await ReleaseAccount(accountEntityId);
        }

        public class VerifyTokenResponse
        {
            public bool Anon { get; set; }
            public string token { get; set; }
            public string login { get; set; }
            public string[] userTypes { get; set; } = Array.Empty<string>();
            public string[] founderPacks { get; set; } = Array.Empty<string>();
        }
        public class VerifyTokenRequest
        {
            [JsonProperty("userId")]
            public string accountId { get; set; }
            public string code { get; set; }
        }
        private Task<VerifyTokenResponse> VerifyToken(string userId, string code)
        {
            return Post<VerifyTokenResponse>(_webServicesConfig.APIEndpoint, _webServicesConfig.APIHostname, _webServicesConfig.APIToken, "/api/game/tokens/", new VerifyTokenRequest() { accountId = userId, code = code });
        }

        private async Task<VerifyTokenResponse> GetOrGenerateUserData(string userId, string code, Guid possibleGuid)
        {
            if ((!Guid.TryParse(userId, out var _) && !string.IsNullOrWhiteSpace(userId)))
            {
                return new VerifyTokenResponse
                {
                    token = "",
                    login = $"Dev {userId}",
                    Anon = true
                };
            }
            else if (string.IsNullOrEmpty(userId))
            {
                return new VerifyTokenResponse
                {
                    token = "",
                    login = $"Anon {possibleGuid}",
                    Anon = true
                };
            }
            else
            {
                var userDataConfirmed = await VerifyToken(userId, code);
                return userDataConfirmed;
            }
        }

        bool isAnonymous(VerifyTokenResponse data)
        {
            return data.Anon;
        }

        public async ValueTask<AccountList> GetAllAccountsOnlineImpl()
        {
            using (var wrapper = await EntitiesRepository.GetFirstService<ILoginInternalServiceEntity>())
            {
                var entityService = wrapper.GetFirstService<ILoginInternalServiceEntityServer>();

                return new AccountList()
                {
                    Accounts = await entityService.ListAllKnownAccounts()
                };
            }
        }

        public async ValueTask<CharacterList> GetAllCharactersForAccountImpl(Guid accountEntityId)
        {
            if (!await RequestAccount(accountEntityId, string.Empty))
                throw new InvalidOperationException($"Entity {accountEntityId} not found");

            try
            {
                using (var wrapper = await EntitiesRepository.Get<IAccountEntityServer>(accountEntityId))
                {
                    if (wrapper.TryGet<IAccountEntityServer>(accountEntityId, out var accountEntity))
                    {
                        return new CharacterList()
                        {
                            Characters = accountEntity.Characters.Select(v => new CharacterData() { CharacterId = v.Id, CharacterName = v.CharacterName }).ToArray()
                        };
                    }
                    else
                        throw new InvalidOperationException($"Entity {accountEntityId} failed to load");
                }
            }
            finally
            {
                await ReleaseAccount(accountEntityId);
            }
        }

        // #tmp: usings: only1 `WrldCharacter.ResyncAccountExperience`, у кот-го нет using'ов
        public async ValueTask<int> GetAccountExperienceImpl(Guid accountEntityId)
        {
            if (!await RequestAccount(accountEntityId, string.Empty))
                throw new InvalidOperationException($"Entity {accountEntityId} not found");

            try
            {
                using (var wrapper = await EntitiesRepository.Get<IAccountEntityServer>(accountEntityId))
                {
                    if (wrapper.TryGet<IAccountEntityServer>(accountEntityId, out var accountEntity))
                    {
                        return accountEntity.Experience;
                    }
                    else
                        throw new InvalidOperationException($"Entity {accountEntityId} failed to load");
                }
            }
            finally
            {
                await ReleaseAccount(accountEntityId);
            }
        }

        // #tmp: usings: НЕТ
        public async ValueTask<int> AddAccountExperienceImpl(Guid accountEntityId, int expToGive)
        {
            if (!await RequestAccount(accountEntityId, string.Empty))
                throw new InvalidOperationException($"Entity {accountEntityId} not found");

            try
            {
                using (var wrapper = await EntitiesRepository.Get<IAccountEntityServer>(accountEntityId))
                {
                    if (wrapper.TryGet<IAccountEntityServer>(accountEntityId, out var accountEntity))
                    {
                        return await accountEntity.AddExperience(expToGive);
                    }
                    else
                        throw new InvalidOperationException($"Entity {accountEntityId} failed to load");
                }
            }
            finally
            {
                await ReleaseAccount(accountEntityId);
            }
        }



        public async ValueTask<bool> DeleteAllCharactersImpl(Guid accountEntityId)
        {
            using (var wrapper = await EntitiesRepository.Get<IAccountEntityServer>(accountEntityId))
            {
                if (wrapper.TryGet<IAccountEntityServer>(accountEntityId, out var accountEntity))
                {
                    throw new InvalidOperationException($"Cannot delete characters while account {accountEntityId} is online");
                }
            }


            await EntitiesRepository.Load<IAccountEntity>(accountEntityId);
            try
            {
                using (var wrapper = await EntitiesRepository.Get<IAccountEntityServer>(accountEntityId))
                {
                    if (wrapper.TryGet<IAccountEntityServer>(accountEntityId, out var accountEntity))
                    {
                        if (!accountEntity.Characters.Any())
                            return false;

                        foreach (var character in accountEntity.Characters.Select(v => v.Id).ToArray())
                            await accountEntity.DeleteAccountCharacter(character);

                        if (accountEntity.Characters.Any())
                            throw new InvalidOperationException("Failed to delete some characters");
                    }
                    else
                        throw new InvalidOperationException($"Entity {accountEntityId} not found");
                }
                await Task.Delay(TimeSpan.FromSeconds(1));
                return true;
            }
            finally
            {
                await EntitiesRepository.Destroy<IAccountEntity>(accountEntityId, true);
            }
        }

        private async ValueTask<bool> RequestAccount(Guid accountEntityId, string userName) // empty username = do not create if does not exist
        {
            AccountEntityRequests.TryGetValue(accountEntityId, out var count);
            if (count > 0)
            {
                count++;
                AccountEntityRequests[accountEntityId] = count;
                return true;
            }

            if (await EntitiesRepository.Load<IAccountEntity>(accountEntityId) != null)
            {
                count++;
                AccountEntityRequests[accountEntityId] = count;
                return true;
            }

            if (!string.IsNullOrEmpty(userName))
            {
                await EntitiesRepository.Create<IAccountEntity>(accountEntityId, (entity) => { entity.AccountId = userName; return Task.CompletedTask; });
                Logger.IfInfo()?.Message("Created new account id {account_id} for user name {user_name}", accountEntityId, userName).Write();

                count++;
                AccountEntityRequests[accountEntityId] = count;
                return true;
            }
            else
            {
                Logger.IfError()?.Message("Account with id {account_id} for user {user_name} not found", accountEntityId, userName).Write();
                return false;
            }
        }

        private async ValueTask ReleaseAccount(Guid accountEntityId)
        {
            AccountEntityRequests.TryGetValue(accountEntityId, out var count);
            count--;
            if (count > 0)
            {
                AccountEntityRequests[accountEntityId] = count;
                return;
            }

            AccountEntityRequests.Remove(accountEntityId);
            await EntitiesRepository.Destroy<IAccountEntity>(accountEntityId, true);
        }

        public async Task<LoginResult> LoginImpl(Platform platform, string version, string userId, string code)
        {
            Guid anonGuidPossible = Guid.NewGuid();
            var devAnonGuid = GuidUtility.Create(GuidUtility.DnsNamespace, userId, 3);
            bool isDev = false;
            if (!Guid.TryParse(userId, out var _))
            {
                code = $"test|{userId}";
                if (!string.IsNullOrEmpty(userId))
                {
                    userId = devAnonGuid.ToString();
                }
                else
                {
                    // just anon
                }
                isDev = true;
            }
            VerifyTokenResponse userDataConfirmed = await GetOrGenerateUserData(userId, code, anonGuidPossible);
            if (userDataConfirmed == null)
            {
                Logger.IfWarn()?.Message("Declining login - cant verify token. Platform {platform_id} version {client_version} code {code}", platform, version, code).Write();
                return new LoginResult { Result = ELoginResult.ErrorUserIdConfirmedIsEmpty };
            }
            
            var parsed = Guid.TryParse(userId, out var possibleResult);
            anonGuidPossible = string.IsNullOrWhiteSpace(userId) ? anonGuidPossible : devAnonGuid;
            Guid accountId = userDataConfirmed.Anon && _enableAnonymousLogin ? anonGuidPossible : possibleResult;

            if (!parsed)
            {
                userDataConfirmed.Anon = true;
                Logger.IfInfo()?.Message("Player is anon login={login} accountId={accountId}", userDataConfirmed.login, accountId).Write();
            }

            var userName = userDataConfirmed.login;
            var accountEntityId = accountId;
            
            var tokenForClient = userDataConfirmed.token;

            if (isAnonymous(userDataConfirmed) && !_enableAnonymousLogin)
            {
                Logger.IfWarn()?.Message("Declining login - anonymous disabled. Platform {platform_id} version {client_version} token {user_token} code {code}", platform, version, tokenForClient, code).Write();
                return new LoginResult { Result = ELoginResult.ErrorAnonymousDisabled };
            }
            AccountType accountType = AccountType.None;
            if (isDev)
                userDataConfirmed.userTypes = new string[] { AccountType.Everything.ToString() };
            
            foreach (var ut in userDataConfirmed.userTypes)
                if (Enum.TryParse<AccountType>(ut, true, out var en))
                    accountType |= en;
            

            if (accountType.HasFlag(AccountType.User))
            {
                var currentCcu = GetCurrentCCU();
                if (currentCcu >= _maxCCU)
                {
                    Logger.IfWarn()?.Message("Declining login - server is full. CCU {ccu} >= MaxCCU {max_ccu}. User id {account_id} name {user_name} Platform {platform_id} version {client_version} token {user_token} code {code}", currentCcu, _maxCCU, platform.ToString(), version, tokenForClient, code).Write();
                    return new LoginResult { Result = ELoginResult.ErrorServerIsFull };
                }
            }

            await RequestAccount(accountEntityId, userName);
            Logger.IfInfo()?.Message("Logging in user id {account_id} name {user_name} Platform {platform_id} version {client_version} token {user_token} code {code}",
                accountEntityId, userName, platform, version, tokenForClient, code).Write();

            await EntitiesRepository.SubscribeReplication(
                ReplicaTypeRegistry.GetIdByType(typeof(IAccountEntity)),
                accountEntityId,
                Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId,
                ReplicationLevel.ClientFull
                );

            using (var wrapper2 = await EntitiesRepository.Get<IAccountEntityServer>(accountEntityId))
            {
                var accountEntity = wrapper2?.Get<IAccountEntityServer>(accountEntityId);
                if (accountEntity == null)
                {
                    Logger.IfError()?.Message("Error creating account entity {0} userId {1}", accountEntityId, userName).Write();
                    return new LoginResult { Result = ELoginResult.ErrorCreatingAccount };
                }

                var currentRealm = accountEntity.CharRealmData.CurrentRealm;
                if (currentRealm != OuterRef<IEntity>.Invalid)
                {
                    IEntityRef realmEntityRef = await EntitiesRepository.Load<IRealmEntity>(currentRealm.Guid);
                    if (realmEntityRef == null)
                    {
                        //we're on a new realm, have to auto-consume rewards
                        await accountEntity.ClearAndConsumeOldRealmRewards();
                    }
                    else
                    {
                        await EntitiesRepository.SubscribeReplication(
                            realmEntityRef.TypeId,
                            realmEntityRef.Id,
                            Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId,
                            ReplicationLevel.ClientFull
                        );
                    }
                }
                else
                {
                    Logger.IfInfo()?.Message("No CharRealmData.CurrentRealm").Write();
                }

                //TODO Remove After Replication Fix
                //хак до тех пор пока не починят гарантию репликации энтити до возвращения метода ее отреплицировавшего
                // await Task.Delay(3000);

                if (accountEntity.Characters.Count == 0)
                {
                    var createdNewCharacterResult = await accountEntity.CreateNewCharacter(userDataConfirmed.login, accountEntityId);
                    if ((createdNewCharacterResult?.Result ?? CreateNewCharacterResultType.None) !=
                        CreateNewCharacterResultType.Success)
                    {
                        Logger.IfError()?.Message("Error creating new character entity {0} userId {1}", accountEntityId, userName).Write();
                        return new LoginResult { Result = ELoginResult.ErrorCreatingCharacter };
                    }
                }

            }

            Guid characterId;
            int accountExp;
            using (var wrapper = await EntitiesRepository.Get<IAccountEntityServer>(accountEntityId))
            {
                var accountEntity = wrapper?.Get<IAccountEntityServer>(accountEntityId);
                if (accountEntity == null)
                {
                    Logger.IfError()?.Message("Account entity {0} not found for userId {1}", accountEntityId, userName).Write();
                    return new LoginResult { Result = ELoginResult.ErrorAccountNotFound };
                }

                accountExp = accountEntity.Experience; // is used only for CharacterData, but assigned by it CharacterData prop is unused 
                var accountCharacter = accountEntity.Characters.FirstOrDefault();

                if (accountCharacter == null)
                {
                    Logger.IfError()?.Message("Account entity {0} has no charcter for userId {1}", accountEntityId, userName).Write();
                    return new LoginResult { Result = ELoginResult.ErrorCharacterNotFound };
                }

                var oldUserId = await accountEntity.GetCurrentUserId();
                if (oldUserId != Guid.Empty && oldUserId != GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId)
                {
                    Logger.IfWarn()?.Message("Account {0} user {1} already connected with repositoryId {2}. New repositoryId {3}. Disconnect old user", accountEntityId, userName, oldUserId, GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId).Write();
                    using (var wrapper2 = await EntitiesRepository.Get<IClientCommunicationEntityServer>(oldUserId))
                    {
                        var clientCommunicationEntity = wrapper2?.Get<IClientCommunicationEntityServer>(oldUserId);
                        if (clientCommunicationEntity != null)
                        {
                            await logout(oldUserId);
                            await clientCommunicationEntity.DisconnectByAnotherConnection();
                        }
                        else
                        {
                            Logger.IfError()?.Message("Client communication entity {0} not found", oldUserId).Write();
                            _accountIdByUserId.TryRemove(oldUserId, out _);
                        }
                    }
                }

                characterId = accountCharacter.Id;
                _accountIdByUserId[GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId] = accountEntity.Id;
                var currentCCU = GetCurrentCCU();
                Statistics<CCUStatistics>.Instance.SetCurrentCCU(currentCCU);
                await accountEntity.SetCurrentUserId(GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId);
            }

            using (var wrapper = await EntitiesRepository.GetMasterService<IBakenCoordinatorServiceEntityServer>())
            {
                var bakenCoordinator = wrapper.GetFirstService<IBakenCoordinatorServiceEntityServer>();
                await bakenCoordinator.OnUserConnected(characterId, GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId);
            }

            var charData = new CharacterData()
            {
                CharacterId = characterId,
                CharacterName = userDataConfirmed.login,
                Packs = userDataConfirmed.founderPacks?.ToList() ?? new List<string>()
            };

            var accountData = new AccountData()
            {
                AccountId = accountEntityId,
                AccountName = userDataConfirmed.login
            };

            using (var wrapper = await EntitiesRepository.GetFirstService<ILoginInternalServiceEntity>())
            {
                var entityService = wrapper.GetFirstService<ILoginInternalServiceEntityServer>();
                await entityService.AddAccount(GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId, accountData);
                await entityService.AddCharacter(GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId, charData);
            }

            using (var wrapper = await EntitiesRepository.GetFirstService<IAccountTypeServiceEntityServer>((object)"accountTypeService"))
            {
                var accountTypeService = wrapper?.GetFirstService<IAccountTypeServiceEntityServer>();
                if (accountTypeService != null)
                    await accountTypeService.SetAccountType(
                        GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId, (long)accountType);
                else
                    Logger.IfError()?.Message("IAccountTypeServiceEntity not found for set accountType user {0} accountType", userName, accountType).Write();
            }

            return new LoginResult
            {
                CharacterId = characterId,
                AccountData = accountData,
                UserId = Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId,
                Result = ELoginResult.Success,
                PlatformApiToken = tokenForClient
            };
        }

        public async Task<Guid> GetUserRealmImpl(Guid userId)
        {

            if (!_accountIdByUserId.TryGetValue(userId, out var accountEntityId))
                return default;
            using (var wrapper = await EntitiesRepository.Get<IAccountEntityServer>(accountEntityId))
            {
                var accountEntity = wrapper?.Get<IAccountEntityServer>(accountEntityId);
                if (accountEntity == null)
                {
                    Logger.IfError()?.Message("Account entity {0} not found for userId", accountEntityId).Write();
                    return default;
                }
                var accountCharacter = accountEntity.Characters.FirstOrDefault();

                if (accountCharacter == null)
                {
                    Logger.IfError()?.Message("Account entity {0} has no charcter for userId", accountEntityId).Write();
                    return default;
                }

                return accountEntity.CharRealmData.CurrentRealm.Guid;
            }
        }
        public async ValueTask<AccountStatsData> GetAccountDataForAccStatsImpl(Guid accountId)
        {
            //#note: looks Request/Release are no-need here - acc-entity is already exist (cos we are on Login node, where its master-copy resides
            // if (!await RequestAccount(accountId, string.Empty)) ///PZ-16219: Is reques needed?
            //     throw new InvalidOperationException($"Entity {accountId} not found");
            // 
            // try
            // {
            using (var wrapper = await EntitiesRepository.Get<IAccountEntityServer>(accountId))
            {
                if (wrapper.TryGet<IAccountEntityServer>(accountId, out var accountEntity))
                    return new AccountStatsData()
                    {
                        Experience = accountEntity.Experience,
                        Gender = accountEntity.Gender
                    };
                else
                    throw new InvalidOperationException($"Entity {accountId} failed to load");
            }
            // }
            // finally
            // {
            //     await ReleaseAccount(accountId);
            // }
        }



        private int GetCurrentCCU()
        {
            var commEntityCollection = ((EntitiesRepository)EntitiesRepository).GetEntitiesCollection(typeof(IRepositoryCommunicationEntity));
            var clientConnections = commEntityCollection.Select(v => (IRepositoryCommunicationEntity)((IEntityRefExt)v.Value).GetEntity())
                .Where(v => v.CloudNodeType == SharedCode.Cloud.CloudNodeType.Client)
                .Select(v => v.Id)
                .Where(v => _accountIdByUserId.ContainsKey(v));
            var ccu = clientConnections.Count();
            return ccu;
        }
        public async Task<bool> IsMapDeadImpl(Guid map, Guid realmId)
        {
            var realmD = (await GetRealms(null, realmId)).FirstOrDefault();
            if (realmD.Key == default)
                return true;
            else
            {
                using (var rew = await EntitiesRepository.Get<IRealmEntity>(realmId))
                {
                    var realm = rew.Get<IRealmEntity>(realmId);
                    if (realm.Maps.TryGetValue(map, out var mapMeta))
                    {
                        return mapMeta.IsDead;
                    }
                }
            }
            return true;
        }

        public async ValueTask<bool> RequestUpdateAccountDataImpl(Guid accId)
        {
            Guid userId;
            if (!await RequestAccount(accId, string.Empty))
                throw new InvalidOperationException($"Entity {accId} not found");

            try
            {
                using (var wrapper = await EntitiesRepository.Get<IAccountEntityServer>(accId))
                {
                    if (wrapper.TryGet<IAccountEntityServer>(accId, out var accountEntity))
                    {
                        userId = await accountEntity.GetCurrentUserId();
                    }
                    else
                        throw new InvalidOperationException($"Entity {accId} failed to load");
                }
            }
            finally
            {
                await ReleaseAccount(accId);
            }
            var resp = await Get<ResponseUserDataById>(_webServicesConfig.APIEndpoint, _webServicesConfig.APIHostname, _webServicesConfig.APIToken, $"/api/game/users/{accId}/", new RequestUserDataById() { 
                accountId = accId.ToString()
            });

            using (var wrapper = await EntitiesRepository.GetFirstService<ILoginInternalServiceEntity>())
            {
                var entityService = wrapper.GetFirstService<ILoginInternalServiceEntityServer>();
                await entityService.UpdateAccountData(userId, resp.packs);
            }
            return true;
        }

        public async Task LogoutImpl()
        {
            Logger.IfInfo()?.Message("LoginServiceEntity User Logout {user_id}", GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId).Write();
            await logout(GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId);
        }

        public async Task<ELogoutResult> KickImpl(Guid id)
        {
            Logger.IfInfo()?.Message("Try kick user id: {user_id}", id).Write();
            Guid userId;
            using (var wrapper = await EntitiesRepository.GetFirstService<ILoginInternalServiceEntity>())
            {
                var entityService = wrapper.GetFirstService<ILoginInternalServiceEntityServer>();
                userId = await entityService.GetUserIdByAccountId(id);
            }

            if (userId != Guid.Empty)
            {
                Logger.IfInfo()?.Message("User id: {user_id} repositoryId: {repo_id} kicked from server", id, userId).Write();
                await logout(userId);
                return ELogoutResult.Success;
            }

            return ELogoutResult.ErrorUnknown;
        }

        public ValueTask<bool> SetMaxCCUImpl(int ccu)
        {
            if (ccu <= 1)
                return new ValueTask<bool>(false);

            _maxCCU = ccu;
            Statistics<CCUStatistics>.Instance.SetMaxCCU(_maxCCU);
            Logger.IfInfo()?.Message("Max ccu changed to {max_ccu} by user {user_id} ", _maxCCU, GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId).Write();
            return new ValueTask<bool>(true);
        }

        public ValueTask<int> GetCCUImpl() => new ValueTask<int>(GetCurrentCCU());
        public ValueTask<int> GetMaxCCUImpl() => new ValueTask<int>(_maxCCU);
        
        public class LeaveRealmRequest
        {
            [JsonProperty("userId")]
            public string accountId { get; set; }
            public string realmId { get; set; }
            public bool permanent { get; set; }
        }
        public class LeaveRealmResponse
        {
        }
        
        public async ValueTask<bool> NotifyPlatformOfRealmGiveUpImpl(Guid accId, Guid realmId)
        {
            var r = await Post<LeaveRealmResponse>(_webServicesConfig.APIEndpoint, _webServicesConfig.APIHostname, _webServicesConfig.APIToken, "/api/game/users/realms/left/", new LeaveRealmRequest()
            {
                accountId = accId.ToString(),
                permanent = true,
                realmId = realmId.ToString()
            });
            return true;
        }
        class JoinRealmRequest
        {
            [JsonProperty("userId")]
            public string accountId { get; set; }
            public string realmId { get; set; }
        }
        class JoinRealmResponse
        {
            [JsonProperty("id")]
            public string realmId { get; set; }
            public string version { get; set; }
            public string host { get; set; }
            public int loginPort { get; set; }
            public string definition { get; set; }
            public string status { get; set; }
            public string whenCreated { get; set; }
        }

        class RealmIsReadyRequest
        {
            public string realmId { get; set; }
        }
        class RealmIsReadyResponse
        {
            public string realmId { get; set; }
        }

        public async ValueTask<bool> GiveUpRealmOnDeathImpl(Guid accId)
        {

            if (!await RequestAccount(accId, string.Empty))
                throw new InvalidOperationException($"Entity {accId} not found");

            try
            {
                Guid userId = Guid.Empty;
                using (var wrapper = await EntitiesRepository.Get<IAccountEntityServer>(accId))
                {
                    if (wrapper.TryGet<IAccountEntityServer>(accId, out var accountEntity))
                    {
                        await accountEntity.CharRealmData.GiveUpCurrentRealm();
                    }
                    else
                        throw new InvalidOperationException($"Entity {accId} failed to load");
                }

                return true;
            }
            finally
            {
                await ReleaseAccount(accId);
            }
            return false;
        }
        public async Task<Guid> GetAccountIdByUserIdImpl(Guid userId)
        {
            _accountIdByUserId.TryGetValue(userId, out var accId);
            return accId;
        }
        public async Task<bool> AssignAccountToMapImpl(Guid accId, Guid mapId)
        {
            if (!await RequestAccount(accId, string.Empty))
                throw new InvalidOperationException($"Entity {accId} not found");

            try
            {
                bool changesMap = false;
                using (var wrapper2 = await EntitiesRepository.Get<IAccountEntityServer>(accId))
                {
                    var accountEntity = wrapper2?.Get<IAccountEntityServer>(accId);
                    var accChar = accountEntity.Characters.FirstOrDefault();
                    if (accChar.MapId != mapId)
                        changesMap = true;
                    await accChar.AssignData(mapId);
                }
                return changesMap;
            }
            finally
            {
                await ReleaseAccount(accId);
            }
        }


        public async Task<bool> GrantAccountRewardImpl(Guid accountId, RewardDef reward)
        {
            if (!await RequestAccount(accountId, string.Empty))
                throw new InvalidOperationException($"Entity {accountId} not found");

            try
            {

                using (var wrap = await EntitiesRepository.Get<IAccountEntityServer>(accountId))
                {
                    var accEntity = wrap.Get<IAccountEntityServer>(accountId);
                    if (accEntity == null)
                    {
                        Logger.Error("Cant get account with id {0}", accountId);
                        return false;
                    }
                    var accChar = accEntity.Characters.First();
                    return await accChar.GrantReward(reward);
                }
            }
            finally
            {
                await ReleaseAccount(accountId);
            }

        }
        
        public async Task<RealmStatus> GetRealmStatusImpl()
        {
            RealmStatus s = new RealmStatus();
            var r = await GetRealms(null, default);
            if (r.Count == 0)
                return s;
            var realm = r[0];
            s.RealmDef = ((IResource)realm.Value).Address.ToString();
            s.RealmId = realm.Key.ToString();
            s.CurrentCCU = await GetCCU();
            s.CurrentMaxCCU = await GetMaxCCU();
            using (var realmW = await EntitiesRepository.Get<IRealmEntity>(realm.Key))
            {
                var realmEntity = realmW.Get<IRealmEntity>(realm.Key);
                s.AttachedAccountsCount = realmEntity.AttachedAccounts.Count;
                s.AllowsToJoin = realmEntity.AllowsToJoin;
                s.IsDead = realmEntity.Dead;
                Logger.Info($"Realm status {s} {SyncTime.FromHours(realmEntity.Def.CanJoinHours)} {SyncTime.Now} {realmEntity.StartTime} {SyncTime.FromHours(realmEntity.Def.CanJoinHours) > SyncTime.Now - realmEntity.StartTime}");
            }
            return s;
        }
        public async ValueTask<bool> NotifyPlatformOfJoiningImpl(Guid accId, Guid realmId)
        {
            var r = await Post<JoinRealmResponse>(_webServicesConfig.APIEndpoint, _webServicesConfig.APIHostname, _webServicesConfig.APIToken, "/api/game/users/realms/joined/", new JoinRealmRequest()
            {
                accountId = accId.ToString(),
                realmId = realmId.ToString()
            });
            return r != null;
        }
        public async ValueTask<bool> NotifyPlatformOfLeavingImpl(Guid accId, Guid realmId)
        {
            if (realmId == default)
                return true;
            var r = await Post<LeaveRealmResponse>(_webServicesConfig.APIEndpoint, _webServicesConfig.APIHostname, _webServicesConfig.APIToken, "/api/game/users/realms/left/", new LeaveRealmRequest()
            {
                accountId = accId.ToString(),
                permanent = false,
                realmId = realmId.ToString()
            });
            return true;
        }
        public class RequestUserDataById
        {
            [JsonProperty("userId")]
            public string accountId;
        }


        public class ResponseUserDataById
        {
            [JsonProperty("userId")]
            public string accountId;
            public string[] packs;
        }
        public async Task<object> MockGet(object message)
        {
            if (message is RequestUserDataById rdid)
                return new ResponseUserDataById() {
                    accountId = rdid.accountId,
                    packs = new string[] { "commander", "email", "sentinel", "pioneer", "twitch_helmet", "twitch_standard" }
                };

            return null;
        }
        public async Task<object> MockPost(object message)
        {
            if (message is VerifyTokenRequest vtr)

            {


                return new VerifyTokenResponse()
                {
                    founderPacks = new string[] { "commander", "pioneer", "sentinel" },
                    login = vtr.code.Split('|')[1],
                    token = _webServicesConfig.APIToken,
                    userTypes = new string[] { AccountType.Developer.ToString() }
                };
            }
            if (message is BookRealmRequest brr)
            {
                var availableRealms = await GetRealms(GameResourcesHolder.Instance.LoadResource<RealmRulesQueryDef>(brr.definition), default);
                if (availableRealms.Count == 0)
                    return new BookRealmResponse()
                    {
                        realmId = Guid.NewGuid().ToString()
                    };
                else
                    return new BookRealmResponse()
                    {
                        realmId = availableRealms.First().Key.ToString()
                    };
            }
            if (message is JoinRealmRequest jrr)
            {
                return new JoinRealmResponse();
            }
            if (message is LeaveRealmRequest lrr)
            {
                return new LeaveRealmResponse();
            }
            if (message is RealmIsReadyRequest rrr)
            {
                return new RealmIsReadyResponse();
            }

            return null;

        }


        public async Task<R> Post<R>(string urlHost, string apiHostname, string authToken, string apiEndpoint, object message)
        {
            if (!LoginServiceEntity.CallPlatform)
            {
                return (R)await MockPost(message);
            }
            return await HttpHelperUtility.OkOnlyHttpSend<R>(HttpMethod.Post, apiHostname, urlHost, apiEndpoint, authToken, message);
        }
        public async Task<R> Get<R>(string urlHost, string apiHostname, string authToken, string apiEndpoint, object message)
        {
            if (!LoginServiceEntity.CallPlatform)
            {
                return (R)await MockGet(message);
            }
            return await HttpHelperUtility.OkOnlyHttpSend<R>(HttpMethod.Get, apiHostname, urlHost, apiEndpoint, authToken, message);
        }
    }

    //я не придумал ничего лучше, кроме как генерировать детерменистичные 
    //гуиды из имён девелоперских аккаунтов, чтобы обойтись без платформы
    //надеюсь это никого не укусит за задницу

    public static class GuidUtility
    { 
        /// <summary>
        /// Tries to parse the specified string as a <see cref="Guid"/>.  A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="value">The GUID string to attempt to parse.</param>
        /// <param name="guid">When this method returns, contains the <see cref="Guid"/> equivalent to the GUID
        /// contained in <paramref name="value"/>, if the conversion succeeded, or Guid.Empty if the conversion failed.</param>
        /// <returns><c>true</c> if a GUID was successfully parsed; <c>false</c> otherwise.</returns>
        public static bool TryParse(string value, out Guid guid) => Guid.TryParse(value, out guid);

        /// <summary>
        /// Converts a GUID to a lowercase string with no dashes.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns>The GUID as a lowercase string with no dashes.</returns>
        public static string ToLowerNoDashString(this Guid guid) => guid.ToString("N");

        /// <summary>
        /// Attempts to convert a lowercase, no dashes string to a GUID.
        /// </summary>
        /// <param name="value">The string.</param>
        /// <returns>The GUID, if the string could be converted; otherwise, null.</returns>
        public static Guid? TryFromLowerNoDashString(string value) => !TryParse(value, out var guid) || value != guid.ToLowerNoDashString() ? default(Guid?) : guid;

        /// <summary>
        /// Creates a name-based UUID using the algorithm from RFC 4122 §4.3.
        /// </summary>
        /// <param name="namespaceId">The ID of the namespace.</param>
        /// <param name="name">The name (within that namespace).</param>
        /// <returns>A UUID derived from the namespace and name.</returns>
        public static Guid Create(Guid namespaceId, string name) => Create(namespaceId, name, 5);

        /// <summary>
        /// Creates a name-based UUID using the algorithm from RFC 4122 §4.3.
        /// </summary>
        /// <param name="namespaceId">The ID of the namespace.</param>
        /// <param name="name">The name (within that namespace).</param>
        /// <param name="version">The version number of the UUID to create; this value must be either
        /// 3 (for MD5 hashing) or 5 (for SHA-1 hashing).</param>
        /// <returns>A UUID derived from the namespace and name.</returns>
        public static Guid Create(Guid namespaceId, string name, int version)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            // convert the name to a sequence of octets (as defined by the standard or conventions of its namespace) (step 3)
            // ASSUME: UTF-8 encoding is always appropriate
            return Create(namespaceId, Encoding.UTF8.GetBytes(name), version);
        }

        /// <summary>
        /// Creates a name-based UUID using the algorithm from RFC 4122 §4.3.
        /// </summary>
        /// <param name="namespaceId">The ID of the namespace.</param>
        /// <param name="nameBytes">The name (within that namespace).</param>
        /// <returns>A UUID derived from the namespace and name.</returns>
        public static Guid Create(Guid namespaceId, byte[] nameBytes) => Create(namespaceId, nameBytes, 5);

        /// <summary>
        /// Creates a name-based UUID using the algorithm from RFC 4122 §4.3.
        /// </summary>
        /// <param name="namespaceId">The ID of the namespace.</param>
        /// <param name="nameBytes">The name (within that namespace).</param>
        /// <param name="version">The version number of the UUID to create; this value must be either
        /// 3 (for MD5 hashing) or 5 (for SHA-1 hashing).</param>
        /// <returns>A UUID derived from the namespace and name.</returns>
        public static Guid Create(Guid namespaceId, byte[] nameBytes, int version)
        {
            if (version != 3 && version != 5)
                throw new ArgumentOutOfRangeException(nameof(version), "version must be either 3 or 5.");

            // convert the namespace UUID to network order (step 3)
            byte[] namespaceBytes = namespaceId.ToByteArray();
            SwapByteOrder(namespaceBytes);

            // compute the hash of the namespace ID concatenated with the name (step 4)
            byte[] data = namespaceBytes.Concat(nameBytes).ToArray();
            byte[] hash;
            using (var algorithm = version == 3 ? (HashAlgorithm)MD5.Create() : SHA1.Create())
                hash = algorithm.ComputeHash(data);

            // most bytes from the hash are copied straight to the bytes of the new GUID (steps 5-7, 9, 11-12)
            byte[] newGuid = new byte[16];
            Array.Copy(hash, 0, newGuid, 0, 16);

            // set the four most significant bits (bits 12 through 15) of the time_hi_and_version field to the appropriate 4-bit version number from Section 4.1.3 (step 8)
            newGuid[6] = (byte)((newGuid[6] & 0x0F) | (version << 4));

            // set the two most significant bits (bits 6 and 7) of the clock_seq_hi_and_reserved to zero and one, respectively (step 10)
            newGuid[8] = (byte)((newGuid[8] & 0x3F) | 0x80);

            // convert the resulting UUID to local byte order (step 13)
            SwapByteOrder(newGuid);
            return new Guid(newGuid);
        }

        /// <summary>
        /// The namespace for fully-qualified domain names (from RFC 4122, Appendix C).
        /// </summary>
        public static readonly Guid DnsNamespace = new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8");

        /// <summary>
        /// The namespace for URLs (from RFC 4122, Appendix C).
        /// </summary>
        public static readonly Guid UrlNamespace = new Guid("6ba7b811-9dad-11d1-80b4-00c04fd430c8");

        /// <summary>
        /// The namespace for ISO OIDs (from RFC 4122, Appendix C).
        /// </summary>
        public static readonly Guid IsoOidNamespace = new Guid("6ba7b812-9dad-11d1-80b4-00c04fd430c8");

        // Converts a GUID (expressed as a byte array) to/from network order (MSB-first).
        internal static void SwapByteOrder(byte[] guid)
        {
            SwapBytes(guid, 0, 3);
            SwapBytes(guid, 1, 2);
            SwapBytes(guid, 4, 5);
            SwapBytes(guid, 6, 7);
        }

        private static void SwapBytes(byte[] guid, int left, int right)
        {
            byte temp = guid[left];
            guid[left] = guid[right];
            guid[right] = temp;
        }
    }
}
