using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using static Assets.Src.BuildingSystem.VisualBehaviour;

namespace Assets.Src.BuildingSystem.Editor
{
    [CustomEditor(typeof(VisualBehaviour))]
    public class VisualEditor : UnityEditor.Editor
    {
        private bool initialized = false;
        public bool needUpdatePrefab = false;

        public GUIContent State_Label = new GUIContent("State");

        private static List<GameObject> GetChildren(VisualBehaviour visualBehaviour)
        {
            var children = new List<GameObject>();
            var childCount = visualBehaviour.gameObject.transform.childCount;
            if (childCount > 0)
            {
                for (var childIndex = 0; childIndex < childCount; ++childIndex)
                {
                    children.Add(visualBehaviour.gameObject.transform.GetChild(childIndex).gameObject);
                }
            }
            return children;
        }

        private static void CreateEmptyPrefab(VisualBehaviour visualBehaviour)
        {
            visualBehaviour.gameObject.transform.position = Vector3.zero;
            visualBehaviour.gameObject.transform.rotation = Quaternion.identity;
            visualBehaviour.gameObject.transform.localScale = Vector3.one;

            // delete children
            var children = GetChildren(visualBehaviour);
            foreach (var child in children)
            {
                DestroyImmediate(child);
            }

            // clear outlines
            var outlines = visualBehaviour.gameObject.transform.GetComponentsInChildren<OutlineEffect.Outline>(true);
            if (outlines != null)
            {
                for (var index = 0; index < outlines.Length; ++index)
                {
                    DestroyImmediate(outlines[index]);
                }
            }

            // create children
            visualBehaviour.StateElements = new GameObject[6];
            for (var index = 0; index < visualBehaviour.StateElements.Length; ++index)
            {
                visualBehaviour.StateElements[index] = new GameObject();
                visualBehaviour.StateElements[index].name = Enum.GetName(typeof(VisualState), index);
                visualBehaviour.StateElements[index].transform.parent = visualBehaviour.gameObject.transform;
                visualBehaviour.StateElements[index].transform.position = Vector3.zero;
            }
        }

        private static bool GenerateEmptyPrefab(VisualBehaviour visualBehaviour)
        {
            CreateEmptyPrefab(visualBehaviour);

            for (var stateElementIndex = (int)(VisualState.Invisible); stateElementIndex <= (int)(VisualState.Destroyed); ++stateElementIndex)
            {
                var stateElement = visualBehaviour.StateElements[stateElementIndex];
                stateElement.SetActive(false);
            }

            visualBehaviour.SetVisualState(VisualState.Invisible, OutlineEffect.OutlineColor.Blue, string.Empty, string.Empty, true, 1.0f);
            return true;
        }

        [MenuItem("CONTEXT/VisualBehaviour/Create Empty Prefab")]
        private static void CreateEmptyPrefab(MenuCommand menuCommand)
        {
            var visualBehaviour = menuCommand.context as VisualBehaviour;
            if (visualBehaviour != null)
            {
                GenerateEmptyPrefab(visualBehaviour);
            }
        }

        public override void OnInspectorGUI()
        {
            var visualBehaviour = target as VisualBehaviour;
            if (visualBehaviour == null)
            {
                EditorGUILayout.HelpBox("Target object is null or not a VisualBehaviour.", MessageType.Error, true);
                return;
            }
            EditorGUI.BeginChangeCheck();
            visualBehaviour.State = (VisualState)EditorGUILayout.EnumPopup(State_Label, visualBehaviour.State);
            if (EditorGUI.EndChangeCheck())
            {
                visualBehaviour.SetVisualState(visualBehaviour.State, OutlineEffect.OutlineColor.Blue, string.Empty, string.Empty, false, 1.0f);
            }
        }
    }
}