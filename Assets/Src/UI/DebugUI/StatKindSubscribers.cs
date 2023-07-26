using System.Collections.Generic;
using System.Linq;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ContainerApis;
using EnumerableExtensions;
using UnityEngine;

namespace Uins
{
    public class StatKindSubscribers
    {
        private StatKind _statKind;
        private StatDebugViewModel _statDebugViewModelPrefab;
        private Transform _vmRoot;

        public readonly Dictionary<StatResource, StatDebugViewModel> StatDebugViewModels =
            new Dictionary<StatResource, StatDebugViewModel>();


        //=== Props ===========================================================

        public bool IsSubscribed { get; private set; }

        private string _filter = "";

        public string Filter
        {
            get => _filter;
            set
            {
                if (value != _filter)
                {
                    _filter = value;
                    OnFilterChanged();
                }
            }
        }

        private bool IsInited => _statKind != StatKind.None && _statDebugViewModelPrefab != null && _vmRoot != null;


        //=== Public ==========================================================

        public StatKindSubscribers(StatKind statKind, StatDebugViewModel statDebugViewModelPrefab, Transform vmRoot)
        {
            _statKind = statKind;
            _statDebugViewModelPrefab = statDebugViewModelPrefab;
            _vmRoot = vmRoot;
        }


        //=== Public ==========================================================

        public void OnActivityChanged(bool isActive, HasStatsBroadcastApi hasStatsBroadcastApi, HasStatsFullApi hasStatsFullApi)
        {
            if (hasStatsBroadcastApi.AssertIfNull(nameof(hasStatsBroadcastApi)) ||
                hasStatsFullApi.AssertIfNull(nameof(hasStatsFullApi)))
                return;

            if (isActive)
                OnShow(hasStatsBroadcastApi, hasStatsFullApi);
            else
                OnHide(hasStatsBroadcastApi, hasStatsFullApi);
        }


        //=== Private =========================================================

        private void OnShow(HasStatsBroadcastApi hasStatsBroadcastApi, HasStatsFullApi hasStatsFullApi)
        {
            if (IsSubscribed || !IsInited)
                return;

            var statResources = hasStatsBroadcastApi.GetStatResourcesOfStatKind(_statKind);
            statResources.ForEach(statResource => GetOrCreateStatDebugViewModel(statResource).Subscribe(statResource, hasStatsBroadcastApi, true));

            statResources = hasStatsFullApi.GetStatResourcesOfStatKind(_statKind);
            statResources.ForEach(statResource => GetOrCreateStatDebugViewModel(statResource).Subscribe(statResource, hasStatsFullApi, false));
            IsSubscribed = true;
        }

        private void OnHide(HasStatsBroadcastApi hasStatsBroadcastApi, HasStatsFullApi hasStatsFullApi)
        {
            if (!IsSubscribed || !IsInited)
                return;

            StatDebugViewModels.Values.Where(vm => vm.IsSubscribed).ForEach(vm =>
            {
                if (vm.IsBroadcast)
                    vm.Unsubscribe(hasStatsBroadcastApi);
                else
                    vm.Unsubscribe(hasStatsFullApi);
            });
            IsSubscribed = false;
        }

        private void OnFilterChanged()
        {
            StatDebugViewModels.Values.ForEach(vm => vm.OnFilterChanged(Filter));
        }

        private StatDebugViewModel GetOrCreateStatDebugViewModel(StatResource statResource)
        {
            if (statResource.AssertIfNull(nameof(statResource)))
                return null;

            if (!StatDebugViewModels.TryGetValue(statResource, out var vm))
            {
                vm = Object.Instantiate(_statDebugViewModelPrefab, _vmRoot);
                if (vm.AssertIfNull(nameof(vm)))
                    return null;

                StatDebugViewModels.Add(statResource, vm);
                vm.name = statResource.____GetDebugRootName();

                ItemsSorting.SortSiblings(StatDebugViewModels.OrderBy(sdvm => sdvm.Key.DebugName).Select(kvp => kvp.Value).ToList());
            }

            return vm;
        }
    }
}