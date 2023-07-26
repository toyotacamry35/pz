using System;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ContainerApis;
using Assets.Src.ResourceSystem.L10n;
using Assets.Src.SpawnSystem;
using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;

namespace Uins
{
    public class HasHealthBadgePoint : BadgePoint, IHasHealthBadgePoint
    {
        [SerializeField, UsedImplicitly]
        private StatResourceRef _healthStatRef;

        private EntityApiWrapper<HasStatsBroadcastApi> _hasStatsBroadcastApiWrapper;

        protected DisposableComposite tmpD = new DisposableComposite();


        //=== Props ===========================================================

        public PeriodicallyUpdatableStatValue CurrentHealthPu { get; private set; }

        public ReactiveProperty<float> MaxHealthRp { get; } = new ReactiveProperty<float>();

        ReactiveProperty<float> IHasHealthBadgePoint.CurrentHealthRp => CurrentHealthPu.ValueRp;
        

        //=== Unity ===========================================================

        private void Awake()
        {
            CurrentHealthPu = new PeriodicallyUpdatableStatValue(D, PlayerMainStatsViewModel.Roughness100, true);
        }

        protected override void Start()
        {
            // Profiler.BeginSample($"0 {_count++} {transform.FullName()}", gameObject);
            base.Start();
            // Profiler.EndSample();
            if (VisualMarker == null ||
                _healthStatRef.Target.AssertIfNull(nameof(_healthStatRef)) ||
                SurvivalGuiNode.Instance?.PawnChangesStream == null)
                return;

            VisualMarker.IsOurPlayerRp
                .Zip(D, VisualMarker.IsNearRp)
                .Zip(D, SurvivalGuiNode.Instance.PawnChangesStream)
                .Func(D, OnChanges)
                .Action(D, OnHasClient);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            IsNeedForGuiRp.Value = false;
            IsVisibleLogicallyRp.Value = false;

            if (_hasStatsBroadcastApiWrapper != null)
            {
                _hasStatsBroadcastApiWrapper.Dispose();
                _hasStatsBroadcastApiWrapper = null;
            }
        }


        //=== Protected =========================================================

        protected virtual void OnHasClient(bool hasClient)
        {
            var ego = VisualMarker.Ego;
            if (ego.AssertIfNull(nameof(ego)))
                return;

            //UI.CallerLog($"hasClient{hasClient.AsSign()} -- {transform.FullName()}"); //2del

            if (hasClient)
            {
                VisualMarker.IsImportantBadgeShownRp.Subscribe(tmpD, OnIsImportantBadgeShownRp, IsVisibleLogicallyRpAtEnd);

                _hasStatsBroadcastApiWrapper = EntityApi.GetWrapper<HasStatsBroadcastApi>(ego.OuterRef, true);
                _hasStatsBroadcastApiWrapper.EntityApi.SubscribeToStats(_healthStatRef.Target, OnHealthStatChanged);
                
                TimeTicker.Instance.GetLocalTimer(0.3f).Action(tmpD, OnHealthCurrentStatStateUpdate);
                VisualMarker.IsSelectedRp.Bind(tmpD, IsSelectedRp);
                VisualMarker.IsInRangeRp.Bind(tmpD, IsNeedForGuiRp);
            }
            else
            {
                tmpD.Clear();

                if (_hasStatsBroadcastApiWrapper != null)
                {
                    _hasStatsBroadcastApiWrapper.Dispose();
                    _hasStatsBroadcastApiWrapper = null;
                }
            }
        }


        //=== Private =========================================================

        private bool OnChanges(bool isOurPlayer, bool gotClient, (EntityGameObject, EntityGameObject) pawnEgoTuple)
        {
            //UI.CallerLog($"isOurPlayer{isOurPlayer.AsSign()}, gotClient{gotClient.AsSign()}, pawnEgo={pawnEgoTuple.Item2} -- {transform.FullName()}"); //2del
            return !isOurPlayer && pawnEgoTuple.Item2 != null && gotClient;
        }

        private void OnHealthStatChanged(StatResource statResource, AnyStatState statState)
        {
            MaxHealthRp.Value = statState.Max;
            CurrentHealthPu.StatStateRp.Value = statState;
        }

        private void OnIsImportantBadgeShownRp(bool isOn)
        {
            IsVisibleLogicallyRp.Value = !isOn;
        }

        private void IsVisibleLogicallyRpAtEnd()
        {
            OnIsImportantBadgeShownRp(false);
        }

        private void OnHealthCurrentStatStateUpdate(DateTime dt)
        {
            CurrentHealthPu?.UpdateIfNeed();
        }
    }
}