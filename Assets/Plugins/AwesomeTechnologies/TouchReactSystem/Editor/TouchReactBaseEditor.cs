using Assets.Src.Lib.ProfileTools;
using UnityEditor;
using UnityEngine;

namespace AwesomeTechnologies.TouchReact
{
    public class TouchReactBaseEditor : Editor
    {
        public bool LargeLogo = false;
        private Texture2D _logoTexture;
        private Texture2D _logoTextureSmall;

        public GUIStyle LabelStyle;
        public string HelpTopic = "";

        public virtual void Awake()
        {
            LabelStyle = new GUIStyle("Label") { fontStyle = FontStyle.Italic };
            if (EditorGUIUtility.isProSkin)
            {
                LabelStyle.normal.textColor = new Color(1f, 1f, 1f);
                _logoTexture = Profile.Load<Texture2D>("TouchReactSplash");
                _logoTextureSmall = Profile.Load<Texture2D>("Touch_React_logo_inspector");
            }
            else
            {
                LabelStyle.normal.textColor = new Color(0f, 0f, 0f);
                _logoTexture = Profile.Load<Texture2D>("TouchReactSplash");
                _logoTextureSmall = Profile.Load<Texture2D>("Touch_React_logo_inspector");
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUIUtility.labelWidth = 200;

            Texture2D selectedLogoTexture = _logoTextureSmall;
            if (LargeLogo) selectedLogoTexture = _logoTexture;
            GUILayoutUtility.GetRect(1, 3, GUILayout.ExpandWidth(false));
            Rect space = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(selectedLogoTexture.height));
            float width = space.width;

            space.xMin = (width - selectedLogoTexture.width + 18) / 2;
            if (space.xMin < 0) space.xMin = 0;

            space.width = selectedLogoTexture.width;
            space.height = selectedLogoTexture.height;
            GUI.DrawTexture(space, selectedLogoTexture, ScaleMode.ScaleToFit, true, 0);

            if (LargeLogo)
            {
                EditorGUILayout.LabelField("Version: 1.0", LabelStyle);
                EditorGUILayout.LabelField("", LabelStyle);
            }

            GUILayoutUtility.GetRect(1, 3, GUILayout.ExpandWidth(false));
        }
    }
}
