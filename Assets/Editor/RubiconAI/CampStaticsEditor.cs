using Assets.Src.ResourceSystem;
using Assets.Src.RubiconAI;
using Assets.Src.RubiconAI.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.RubiconAI
{
    [CustomEditor(typeof(CampStatics))]
    class CampStaticsEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Search for POIs"))
            {
                var cs = ((CampStatics)target);
                var position = cs.transform.position;
                var pois = cs.gameObject.scene.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<CampPOI>());
                var closePois = pois.Where(x => Vector3.Distance(x.transform.position, position) < cs.Radius);
                // Using Unity under-the-hood links of prefab instances to their prefab working not stable (from version to version). So we're using names here:
                var properPois = closePois.Where(x =>PrefabUtility.FindPrefabRoot(x.gameObject).name == cs.PrefabPOI.name);
                var closePoisList = properPois.Select(x => x.gameObject).ToList();
                var so = new SerializedObject(cs);
                var prop = so.FindProperty(nameof(cs.FoundLinks));
                Debug.Log($"Attached {closePoisList.Count}");
                prop.arraySize = closePoisList.Count;
                for (int i = 0; i < closePoisList.Count; i++)
                    prop.GetArrayElementAtIndex(i).objectReferenceValue = closePoisList[i];
                so.ApplyModifiedProperties();
            }
            GUILayout.EndHorizontal();
        }
    }
}
