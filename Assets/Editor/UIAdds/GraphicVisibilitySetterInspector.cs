using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using WeldAdds;

[CustomEditor(typeof(GraphicVisibilitySetter))]
public class GraphicVisibilitySetterInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var canvasGroupVisibilitySetter = (GraphicVisibilitySetter) target;
        if (canvasGroupVisibilitySetter.Graphic != null)
        {
            var graphic = canvasGroupVisibilitySetter.GetComponent<Graphic>();
            if (graphic != null && graphic != canvasGroupVisibilitySetter.Graphic)
                EditorGUILayout.HelpBox(
                    $"Transform has {nameof(CanvasGroup)} component, but '{nameof(GraphicVisibilitySetter.Graphic)}' link leads to " +
                    $"another object: '{canvasGroupVisibilitySetter.Graphic.transform.name}'",
                    MessageType.Warning);
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button("Fill empty fields", GUILayout.MaxWidth(200)))
        {
            if (canvasGroupVisibilitySetter.Graphic == null)
            {
                var graphic = canvasGroupVisibilitySetter.GetComponent<Graphic>();
                if (graphic != null)
                {
                    var serializedProperty = serializedObject.FindProperty(nameof(GraphicVisibilitySetter.Graphic));
                    serializedProperty.objectReferenceValue = graphic;
                }
            }

            if (canvasGroupVisibilitySetter.LayoutElement == null)
            {
                var selfLayoutElement = canvasGroupVisibilitySetter.GetComponent<LayoutElement>();
                if (selfLayoutElement != null)
                {
                    var serializedProperty = serializedObject.FindProperty(nameof(GraphicVisibilitySetter.LayoutElement));
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