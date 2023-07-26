using Uins;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(PassiveToggle), true)]
[CanEditMultipleObjects]
public class PassiveToggleEditor : ToggleEditor
{
    private SerializedProperty _onClick;

    protected override void OnEnable()
    {
        _onClick = serializedObject.FindProperty(nameof(PassiveToggle.OnClick));
            
        base.OnEnable();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(_onClick);

        serializedObject.ApplyModifiedProperties();
    }
}