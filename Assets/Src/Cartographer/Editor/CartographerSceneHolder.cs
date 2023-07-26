using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace Assets.Src.Cartographer.Editor
{
    public class CartographerSceneHolder
    {
        public static int DefaultKeepCount = 64;
        private int keepCount = 0;
        private bool keepFirst = false;
        private List<Scene> openedScenes = new List<Scene>();

        private void Close(bool all)
        {
            if (openedScenes.Count > 0)
            {
                for (var index = (all ? 0 : 1); index < openedScenes.Count; ++index)
                {
                    EditorSceneManager.CloseScene(openedScenes[index], true);
                }
                if (all)
                {
                    openedScenes.Clear();
                }
                else
                {
                    var firstScene = openedScenes[0];
                    openedScenes.Clear();
                    openedScenes.Add(firstScene);
                }
                CartographerCommon.CleanupMemory();
            }
        }

        public void Initialize(int _keepCount, bool _keepFirst)
        {
            Close(true);
            keepCount = _keepCount;
            keepFirst = _keepFirst;
        }

        public Scene OpenScene(string scenePath, bool exclusive)
        {
            if (exclusive || (openedScenes.Count >= keepCount))
            {
                Close(exclusive || !keepFirst);
            }
            var result = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            if (result.IsValid())
            {
                openedScenes.Add(result);
            }
            return result;
        }

        public void Deinitialize()
        {
            Close(true);
            keepCount = 0;
            keepFirst = false;
        }
    }
};