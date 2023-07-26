using Assets.Src.SpatialSystem;
using Assets.Src.SpawnSystem;
using ColonyDI;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;
using UnityEngine;

namespace Uins
{
    public class MapGuiNode : DependencyNodeWithChildren
    {
        [Dependency]
        private SurvivalGuiNode SurvivalGui { get; set; }

        [Dependency]
        private LobbyGuiNode LobbyGuiNode { get; set; }

        [SerializeField, UsedImplicitly]
        private MapGuiWindow _mapGuiWindow;

        [SerializeField, UsedImplicitly]
        private MapIndicatorsPositions _mapIndicatorsPositions;

        [SerializeField, UsedImplicitly]
        private UserMarkers _userMarkers;

        [SerializeField]
        public FogOfWarView MapFogOfWarView;

        private RegionVisitNotifier _regionVisitNotifier;

        //=== Unity ===========================================================

        private void Awake()
        {
            _mapGuiWindow.AssertIfNull(nameof(_mapGuiWindow));
            _mapIndicatorsPositions.AssertIfNull(nameof(_mapIndicatorsPositions));
            _userMarkers.AssertIfNull(nameof(_userMarkers));
        }


        //=== Protected =======================================================

        public override void AfterDependenciesInjected()
        {
            SurvivalGui.PawnChangesStream.Action(D, OnOurPawnChanged);
            _mapIndicatorsPositions.Init(SurvivalGui, NavigationProvider.Instance, LobbyGuiNode, _mapGuiWindow.CurrentZoomRatioRp);
            _userMarkers.Init(SurvivalGui);

            var rootRegionVM = new RootRegionVM(SurvivalGui);
            var fogOfWarVM = new FogOfWarVM(SurvivalGui);
            var mapFogOfWarVM = new MapFogOfWarVM(rootRegionVM.RootRegion, rootRegionVM.DefToRegionMap, fogOfWarVM.RegionDefs);

            MapFogOfWarView.Init(mapFogOfWarVM);

            var regionVisitNotifierVM = new RegionGroupVisitVM(
                rootRegionVM.DefToRegionMap,
                fogOfWarVM.CurrentRegionDefs,
                fogOfWarVM.CurrentRegionGroupDefs
            );
            _regionVisitNotifier = new RegionVisitNotifier(regionVisitNotifierVM);
        }

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
            {
                if (_mapGuiWindow.State.Value != GuiWindowState.Closed)
                    WindowsManager.Close(_mapGuiWindow);
            }
        }
    }
}