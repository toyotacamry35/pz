using System;
using System.Collections.Generic;
using System.Linq;
using Core.Reflection;
using Assets.Src.RubiconAI;
using Assets.Src.RubiconAI.BehaviourTree;
using ColonyShared.SharedCode.Utils;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Assets.Src.Detective.Editor
{
    class InvestigatorWindow : EditorWindow
    {
        [MenuItem("Debug/Investigate")]
        static void Investigate()
        {
            GetWindow<InvestigatorWindow>();
        }

        private long _lastUtcSample;
        void OnEnable()
        {
            _drawers.Clear();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssembliesSafe())
            {
                foreach (var type in assembly.GetTypesSafe())
                {
                    if (type.BaseType != null && type.BaseType.IsGenericType &&
                        type.BaseType.GetGenericTypeDefinition() == typeof(EventDrawer<>))
                    {
                        var forType = type.BaseType.GetGenericArguments()[0];
                        _drawers[forType] = (IEventDrawer)Activator.CreateInstance(type);
                    }
                }
            }
            EditorApplication.update += Update;
        }

        void OnDisable()
        {
            EditorApplication.update -= Update;
        }
        private void Update()
        {
            _lastUtcSample = SyncTime.Now;
            if (Application.isPlaying)
                _investigations = Office.Visit().Archive.GetInvestigations();
        }


        System.Random _rand = new System.Random();
        private List<KeyValuePair<ArchiveId, Investigation>> _investigations;
        private ArchiveId _currentArchiveId;
        private Investigation _currentInvestigation;
        private TreeViewItem _item;
        private Vector2 _scroll = Vector2.zero;
        Dictionary<Type, IEventDrawer> _drawers = new Dictionary<Type, IEventDrawer>();
        void OnGUI()
        {
            if (Office.Visit() == null)
                return;
            if (_investigations != null)
            {
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                DrawTimeLine(_investigations);
                GUILayout.EndHorizontal();
                GUILayout.BeginArea(Rect.MinMaxRect(0, 50, position.width, position.height));
                GUILayout.BeginVertical();
                DrawCurrentEventForId(_investigations);
                GUILayout.EndVertical();
                GUILayout.EndArea();
                GUILayout.EndVertical();
            }
            if (_rand.Next() > 0.3)
                Repaint();
        }
        private void DrawCurrentEventForId(List<KeyValuePair<ArchiveId, Investigation>> investigations)
        {
            if (_currentArchiveId == null)
                return;
            DrawEvent(_currentInvestigation.CurrentEvent);
        }

        public void DrawEvent(Event valueCurrentEvent)
        {
            var type = valueCurrentEvent.GetType();
            IEventDrawer drawer = null;
            if (_drawers.TryGetValue(type, out drawer))
                drawer.Draw(this, valueCurrentEvent);
        }

        private void DrawTimeLine(List<KeyValuePair<ArchiveId, Investigation>> investigations)
        {
            if (investigations.Count == 0)
                return;
            GUI.color = Color.yellow;
            GUI.Box(new Rect(0, 0, this.position.width, 30), "");
            GUI.color = Color.white;
            if (UnityEngine.Event.current.type == EventType.MouseUp)
            {
                var mousePos = UnityEngine.Event.current.mousePosition;
                if (new Rect(0, 0, this.position.width, 30).Contains(mousePos))
                {
                    float pos = 1 - mousePos.x / this.position.width;
                    int index = (int)(investigations.Count * pos);
                    index = Mathf.Clamp(index, 0, investigations.Count);
                    _currentArchiveId = investigations[index].Key;
                    _currentInvestigation = investigations[index].Value;
                }
            }
            var start = _lastUtcSample;
            int iIndex = 0;
            int selectedIndex = -1;
            foreach (var investigation in investigations)
            {
                if (investigation.Key == _currentArchiveId)
                    selectedIndex = iIndex;
                iIndex++;
                var delta = start - investigation.Key.Time;
                var startX = this.position.width - this.position.width * ((float)iIndex / (float)investigations.Count);
                if (delta > SyncTime.Second * 2.5)
                {
                    start = investigation.Key.Time;
                    GUI.Box(new Rect(startX, 30, 50, 30), ((double)(_lastUtcSample - investigation.Key.Time) / (double)SyncTime.Second).ToString("##.###"));
                }
            }
            iIndex = 0;
            foreach (var investigation in investigations)
            {
                if (investigation.Key == _currentArchiveId)
                    selectedIndex = iIndex;
                iIndex++;
                var startX = this.position.width - this.position.width * ((float)iIndex / (float)investigations.Count);

                if (investigation.Value != null)
                    if (investigation.Value.CurrentEvent.SubEvents.Count > 0)
                    {
                        GUI.color = Color.yellow;
                        GUI.Box(new Rect(startX, 30, 10, 30), investigation.Value.CurrentEvent.SubEvents.Count.ToString());
                        GUI.color = Color.white;
                    }
            }
            if (selectedIndex == -1)
            {
                _currentArchiveId = null;
                _currentInvestigation = null;
            }
            if (_currentArchiveId != null)
            {
                GUI.color = Color.green;
                var startX = this.position.width - this.position.width * ((float)selectedIndex / (float)investigations.Count);
                GUI.Box(new Rect(startX, 0, 5, 30), "");

                if (UnityEngine.Event.current.type == EventType.KeyDown)
                {
                    if (UnityEngine.Event.current.keyCode == KeyCode.RightArrow)
                    {
                        while (selectedIndex > 1)
                        {
                            _currentArchiveId = investigations[selectedIndex - 1].Key;
                            _currentInvestigation = investigations[selectedIndex - 1].Value;
                            if (_currentInvestigation.CurrentEvent.SubEvents.Count > 0)
                                break;
                            selectedIndex--;
                        }

                    }
                    else if (UnityEngine.Event.current.keyCode == KeyCode.LeftArrow)
                    {
                        while (selectedIndex < _investigations.Count - 1)
                        {
                            _currentArchiveId = investigations[selectedIndex + 1].Key;
                            _currentInvestigation = investigations[selectedIndex + 1].Value;
                            if (_currentInvestigation.CurrentEvent.SubEvents.Count > 0)
                                break;
                            selectedIndex++;
                        }
                    }
                }

            }
        }
    }

    interface IEventDrawer
    {
        void Draw(InvestigatorWindow w, Event e);
    }
    abstract class EventDrawer<T> : IEventDrawer where T : Event
    {
        protected static bool ShouldDraw =>
            UnityEngine.Event.current.type == EventType.Repaint || UnityEngine.Event.current.type == EventType.Layout;
        protected static bool ShouldDrawDebug =>
            UnityEngine.Event.current.type == EventType.Repaint;
        public void Draw(InvestigatorWindow w, Event e)
        {
            Draw(w, (T)e);
        }
        protected abstract void Draw(InvestigatorWindow w, T e);
    }
    class ObjectFrameDrawer : EventDrawer<ObjectFrameEvent>
    {
        int _selectedTab = 0;
        Vector2 _scroll;
        protected override void Draw(InvestigatorWindow w, ObjectFrameEvent e)
        {
            if (e.Go == null)
                return;
            DebugExtension.DebugBox(e.Go.transform.position, Vector3.one, Quaternion.identity, Color.green, 1f);
            if (e.SubEvents.Count > 0)
            {
                _selectedTab = Tabs(e.SubEvents.Select(x => x.ToString()).ToArray(), _selectedTab);
                var window = EditorWindow.GetWindow<InvestigatorWindow>();
                _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Width(window.position.width), GUILayout.Height(window.position.height - 100));
                w.DrawEvent(e.SubEvents[_selectedTab]);
                GUILayout.EndScrollView();
            }

        }

        public static int Tabs(string[] options, int selected)
        {
            const float DarkGray = 0.4f;
            const float LightGray = 0.9f;

            Color storeColor = GUI.backgroundColor;
            Color highlightCol = new Color(LightGray, LightGray, LightGray);
            Color bgCol = new Color(DarkGray, DarkGray, DarkGray);

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.padding.bottom = 8;

            GUILayout.BeginHorizontal();
            {   //Create a row of buttons
                for (int i = 0; i < options.Length; ++i)
                {
                    GUI.backgroundColor = i == selected ? highlightCol : bgCol;
                    if (GUILayout.Button(options[i], buttonStyle))
                    {
                        selected = i; //Tab click
                    }
                }
            }
            GUILayout.EndHorizontal();
            //Restore color
            GUI.backgroundColor = storeColor;

            return selected;
        }
    }

    class BehaviourNodeTickDrawer : EventDrawer<BehaviourNodeTickEvent>
    {
        protected override void Draw(InvestigatorWindow w, BehaviourNodeTickEvent e)
        {
            if (ShouldDraw)
                GUILayout.Label($"{e.ToString()}");
            foreach (var eSubEvent in e.SubEvents)
            {
                w.DrawEvent(eSubEvent);
            }
        }
    }
}
