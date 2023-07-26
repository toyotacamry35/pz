using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
namespace DeepSky.Haze
{
    [CustomEditor(typeof(DS_HazeCore))]
    public class DS_HazeCoreEditor : Editor
    {
        DS_HazeCore core;
        DS_HazeZoneSphere current = null;
        public override void OnInspectorGUI()
        {

            serializedObject.Update();
            EditorGUILayout.Space();
            SerializedProperty guiProp = serializedObject.FindProperty("m_ShowDebugGUI");
            SerializedProperty guiPositionProp = serializedObject.FindProperty("m_DebugGUIPosition");
            core = target as DS_HazeCore;

            bool texChange = false;
            /*
            if (GUILayout.Button(DS_HazeCore.isGizmos ? "Viewed" : "Hided"))
            {
                DS_HazeCore.isGizmos = !DS_HazeCore.isGizmos;
                core = target as DS_HazeCore;
                List<GameObject> selected = new List<GameObject>();
                foreach (DS_HazeZoneSphere currentSphere in core.m_ZoneSpheres)
                {
                    selected.Add(currentSphere.gameObject);
                }
                Selection.objects = selected.ToArray();
            }
            */
            EditorGUILayout.BeginVertical();
            {
                foreach (DS_HazeZoneSphere currentSphere in core.m_ZoneSpheres)
                {
                    Color temp = GUI.color;
                    if (currentSphere.Equals(current))
                        GUI.color = Color.yellow;
                    else
                        GUI.color = Color.gray;

                    Rect rect = GUILayoutUtility.GetRect(0f, 0f, GUILayout.ExpandWidth(true), GUILayout.Height(35));
                    GUI.Box(rect, "");
                    GUI.color = temp;
                    if (GUI.Button(new Rect(rect.x + 2, rect.y + 2, rect.width/3, rect.height - 4), currentSphere.gameObject.name))
                    {
                        current = currentSphere;
                        Selection.activeGameObject = currentSphere.gameObject;
                    }



                    EditorGUI.BeginChangeCheck();
                    currentSphere.defaultPreset = (DS_HazeContextAsset)EditorGUI.ObjectField(new Rect(rect.x + 4 + rect.width / 3, rect.y + 2, rect.width - rect.width / 3 - 55, rect.height - 2), currentSphere.defaultPreset, typeof(DS_HazeContextAsset), false);
                    if (EditorGUI.EndChangeCheck())
                    {
                        currentSphere.LoadFromContextPreset(currentSphere.defaultPreset);
                    }
                    if (GUI.Button(new Rect(rect.width - 30, rect.y + 2, 35, rect.height - 4), "X"))
                    {
                        DestroyImmediate(currentSphere);
                    }
                }

                if (GUILayout.Button("+", GUILayout.Width(200)))
                {
                    if (UnityEditor.SceneView.lastActiveSceneView != null)
                    {
                        if (UnityEditor.SceneView.lastActiveSceneView.camera != null)
                        {
                            Ray ray = UnityEditor.SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                            RaycastHit hit;
                            if (Physics.Raycast(ray, out hit))
                            {
                                GameObject hazeSphere = new GameObject();
                                hazeSphere.name = "HazePoint";
                                hazeSphere.transform.position = hit.point;
                                hazeSphere.AddComponent<DS_HazeZoneSphere>();
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndVertical();

           

            serializedObject.ApplyModifiedProperties();

            if (texChange) core.SetGlobalNoiseLUT();
        }
    }
}