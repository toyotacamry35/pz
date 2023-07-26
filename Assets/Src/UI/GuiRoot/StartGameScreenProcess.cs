using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Cloud;
using Uins;

namespace Assets.Src.GuiRoot
{
    public static class StartGameScreenProcess
    {
        public static Task EnterStartGameSequenceInf(LobbyGuiNode lobby, PlayParams playParams, InnerProcess next, CancellationToken ct)
        {
            var startGame = lobby.StartGameGuiNode;

            AsyncProcess showStartGame = (innerInner, ctInner) =>
                ShowStartGameScreen(startGame, innerInner, ctInner);
            AsyncProcess mainStartGame = (innerInner, ctInner) =>
                startGame.MainStartGameProcess(innerInner, ctInner);
            if (playParams.AutoPlay)
            {
                AsyncProcess autoPlay = (innerInner, ctInner) =>
                    AutoPlay(lobby, startGame, playParams, innerInner, ctInner);
                mainStartGame = mainStartGame.Then(autoPlay);
            }

            AsyncProcess hideLoadingScreen = (innerInner, ctInner) =>
                LoadLobbyProcess.HideLoadingScreenInf(lobby.LoadingScreenUtility, innerInner, ctInner);

            return showStartGame.Then(mainStartGame).Then(hideLoadingScreen).Exec(next, ct);
        }

        public static Task LeaveStartGameSequenceInf(LobbyGuiNode lobby, InnerProcess next, CancellationToken ct)
        {
            var startGame = lobby.StartGameGuiNode;
            var loadingScreen = lobby.LoadingScreenUtility;

            AsyncProcess showLoadingScreen = (innerInner, ctInner) =>
                LoadLobbyProcess.ShowLoadingScreenInf(loadingScreen, innerInner, ctInner);
            AsyncProcess hideStartGame = (innerInner, ctInner) => HideStartGameScreen(startGame, innerInner, ctInner);
            AsyncProcess hideAdditionalScenes = (innerInner, ctInner) =>
                LoadLobbyProcess.UnloadAdditionalScenesInf(lobby, innerInner, ctInner);

            return showLoadingScreen.Then(hideStartGame).Then(hideAdditionalScenes).Exec(next, ct);
        }

        private static async Task AutoPlay(
            LobbyGuiNode lobby,
            StartGameGuiNode startGame,
            PlayParams playParams,
            InnerProcess inner,
            CancellationToken ct)
        {
            await UnityQueueHelper.RunInUnityThread(() => startGame.AutoPlayCommand(lobby, playParams));

            await inner(ct);
        }


        private static async Task ShowStartGameScreen(
            StartGameGuiNode startGameGuiNode,
            InnerProcess next,
            CancellationToken ct)
        {
            await startGameGuiNode.OpenPreferredWindow();

            try
            {
                await next(ct);
            }
            finally
            {
                await startGameGuiNode.CloseStartGameWindow();
            }
        }

        private static async Task HideStartGameScreen(
            StartGameGuiNode startGameGuiNode,
            InnerProcess next,
            CancellationToken ct)
        {
            await startGameGuiNode.CloseStartGameWindow();

            try
            {
                await next(ct);
            }
            finally
            {
                await startGameGuiNode.OpenPreferredWindow();
            }
        }
    }
}