using System;
using Assets.Src.Scenes;
using Infrastructure.Cloud;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Uins.Sound;

namespace Assets.Src.GuiRoot
{
    public static class LoadLobbyProcess
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static Task EnterLobbySequenceInf(LobbyGuiNode lobby, InnerProcess inner, CancellationToken ct)
        {
            AsyncProcess loadScenes = (innerInner, ctInner) => LoadAdditionalScenesInf(lobby, innerInner, ctInner);
            AsyncProcess showLobby = (innerInner, ctInner) => ShowLobbyScreen(lobby, innerInner, ctInner);

            return loadScenes.Then(showLobby).Exec(inner, ct);
        }

        public static Task LeavingLobbySequenceInf(LobbyGuiNode lobby, InnerProcess inner, CancellationToken ct)
        {
            AsyncProcess showLoadingScreen = (innerInner, ctInner) => ShowLoadingScreenInf(lobby.LoadingScreenUtility, innerInner, ctInner);
            AsyncProcess hideLobby = (innerInner, ctInner) => HideLobbyScreen(lobby, innerInner, ctInner);

            return showLoadingScreen.Then(hideLobby).Exec(inner, ct);
        }

        public static async Task SetStartupParams(LobbyGuiNode lobby, StartupParams startupParams, InnerProcess inner, CancellationToken ct)
        {
            await UnityQueueHelper.RunInUnityThread(() => lobby.SetStartup(startupParams));

            await inner(ct);
        }

        private static async Task HideLobbyScreen(LobbyGuiNode lobby, InnerProcess inner, CancellationToken ct)
        {
            await UnityQueueHelper.RunInUnityThread(() => { lobby.IsVisibleRp.Value = false; });

            try
            {
                await inner(ct);
            }
            finally
            {
                await UnityQueueHelper.RunInUnityThread(
                    () =>
                    {
                        lobby.IsVisibleRp.Value = true;
                    });
            }
        }

        private static async Task ShowLobbyScreen(LobbyGuiNode lobby, InnerProcess inner, CancellationToken ct)
        {
            await UnityQueueHelper.RunInUnityThread(
                () =>
                {
                    lobby.IsVisibleRp.Value = true;
                });

            try
            {
                await inner(ct);
            }
            finally
            {
                await UnityQueueHelper.RunInUnityThread(() => { lobby.IsVisibleRp.Value = false; });
            }
        }

        public static async Task UnloadAdditionalScenesInf(LobbyGuiNode lobby, InnerProcess inner, CancellationToken ct)
        {
            await UnloadAdditionalLobbyScenes(lobby.LobbyScenes);

            try
            {
                await inner(ct);
            }
            finally
            {
                await LoadAdditionalLobbyScenes(lobby.LobbyScenes);
            }
        }

        private static async Task LoadAdditionalScenesInf(LobbyGuiNode lobby, InnerProcess inner, CancellationToken ct)
        {
            await LoadAdditionalLobbyScenes(lobby.LobbyScenes);

            try
            {
                await inner(ct);
            }
            finally
            {
                await UnloadAdditionalLobbyScenes(lobby.LobbyScenes);
            }
        }

        public static async Task ShowLoadingScreenInf(LoadingScreenNode loadingNode, InnerProcess inner, CancellationToken ct)
        {
            using (await loadingNode.ShowAsync(nameof(ShowLoadingScreenInf)))
            {
                await inner(ct);
            }
        }

        public static async Task HideLoadingScreenInf(LoadingScreenNode loadingScreen, InnerProcess inner, CancellationToken ct)
        {
            using (await loadingScreen.HideAsync(nameof(HideLoadingScreenInf)))
            {
                await inner(ct);
            }
        }

        private static async Task LoadAdditionalLobbyScenes(IEnumerable<string> scenes)
        {
            var sceneNames = scenes as string[] ?? scenes.ToArray();
            if (sceneNames.Any())
                await ScenesLoader.LoadScenesAsync(sceneNames);
            await UnityQueueHelper.RunInUnityThread(ScenesLoader.ClearAfterSceneLoading);

            await UnityQueueHelper.RunInUnityThread(
                () =>
                {
                    if (SoundControl.Instance != null)
                        SoundControl.Instance.PlayMusicEvent(SoundControl.Instance.MainThemeMusicEvent, true);
                });
        }

        private static async Task UnloadAdditionalLobbyScenes(IEnumerable<string> scenes)
        {
            var sceneNames = scenes as string[] ?? scenes.ToArray();
            if (sceneNames.Any())
                await ScenesLoader.UnloadScenesAsync(sceneNames);
        }
    }
}