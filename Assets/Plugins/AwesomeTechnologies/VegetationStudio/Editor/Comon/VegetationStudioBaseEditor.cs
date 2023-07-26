using UnityEditor;
using UnityEngine;

namespace AwesomeTechnologies
{
    public class VegetationStudioBaseEditor : Editor
    {
        public bool LargeLogo = false;
        public bool ShowLogo = true;
        public bool IsScriptableObject = false;
        public string OverrideLogoTextureName;
        private Texture2D _overrideLogoTextureSmall;
        public GUIStyle LabelStyle;
        public string HelpTopic = "";

        public virtual void Awake()
        {
            LabelStyle = new GUIStyle("Label") { fontStyle = FontStyle.Italic };
            if (EditorGUIUtility.isProSkin)
            {
                LabelStyle.normal.textColor = new Color(1f, 1f, 1f);
            }
            else
            {
                LabelStyle.normal.textColor = new Color(0f, 0f, 0f);
            }
        }

        public override void OnInspectorGUI()
        {
            if (LabelStyle == null)
            {
                LabelStyle = new GUIStyle("Label") { fontStyle = FontStyle.Italic };
            }
        }
    }
}