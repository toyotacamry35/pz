using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Src.Aspects.Impl.Traumas.Template;
using Assets.Src.ContainerApis;
using Assets.Src.SpawnSystem;
using JetBrains.Annotations;
using ReactivePropsNs;
using ReactivePropsNs.ThreadSafe;
using Uins.Inventory;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class CharStatsContextViewModel : DependencyEndNode
    {
        [SerializeField, UsedImplicitly]
        private ContextViewWithParams _contextView;

        [SerializeField, UsedImplicitly]
        private Template _playerMainStatsTemplate;

        [SerializeField, UsedImplicitly]
        private Transform _traumasTransform;

        [SerializeField, UsedImplicitly]
        private TraumaIndicatorContr _traumaIndicatorContrPrefab;

        private bool _isBindedToPlayerMainStats;
        private HudGuiNode _hudGui;
        private EntityApiWrapper<HasTraumasFullApi> _hasTraumasFullApiWrapper;
        private ListStream<TraumaDef> _traumaGiverDefs = new ListStream<TraumaDef>();


        //=== Props ===========================================================

        [Binding]
        public bool IsVisible { get; private set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            _contextView.AssertIfNull(nameof(_contextView));
            _playerMainStatsTemplate.AssertIfNull(nameof(_playerMainStatsTemplate));
            _traumaIndicatorContrPrefab.AssertIfNull(nameof(_traumaIndicatorContrPrefab));
            _traumasTransform.AssertIfNull(nameof(_traumasTransform));
        }


        //=== Public ==========================================================

        public void Init(IPawnSource pawnSource, HudGuiNode hudGui)
        {
            _hudGui = hudGui;
            hudGui.AssertIfNull(nameof(hudGui));

            var isVisibleStream = _contextView.Vmodel
                .SubStream(D, vm => vm.CurrentContext)
                .Func(D, target => target as CharStatsSideContextViewTarget != null);
            Bind(isVisibleStream, () => IsVisible);
            isVisibleStream.Action(D, SetIsShown);

            if (!pawnSource.AssertIfNull(nameof(pawnSource)))
                pawnSource.PawnChangesStream.Action(D, OnOurPawnChanged);

            var bindingControllersPool = new BindingControllersPoolWithGoActivation<TraumaDef>(_traumasTransform, _traumaIndicatorContrPrefab);
            bindingControllersPool.Connect(_traumaGiverDefs);
        }


        //=== Private =========================================================

        private void SetIsShown(bool isVisible)
        {
            if (isVisible && !_isBindedToPlayerMainStats)
            {
                _isBindedToPlayerMainStats = true;
                _playerMainStatsTemplate.InitChildBindings(_hudGui.PlayerMainStatsViewModel);
            }
        }

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
            {
                _hasTraumasFullApiWrapper.EntityApi.UnsubscribeFromActivityChanged(OnTraumasChanged);
                _hasTraumasFullApiWrapper.Dispose();
                _hasTraumasFullApiWrapper = null;
            }

            if (newEgo != null)
            {
                _hasTraumasFullApiWrapper = EntityApi.GetWrapper<HasTraumasFullApi>(newEgo.OuterRef);
                _hasTraumasFullApiWrapper.EntityApi.SubscribeToTraumasChanged(OnTraumasChanged);
            }
        }

        private void OnTraumasChanged(IReadOnlyDictionary<string, TraumaDef> newTraumas)
        {
            var defsToRemove = _traumaGiverDefs.Where(def => !newTraumas.Values.Contains(def)).ToList();
            foreach (var defToRemove in defsToRemove)
                _traumaGiverDefs.Remove(defToRemove);

            var defsToAdd = newTraumas
                .Where(newTr => newTr.Value.Icon() != null && _traumaGiverDefs.All(oldTr => oldTr != newTr.Value))
                .ToList();
            foreach (var defToAdd in defsToAdd)
                _traumaGiverDefs.Add(defToAdd.Value);
        }
    }
}