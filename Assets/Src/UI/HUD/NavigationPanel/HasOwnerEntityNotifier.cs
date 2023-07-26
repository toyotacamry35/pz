using System;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl;
using Assets.Src.ContainerApis;
using Assets.Src.SpawnSystem;
using Core.Environment.Logging.Extension;
using NLog;
using ReactivePropsNs;

namespace Uins
{
    public abstract class HasOwnerEntityNotifier : NavigationIndicatorNotifier
    {
        public readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private Guid _ourPlayerGuid;

        private Guid _ownerGuid;

        private bool _pointMarkerHasBeenRemoved;

        private Guid _selfMarkerGuid;

        private EntityApiWrapper<EntityGuidApi> _ourPawnEntityApiWrapper;
        private EntityApiWrapper<HasOwnerBroadcastApi> _ownerEntityApiWrapper;

        //=== Props ===========================================================

        protected bool IsOwned => _ownerGuid != Guid.Empty && _ourPlayerGuid == _ownerGuid;


        //=== Unity ===========================================================

        private void Awake()
        {
            if (GetComponent<HasOwnerEntityNotifierEgoComponent>() == null)
                Logger.IfError()?.Message($"<{nameof(HasOwnerEntityNotifier)}> also should be with <{nameof(HasOwnerEntityNotifierEgoComponent)}>!").Write();
        }

        protected override void OnDestroy()
        {
            D.Dispose();
            RemoveNavigationPointMarker();
        }


        //=== Piblic ===========================================================

        public void SubscribeClient(Guid selfMarkerGuid)
        {
            _selfMarkerGuid = selfMarkerGuid;
            var selfEgo = gameObject.GetComponent<EntityGameObject>();
            if (selfEgo.AssertIfNull(nameof(selfEgo), gameObject))
                return;

            _ownerEntityApiWrapper = EntityApi.GetWrapper<HasOwnerBroadcastApi>(selfEgo.OuterRef);
            _ownerEntityApiWrapper.EntityApi.SubscribeToOwnerGuid(OnOwnerGuidReceived);

            if (SurvivalGuiNode.Instance != null)
                SurvivalGuiNode.Instance.PawnChangesStream.Action(D, OnOurPawnChanged);
        }

        public void UnsubscribeClient()
        {
            D.Clear();
            IsDisplayable = false;
            _ownerEntityApiWrapper.EntityApi.UnsubscribeFromOwnerGuid(OnOwnerGuidReceived);
            _ownerEntityApiWrapper.Dispose();
            _ownerEntityApiWrapper = null;

            _ownerGuid = Guid.Empty;
        }


        //=== Protected =======================================================

        protected override bool GetIsDisplayable()
        {
            return IsOwned;
        }

        protected override void OnDisplayableChanged()
        {
            if (!IsDisplayable || _pointMarkerHasBeenRemoved)
                return;

            PointMarkersApi.AddPointMarker(_selfMarkerGuid, NavIndicatorDef, gameObject.transform.position, _ourPlayerGuid);
            // UI.CallerLog($"{transform.FullName()} ---ADD--- pos={gameObject.transform.position}, guid={_selfMarkerGuid}"); //DEBUG
        }

        public void RemoveNavigationPointMarker()
        {
            if (_pointMarkerHasBeenRemoved || !IsOwned)
                return;

            _pointMarkerHasBeenRemoved = true;
            PointMarkersApi.RemovePointMarker(_selfMarkerGuid, _ourPlayerGuid);
            // UI.CallerLog($"{transform.FullName()} ---REMOVE--- pos={gameObject.transform.position}, guid={_selfMarkerGuid}"); //DEBUG
        }


        //=== Private =========================================================

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null && _ourPawnEntityApiWrapper != null)
            {
                _ourPawnEntityApiWrapper.EntityApi.UnsubscribeFromEntityGuid(OnOurPlayerGuidReceived);
                _ourPawnEntityApiWrapper.Dispose();
                _ourPawnEntityApiWrapper = null;
            }

            if (newEgo != null)
            {
                _ourPawnEntityApiWrapper = EntityApi.GetWrapper<EntityGuidApi>(newEgo.OuterRef);
                _ourPawnEntityApiWrapper.EntityApi.SubscribeOnEntityGuid(OnOurPlayerGuidReceived);
            }
        }

        private void OnOurPlayerGuidReceived(Guid entityGuid)
        {
            _ourPlayerGuid = entityGuid;
            IsDisplayable = GetIsDisplayable();
        }

        private void OnOwnerGuidReceived(Guid ownerGuid)
        {
            _ownerGuid = ownerGuid;
            IsDisplayable = GetIsDisplayable();
        }
    }
}