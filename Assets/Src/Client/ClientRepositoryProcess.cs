using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.EntitySystem;
using GeneratedCode.Repositories;
using Infrastructure.Cloud;
using NLog;
using SharedCode.Aspects.Sessions;
using SharedCode.Data;
using SharedCode.Entities;
using SharedCode.Entities.Cloud;
using SharedCode.EntitySystem;
using SharedCode.Repositories;
using SharedCode.Serializers;
using Uins;
using UnityEngine;
using Logger = NLog.Logger;

namespace Assets.Src.Client
{
    public class ClientRepositoryProcess : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly TaskCompletionSource<bool> _disconnectedTcs;
        private readonly TaskCompletionSource<bool> _loginConfirmedTcs;

        private readonly EntitiesRepository _repository;
        private OuterRef<ILoginServiceEntity> _loginEntityRef;

        public ClientRepositoryProcess(EntitiesRepository repository)
        {
            _repository = repository;

            _disconnectedTcs = new TaskCompletionSource<bool>();
            _loginConfirmedTcs = new TaskCompletionSource<bool>();
        }

        public Task DisconnectionTask => _disconnectedTcs.Task;

        public void Dispose()
        {
            try
            {
                if (_loginEntityRef.IsValid)
                    _repository.UnsubscribeOnDestroyOrUnload(_loginEntityRef.TypeId, _loginEntityRef.Guid, OnLoginServiceDestroyOrUnload);
            }
            catch (Exception e)
            {
                Logger.IfWarn()?.Message($"Dispose Can't Unsubscribe {e}").Write();
            }
        }

        public event Action OnGracefulLogout;

        public async Task FindGame(Guid accountId, RealmRulesQueryDef realmRulesQueryDef, InnerProcess inner, CancellationToken ct)
        {
            await AsyncUtils.RunAsyncTask(() => FindGameInternal(accountId, realmRulesQueryDef));
            await inner(ct);
        }

        private async Task FindGameInternal(Guid accountId, RealmRulesQueryDef realmRulesQueryDef)
        {
            using (var accountEntityWrapper = await _repository.Get<IAccountEntityClientFull>(accountId))
            {
                var entity = accountEntityWrapper?.Get<IAccountEntityClientFull>(accountId);
                if (entity == null)
                    throw new Exception(string.Format($"[{typeof(ClientRepositoryProcess)}] AccountEntity not found"));

                var realmOperationResult = await entity.CharRealmData.FindRealm(realmRulesQueryDef);

                GameState.Instance.CharacterRuntimeData.CharacterId = entity.Characters.First().Id;
                if (realmOperationResult.CantGetRealm)
                    throw new Exception($"Can't get realm with query {realmRulesQueryDef?.____GetDebugShortName()}");
                if (realmOperationResult.ReconnectRequired)
                    AppStart.Restart();
            }
        }

        public async Task EnterGame(Guid accountId, bool autoPlay, InnerProcess inner, CancellationToken ct)
        {
            await AsyncUtils.RunAsyncTask(() => EnterGameInternal(accountId, autoPlay, inner, ct));
        }

        private async Task EnterGameInternal(Guid accountId, bool autoPlay, InnerProcess next, CancellationToken ct)
        {
            RealmOperationResult realmOperationResult;
            using (var accountEntityWrapper = await _repository.Get<IAccountEntityClientFull>(accountId))
            {
                var entity = accountEntityWrapper?.Get<IAccountEntityClientFull>(accountId);
                if (entity == null)
                    throw new Exception(string.Format($"[{GetType()}] AccountEntity not found"));

                realmOperationResult = await entity.CharRealmData.EnterCurrentRealm(autoPlay);
            }

            if (realmOperationResult.ReconnectRequired)
            {
                AppStart.Restart();
                return;
            }

            if (realmOperationResult.CantGetRealm)
            {
                Logger.IfInfo()?.Message(string.Format($"[{GetType()}] Can't Get Realm Data"), false).Write();
                return;
            }

            await Task.Delay(1000, ct);

            var timeout = await UnityQueueHelper.RunInUnityThread(() => Application.isEditor)
                ? TimeSpan.FromMinutes(3)
                : TimeSpan.FromSeconds(30);
            var timeoutTask = CreateTimeoutTask("Timeout while trying to receive character", timeout, ct);
            var loginConfirmedTask = _loginConfirmedTcs.Task;
            // Disconnect And Timeout Always Throws Exception
            await Task.WhenAny(loginConfirmedTask, timeoutTask).Unwrap();

            try
            {
                await next(ct);
            }
            catch (OperationCanceledException)
            {
                 Logger.IfInfo()?.Message("Game Process Cancelled").Write();;
            }
        }

        public async Task SubscribeDisconnectedEvent()
        {
            using (var wrapper = await _repository.GetMasterService<IClientCommunicationEntity>())
            {
                var clientEntity = wrapper.GetMasterService<IClientCommunicationEntity>();
                clientEntity.DisconnectedByAnotherConnection += OnClientEntityDisconnectedByAnotherConnection;
                clientEntity.DisconnectedByError += OnClientEntityDisconnectedByError;
                clientEntity.LoginConfirmed += OnClientEntityLoginConfirmed;
                clientEntity.GracefullLogoutEvent += OnClientGracefulLogout;
            }
        }

        public async Task UnsubscribeDisconnectedEvent()
        {
            using (var wrapper = await _repository.GetMasterService<IClientCommunicationEntity>())
            {
                var clientEntity = wrapper.GetMasterService<IClientCommunicationEntity>();
                clientEntity.DisconnectedByAnotherConnection -= OnClientEntityDisconnectedByAnotherConnection;
                clientEntity.DisconnectedByError -= OnClientEntityDisconnectedByError;
                clientEntity.LoginConfirmed -= OnClientEntityLoginConfirmed;
                clientEntity.GracefullLogoutEvent -= OnClientGracefulLogout;
            }
        }

        private Task OnLoginServiceDestroyOrUnload(int typeId, Guid id, IEntity entity)
        {
            _disconnectedTcs.TrySetException(new Exception("Connection Lost"));
            return Task.CompletedTask;
        }

        private Task OnClientEntityDisconnectedByAnotherConnection()
        {
            _disconnectedTcs.TrySetException(new Exception("Disconnected by another connection"));
            return Task.CompletedTask;
        }

        private async Task OnClientGracefulLogout()
        {
            // _disconnectedTcs.TrySetException(new Exception($"Graceful logout TEMPORARY"));
            await UnityQueueHelper.RunInUnityThread(
                () => { OnGracefulLogout?.Invoke(); }
            );
        }

        private Task OnClientEntityDisconnectedByError(string reason, string details)
        {
            _disconnectedTcs.TrySetException(new Exception($"Internal server error: {reason} {details}"));
            return Task.CompletedTask;
        }

        private Task OnClientEntityLoginConfirmed()
        {
            _loginConfirmedTcs.SetResult(true);
            return Task.CompletedTask;
        }

        private static async Task CreateTimeoutTask(string opText, TimeSpan duration, CancellationToken baseToken)
        {
            await Task.Delay(duration, baseToken);
            throw new Exception(opText);
        }

        // private static async Task<LoginInterop.VerifyUserLoginTokenResponse> GetShortTermToken(
        //     string verifyServerAddress,
        //     string longTermToken)
        // {
        //     if (string.IsNullOrEmpty(longTermToken))
        //         return null;
        //
        //     var res = await LoginInterop.VerifyToken(verifyServerAddress, longTermToken, CancellationToken.None);
        //     return res;
        // }

        public async Task<(CharacterRuntimeData runtimeData, LoginResult loginResult, Guid loginEntityId)> ConnectToCluster(
            string verifyServerAddress,
            string code,
            string userId,
            string version,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var (loginResult, loginEntityId) = await ConnectToLoginNodeAndLogin(verifyServerAddress, version, code, userId, ct);

             Logger.IfInfo()?.Message("Login success. Character {0} User {1}",  loginResult.CharacterId, loginResult.UserId).Write();


            var runtimeData = new CharacterRuntimeData
            {
                CharacterId = loginResult.CharacterId,
                UserId = loginResult.UserId
            };
            if (ClientCommRuntimeDataProvider.CurrentPrimaryWorldSceneRepositoryId != Guid.Empty)
                runtimeData.CurrentPrimaryWorldSceneRepositoryId = ClientCommRuntimeDataProvider.CurrentPrimaryWorldSceneRepositoryId;

            return (runtimeData, loginResult, loginEntityId);
        }

        private async Task<(LoginResult result, Guid loginEntityId)> ConnectToLoginNodeAndLogin(
            string verifyServerAddress,
            string version,
            string code,
            string userId,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var retryCount = 5;
            GameState.Instance.NetIssueText = null;
            while (true)
            {
                try
                {
                    /*string shortTermToken = string.Empty;
                    LoginInterop.VerifyUserLoginTokenResponse response = default;
                    if (!string.IsNullOrWhiteSpace(verifyServerAddress) && !string.IsNullOrWhiteSpace(token))
                    {
                        response = await GetShortTermToken(verifyServerAddress, token);
                        shortTermToken = response?.data.auth_token ?? string.Empty;
                    }
                    else
                    {
                        Logger.Warn(
                            "Verify server is '{0}', long term token is '{1}'. Trying to log in anonymously",
                            verifyServerAddress,
                            token);
                    }*/

                    using (var loginServiceWrapper = await _repository.GetFirstService<ILoginServiceEntityServer>())
                    {
                        var entity = loginServiceWrapper?.GetFirstService<ILoginServiceEntityServer>();
                        if (entity != null)
                        {
                            var result = await entity.Login(Platform.Dev, version, userId, code);
                            if (result != null)
                                switch (result.Result)
                                {
                                    case ELoginResult.Success:
                                    {
                                        var accountData = result.AccountData;
                                        UnityQueueHelper.RunInUnityThreadNoWait(
                                            () =>
                                            {
                                                if (WatermarkScreen.Instance != null)
                                                    WatermarkScreen.Instance.SetParams(accountData.AccountName, accountData.AccountId.ToString());
                                            });
                                        _loginEntityRef = new OuterRef<ILoginServiceEntity>(
                                            entity.Id,
                                            ReplicaTypeRegistry.GetIdByType(typeof(ILoginServiceEntity)));
                                        _repository.SubscribeOnDestroyOrUnload(
                                            _loginEntityRef.TypeId,
                                            _loginEntityRef.Guid,
                                            OnLoginServiceDestroyOrUnload
                                        );

                                        return (result, entity.Id);
                                    }
                                    case ELoginResult.ErrorAnonymousDisabled:
                                        throw new UnauthorizedAccessException("Anonymous login is disabled");
                                    case ELoginResult.ErrorUserIdConfirmedIsEmpty:
                                        if (retryCount - 1 < 0)
                                            GameState.Instance.NetIssueText = $"Error entering the game. Reason: {result.Result}. Your authorization info is expired. Please, try restarting your client. If this problem persists, try contacting our support.";
                                        break;
                                    case ELoginResult.None:
                                    case ELoginResult.ErrorBanned:
                                    case ELoginResult.ErrorServerIsFull:
                                    case ELoginResult.ErrorCreatingAccount:
                                    case ELoginResult.ErrorCreatingCharacter:
                                    case ELoginResult.ErrorAccountNotFound:
                                    case ELoginResult.ErrorCharacterNotFound:
                                    case ELoginResult.ErrorWorldNode:
                                    case ELoginResult.ErrorSceneNotFound:
                                    case ELoginResult.ErrorUnknown:
                                        {
                                            if (retryCount - 1 < 0)
                                                GameState.Instance.NetIssueText = $"Error entering the game. Reason: {result.Result}. Please, try restarting your client. If this problem persists, try contacting our support.";
                                        }
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }

                            if (--retryCount < 0)
                                // TODO Vabavia Exit Steam App on Login Failed
                                throw new Exception($"Login failed: {result?.Result.ToString() ?? "null"}");
                            Logger.IfWarn()?.Message("Login failed: {0}, retrying", result?.Result.ToString() ?? "null").Write();
                        }
                        else
                        {
                            if (--retryCount < 0)
                                throw new Exception("Login service not found");
                            Logger.IfWarn()?.Message("Login service not found, retrying").Write();
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5), ct);
                }
                catch (RpcTimeoutException e)
                {
                    if (--retryCount < 0)
                    {
                        GameState.Instance.NetIssueText = $"Error entering the game. Reason: timeout. Please try restarting your client. If this problem persists, it can mean our servers are on maintance.";
                        throw;
                    }

                    Logger.IfWarn()?.Message(e, "Login timeout, retrying").Write();
                }
            }
        }
    }
}