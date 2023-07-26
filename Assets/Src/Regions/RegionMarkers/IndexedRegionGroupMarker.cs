using System;
using System.Linq;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.SpatialSystem;
using L10n;
using SharedCode.Aspects.Regions;
using UnityEngine;

namespace Assets.Src.Regions.RegionMarkers
{
    [ExecuteInEditMode]
    public class IndexedRegionGroupMarker : RegionMarker
    {
        private IndexTextureRegionMarker _parent;
        private string LogName => $"{GetType().Name} {name}";

        private IndexTextureRegionMarker ParentIndexTextureRegionMarker =>
            _parent ? _parent : _parent = GetParentIndexTextureRegionMarker();

        [SerializeField]
        private string Guid;
        
        [SerializeField]
        private LocalizationKeyProp Title;

        private void OnEnable()
        {
            if (string.IsNullOrWhiteSpace(Guid)) Guid = System.Guid.NewGuid().ToString();
        }

        public override ARegionDef BuildDefs()
        {
            if (ParentIndexTextureRegionMarker == null)
                throw new Exception($"Error {LogName} Has No Parent IndexTextureRegionMarker");

            var regionDef = new IndexedRegionGroupDef
            {
                ChildRegions = GetChildMarkers()
                    .Select(
                        childReg => new ResourceRef<ARegionDef>(childReg.BuildDefs())
                    )
                    .ToArray(),
                Data = GetRegionData(),
                Id = System.Guid.Parse(Guid),
                TitleJdbRef = Title.Ref.Target,
                TitleJdbKey = Title.Key
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