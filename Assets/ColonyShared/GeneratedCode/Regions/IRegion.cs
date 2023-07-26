using SharedCode.Aspects.Regions;
using System.Collections.Generic;
using SharedCode.Utils;
using SharedCode.Entities;

namespace Assets.ColonyShared.GeneratedCode.Regions
{
    public interface IRegion
    {
        IRegion ParentRegion { get; set; }
        List<IRegion> ChildRegions { get; }
        void AddChild(IRegion buildRegion);
        int Level { get; set; }

        bool IsInside(Vector3 coords);
        void GetAllContainingRegionsNonAlloc(List<IRegion> regions, Vector3 pointCoords);
        void InitRegionWithDef(ARegionDef def, Transform providedTransform = default);
        ARegionDef RegionDef { get; set; }
    }
}
