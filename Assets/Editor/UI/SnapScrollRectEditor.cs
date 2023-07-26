using Uins;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(SnapScrollRect), true)]
[CanEditMultipleObjects]
public class SnapScrollRectEditor : ScrollRectEditor
{
    private SerializedProperty _scrollStep;
    private SerializedProperty _onScrollPosChanged;

    protected override void OnEnable()
    {
        _scrollStep = serializedObject.FindProperty("ScrollStep");
        _onScrollPosChanged = serializedObject.FindProperty("OnScrollPosChanged");

        base.OnEnable();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(_scrollStep);
        EditorGUILayout.PropertyField(_onScrollPosChanged);

        serializedObject.ApplyModifiedProperties();
    }
}