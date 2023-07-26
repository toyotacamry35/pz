using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Regions;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Src.Regions.RegionMarkers
{
    public class RootRegionMarker : RegionMarker
    {
        public override ARegionDef BuildDefs()
        {
            var regDef = new RootRegionDef();
            var childRegions = GetChildMarkers();
            regDef.ChildRegions = childRegions
                .Select(childReg => childReg.BuildDefs())
                .Select(dummy => (ResourceRef<ARegionDef>) dummy)
                .ToArray();

            regDef.Data = GetRegionData();
            return regDef;
        }
    }
}