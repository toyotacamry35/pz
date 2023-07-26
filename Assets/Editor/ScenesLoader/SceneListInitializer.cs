using ResourcesSystem.Loader;
using Assets.Src.ResourceSystem.Editor;
using Assets.Src.Scenes;
using Infrastructure.Config;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Editor.ScenesLoader
{
    public class SceneListInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitMapList()
        {
            ScenesInBuild.Scenes = GetScenes().ToArray();
        }

        private static IEnumerable<string> GetScenes()
        {
            var gr = new GameResources(new FolderLoader(Application.dataPath));
            gr.ConfigureForUnityEditor();

            var mapRoot = gr.LoadResource<MapRootDef>("/Scenes/MapRoot");

            foreach (var systemScene in mapRoot.SystemScenes)
            {
                yield return systemScene;
            }

            foreach (var systemScene in mapRoot.KeyDependencyScenes)
            {
                yield return systemScene.Path;
            }

            foreach (var map in mapRoot.Maps)
            {
                var mapDef = map.Target;
                foreach (var globalClientScene in mapDef.GlobalScenesClient)
                {
                    yield return globalClientScene;
                }

                foreach (var globalServerScene in mapDef.GlobalScenesServer)
                {
                    yield return globalServerScene;
                }

                foreach (var localScene in mapDef.LocalScenesClient)
                {
                    yield return localScene.SceneName;
                }
            }
        }
    }
}
