using System.Linq;
using Assets.Src.Regions.RegionMarkers;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.SceneProcessors
{
    [CustomEditor(typeof(IndexedRegionMarker))]
    public class IndexedRegionMarkerEditor : UnityEditor.Editor
    {
        private IndexedRegionMarker[] _duplicates;
        private bool _notInIndex;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawStatus();

            EditorGUILayout.Separator();

            if (GUILayout.Button("Pick Color"))
                PickColor();

            if (GUILayout.Button("Move To Color"))
                MoveToColor();

            if (GUILayout.Button("Create Group"))
                CreateGroup();
        }

        private void MoveToColor()
        {
            var marker = (IndexedRegionMarker) target;
            var position = marker.ParentIndexTextureRegionMarker.GetPositionOf(marker.Color);
            marker.transform.position = position;
        }

        private void PickColor()
        {
            var marker = (IndexedRegionMarker) target;
            var color = marker.ParentIndexTextureRegionMarker.GetColorAt(marker.transform.position);
            marker.Color = color;
        }

        private void CreateGroup()
        {
            var marker = (IndexedRegionMarker) target;
            var transform = marker.transform;
            var index = transform.GetSiblingIndex();
            var go = new GameObject($"Group From {marker.name}");
            var goTransform = go.transform;
            goTransform.parent = transform.parent;
            goTransform.SetSiblingIndex(index);
            go.AddComponent<IndexedRegionGroupMarker>();

            transform.parent = goTransform;
        }

        private void DrawStatus()
        {
            var marker = (IndexedRegionMarker) target;
            if (Event.current.type == EventType.Layout)
            {
                var indexedColors = marker.ParentIndexTextureRegionMarker.IndexedColors.Keys;
                var siblings = marker.ParentIndexTextureRegionMarker
                    .GetComponentsInChildren<IndexedRegionMarker>(true)
                    .Where(c => c != marker);
                _duplicates = siblings.Where(c => c.Color.Equals(marker.Color)).ToArray();
                _notInIndex = !indexedColors.Contains(marker.Color);
            }

            if (_duplicates.Any())
            {
                var names = _duplicates.Select(c => $"'{c.name}'");
                EditorGUIUtils.DrawError($"Siblings Duplicates: \n{string.Join(", ", names)}");
            }

            if (_notInIndex)
                EditorGUIUtils.DrawError($"Not In Index: {marker.Color}");
        }
    }
}