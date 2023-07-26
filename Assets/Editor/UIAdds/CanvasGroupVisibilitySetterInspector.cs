using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using WeldAdds;

[CustomEditor(typeof(CanvasGroupVisibilitySetter))]
public class CanvasGroupVisibilitySetterInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var canvasGroupVisibilitySetter = (CanvasGroupVisibilitySetter) target;
        if (canvasGroupVisibilitySetter.CanvasGroup != null)
        {
            var selfCanvasGroup = canvasGroupVisibilitySetter.GetComponent<CanvasGroup>();
            if (selfCanvasGroup != null && selfCanvasGroup != canvasGroupVisibilitySetter.CanvasGroup)
                EditorGUILayout.HelpBox(
                    $"Transform has {nameof(CanvasGroup)} component, but '{nameof(CanvasGroupVisibilitySetter.CanvasGroup)}' link leads to " +
                    $"another object: '{canvasGroupVisibilitySetter.CanvasGroup.transform.name}'",
                    MessageType.Warning);
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button("Fill empty fields", GUILayout.MaxWidth(200)))
        {
            if (canvasGroupVisibilitySetter.CanvasGroup == null)
            {
                var selfCanvasGroup = canvasGroupVisibilitySetter.GetComponent<CanvasGroup>();
                if (selfCanvasGroup != null)
                {
                    var serializedProperty = serializedObject.FindProperty(nameof(CanvasGroupVisibilitySetter.CanvasGroup));
                    serializedProperty.objectReferenceValue = selfCanvasGroup;
                }
            }

            if (canvasGroupVisibilitySetter.LayoutElement == null)
            {
                var selfLayoutElement = canvasGroupVisibilitySetter.GetComponent<LayoutElement>();
                if (selfLayoutElement != null)
                {
                    var serializedProperty = serializedObject.FindProperty(nameof(CanvasGroupVisibilitySetter.LayoutElement));
                    serializedProperty.objectReferenceValue = selfLayoutElement;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();
    }
}