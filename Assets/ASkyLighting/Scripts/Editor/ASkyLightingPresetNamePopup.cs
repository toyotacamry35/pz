using UnityEngine;
using UnityEditor;

namespace TOD
{
    public delegate void ASkyLightingPresetEventHandler(string name);

    /// <summary>
    /// Basic popup window for entering a name when saving a Context preset.
    /// </summary>
    public class ASkyLightingPresetNamePopup : PopupWindowContent
    {
        public string m_Name = "";
        public event ASkyLightingPresetEventHandler OnCreate;

        public override Vector2 GetWindowSize()
        {
            return new Vector2(200, 75);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("Preset Name:", EditorStyles.boldLabel);
            m_Name = EditorGUILayout.TextField(m_Name);

            if (GUILayout.Button("Create"))
            {
                if (OnCreate != null && m_Name != "")
                {
                    OnCreate(m_Name);
                    editorWindow.Close();
                }
            }
        }
    }
}
