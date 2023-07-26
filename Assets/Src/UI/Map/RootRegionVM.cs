using System;
using System.Collections.Immutable;
using System.Threading;
using Assets.ColonyShared.GeneratedCode.Regions;
using Assets.ColonyShared.SharedCode.Shared;
using GeneratedCode.DeltaObjects;
using ReactivePropsNs;
using SharedCode.Aspects.Regions;

namespace Uins
{
    public class RootRegionVM : BindingVmodel
    {
        // private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ReactiveProperty<IRegion> _rootRegion = new ReactiveProperty<IRegion>();

        private readonly ReactiveProperty<ImmutableDictionary<ARegionDef, IRegion>> _defToRegionMap =
            new ReactiveProperty<ImmutableDictionary<ARegionDef, IRegion>>();

        public IReactiveProperty<IRegion> RootRegion => _rootRegion;

        public IReactiveProperty<ImmutableDictionary<ARegionDef, IRegion>> DefToRegionMap => _defToRegionMap;

        public RootRegionVM(IPawnSource pawnSource)
        {
            D.Add(RootRegion);
            D.Add(DefToRegionMap);

            var sceneIdStream = pawnSource.PawnChangesStream
                .Func(
                    D,
                    (_, ego) => ego != null ? NodeAccessor.Repository.GetSceneForEntity(ego.OuterRefEntity) : default
                );

            sceneIdStream.Action(
                D,
                sceneId =>
                {
                    if (sceneId != Guid.Empty)
                    {
                        var (rootRegion, regionsByDef) = RegionsHolder.GetRegionsByGuid(sceneId);
                        _rootRegion.Value = rootRegion;
                        _defToRegionMap.Value = regionsByDef?.ToImmutableDictionary();
                    }
                    else
                    {
                        _rootRegion.Value = null;
                        _defToRegionMap.Value = null;
                    }
                }
            );
        }
    }
}