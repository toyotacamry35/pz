using SharedCode.EntitySystem;
using System;
using System.Threading.Tasks;
using GeneratedCode.Custom.Config;
using Assets.Src.Scenes;
using Assets.Src.Aspects;
using Assets.Src.Cartographer;
using System.Threading;
using Infrastructure.Cloud;
using SharedCode.Aspects.Item.Templates;
using System.Linq;
using Assets.Src.Server.Impl;
using Core.Environment.Logging.Extension;

namespace Assets.Src.Client
{
    public class MapLoader
    {
        private const string LoadingScreenKey = "SceneStreamerPWL";

        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly CharacterRuntimeData _charData;

        private LoadingScreenNode.Token _token;

        public MapLoader(CharacterRuntimeData charData)
        {
            _charData = charData;
        }

//         public static async Task LoadInf(MapDef mapDef, CharacterRuntimeData charData, InnerProcess next, CancellationToken ct)
//         {
//              Logger.IfInfo()?.Message("Invoke map loader with {0}",  mapDef).Write();
//
//             /*var loadedScenes = mapDef.GlobalScenesClient;
//             if (Constants.WorldConstants.DevMode)
//                 loadedScenes = loadedScenes.Where(x => !mapDef.ExcludeInDevMode.Contains(x)).ToArray();
//             if (ServerProvider.IsHost && mapDef.NeedsUnity)
//                 loadedScenes = loadedScenes.Where(x => !mapDef.GlobalScenesServer.Contains(x)).ToArray();
//             await Bootstrapper.LoadFolder(mapDef.DebugName + "-Pack");
//
//             var cachedGameState = GameState.Instance;
//
//             using (((IEntitiesRepositoryExtension)ClusterCommands.ClientRepository)?.DisableRepositoryEntityUnlockTimeout())
//             {
//                 await ScenesLoader.LoadScenesAsync(loadedScenes);
//                 await ScenesLoader.ClearAfterSceneLoading();
//             }
//
//             cachedGameState.CurrentMap = mapDef;
//
//              Logger.IfInfo()?.Message("Initialize streamer").Write();;
//             var ml = new MapLoader(charData);
//             var streamer = SceneStreamerSystem.Streamer;
//             await UnityQueueHelper.RunInUnityThread(() =>
//             {
//                 streamer.StatusChanged += ml.OnStreamerStatusChanged;
//                 streamer.Initialize(mapDef.SceneCollectionClient, mapDef.SceneStreamerClient);
//             });*/
//
//             try
//             {
//                 await next(ct);
//             }
//             finally
//             {
//                 /*await UnityQueueHelper.RunInUnityThread(() =>
//                 {
//                     streamer.StatusChanged -= ml.OnStreamerStatusChanged;
//                     streamer.Deinitialize();
//                 });
//
//                 cachedGameState.CurrentMap = null;
//                 await ScenesLoader.UnloadScenesAsync(loadedScenes);
//                 */
//             }
//         }

        public void OnStreamerStatusChanged(System.Guid id, SceneStreamerStatus oldStatus, SceneStreamerStatus newStatus)
        {
            if ((_charData != null) && id.Equals(_charData.CharacterId))
            {
                if (newStatus >= SceneStreamerStatus.ImportantReady)
                {
                     Logger.IfInfo()?.Message("Streamer is ready now: Hide loading screen").Write();;
                    SceneStreamerSystem.Streamer.Mode = SceneStreamerMode.Default;
                    if (LoadingScreenNode.Instance) { if (_token != null) { _token.Dispose(); _token = null; } }
                }
                else if (newStatus <= SceneStreamerStatus.NotReady)
                {
                    SceneStreamerSystem.Streamer.Mode = SceneStreamerMode.OptimiseLoadtime;
                     Logger.IfInfo()?.Message("Streamer not ready: Show loading screen").Write();;
                    if (LoadingScreenNode.Instance) { if (_token != null) { _token.Dispose();  Logger.IfError()?.Message("Token already exists").Write();; } _token = LoadingScreenNode.Instance.Show(LoadingScreenKey); }
                }
            }
        }
    }
}
