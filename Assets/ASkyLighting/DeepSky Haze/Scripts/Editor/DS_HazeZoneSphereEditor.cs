using UnityEngine;
using UnityEditor;
using TOD;
using System.IO;

namespace DeepSky.Haze
{
    [CanEditMultipleObjects]
    [ExecuteInEditMode]
    [CustomEditor(typeof(DS_HazeZoneSphere), true)]
    public class DS_HazeZoneSphereEditor : DS_HazeZoneEditor
    {
        
        DS_HazeZoneSphere hazeZoneSphere;
        public override void OnInspectorGUI()
        {
            buttonLeft = GUI.skin.FindStyle("ButtonLeft");
            buttonRight = GUI.skin.FindStyle("ButtonRight");
            helpIconImage = EditorGUIUtility.FindTexture("console.infoicon.sml");
            helpIconStyle = new GUIStyle();
            helpIconStyle.normal.background = helpIconImage;
            helpIconStyle.onNormal.background = helpIconImage;
            helpIconStyle.active.background = helpIconImage;
            helpIconStyle.onActive.background = helpIconImage;
            helpIconStyle.focused.background = helpIconImage;
            helpIconStyle.onFocused.background = helpIconImage;

            hazeZoneSphere = target as DS_HazeZoneSphere;

            hazeZoneSphere.m_PrioritySphere = EditorGUILayout.IntSlider("Priority", hazeZoneSphere.m_PrioritySphere, 1, 10);
            hazeZoneSphere.m_BlendRangeSphere = EditorGUILayout.Slider("Size", hazeZoneSphere.m_BlendRangeSphere, 0, 100f);
            hazeZoneSphere.m_BlendRangeOutSphere = EditorGUILayout.Slider("Blend Size", hazeZoneSphere.m_BlendRangeOutSphere, 0, 50f);

            EditorGUILayout.Space();
            hazeZoneSphere.innerColor = EditorGUILayout.ColorField("Inner Color", hazeZoneSphere.innerColor);
            hazeZoneSphere.outerColor = EditorGUILayout.ColorField("Outer Color", hazeZoneSphere.outerColor);
			
			EditorGUILayout.Space();

            HeaderLoad();

            EditorGUILayout.Space();

            if (Event.current.type == EventType.Repaint && hazeZoneSphere.m_WaitingToLoad != null)
            {
                DS_HazeZoneSphere editingObject = (DS_HazeZoneSphere)target;
                editingObject.LoadFromContextPreset(hazeZoneSphere.m_WaitingToLoad);
                hazeZoneSphere.m_WaitingToLoad = null;
            }
        }
        /*
        private void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= this.SceneUpdate;
        }

        public override void OnEnable()
        {
            
            base.OnEnable();
            SceneView.onSceneGUIDelegate += this.SceneUpdate;
        }
        */


        void OnSceneGUI() 
        //void SceneUpdate(SceneView sceneView)
        {
            hazeZoneSphere = target as DS_HazeZoneSphere;

            GUIStyle bigLabel = new GUIStyle();
            bigLabel.fontSize = 18;
            bigLabel.alignment = TextAnchor.UpperCenter;
            bigLabel.fontStyle = FontStyle.Bold;

            GUIStyle smallLabel = new GUIStyle();
            smallLabel.fontSize = 20;
            smallLabel.alignment = TextAnchor.LowerCenter;
            smallLabel.fontStyle = FontStyle.Normal;

            Handles.color = hazeZoneSphere.innerColor;
            Handles.DrawSolidDisc(hazeZoneSphere.transform.position + Vector3.up, Vector3.up, hazeZoneSphere.m_BlendRangeSphere);
            Handles.color = hazeZoneSphere.outerColor;
            Handles.DrawWireDisc(hazeZoneSphere.transform.position, Vector3.up, hazeZoneSphere.m_BlendRangeSphere + hazeZoneSphere.m_BlendRangeOutSphere);
            Handles.color = hazeZoneSphere.innerColor;
            Handles.DrawSolidArc(hazeZoneSphere.transform.position + Vector3.up, Vector3.forward, Vector3.up, 90f, hazeZoneSphere.m_BlendRangeSphere);


            Handles.Label(hazeZoneSphere.transform.transform.position + Vector3.up * 4f, hazeZoneSphere.transform.name, bigLabel);
            Handles.Label(hazeZoneSphere.transform.transform.position + Vector3.up * 1f, (hazeZoneSphere.defaultPreset) ? hazeZoneSphere.defaultPreset.name : "---", smallLabel);
        }

    }
}