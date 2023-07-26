using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ThreeEyedGames
{
    [CustomEditor(typeof(DecalLimitGroupManager))]
    public class DecalLimitGroupManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DecalLimitGroupManager mgr = target as DecalLimitGroupManager;

            var limitGroups = serializedObject.FindProperty("LimitGroups");
            int numProperties = limitGroups.arraySize;
            for (int i = 0; i < numProperties; ++i)
            {
                EditorGUILayout.BeginHorizontal();
                var prop = limitGroups.GetArrayElementAtIndex(i);
                EditorGUILayout.LabelField("Group " + (i + 1), GUILayout.Width(80));
                EditorGUILayout.PropertyField(prop, GUIContent.none);
                if (prop.objectReferenceValue == null && GUILayout.Button("+", GUILayout.Width(20)))
                {
                    var go = new GameObject("Group " + (i + 1));
                    go.transform.parent = mgr.transform;
                    prop.objectReferenceValue = go.AddComponent<DecalLimitGroup>();
                }
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();

            // TODO: Add a button here allowing to add the DECALICIOUS_MORE_LIMIT_GROUPS macro
        }
    }
}
