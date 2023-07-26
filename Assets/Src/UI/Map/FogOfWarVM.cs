using Assets.Src.SpatialSystem;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;

namespace Uins
{
    public class FogOfWarVM : BindingVmodel
    {
        // private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public IDictionaryStream<IndexedRegionDef, bool> RegionDefs { get; }
        public IDictionaryStream<IndexedRegionGroupDef, bool> RegionGroupDefs { get; }
        public IHashSetStream<IndexedRegionDef> CurrentRegionDefs { get; }
        public IHashSetStream<IndexedRegionGroupDef> CurrentRegionGroupDefs { get; }

        public FogOfWarVM(IPawnSource pawnSource)
        {
            var fogOfWarTouchable = pawnSource.TouchableEntityProxy.Child(D, character => character.FogOfWar);

            RegionDefs = fogOfWarTouchable.ToDictionaryStream(D, fogOfWar => fogOfWar.Regions);
            CurrentRegionDefs = fogOfWarTouchable.ToHashSetStream(D, fogOfWar => fogOfWar.CurrentRegions);
            RegionGroupDefs = fogOfWarTouchable.ToDictionaryStream(D, fogOfWar => fogOfWar.RegionGroups);
            CurrentRegionGroupDefs = fogOfWarTouchable.ToHashSetStream(D, fogOfWar => fogOfWar.CurrentGroups);
        }
    }
}