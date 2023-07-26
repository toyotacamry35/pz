using ResourcesSystem.Loader;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.ResourceSystem.Editor
{
    [InitializeOnLoad]
    class GameResourcesEditorInitializer
    {
        static GameResourcesEditorInitializer()
        {
            if (Application.isPlaying)
                return;
            GameResourcesHolder.Instance = new GameResources(new FolderLoader(Application.dataPath));
            GameResourcesHolder.Instance.ConfigureForUnityEditor();
            EditorGameResourcesForMonoBehaviours.NewGR = () =>
            {
                var gr = new GameResources(new FolderLoader(Application.dataPath));
                gr.ConfigureForUnityEditor();
                return gr;
            };
        }
    }
}
