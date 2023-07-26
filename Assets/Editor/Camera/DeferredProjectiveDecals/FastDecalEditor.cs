using Assets.Src.Camera.Effects.DeferredProjectiveDecals;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Camera.DeferredProjectiveDecals
{
    [CustomEditor(typeof(FastDecal))]
    [CanEditMultipleObjects]
    public class FastDecalEditor : UnityEditor.Editor
    {
        protected FastDecal _decal;

        public override void OnInspectorGUI()
        {
            _decal = target as FastDecal;

            SerializedProperty material = serializedObject.FindProperty("Material");
            SerializedProperty renderOrder = serializedObject.FindProperty("RenderOrder");
            SerializedProperty fade = serializedObject.FindProperty("Fade");

            EditorGUILayout.PropertyField(material);
            EditorGUILayout.PropertyField(renderOrder);
            FastDecal.DecalRenderMode dRenderMode = _decal.RenderMode;
            switch (dRenderMode)
            {
                case FastDecal.DecalRenderMode.Deferred:
                    EditorGUILayout.Space();
                    if (UnityEngine.Camera.main != null && UnityEngine.Camera.main.actualRenderingPath != RenderingPath.DeferredShading)
                    {
                        EditorGUILayout.HelpBox("Main camera is not using the Deferred rendering path. " +
                            "Deferred decals will not be drawn. Current path: " + UnityEngine.Camera.main.actualRenderingPath, MessageType.Error);
                    }
                    EditorGUILayout.PropertyField(fade);
                    break;
                case FastDecal.DecalRenderMode.Invalid:
                default:
                    EditorGUILayout.HelpBox("Please select a Material with a decal shader.", MessageType.Info);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
