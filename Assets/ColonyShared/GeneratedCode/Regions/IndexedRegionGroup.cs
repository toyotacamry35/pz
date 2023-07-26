using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Src.SpatialSystem;
using SharedCode.Aspects.Regions;
using SharedCode.Entities;
using SharedCode.Utils;

namespace Assets.ColonyShared.GeneratedCode.Regions
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class IndexedRegionGroup : BaseRegion
    {
        private string LogName => $"{GetType().Name}";

        private IIndexRegion _parentIndexRegion;
        public IIndexRegion ParentIndexRegion => _parentIndexRegion ?? (_parentIndexRegion = GetParentIndexRegion(ParentRegion));

        public IndexedRegionGroupDef IndexedRegionGroupDef { get; private set; }

        public List<IndexedRegion> IndexedChildRegions { get; set; }
        
        public override bool IsInside(Vector3 coords)
        {
            if (ParentIndexRegion == null)
                throw new Exception($"Error {LogName} Has No Parent Index Region");

            var index = ParentIndexRegion.GetIndexAt(coords);
            if (index != -1)
            {
                foreach (var indexedChildRegion in IndexedChildRegions)
                {
                    if (indexedChildRegion.Index == index)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override void InitRegionWithDef(ARegionDef def, Transform providedTransform = default)
        {
            IndexedRegionGroupDef = def as IndexedRegionGroupDef;
            RegionDef = def;
        }
        
        private static IIndexRegion GetParentIndexRegion(IRegion parent)
        {
            while (parent != null && !(parent is IIndexRegion))
                parent = parent.ParentRegion;

            return parent as IIndexRegion;
        }
    }
}