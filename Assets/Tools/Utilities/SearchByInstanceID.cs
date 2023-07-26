#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utilities
{
    public class SearchByInstanceID : EditorWindow
    {

        [MenuItem("Tools/Asset by instance search")]
        static void Init()
        {
            var window = EditorWindow.GetWindow<SearchByInstanceID>();
        }
        private int _instanceToSearch;
        void OnGUI()
        {
            _instanceToSearch = EditorGUILayout.IntField("instance id to search", _instanceToSearch);
            if (GUILayout.Button("Search"))
            {
                var obj = EditorUtility.InstanceIDToObject(_instanceToSearch);
                Debug.Log(obj);
                EditorGUIUtility.PingObject(obj);
            }
        }
        
    }

}
#endif