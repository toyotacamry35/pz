using UnityEditor;
using UnityEngine;
using ECamera = Assets.Tools.EditorCamera.EditorCamera;

namespace Assets.Tools.Editor.EditorCamera.Editor
{
    [CustomEditor(typeof(ECamera))]
    public class EditorCameraEditor : UnityEditor.Editor
    {
        private static bool isAdvancedOptions;

        public override void OnInspectorGUI()
        {

            ECamera current = (ECamera)target;

            EditorGUILayout.HelpBox("<move>+Shift x10 for speed\n<move>+Ctrl /10 for speed", MessageType.Info);

            current.speed = EditorGUILayout.Slider("Speed", current.speed, 0.1f, 10);
            current.sensitivity = EditorGUILayout.Slider("Sensitivity", current.sensitivity, 0.1f, 10);
            Color currentColor = GUI.color;
            GUI.color = current.isSaveOnScene ? Color.red : Color.green;
            if (GUILayout.Button(current.isSaveOnScene ? "Save on Scene" : "Dont save on Scene"))
            {
                current.isSaveOnScene = !current.isSaveOnScene;
                current.gameObject.hideFlags = current.isSaveOnScene ? HideFlags.None : HideFlags.DontSave;
            }
            GUI.color = currentColor;
            if (GUILayout.Button("Advanced Options   " + (isAdvancedOptions ? "[-]" : "[+]")))
                isAdvancedOptions = !isAdvancedOptions;
            if (isAdvancedOptions)
            {
                current.forwardSpeedScale = EditorGUILayout.FloatField("Forward-back speed scale: ", current.forwardSpeedScale);
                current.rightSpeedScale = EditorGUILayout.FloatField("Right-left speed scale: ", current.rightSpeedScale);
                current.upSpeedScale = EditorGUILayout.FloatField("Up-down speed scale: ", current.upSpeedScale);
                current.sensitivityX = EditorGUILayout.FloatField("Mouse sensitivity by X", current.sensitivityX);
                current.sensitivityY = EditorGUILayout.FloatField("Mouse sensitivity by Y", current.sensitivityY);
            }
        }
    }
}