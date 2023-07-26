#if UNITY_EDITOR
using System;
using UnityEngine;

[ExecuteInEditMode]
public class TextOnEditorScreen : MonoBehaviour
{
    private static long interval = 100;
    private DateTime lastUpdate = DateTime.Now;

    //public static void SetLine(int index, string line, Color color)
    //{
    //}

    void Awake()
    {
        lastUpdate = DateTime.Now;
    }

    private void DrawTextLines()
    {
        string textTimeLabel = "Time:";
        string textCameraLabel = "Camera:";
        string textTime = DateTime.Now.ToLongTimeString();
        string textCamera = string.Empty;
        var activeSceneView = UnityEditor.SceneView.lastActiveSceneView;
        if (activeSceneView != null)
        {
            var mainCamera = activeSceneView.camera;
            if (mainCamera != null)
            {
                var tr = mainCamera.transform;
                textCamera = String.Format("{0:0.00} {1:0.00} {2:0.00}, view: {3:0.00} {4:0.00} {5:0.00}, anchor: {6:0.00} {7:0.00} {8:0.00}, distance: {9:0.00}",
                tr.position.x,
                tr.position.y,
                tr.position.z,
                tr.rotation.eulerAngles.x,
                tr.rotation.eulerAngles.y,
                tr.rotation.eulerAngles.z,
                activeSceneView.pivot.x,
                activeSceneView.pivot.y,
                activeSceneView.pivot.z,
                activeSceneView.cameraDistance);
            }
        }
        UnityEditor.Handles.BeginGUI();
        var view = UnityEditor.SceneView.currentDrawingSceneView;
        Vector2 sizeTimeLabel = GUI.skin.label.CalcSize(new GUIContent(textTimeLabel));
        Vector2 sizeCameraLabel = GUI.skin.label.CalcSize(new GUIContent(textCameraLabel));
        Vector2 sizeTime = GUI.skin.label.CalcSize(new GUIContent(textTime));
        Vector2 sizeCamera = GUI.skin.label.CalcSize(new GUIContent(textCamera));
        GUI.color = Color.black;
        GUI.Label(new Rect(1, 1, sizeCameraLabel.x, sizeCameraLabel.y), textCameraLabel);
        GUI.Label(new Rect(1, 1 + sizeCamera.y - 2, sizeTimeLabel.x, sizeTimeLabel.y), textTimeLabel);
        GUI.Label(new Rect(1 + sizeCameraLabel.x - 2, 1, sizeCamera.x, sizeCamera.y), textCamera);
        GUI.Label(new Rect(1 + sizeCameraLabel.x - 2, 1 + sizeCamera.y - 2, sizeTime.x, sizeTime.y), textTime);
        GUI.color = Color.green;
        GUI.Label(new Rect(0, 0, sizeCameraLabel.x, sizeCameraLabel.y), textCameraLabel);
        GUI.Label(new Rect(0, 0 + sizeCamera.y - 2, sizeTimeLabel.x, sizeTimeLabel.y), textTimeLabel);
        GUI.Label(new Rect(0 + sizeCameraLabel.x - 2, 0, sizeCamera.x, sizeCamera.y), textCamera);
        GUI.Label(new Rect(0 + sizeCameraLabel.x - 2, 0 + sizeCamera.y - 2, sizeTime.x, sizeTime.y), textTime);
        UnityEditor.Handles.EndGUI();
    }

    void OnDrawGizmos()

    {
        if ((DateTime.Now - lastUpdate).TotalMilliseconds > interval)
        {
            DrawTextLines();
        }
    }
}
#endif
