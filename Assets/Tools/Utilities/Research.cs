using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
public class Research
{
    static Research()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        // Do your general-purpose scene gui stuff here...
        // Applies to all scene views regardless of selection!

        // You'll need a control id to avoid messing with other tools!
        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        if (Event.current.GetTypeForControl(controlID) == EventType.KeyUp)
        {
            if (Event.current.keyCode == KeyCode.T)
            {
                var allObjects = GameObject.FindObjectsOfType<UnityEngine.Object>();
                foreach (var obj in allObjects)
                {
                    obj.hideFlags = HideFlags.None;


                }
            }

        }


    }
}
#endif
