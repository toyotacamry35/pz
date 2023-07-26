using Assets.Src.ResourceSystem;
using Assets.Src.SpawnSystem;
using UnityEditor;

[CustomEditor(typeof(EntityGameObject))]
public class EntityGameObjectInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var ego = target as EntityGameObject;
        if (ego != null)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(
                nameof(ego.EntityDef),
                ego.EntityDef == null
                    ? null
                    : JdbMetadata.GetFromResource(ego.EntityDef), typeof(JdbMetadata),
                false);
            EditorGUI.EndDisabledGroup();
        }

        DrawDefaultInspector();
    }
}