using Assets.ColonyShared.SharedCode.CustomData;
using UnityEditor;

namespace Assets.Editor.Utils
{
    [InitializeOnLoad]
    static class PlaymodeStateChangedHandler
    {
        static PlaymodeStateChangedHandler()
        {
            //UnityEngine.Debug.Log("#DBG: PlaymodeStateChangedHandler - [InitializeOnLoad].");
            EditorApplication.playModeStateChanged += ModeChanged;
        }

        static void ModeChanged(PlayModeStateChange pmsch)
        {
            if (pmsch == PlayModeStateChange.ExitingPlayMode)
            {
                //UnityEngine.Debug.Log("#DBG: PlaymodeStateChangedHandler - Exit Play Mode.");
                PawnDataSharedProxy.Clean();
            }
        }

    }
}
