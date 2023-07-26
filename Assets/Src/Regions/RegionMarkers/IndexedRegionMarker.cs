using System;
using System.Linq;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.SpatialSystem;
using SharedCode.Aspects.Regions;
using UnityEngine;

namespace Assets.Src.Regions.RegionMarkers
{
    [ExecuteInEditMode]
    public class IndexedRegionMarker : RegionMarker
    {
        public Color32 Color;
        private IndexTextureRegionMarker _parent;
        private string LogName => $"{GetType().Name} {name}";

        public IndexTextureRegionMarker ParentIndexTextureRegionMarker => _parent ? _parent : _parent = GetParentIndexTextureRegionMarker();

        [SerializeField]
        private string Guid;

        private void OnEnable()
        {
            if (string.IsNullOrWhiteSpace(Guid)) Guid = System.Guid.NewGuid().ToString();
        }

        public override ARegionDef BuildDefs()
        {
            if (ParentIndexTextureRegionMarker == null)
                throw new Exception($"Error {LogName} Has No Parent IndexTextureRegionMarker");

            if (!ParentIndexTextureRegionMarker.IndexedColors.TryGetValue(Color, out var indexedColor))
                throw new Exception($"Error {LogName} Color {Color} Not Exists In IndexTextureRegionMarker");

            var regionDef = new IndexedRegionDef
            {
                Index = indexedColor,
                ChildRegions = GetChildMarkers()
                    .Select(
                        childReg => new ResourceRef<ARegionDef>(childReg.BuildDefs())
                    )
                    .ToArray(),
                Data = GetRegionData(),
                Id = System.Guid.Parse(Guid)
            };
            return regionDef;
        }

        private IndexTextureRegionMarker GetParentIndexTextureRegionMarker()
        {
            var parent = transform;
            IndexTextureRegionMarker marker = null;
            while(parent != null && marker == null)
            {
                marker = parent.GetComponent<IndexTextureRegionMarker>();
                parent = parent.parent;
            }

            return marker;
        }
    }
}