using System.Collections.Generic;
using SharedCode.Aspects.Regions;
using SharedCode.Entities;
using SharedCode.Utils;

namespace Assets.ColonyShared.GeneratedCode.Regions
{
    public class GeoRegionRoot : BaseRegion, ISpatialTableRegion
    {
        public IObjectContainer<IRegion> SpatialTable { get; set; } = new AABBHashedContainer<IRegion>();

        public override void GetAllContainingRegionsNonAlloc(List<IRegion> regions, Vector3 pointCoords)
        {
            if (IsInside(pointCoords))
            {
                regions.Add(this);
                if (SpatialTable.GetByPoint(out var regionsSp, out var bigObjects, pointCoords))
                {
                    foreach (var spReg in regionsSp)
                    {
                        if (spReg.IsInside(pointCoords))
                            regions.Add(spReg);
                    }

                    foreach (var spReg in bigObjects)
                    {
                        if (spReg.IsInside(pointCoords))
                            regions.Add(spReg);
                    }

                }

            }
        }
        
        public override void InitRegionWithDef(ARegionDef def, Transform providedTransform = default)
        {
            var defClass = def as GeoRegionRootDef;
            RegionDef = defClass;
        }

        public override bool IsInside(Vector3 coords) => true;
    }
}
