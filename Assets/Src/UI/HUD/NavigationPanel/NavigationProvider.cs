using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.Src.ResourceSystem;
using Assets.Src.SpawnSystem;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using ReactivePropsNs;
using Src.Aspects.Impl;
using UnityEngine;

namespace Uins
{
    public class NavigationProvider : HasDisposablesMonoBehaviour, IMapNotificationProvider, INavigationNotificationProvider
    {
        public event AddMapTargetDelegate AddMapIndicatorTarget;
        public event TargetGuidAndTransformDelegate RemoveMapIndicatorTarget;

        public event AddNavigationTargetDelegate AddNavigationIndicatorTarget;
        public event TargetTransformDelegate RemoveNavigationIndicatorTarget;
        public event TargetTransformDelegate SelectedTargetChanged;

        [SerializeField, UsedImplicitly]
        private NavIndicatorDefRef _myIndicatorDefRef;

        public static NavigationProvider Instance { get; private set; }

        private HashSet<NavigationIndicatorNotifierBase> _allNotifiers = new HashSet<NavigationIndicatorNotifierBase>();

        private IQuestTrackingContext _questTrackingContext;

        private Transform _selectedTarget;
        private NavIndicator _myNavIndicator;


        //=== Unity ===========================================================

        private void Awake()
        {
            if (!(_myIndicatorDefRef?.Target).AssertIfNull(nameof(_myIndicatorDefRef)))
                _myNavIndicator = new NavIndicator(_myIndicatorDefRef.Target);
            Instance = SingletonOps.TrySetInstance(this, Instance);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (Instance == this)
                Instance = null;
        }


        //=== Public ==========================================================

        public void Init(IPawnSource pawnSource, IQuestTrackingContext questTrackingContext)
        {
            if (!pawnSource.AssertIfNull(nameof(pawnSource)))
                pawnSource.PawnChangesStream.Action(D, OnOurPawnChanged);

            _questTrackingContext = questTrackingContext;
            if (!_questTrackingContext.AssertIfNull(nameof(_questTrackingContext)))
                _questTrackingContext.TrackedQuestPoiList.Action(D, OnTrackedQuestPoiListChanged);

            PointOfInterestNotifier.Notifiers.InsertStream.Action(D, OnPoiNotifierInsert);
            PointOfInterestNotifier.Notifiers.RemoveStream.Action(D, OnPoiNotifierRemove);
        }

        private void OnPoiNotifierRemove(RemoveEvent<PointOfInterestNotifier> removeEvent)
        {
            removeEvent.Item.Active = false;
        }

        private void OnPoiNotifierInsert(InsertEvent<PointOfInterestNotifier> insertEvent)
        {
            var notifier = insertEvent.Item;
            var poiDefs = _questTrackingContext.TrackedQuestPoiList.Value;
            notifier.Active = poiDefs != null && poiDefs.Contains(notifier.Identifier);
        }

        public void AddNavigationIndicatorNotifier(NavigationIndicatorNotifierBase notifier)
        {
            if (CanBeAddedToUi(notifier))
                AddIndicator(notifier);
        }

        public void RemoveNavigationIndicatorNotifier(NavigationIndicatorNotifierBase notifier)
        {
            if (CanBeRemovedFromUi(notifier))
                RemoveIndicator(notifier);
        }

        public void SetSelectedTarget(Transform newTarget)
        {
            if (_selectedTarget == newTarget)
                return;

            _selectedTarget = newTarget;
            try
            {
                SelectedTargetChanged?.Invoke(newTarget);
            }
            catch (Exception e)
            {
                UI.Logger.IfError()?.Message(e.ToString()).Write();
            }
        }


        //=== Private =========================================================

        private void AddIndicator(NavigationIndicatorNotifierBase notifier)
        {
            try
            {
                AddNavigationIndicatorTarget?.Invoke(notifier.transform, notifier.NavigationIndicatorSettings);
                AddMapIndicatorTarget?.Invoke(notifier.transform, GetGuid(notifier), notifier.MapIndicatorSettings, true);
            }
            catch (Exception e)
            {
                UI.Logger.IfError()?.Message(e.ToString()).Write();
            }
        }

        private void RemoveIndicator(NavigationIndicatorNotifierBase notifier)
        {
            try
            {
                RemoveNavigationIndicatorTarget?.Invoke(notifier.transform);
                RemoveMapIndicatorTarget?.Invoke(GetGuid(notifier), notifier.transform);
            }
            catch (Exception e)
            {
                UI.Logger.IfError()?.Message(e.ToString()).Write();
            }
        }

        private bool CanBeAddedToUi(NavigationIndicatorNotifierBase notifier)
        {
            if (_allNotifiers.Contains(notifier))
            {
                UI.Logger.IfWarn()?.Message($"{nameof(notifier)}='{notifier}' already exists in {nameof(_allNotifiers)}").Write();
                return false;
            }

            _allNotifiers.Add(notifier);
            return true;
        }

        private bool CanBeRemovedFromUi(NavigationIndicatorNotifierBase notifier)
        {
            if (!_allNotifiers.Contains(notifier))
            {
                UI.Logger.IfError()?.Message($"{nameof(RemoveNavigationIndicatorNotifier)}() not found {nameof(notifier)}={notifier}").Write();
                return false;
            }

            _allNotifiers.Remove(notifier);
            return true;
        }

        private Guid GetGuid(NavigationIndicatorNotifierBase notifier)
        {
            return notifier is NavigationIndicatorNotifier ? ((NavigationIndicatorNotifier) notifier).MarkerGuid : Guid.Empty;
        }

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
            {
                RemoveMapIndicatorTarget?.Invoke(Guid.Empty, prevEgo.transform);
            }

            if (newEgo != null)
            {
                AddMapIndicatorTarget?.Invoke(
                    newEgo.transform,
                    Guid.Empty,
                    _myNavIndicator,
                    false,
                    newEgo.GetComponent<ICharacterPawn>()?.CameraGuideProvider);
            }
        }

        private void OnTrackedQuestPoiListChanged(PointOfInterestDef[] poiDefs)
        {
            for (int i = 0; i < PointOfInterestNotifier.Notifiers.Count; i++)
            {
                var notifier = PointOfInterestNotifier.Notifiers[i];
                notifier.Active = poiDefs != null && poiDefs.Contains(notifier.Identifier);
            }
        }
    }
}