using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameplayDebugger
{
    public class FramesLogEditorWindow : EditorWindow
    {
        VisualElement _otherWindows;
        VisualElement _frames;
        [MenuItem("Debug/GameplayDebugger")]
        public static void GetStats()
        {
            var w = GetWindow<FramesLogEditorWindow>();
        }
        private void OnEnable()
        {
            var root = this.rootVisualElement;
            root.Add(_otherWindows = new IMGUIContainer(OnDrawOtherWindowsButtons) { style = { height = 100 } });
            root.Add(_frames = new IMGUIContainer(OnDrawFrames));
        }

        private void OnDisable()
        {
            this.rootVisualElement.Clear();
        }

        void OnDrawOtherWindowsButtons()
        {

        }

        private void Update()
        {
            if (SelectCurrent)
                CurrentlySelectedFrame = FramesLog.Instance.GetLastRecordedFrame();
        }

        public Frame CurrentlySelectedFrame;
        public bool SelectCurrent = true;
        void OnDrawFrames()
        {
            EditorGUILayout.BeginHorizontal();
            var orderedFrames = FramesLog.Instance.GetFrames();
            foreach (var orderedFrame in orderedFrames)
            {
                GUILayout.Box("|");
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}

