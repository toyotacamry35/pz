using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Utilities
{
    public static class TemporaryObjectsSceneHelper
    {
        private static Scene _tempScene;
        private static readonly string TempScenePath = "Assets/Scenes/TemporaryObjectsScene.unity";

        public static Scene TempScene
        {
            get
            {
                if (!_tempScene.IsValid()) _tempScene = LoadTempScene();
                if (!_tempScene.IsValid())
                    return SceneManager.GetActiveScene();
                return _tempScene;
            }
        }

        private static Scene LoadTempScene()
        {
            if (Application.isPlaying)
                return default(Scene);
            EditorSceneManager.preventCrossSceneReferences = false;
            EditorSceneManager.OpenScene(TempScenePath, OpenSceneMode.Additive);
            var tempScene = SceneManager.GetSceneByPath(TempScenePath);
            return tempScene;
        }
    }
}
#endif