using Assets.Src.GameplayDebugger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameplayDebugger
{
    class ScopeSelectionPanel : EditorWindow
    {
        [MenuItem("Debug/ScopeSelectionPanel")]
        public static void GetStats()
        {
            var w = GetWindow<ScopeSelectionPanel>();
        }

        [NonSerialized]
        GameObject _rootObject;
        [NonSerialized]
        VisualElement _filteredObjectsView;
        HashSet<GameObject> _filteredObjects = new HashSet<GameObject>();
        private void OnEnable()
        {
            var root = rootVisualElement;
            root.Add(new IMGUIContainer(DrawFilters) { style = { height = 200 } });
            var scrollView = new ScrollView();
            _filteredObjectsView = new VisualElement();
            scrollView.Add(_filteredObjectsView);
            var scrollViewParent = new VisualElement() { style = { height = 400 } };
            root.Add(scrollViewParent);
            scrollViewParent.Add(scrollView);
            scrollView.StretchToParentSize();
            scrollView.contentContainer.style.flexDirection = FlexDirection.Column;
            scrollView.showVertical = true;
            scrollView.showHorizontal = false;
            DebuggedObjectsScope.Instance.ObjectAdded -= OnObjectAddedToScope;
            DebuggedObjectsScope.Instance.ObjectRemoved -= OnObjectRemovedFromScope;
            DebuggedObjectsScope.Instance.ObjectAdded += OnObjectAddedToScope;
            DebuggedObjectsScope.Instance.ObjectRemoved += OnObjectRemovedFromScope;
        }

        private void OnDisable()
        {
            foreach (var fObject in _filteredObjects)
                DebuggedObjectsScope.Instance.RemoveObjectFromScope(fObject);
            DebuggedObjectsScope.Instance.ObjectAdded -= OnObjectAddedToScope;
            DebuggedObjectsScope.Instance.ObjectRemoved -= OnObjectRemovedFromScope;
        }
        [NonSerialized]
        float _lastTimeUpdated = 0f;
        [NonSerialized]
        float _updateTime = 1f;
        FramesLogEditorWindow w;
        private void Update()
        {
            if (!Application.isPlaying)
            {
                _filteredObjectsView.Clear();
                return;
            }
            if (_lastTimeUpdated < Time.realtimeSinceStartup)
            {
                BuildFilteredView();
                _lastTimeUpdated = Time.realtimeSinceStartup + _updateTime;
            }
            FramesLog.Instance.FinishFrame();
            if (w == null)
                w = GetWindow<FramesLogEditorWindow>();
            if (w.CurrentlySelectedFrame == null)
                return;
        }

        void OnObjectAddedToScope(object obj)
        {
            if (!(obj is GameObject))
                return;
            var go = ((GameObject)obj);
            if (go == null)
                return;
            var d = go.AddComponent<DebuggerDrawer>();
            d.OnGuiAction = () =>
            {
                var snapshots = FramesLog.Instance.GetLastRecordedFrame()?.ObjectSnapshots.Where(x => x.ObjectId.Is(go)).ToArray();
                if (snapshots.Length > 0)
                    foreach (var snapshot in snapshots)
                        d.LastSnapshots[snapshot.PosterType] = snapshot;
                foreach (var snapshot in d.LastSnapshots.Values)
                {
                    snapshot.DrawSelf?.Invoke();
                }
            };
            d.OnGizmoAction = () =>
            {
                var snapshots = FramesLog.Instance.GetLastRecordedFrame()?.ObjectSnapshots.Where(x => x.ObjectId.Is(go)).ToArray();
                if (snapshots.Length > 0)
                    foreach (var snapshot in snapshots)
                        d.LastSnapshots[snapshot.PosterType] = snapshot;
                foreach (var snapshot in d.LastSnapshots.Values)
                {
                    snapshot.GizmoSelf?.Invoke();
                }
            };
        }

        void OnObjectRemovedFromScope(object obj)
        {
            if (!(obj is GameObject))
                return;
            var go = ((GameObject)obj);
            if (go == null)
                return;
            Destroy(go.GetComponent<DebuggerDrawer>());
        }

        void BuildFilteredView()
        {
            _filteredObjectsView.Clear();
            _rootObject = GameObject.FindGameObjectsWithTag("Player").SingleOrDefault(x => x.name.Contains("_me"));
            if (_rootObject == null && _distanceFilter)
                return;
            var intLayer = LayerMask.NameToLayer("Interactive");
            var activeLayer = LayerMask.NameToLayer("Active");
            var unfilteredObjects = FindObjectsOfType<GameObject>().Where(x => x.layer == intLayer || x.layer == activeLayer);
            foreach (var unfilteredObject in unfilteredObjects)
            {
                if (Filter(unfilteredObject))
                {
                    if (_filteredObjects.Add(unfilteredObject))
                    {
                        DebuggedObjectsScope.Instance.AddObjectToScope(unfilteredObject);
                    }
                    _filteredObjectsView.Add(new IMGUIContainer(() => DrawFilteredObject(unfilteredObject)) { style = { height = 40 } });
                }
                else if (_filteredObjects.Remove(unfilteredObject))
                    DebuggedObjectsScope.Instance.RemoveObjectFromScope(unfilteredObject);
            }
            foreach (var deletedObject in DebuggedObjectsScope.Instance.GetAllObjects().Where(x => x is GameObject && ((GameObject)x) == null))
                DebuggedObjectsScope.Instance.RemoveObjectFromScope(deletedObject);
        }

        bool Filter(GameObject obj)
        {
            if (!_distanceFilter)
                return true;
            if (_rootObject == null)
                return false;
            if ((_rootObject.transform.position - obj.transform.position).sqrMagnitude < _distance * _distance)
                return true;
            else
                return false;
        }

        void DrawFilteredObject(GameObject obj)
        {
            GUILayout.BeginVertical();
            GUILayout.Label(obj.name);
            GUILayout.EndVertical();
        }

        bool _distanceFilter = true;
        float _distance = 10f;
        void DrawFilters()
        {
            GUILayout.BeginVertical();
            _distanceFilter = EditorGUILayout.Toggle("Filter by distance", _distanceFilter);
            if (_distanceFilter)
                _distance = EditorGUILayout.FloatField("Distance to root: ", _distance);
            GUILayout.EndVertical();
        }
    }
}
