using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Assets.ColonyShared.GeneratedCode.Regions;
using Assets.Src.SpatialSystem;
using ReactivePropsNs;
using SharedCode.Aspects.Regions;

namespace Uins
{
    public class MapFogOfWarVM : BindingVmodel
    {
        // private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public DictionaryStream<IndexedRegion, bool> Regions { get; }

        public IStream<IIndexRegion> IndexRegion { get; }
        public IHashSetStream<IndexedRegion> Discovered { get; }

        public MapFogOfWarVM(
            IStream<IRegion> rootRegionStream,
            IStream<ImmutableDictionary<ARegionDef, IRegion>> defToRegionMapStream,
            IDictionaryStream<IndexedRegionDef, bool> discoveredRegionDefs)
        {
            // var mapDefStream = GameObjectCreator.ClusterSpawnInstance.CurrentMap;
            IndexRegion = rootRegionStream.Func(D, FindFogOfWarIndexRegion);
            Regions = CreateRegionsDictionaryStream(defToRegionMapStream, discoveredRegionDefs);
            Discovered = CreateDiscoveredRegionsStream(Regions);
        }

        private DictionaryStream<IndexedRegion, bool> CreateRegionsDictionaryStream(
            IStream<ImmutableDictionary<ARegionDef, IRegion>> defToRegionMapStream,
            IDictionaryStream<IndexedRegionDef, bool> regionDefs)
        {
            var converterStream = defToRegionMapStream
                .Func<
                    ImmutableDictionary<ARegionDef, IRegion>,
                    Func<(IndexedRegionDef, bool), ICollection<IDisposable>, (bool result, IndexedRegion region, bool visited)>
                >(
                    D,
                    defToRegionMap =>
                    {
                        return (tuple, localD) =>
                        {
                            var (indexedRegionDef, visited) = tuple;
                            if (defToRegionMap != null &&
                                defToRegionMap.TryGetValue(indexedRegionDef, out var region) &&
                                region is IndexedRegion indexedRegion
                            )
                                return (true, indexedRegion, visited);

                            return (false, null, default);
                        };
                    }
                );
            return regionDefs.FuncMutable(D, converterStream);
        }

        private IHashSetStream<IndexedRegion> CreateDiscoveredRegionsStream(IDictionaryStream<IndexedRegion, bool> regions)
        {
            var discovered = new HashSetStream<IndexedRegion>();
            D.Add(discovered);

            var localD = D.CreateInnerD();
            IndexRegion.Subscribe(
                D,
                indexRegion =>
                {
                    D.DisposeInnerD(localD);
                    discovered.Clear();

                    localD = D.CreateInnerD();
                    //Changes Ignored
                    regions.AddStream.Action(
                        localD,
                        e =>
                        {
                            var indexedRegion = e.Key;
                            if (indexedRegion != null && indexedRegion.ParentIndexRegion == indexRegion)
                                discovered.Add(indexedRegion);
                        }
                    );
                    regions.RemoveStream.Subscribe(
                        localD,
                        e =>
                        {
                            var indexedRegion = e.Key;
                            if (indexedRegion != null)
                                discovered.Remove(indexedRegion);
                        },
                        () => { discovered.Clear(); }
                    );
                },
                () =>
                {
                    D.DisposeInnerD(localD);
                    discovered.Clear();
                }
            );

            return discovered;
        }

        private static IIndexRegion FindFogOfWarIndexRegion(IRegion root)
        {
            if (root is IIndexRegion indexRegion &&
                (indexRegion.RegionDef?.Data?.Any(resourceRef => resourceRef.Target is FogOfWarRegionDef) ?? false))
                return indexRegion;
            return root?.ChildRegions?.Select(FindFogOfWarIndexRegion)
                .FirstOrDefault(indexSubRegion => indexSubRegion != null);
        }
    }
}