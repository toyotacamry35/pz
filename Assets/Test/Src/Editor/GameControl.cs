using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Assets.Test.Src.Editor
{
    public static class GameControl
    {
        static void SetPlayModeStartScene(string scenePath)
        {
            SceneAsset myWantedStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            if (myWantedStartScene != null)
                EditorSceneManager.playModeStartScene = myWantedStartScene;
            else
                Debug.Log("Could not find Scene " + scenePath);
        }

        public static void StartGame()
        {
        	Debug.Log("GameControl.StartGame()");

            SetPlayModeStartScene("Assets/Scenes/Main.unity");

            EditorApplication.isPlaying = true;
            var args = System.Environment.GetCommandLineArgs();
            Debug.Log(args);
        }
    }
}
