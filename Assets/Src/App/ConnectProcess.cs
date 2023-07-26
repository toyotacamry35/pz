using Assets.Src.GuiRoot;
using Assets.Src.Server.Impl;
using Infrastructure.Cloud;
using Infrastructure.Config;
using SharedCode.Config;
using System.Threading;
using System.Threading.Tasks;
using Uins.Cursor;
using Assets.Src.Client;
using SharedCode.Aspects.Sessions;
using System;

namespace Assets.Src.App
{
    static class ConnectProcess
    {
        private const string OwnerNameForLoadingScreenUtility = "LoadingContentScenes";

        public static Task HostInf(
            LobbyGuiNode lobby,
            string verifyServerAddress,
            ConnectionParams connectionParams,
            ContainerConfig config,
            CloudSharedDataConfig sharedConfig,
            EntitiesRepositoryConfig clientRepoConfig,
            InnerProcess inner,
            CancellationToken ct)
        {
            AsyncProcess unloadLobby = (innerInner, ctInner) =>
                LoadLobbyProcess.LeavingLobbySequenceInf(lobby, innerInner, ctInner);
            AsyncProcess startServer = (innerInner, ctInner) =>
                SimpleServer.RunHost(config, sharedConfig, innerInner, ctInner);
            AsyncProcess connect = (innerInner, ctInner) => ConnectSequenceInf(
                lobby,
                clientRepoConfig,
                connectionParams,
                verifyServerAddress,
                default,
                innerInner,
                ctInner
            );
            return unloadLobby.Then(startServer).Then(connect).Exec(inner, ct);
        }

        public static Task ConnectInf(
            LobbyGuiNode lobby,
            string verifyServerAddress,
            ConnectionParams connectionParams,
            EntitiesRepositoryConfig clientRepoConfig,
            PlayParams playParams,
            bool hideLoader,
            InnerProcess inner,
            CancellationToken ct)
        {
            AsyncProcess unloadLobby = (innerInner, ctInner) =>
                LoadLobbyProcess.LeavingLobbySequenceInf(lobby, innerInner, ctInner);
            if (hideLoader)
            {
                AsyncProcess hideLoadingScreen = (innerInner, ctInner) =>
                    LoadLobbyProcess.HideLoadingScreenInf(lobby.LoadingScreenUtility, innerInner, ctInner);
                unloadLobby = unloadLobby.Then(hideLoadingScreen);
            }

            AsyncProcess connect = (innerInner, ctInner) => ConnectSequenceInf(
                lobby,
                clientRepoConfig,
                connectionParams,
                verifyServerAddress,
                playParams,
                innerInner,
                ctInner);

            return unloadLobby.Then(connect).Exec(inner, ct);
        }

        public static Task FindAndPlayGameInfinity(
            LobbyGuiNode lobby,
            RealmRulesQueryDef realmRulesQueryDef,
            Guid accountId,
            InnerProcess inner,
            CancellationToken ct)
        {
            var repositoryHost = GameState.Instance.ClientRepositoryHost;

            AsyncProcess leaveStartGame = (innerInner, ctInner) =>
                StartGameScreenProcess.LeaveStartGameSequenceInf(lobby, innerInner, ctInner);
            AsyncProcess findRealm = (innerInner, ctInner) =>
                repositoryHost.FindGame(accountId, realmRulesQueryDef, innerInner, ctInner);
            AsyncProcess enterGame = (innerInner, ctInner) =>
                repositoryHost.EnterGame(accountId, false, innerInner, ctInner);
            AsyncProcess hideLoadingScreen = (innerInner, ctInner) =>
                LoadLobbyProcess.HideLoadingScreenInf(lobby.LoadingScreenUtility, innerInner, ctInner);

            return leaveStartGame
                .Then(findRealm)
                .Then(enterGame)
                .Then(hideLoadingScreen)
                .Then(SwitchOnCursorLock)
                .Then(SwitchIngameFlag)
                .Exec(inner, ct);
        }

        public static Task PlayGameInfinity(
            LobbyGuiNode lobby,
            Guid accountId,
            bool hideLoader,
            bool autoPlay,
            InnerProcess inner,
            CancellationToken ct)
        {
            var repositoryHost = GameState.Instance.ClientRepositoryHost;
            AsyncProcess leaveStartGame = (innerInner, ctInner) =>
                StartGameScreenProcess.LeaveStartGameSequenceInf(lobby, innerInner, ctInner);
            if (hideLoader)
            {
                AsyncProcess hideExtraLoadingScreen = (innerInner, ctInner) =>
                    LoadLobbyProcess.HideLoadingScreenInf(lobby.LoadingScreenUtility, innerInner, ctInner);
                leaveStartGame = leaveStartGame.Then(hideExtraLoadingScreen);
            }

            AsyncProcess enterGame = (innerInner, ctInner) => repositoryHost.EnterGame(accountId, autoPlay, innerInner, ctInner);
            AsyncProcess hideLoadingScreen = (innerInner, ctInner) =>
                LoadLobbyProcess.HideLoadingScreenInf(lobby.LoadingScreenUtility, innerInner, ctInner);

            return leaveStartGame
                .Then(enterGame)
                .Then(hideLoadingScreen)
                .Then(SwitchOnCursorLock)
                .Then(SwitchIngameFlag)
                .Exec(inner, ct);
        }

        private static Task ConnectSequenceInf(
            LobbyGuiNode lobby,
            EntitiesRepositoryConfig clientRepoConfig,
            ConnectionParams connectionParams,
            string verifyServerAddress,
            PlayParams playParams,
            InnerProcess inner,
            CancellationToken ct)
        {
            AsyncProcess connect = (innerInner, ctInner) =>
                ClientRepositoryProcessRunner.StartInf(
                    clientRepoConfig,
                    connectionParams,
                    verifyServerAddress,
                    innerInner,
                    ctInner);
            AsyncProcess enterStartGame = (innerInner, ctInner) =>
                StartGameScreenProcess.EnterStartGameSequenceInf(lobby, playParams, innerInner, ctInner);
            return connect.Then(enterStartGame).Exec(inner, ct);
        }


        public static async Task SwitchIngameFlag(InnerProcess inner, CancellationToken ct)
        {
            var gs = GameState.Instance;
            await UnityQueueHelper.RunInUnityThread(() => gs.IsInGameRp.Value = true);
            try
            {
                await inner(ct);
            }
            finally
            {
                await UnityQueueHelper.RunInUnityThread(() => gs.IsInGameRp.Value = false);
            }
        }

        private static async Task SwitchOnCursorLock(InnerProcess next, CancellationToken ct)
        {
            var token = await UnityQueueHelper.RunInUnityThread(() => CursorControl.AddCursorLockRequest(OwnerNameForLoadingScreenUtility));
            try
            {
                await next(ct);
            }
            finally
            {
                await UnityQueueHelper.RunInUnityThread(() => token.Dispose());
            }
        }
    }
}