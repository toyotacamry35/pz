using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Test.Src.Editor
{
    public class DepsFinder : EditorWindow
    {
        private Object SelObj { get; set; }
        private Object[] Deps { get; set; } = System.Array.Empty<Object>();

        [MenuItem("Build/Show object deps")]
        private static void Display()
        {
            var window = GetWindow<DepsFinder>();
            window.Show();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Dependencies finder");
        }

        private void OnSelectionChange()
        {
            var selObj = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(selObj);
            var deps = AssetDatabase.GetDependencies(path)
                .Where(v => AssetDatabase.GetMainAssetTypeAtPath(v) != typeof(MonoScript))
                .Select(v => AssetDatabase.LoadMainAssetAtPath(v)).OrderBy(v => v.name).ToArray();

            Deps = deps;
            SelObj = selObj;
        }

        private Vector2 _scrollPos;

        private void OnGUI()
        {
            GUI.enabled = false;

            EditorGUILayout.LabelField("Object selected: ");

            EditorGUILayout.ObjectField(SelObj, typeof(Object), false);

            EditorGUILayout.LabelField("Dependencies: ");

            GUI.enabled = true;
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            GUI.enabled = false;

            foreach (var obj in Deps)
            {
                EditorGUILayout.ObjectField(obj, typeof(Object), false);
            }

            GUI.enabled = true;
            EditorGUILayout.EndScrollView();
        }
    }
}
