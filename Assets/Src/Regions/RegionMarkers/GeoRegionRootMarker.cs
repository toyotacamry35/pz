using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Regions;
using System.Collections.Generic;
using System.Linq;
using Core.Environment.Logging.Extension;

namespace Assets.Src.Regions.RegionMarkers
{
    public class GeoRegionRootMarker : RegionMarker
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public override ARegionDef BuildDefs()
        {
            var regDef = new GeoRegionRootDef();
            var childRegions = GetChildMarkers();
            if (childRegions.Any(x => !(x is GeoRegionMarker)))
                Logger.IfError()?.Message($"{nameof(GeoRegionRootMarker)} '{gameObject.name}' could not contain anything but {nameof(GeoRegionMarker)}").Write();
            if (childRegions.Count > 0)
            {
                List<ResourceRef<ARegionDef>> childs = new List<ResourceRef<ARegionDef>>(childRegions.Count);
                foreach (var childReg in childRegions)
                    childs.Add(childReg.BuildDefs());
                regDef.ChildRegions = childs.ToArray();
            }
            regDef.Data = GetRegionData();
            return regDef;
        }
    }
}
