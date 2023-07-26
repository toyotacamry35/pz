using System;
using SharedCode.EntitySystem;
using System.Threading;
using System.Threading.Tasks;
using Assets.Src.App;
using ColonyDI;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Infrastructure.Cloud;
using JetBrains.Annotations;
using L10n;
using NLog;
using SharedCode.Aspects.Sessions;
using SharedCode.Entities;
using UnityEngine;
using SharedCode.Serializers;

namespace Uins
{
    public class StartGameGuiNode : DependencyNodeWithChildren
    {
        public enum State
        {
            Closed,
            Account,
            StartGame
        }

        [NotNull]
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private CancellationTokenSource _ownCts = new CancellationTokenSource();
        private Task _ownProcess = Task.CompletedTask;

        private CancellationTokenSource _playCts = new CancellationTokenSource();
        private Task _playProcess = Task.CompletedTask;

        private State _state = State.Closed;

        [SerializeField, UsedImplicitly]
        private AccountGuiWindow AccountGuiWindow;

        [SerializeField, UsedImplicitly]
        private StartGameGuiWindow StartGameGuiWindow;

        [SerializeField, UsedImplicitly]
        private ConfirmationDialog ConfirmationDialog;
        
        [SerializeField, UsedImplicitly]
        private DoomDialog DoomDialog;

        [Dependency]
        protected GameState GameState { get; set; }

        private void Awake()
        {
            StartGameGuiWindow.AssertIfNull(nameof(StartGameGuiWindow));
            AccountGuiWindow.AssertIfNull(nameof(AccountGuiWindow));
        }

        public override void AfterDependenciesInjected()
        {
        }

        public override void AfterDependenciesInjectedOnAllProviders()
        {
        }

        public async Task MainStartGameProcess(InnerProcess innerProc, CancellationToken ct)
        {
            using (var cts = new CancellationTokenSource())
            {
                //Init
                await ShutdownOwnProcess();

                GameState.ClientRepositoryHost.OnGracefulLogout += OnGracefulLogout;

                _ownCts = new CancellationTokenSource();
                _ownProcess = AsyncProcessExtensions.EmptyProcess(_ownCts.Token);

                var outerCancellation = AsyncProcessExtensions.EmptyProcess(ct);
                var inner = innerProc(cts.Token);

                //Idle
                await Task.WhenAny(_ownProcess, inner, outerCancellation);

                //Shutdown
                await ShutdownPlayProcess();

                cts.Cancel();
                try
                {
                    await inner;
                }
                catch (OperationCanceledException)
                {
                    Logger.IfInfo()?.Message($"Start Game Screen Inner Process Cancelled").Write();
                }

                GameState.ClientRepositoryHost.OnGracefulLogout -= OnGracefulLogout;
            }
        }

        private void OnGracefulLogout()
        {
            ExitToStartGame();
        }

        private async Task ShutdownOwnProcess()
        {
            if (_ownCts != null && _ownProcess != null)
            {
                await AsyncProcessExtensions.ShutdownProcess(_ownCts, _ownProcess);
                _ownCts = null;
                _ownProcess = null;
            }
        }

        private async Task ShutdownPlayProcess()
        {
            if (_playCts != null)
            {
                await AsyncProcessExtensions.ShutdownProcess(_playCts, _playProcess);
                _playCts = null;
                _playProcess = null;
            }
        }

        /**
        * Find Realm By Query, Set It Current And Enter Game
        */
        public async Task EnterNewGame(LobbyGuiNode lobby, RealmRulesQueryDef realmRulesQueryDef)
        {
            Task FindAndPlay(InnerProcess inner, CancellationToken ct) =>
                ConnectProcess.FindAndPlayGameInfinity(lobby, realmRulesQueryDef, GameState.AccountId, inner, ct);

            await PlayGameCommand(FindAndPlay);
        }

        /**
        * Enter Current Realm And Load Map
        */
        public async Task EnterCurrentGame(LobbyGuiNode lobby)
        {
            Task Play(InnerProcess inner, CancellationToken ct) =>
                ConnectProcess.PlayGameInfinity(lobby, GameState.AccountId, false, false, inner, ct);

            await PlayGameCommand(Play);
        }

        public void ExitToStartGame() => _playCts?.Cancel();
        public void ExitToLobby() => _ownCts?.Cancel();

        /**
        * Set Surrendered Status 
        */
        public async Task LeaveCurrentRealm()
        {
            await RunInAccount(CallLeave);
        }

        /**
        * Consume Current Realm And All Staged Rewards
        */
        public async Task ConsumeCurrentRealm()
        {
            await RunInAccount(CallConsume);
        }

        public void AutoPlayCommand(LobbyGuiNode lobby, PlayParams playParams)
        {
            var autoPlayLoaderToken = lobby.LoadingScreenUtility.Show(nameof(AutoPlayCommand));

            //Break Async Await
            AutoPlayProcess(lobby, playParams, autoPlayLoaderToken);
        }

        public void OpenConfirmationDialog(LocalizedString description, Action confirmAction, Action cancelAction = null)
        {
            var dialogParams = new ConfirmationDialogParams
            {
                OnConfirmAction = confirmAction,
                OnCancelAction = cancelAction,
                Description = description
            };
            WindowsManager.Open(ConfirmationDialog, null, dialogParams);
        }
        
        public void OpenDoomDialog(LocalizedString description)
        {
            var dialogParams = new DoomDialogParams
            {
                Description = description
            };
            WindowsManager.Open(DoomDialog, null, dialogParams);
        }
        
        public void CloseDoomDialog()
        {
            WindowsManager.Close(DoomDialog);
        }

        private async void AutoPlayProcess(LobbyGuiNode lobby, PlayParams playParams, LoadingScreenNode.Token autoPlayLoaderToken)
        {
            Task AutoPlay(InnerProcess inner, CancellationToken ct) =>
                ConnectProcess.PlayGameInfinity(lobby, GameState.AccountId, true, playParams.AutoPlay, inner, ct);

            using (autoPlayLoaderToken)
            {
                await PlayGameCommand(AutoPlay);

                if (_playProcess != null)
                    try
                    {
                        await _playProcess;
                    }
                    catch (OperationCanceledException)
                    {
                        Logger.IfInfo()?.Message("AutoPlay Process Cancelled").Write();
                    }
            }
        }

        private async Task RunInAccount(Func<IAccountEntityClientFull, Task> call)
        {
            await AsyncUtils.RunAsyncTask(
                async () =>
                {
                    using (var wrapper = await GameState.ClientClusterNode.Get<IAccountEntityClientFull>(GameState.AccountId))
                    {
                        var entity = wrapper?.Get<IAccountEntityClientFull>(GameState.AccountId);
                        if (entity != null)
                            await call(entity);
                        else
                            throw new Exception(string.Format($"[{GetType()}] AccountEntity not found"));
                    }
                }
            );
        }

        private static async Task CallLeave(IAccountEntityClientFull entity)
        {
            var result = await entity.CharRealmData.GiveUpCurrentRealm();
            Logger.IfInfo()?.Message($"Give Up Result {result}").Write();
        }

        private static async Task CallConsume(IAccountEntityClientFull entity)
        {
            var result = await entity.ConsumeRewards();
            Logger.IfInfo()?.Message($"Consume Current Realm Result {result}").Write();
        }

        private async Task PlayGameCommand(AsyncProcess asyncProcess)
        {
            await ShutdownPlayProcess();

            //Break Async Await
            DoPlay(asyncProcess);
        }

        private async void DoPlay(AsyncProcess asyncProcess)
        {
            _playCts = new CancellationTokenSource();
            _playProcess = asyncProcess.Exec(_playCts.Token);
            try
            {
                await _playProcess;
            }
            catch (OperationCanceledException)
            {
                Logger.IfInfo()?.Message("Play Process Cancelled").Write();
            }
        }

        public void SetState(State newState)
        {
            if (newState == _state)
                return;

            ExitState(_state);

            _state = newState;

            EnterState(_state);
        }

        private void EnterState(State state)
        {
            switch (state)
            {
                case State.Account:
                    WindowsManager.Open(AccountGuiWindow);
                    break;
                case State.StartGame:
                    WindowsManager.Open(StartGameGuiWindow);
                    break;
                // ReSharper disable once RedundantCaseLabel
                case State.Closed:
                default:
                    break;
            }
        }

        private void ExitState(State state)
        {
            switch (state)
            {
                case State.Account:
                    WindowsManager.Close(AccountGuiWindow);
                    break;
                case State.StartGame:
                    WindowsManager.Close(StartGameGuiWindow);
                    break;
                // ReSharper disable once RedundantCaseLabel
                case State.Closed:
                default:
                    break;
            }
        }

        public async Task OpenPreferredWindow()
        {
            var state = State.Account;

            try
            {
                var repository = GameState.ClientClusterNode;
                var accountId = GameState.AccountId;

                var (realmCharState, exist, active) = await GetCharAndRealmState(repository, accountId);

                // If Realm Exist And Not Active:
                // Continue Game Screen In Timeout Mode
                // Else If Rewards Available:
                // Rewards Screen
                // Else:
                // Account Screen
                if (exist && !active || realmCharState == RealmCharStateEnum.Success || realmCharState == RealmCharStateEnum.Surrendered)
                    state = State.StartGame;
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message($"Error Request Current Realm State For Window Select {e}").Write();
            }

            await UnityQueueHelper.RunInUnityThread(() => SetState(state));
        }

        public async Task CloseStartGameWindow()
        {
            await UnityQueueHelper.RunInUnityThread(() => SetState(State.Closed));
        }

        private static async Task<(RealmCharStateEnum charState, bool exist, bool active)> GetCharAndRealmState(
            IEntitiesRepository repository,
            Guid accountId)
        {
            return await AsyncUtils.RunAsyncTask(
                async () =>
                {
                    var (currentRealm, b) = await GetCurrentRealmData(repository, accountId);
                    var (c, d) = await GetRealmActivity(repository, currentRealm);
                    return (b, c, d);
                }
            );
        }

        private static async Task<(bool exist, bool active)> GetRealmActivity(IEntitiesRepository repository, OuterRef<IEntity> realm)
        {
            var exist = false;
            var active = false;
            if (realm != OuterRef<IEntity>.Invalid)
                using (var wrapper = await repository.Get(realm.TypeId, realm.Guid))
                {
                    var entity = wrapper?.Get<IRealmEntityClientFull>(realm.Guid);
                    if (entity != null)
                    {
                        var startTime = entity.StartTime;
                        var realmRulesDef = entity.Def;
                        exist = true;
                        active = !RealmEntity.IsRealmDead(realmRulesDef, startTime);
                    }
                    else
                    {
                        Logger.IfError()?.Message("Error Get IRealmEntity").Write();
                    }
                }

            return (exist, active);
        }

        private static async Task<(OuterRef<IEntity> realm, RealmCharStateEnum charState)> GetCurrentRealmData(
            IEntitiesRepository repository,
            Guid accountId)
        {
            OuterRef<IEntity> currentRealm = default;
            var realmCharState = RealmCharStateEnum.Inactive;

            using (var wrapper = await repository.Get<IAccountEntityClientFull>(accountId))
            {
                var entity = wrapper?.Get<IAccountEntityClientFull>(accountId);
                if (entity != null)
                {
                    realmCharState = entity.CharRealmData.CurrentRealmCharState;
                    currentRealm = entity.CharRealmData.CurrentRealm;
                }
                else
                {
                    Logger.IfError()?.Message("Error Get IAccountEntity").Write();
                }
            }

            return (currentRealm, realmCharState);
        }
    }
}