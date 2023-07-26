using System;
using System.ComponentModel;
using System.Linq;
using Assets.Src.Aspects.Impl.Factions.Template;
using ColonyShared.SharedCode.Aspects.Counters.Template;
using Core.Environment.Logging.Extension;
using L10n;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class ShownQuestViewModel : BindingViewModel
    {
        private const int EmptyTargetPhaseIndex = -2;


        //=== Props ===========================================================

        private QuestItemViewModel _target;

        [Binding]
        public QuestItemViewModel Target
        {
            get => _target;
            set
            {
                if (_target != value)
                {
                    if (_target != null)
                        _target.PropertyChanged -= OnCurrentTargetPropertyChanged;

                    var oldHasTarget = HasTarget;
                    _target = value;
                    NotifyPropertyChanged();
                    if (oldHasTarget != HasTarget)
                        NotifyPropertyChanged(nameof(HasTarget));
                    OnCurrentTargetChanged();
                }
            }
        }

        [Binding]
        public bool HasTarget => Target != null;

        private QuestDef _questDef;

        public QuestDef QuestDef
        {
            get => _questDef;
            private set
            {
                if (_questDef != value)
                {
                    var oldIsDone = IsDone;
                    _questDef = value;
                    Rewards = GetRewards();
                    NotifyPropertyChanged();
                    if (oldIsDone != IsDone)
                        NotifyPropertyChanged(nameof(IsDone));
                }
            }
        }

        private LocalizedString _name;

        [Binding]
        public LocalizedString Name
        {
            get => _name;
            private set
            {
                if (!_name.Equals(value))
                {
                    _name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _hasNavPoint;

        [Binding]
        public bool HasNavPoint
        {
            get => _hasNavPoint;
            private set
            {
                if (_hasNavPoint != value)
                {
                    _hasNavPoint = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private float _distToNavPoint;

        [Binding]
        public float DistToNavPoint
        {
            get => _distToNavPoint;
            private set
            {
                if (!Mathf.Approximately(_distToNavPoint, value))
                {
                    _distToNavPoint = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _phaseIndex = EmptyTargetPhaseIndex;

        [Binding]
        public int PhaseIndex
        {
            get => _phaseIndex;
            set
            {
                if (_phaseIndex != value)
                {
                    var oldIsDone = IsDone;
                    _phaseIndex = value;
                    CurrentPhaseDescr = GetCurrentPhaseDescr();
                    NotifyPropertyChanged();
                    if (oldIsDone != IsDone)
                        NotifyPropertyChanged(nameof(IsDone));
                }
            }
        }

        public int CorrectLastPhaseIndex => Target == null ? -1 : (IsDone ? Target.PhasesLength - 1 : Target.PhaseIndex);

        private LocalizedString _currentPhaseDescr;

        [Binding]
        public LocalizedString CurrentPhaseDescr
        {
            get => _currentPhaseDescr;
            set
            {
                if (!_currentPhaseDescr.Equals(value))
                {
                    _currentPhaseDescr = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isTracked;

        [Binding]
        public bool IsTracked
        {
            get => _isTracked;
            set
            {
                if (_isTracked != value)
                {
                    _isTracked = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _trackedIndex;

        [Binding]
        public int TrackedIndex
        {
            get => _trackedIndex;
            set
            {
                if (_trackedIndex != value)
                {
                    _trackedIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _stateIndex;

        [Binding]
        public int StateIndex
        {
            get => _stateIndex;
            set
            {
                if (_stateIndex != value)
                {
                    _stateIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isDone;

        [Binding]
        public bool IsDone => QuestItemViewModel.GetQuestIsDone(QuestDef, PhaseIndex);

        [Binding]
        public bool HasRewards => Rewards != null && Rewards.Length > 0;

        private QuestRewardData[] _rewards;

        public QuestRewardData[] Rewards
        {
            get => _rewards;
            private set
            {
                if (_rewards != value)
                {
                    var oldHasRewards = HasRewards;
                    _rewards = value;
                    NotifyPropertyChanged();
                    if (oldHasRewards != HasRewards)
                        NotifyPropertyChanged(nameof(HasRewards));
                }
            }
        }


        //=== Private =========================================================

        private void OnCurrentTargetChanged()
        {
            if (Target != null)
                Target.PropertyChanged += OnCurrentTargetPropertyChanged;

            Name = GetName();
            var newQuestDef = Target?.QuestDef;
            var newPhaseIndex = GetPhaseIndex();
            if (newQuestDef != null && QuestDef != null && newQuestDef != QuestDef && newPhaseIndex == PhaseIndex)
            {
                PhaseIndex = EmptyTargetPhaseIndex; //сбрасываем чтобы зарегистрировала изменение
            }

            QuestDef = newQuestDef;
            PhaseIndex = newPhaseIndex;
            IsTracked = GetIsTracked();
            DistToNavPoint = GetDistToNavPoint();
            HasNavPoint = GetHasNavPoint();
            TrackedIndex = GetTrackedIndex();
            StateIndex = GetStateIndex();
            CurrentPhaseDescr = GetCurrentPhaseDescr();
        }

        private const string DistToNavPointPropName = nameof(QuestItemViewModel.DistToNavPoint);
        private const string HasNavPointPropName = nameof(QuestItemViewModel.HasNavPoint);
        private const string PhaseIndexPropName = nameof(QuestItemViewModel.PhaseIndex);
        private const string IsTrackedPropName = nameof(QuestItemViewModel.IsTracked);
        private const string TrackedIndexPropName = nameof(QuestItemViewModel.TrackedIndex);
        private const string StateIndexPropName = nameof(QuestItemViewModel.StateIndex);

        private void OnCurrentTargetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propName = e.PropertyName;
            switch (propName)
            {
                case DistToNavPointPropName:
                    DistToNavPoint = GetDistToNavPoint();
                    break;

                case HasNavPointPropName:
                    HasNavPoint = GetHasNavPoint();
                    break;

                case PhaseIndexPropName:
                    PhaseIndex = GetPhaseIndex();
                    break;

                case IsTrackedPropName:
                    IsTracked = GetIsTracked();
                    break;

                case TrackedIndexPropName:
                    TrackedIndex = GetTrackedIndex();
                    break;

                case StateIndexPropName:
                    StateIndex = GetStateIndex();
                    break;
            }
        }

        private int GetStateIndex()
        {
            return Target?.StateIndex ?? 0;
        }

        private int GetTrackedIndex()
        {
            return Target?.TrackedIndex ?? 2;
        }

        private bool GetIsTracked()
        {
            return Target?.IsTracked ?? false;
        }

        private int GetPhaseIndex()
        {
            return Target?.PhaseIndex ?? EmptyTargetPhaseIndex;
        }

        private float GetDistToNavPoint()
        {
            return Target?.DistToNavPoint ?? 0;
        }

        private bool GetHasNavPoint()
        {
            return Target?.HasNavPoint ?? false;
        }

        private LocalizedString GetName()
        {
            if (Target == null)
                return LsExtensions.Empty;

            return Target.Name.IsEmpty() ? LsExtensions.EmptyWarning : Target.Name;
        }

        private LocalizedString GetCurrentPhaseDescr()
        {
            if (CorrectLastPhaseIndex < 0)
                return LsExtensions.EmptyWarning;

            var descriptionLs = Target.QuestDef.Phases[CorrectLastPhaseIndex].Target.DescriptionLs;
            return descriptionLs.IsEmpty() ? LsExtensions.EmptyWarning : descriptionLs;
        }

        private QuestRewardData[] GetRewards()
        {
            if (Target == null || Target.QuestDef == null)
                return null;

            var phases = Target.QuestDef.Phases;
            var lastPhaseImpacts = phases[phases.Length - 1].Target?.OnSuccess;
            if (lastPhaseImpacts == null || lastPhaseImpacts.Length == 0)
                return null;

            var rewardSources = lastPhaseImpacts.Select(sidr => sidr.Target as IRewardSource)
                .Where(mirs => mirs != null);

            var manyRewardSources = lastPhaseImpacts.Select(sidr => sidr.Target as IManyRewardsSource)
                .Where(mirs => mirs != null);

            return rewardSources.Concat(manyRewardSources.SelectMany(mrs => mrs.Rewards))
                .Select(GetQuestRewardData).Where(qrd => qrd != null)
                .ToArray();
        }

        private QuestRewardData GetQuestRewardData(IRewardSource rewardSource)
        {
            if (rewardSource == null)
                return null;

            var questRewardData = new QuestRewardData();
            var itemRewardSource = rewardSource as IItemRewardSource;
            if (itemRewardSource != null)
            {
                questRewardData.RewardType = QuestRewardType.Item;
                questRewardData.Item = itemRewardSource.Item;
                questRewardData.Count = itemRewardSource.Count;
                return questRewardData;
            }

            var recipeRewardSource = rewardSource as IRecipeRewardSource;
            if (recipeRewardSource != null)
            {
                questRewardData.RewardType = QuestRewardType.Recipe;
                questRewardData.Recipe = recipeRewardSource.Recipe;
                return questRewardData;
            }

            var scienceRewardSource = rewardSource as IScienceRewardSource;
            if (scienceRewardSource != null)
            {
                questRewardData.RewardType = QuestRewardType.Science;
                questRewardData.Science = scienceRewardSource.Science;
                questRewardData.Count = scienceRewardSource.Count;
                return questRewardData;
            }

            var techPointRewardSource = rewardSource as ITechPointRewardSource;
            if (techPointRewardSource != null)
            {
                questRewardData.RewardType = QuestRewardType.TechPoint;
                questRewardData.TechPoint = techPointRewardSource.TechPoint;
                questRewardData.Count = techPointRewardSource.Count;
                return questRewardData;
            }

            UI.Logger.IfError()?.Message($"Unhadled IRewardSource subtype: {rewardSource.GetType()}").Write();
            return null;
        }
    }
}