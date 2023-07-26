using UnityEditor;

namespace Assets.Src.Cluster.Editor
{
    [InitializeOnLoad]
    public static class EditorClusterShutdownService
    {
        static EditorClusterShutdownService()
        {
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
        }

        private static void ExitingPlayMode()
        {
            //GameState.Instance?.StopCluster();
        }

        private static void EditorApplication_playModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
                ExitingPlayMode();
        }
    }
}
