#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Camera.DeferredProjectiveDecals
{
    public class FastDecalShaderGUI : ShaderGUI
    {
        protected MaterialEditor _editor;
        protected MaterialProperty[] _properties;
        protected Material _target;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            _target = materialEditor.target as Material;
            _editor = materialEditor;
            _properties = properties;

            DrawTextureSingleLine("_MaskTex", "_MaskMultiplier");
            DrawTextureSingleLine("_MainTex", "_Color");

            if (_target.HasProperty("_NormalTex"))
            {
                EditorGUILayout.Space();
                DrawTextureSingleLine("_NormalTex", "_NormalMultiplier");
            }

            DrawTextureSingleLine("_SpecularTex", "_SpecularMultiplier");
            DrawTextureSingleLine("_SmoothnessTex", "_SmoothnessMultiplier");
            EditorGUILayout.Space();

            //DrawTextureSingleLine("_EmissionMap", "_EmissionColor");
            //EditorGUILayout.Space();

            EditorGUILayout.Space();

            //_editor.FloatProperty(FindProperty("_AngleLimit", _properties), "Angle Limit");
        }

        protected void DrawTextureSingleLine(string baseName, string extraName, bool showScale = false)
        {
            MaterialProperty texture = FindProperty(baseName, _properties, false);
            MaterialProperty extra = FindProperty(extraName, _properties, false);
            if (texture != null)
            {
                if (extra != null && extra.flags != MaterialProperty.PropFlags.PerRendererData)
                    _editor.TexturePropertySingleLine(new GUIContent(texture.displayName), texture, extra);
                else
                    _editor.TexturePropertySingleLine(new GUIContent(texture.displayName), texture);
                if (showScale)
                    _editor.TextureScaleOffsetProperty(texture);
            }
        }
    }
}
#endif