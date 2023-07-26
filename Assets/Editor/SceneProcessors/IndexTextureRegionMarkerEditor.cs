using System.Collections.Generic;
using System.Linq;
using Assets.Src.Regions.RegionMarkers;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.SceneProcessors
{
    [CustomEditor(typeof(IndexTextureRegionMarker))]
    public class IndexTextureRegionMarkerEditor : UnityEditor.Editor
    {
        private const int MaxIndexes = 4096;

        private readonly Color32[] _emptyColors = new Color32[0];
        private IndexedRegionMarker[] _duplicates;
        private IndexedRegionMarker[] _notInIndex;
        private Color32[] _notInChildren;
        private int _childrenCount;
        private int _colorsCount;
        private bool _noIndex;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawStatus();
            EditorGUILayout.Separator();

            if (GUILayout.Button("Sync Children"))
                SyncChildren();
        }

        private void SyncChildren()
        {
            var marker = (IndexTextureRegionMarker) target;
            if (marker.IndexedColors == null)
                return;

            var childrenColors = marker
                .GetComponentsInChildren<IndexedRegionMarker>(true)
                .Select(c => c.Color)
                .Take(MaxIndexes);
            var notInChildren = marker.IndexedColors.Where(c => !childrenColors.Contains(c.Key)).ToArray();
            foreach (var keyValuePair in notInChildren)
            {
                var go = new GameObject(keyValuePair.Value.ToString());
                go.transform.parent = marker.transform;
                var indexedRegionMarker = go.AddComponent<IndexedRegionMarker>();
                indexedRegionMarker.Color = keyValuePair.Key;
            }
        }

        private void DrawStatus()
        {
            if (Event.current.type == EventType.Layout)
            {
                var marker = (IndexTextureRegionMarker) target;
                var indexedColors = marker.IndexedColors != null ? marker.IndexedColors.Keys.ToArray() : _emptyColors;
                var children = marker.GetComponentsInChildren<IndexedRegionMarker>(true);
                var hash = new HashSet<Color32>();
                _noIndex = marker.IndexedColors == null;
                _childrenCount = children.Length;
                _colorsCount = indexedColors.Length;
                _duplicates = children.Where(c => !hash.Add(c.Color)).ToArray();
                _notInIndex = children.Where(c => !indexedColors.Contains(c.Color)).ToArray();
                _notInChildren = indexedColors.Where(c => !children.Select(c1 => c1.Color).Contains(c)).ToArray();
            }

            if (_noIndex) EditorGUIUtils.DrawError($"No Index Bitmap!");

            if (_colorsCount > MaxIndexes)
            {
                EditorGUIUtils.DrawError($"To Many Colors In Index: {_colorsCount} Max: {MaxIndexes}");
                return;
            }

            if (_childrenCount > MaxIndexes)
            {
                EditorGUIUtils.DrawError($"To Many Children: {_childrenCount} Max: {MaxIndexes}");
                return;
            }

            if (_duplicates.Any())
            {
                var names = _duplicates.Select(c => $"'{c.name}'");
                EditorGUIUtils.DrawError($"Children Duplicates: \n{string.Join(", ", names)}");
            }

            if (_notInIndex.Any())
            {
                var names = _notInIndex.Select(c => $"'{c.name}->{c.Color}'");
                EditorGUIUtils.DrawError($"Children Not In Index: \n{string.Join(",\n", names)}");
            }

            if (_notInChildren.Any())
            {
                var names = _notInChildren.Select(c => $"'{c}'");
                EditorGUIUtils.DrawWarning($"Colors Not In Children: \n{string.Join(",\n", names)}");
            }
        }
    }
}