using System.Collections.Generic;
using Assets.Src.Aspects.Impl.Stats;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class StatsViewModel : BindingViewModel
    {
        private const int MaxMainStatsCount = 2;
        private const int MaxRegularStatsCount = 20;

        [SerializeField, UsedImplicitly]
        private StatViewModel _mainStatViewModelPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _mainStatsTransform;

        [SerializeField, UsedImplicitly]
        private StatViewModel _regularStatViewModelPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _regularStatsTransform;

        private List<StatViewModel> _mainStatViewModels;
        private List<StatViewModel> _regularStatViewModels;


        //=== Props ===========================================================

        private bool _hasMainStats;

        [Binding]
        public bool HasMainStats
        {
            get => _hasMainStats;
            set
            {
                if (_hasMainStats != value)
                {
                    _hasMainStats = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _hasRegularStats;

        [Binding]
        public bool HasRegularStats
        {
            get => _hasRegularStats;
            set
            {
                if (_hasRegularStats != value)
                {
                    _hasRegularStats = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            _mainStatViewModelPrefab.AssertIfNull(nameof(_mainStatViewModelPrefab));
            _mainStatsTransform.AssertIfNull(nameof(_mainStatsTransform));
            _regularStatViewModelPrefab.AssertIfNull(nameof(_regularStatViewModelPrefab));
            _regularStatsTransform.AssertIfNull(nameof(_regularStatsTransform));
            OnAwake();
        }


        //=== Public ==========================================================

        public void SetItemStats(List<KeyValuePair<StatResource, float>> mainStats, List<KeyValuePair<StatResource, float>> regularStats)
        {
            //UI.CallerLog($"{mainStats.VarDump(nameof(mainStats))}\n{regularStats.VarDump(nameof(regularStats))}"); //DEBUG
            OnAwake();
            SetStats(mainStats, _mainStatViewModels);
            HasMainStats = mainStats.Count > 0;
            SetStats(regularStats, _regularStatViewModels);
            HasRegularStats = regularStats.Count > 0;
        }


        //=== Private =========================================================

        private void OnAwake()
        {
            if (_mainStatViewModels != null)
                return;

            CreateStats(MaxMainStatsCount, _mainStatViewModelPrefab, _mainStatsTransform, out _mainStatViewModels);
            CreateStats(MaxRegularStatsCount, _regularStatViewModelPrefab, _regularStatsTransform, out _regularStatViewModels);
        }

        private void CreateStats(int maxStatsCount, StatViewModel statViewModelPrefab, Transform statsParent, out List<StatViewModel> statsList)
        {
            statsList = new List<StatViewModel>();
            for (int i = 0; i < maxStatsCount; i++)
            {
                var statViewModel = Instantiate(statViewModelPrefab, statsParent);
                if (statViewModel.AssertIfNull(nameof(statViewModel)))
                    return;

                statViewModel.name = $"{nameof(StatViewModel)}{statsList.Count}";
                statViewModel.gameObject.SetActive(false);
                statsList.Add(statViewModel);
            }
        }

        private void SetStats(List<KeyValuePair<StatResource, float>> stats, List<StatViewModel> statViewModels)
        {
            if (stats.AssertIfNull(nameof(stats)) ||
                statViewModels.AssertIfNull(nameof(statViewModels)))
                return;

            int viewModelsCount = statViewModels.Count;
            for (int i = 0; i < viewModelsCount; i++)
            {
                var statViewModel = statViewModels[i];
                statViewModel.gameObject.SetActive(i < stats.Count);
                if (i < stats.Count)
                {
                    var kvp = stats[i];
                    statViewModel.SetStatResourceAndValue(kvp.Key, kvp.Value);
                }
            }

            if (stats.Count > viewModelsCount)
                UI.Logger.IfWarn()?.Message($"Too much stats ({stats.Count}) for reserved spaces ({viewModelsCount})").Write();
        }
    }
}