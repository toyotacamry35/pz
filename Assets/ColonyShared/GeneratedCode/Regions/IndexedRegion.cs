using System;
using Assets.Src.SpatialSystem;
using SharedCode.Aspects.Regions;
using Transform = SharedCode.Entities.Transform;
using Vector3 = SharedCode.Utils.Vector3;

namespace Assets.ColonyShared.GeneratedCode.Regions
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class IndexedRegion : BaseRegion
    {
        private string LogName => $"{GetType().Name}";

        public short Index => IndexRegionDef.Index;

        public IndexedRegionDef IndexRegionDef { get; private set; }

        private IIndexRegion _parentIndexRegion;
        public IIndexRegion ParentIndexRegion => _parentIndexRegion ?? (_parentIndexRegion = GetParentIndexRegion(ParentRegion));

        private IndexedRegionGroup _parentRegionGroup;
        public IndexedRegionGroup ParentRegionGroup => _parentRegionGroup ?? (_parentRegionGroup = GetParentRegionGroup(ParentRegion));

        public override bool IsInside(Vector3 coords)
        {
            if (ParentIndexRegion == null)
                throw new Exception($"Error {LogName} Has No Parent Index Region");

            var index = ParentIndexRegion.GetIndexAt(coords);
            return index != -1 && index == Index;
        }

        public override void InitRegionWithDef(ARegionDef def, Transform providedTransform = default)
        {
            IndexRegionDef = def as IndexedRegionDef;
            RegionDef = def;
        }

        private static IIndexRegion GetParentIndexRegion(IRegion parent)
        {
            while (parent != null && !(parent is IIndexRegion))
                parent = parent.ParentRegion;

            return parent as IIndexRegion;
        }

        private static IndexedRegionGroup GetParentRegionGroup(IRegion parent)
        {
            while (parent != null && !(parent is IndexedRegionGroup))
                parent = parent.ParentRegion;

            return parent as IndexedRegionGroup;
        }
    }
}