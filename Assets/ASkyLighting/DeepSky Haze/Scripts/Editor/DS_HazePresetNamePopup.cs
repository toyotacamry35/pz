using UnityEngine;
using UnityEditor;

namespace DeepSky.Haze
{
    public delegate void DS_CreateHazePresetEventHandler(string name);

    /// <summary>
    /// Basic popup window for entering a name when saving a Context preset.
    /// </summary>
    public class DS_HazePresetNamePopup : PopupWindowContent
    {
        public string m_Name = "";
        public event DS_CreateHazePresetEventHandler OnCreate;

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
