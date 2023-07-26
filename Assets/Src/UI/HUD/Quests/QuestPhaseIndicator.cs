using System.ComponentModel;
using Assets.Src.Inventory;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class QuestPhaseIndicator : BindingViewModel
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private static int _nextFreeId = 0;
        private int Id;

        private IQuestTrackingContext _questTrackingContext;

        [SerializeField, UsedImplicitly]
        private ShownQuestViewModel _shownQuestViewModel;

        [SerializeField, UsedImplicitly]
        private QuestPhaseViewModel _questPhaseViewModelPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _phaseRootTransform;

        private QuestPhaseViewModel _questPhaseViewModel;

        //=== Props ===========================================================

        private bool _isVisible = true;

        [Binding]
        public bool IsVisible
        {
            get => _isVisible;
            private set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            Id = ++_nextFreeId;
            _shownQuestViewModel.AssertIfNull(nameof(_shownQuestViewModel));
            _questPhaseViewModelPrefab.AssertIfNull(nameof(_questPhaseViewModelPrefab));
            _phaseRootTransform.AssertIfNull(nameof(_phaseRootTransform));
        }


        //=== Public ==========================================================

        public void Init(IQuestTrackingContext questTrackingContext)
        {
            if (questTrackingContext.AssertIfNull(nameof(questTrackingContext)))
                return;

            _shownQuestViewModel.PropertyChanged += OnShownQuestViewModelPropertyChanged;
            _questTrackingContext = questTrackingContext;
            _questTrackingContext.TrackedQuestRp.Action(D, OnTrackedQuestChanged);
            CreatePhase();
        }


        //=== Private =========================================================

        private void OnShownQuestViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var propertyName = args.PropertyName;

            if (propertyName == nameof(ShownQuestViewModel.PhaseIndex))
            {
                _questPhaseViewModel.Fill(GetQuestCurrentPhaseData());
            }
        }

        private QuestPhaseData GetQuestCurrentPhaseData()
        {
            // PZ-15197 // Logger.IfError()?.Message($"$$$$$$$$$$ _questTrackingContext.TrackedQuestRp = {_questTrackingContext.TrackedQuestRp}; Value = {_questTrackingContext.TrackedQuestRp.Value}" ).Write();

            var questPhaseData = new QuestPhaseData(
                _questTrackingContext
                    .TrackedQuestRp
                    // PZ-15197 // .LogError(D, $"$$$$$$$$$$ GetQuestCurrentPhaseData()", toString: trackedQuest => $"quest:{trackedQuest};\n state:{trackedQuest?.questState};\n react:{trackedQuest?.questState?.QuestReactive}\n reactState:{trackedQuest?.questState?.QuestReactive.StreamState()}", logger: Logger)
                    .SubStream(D, trackedQuest => trackedQuest.questState.QuestReactive)
                    // PZ-15197 // .LogError(D, $"$$$$$$$$$$ QuestReactive = ")
            )
            {
                IsCurrent = true,
                IsDelimiterHidden = true
            };

            var target = _shownQuestViewModel.Target;
            if (target == null)
                return questPhaseData;

            if (target.IsDone)
            {
                questPhaseData.IsDone = true;
            }
            else
            {
                questPhaseData.PhaseDef = target.QuestDef.Phases[target.PhaseIndex].Target;
            }

            return questPhaseData;
        }

        private void CreatePhase()
        {
            _questPhaseViewModel = Instantiate(_questPhaseViewModelPrefab, _phaseRootTransform);
            if (_questPhaseViewModel.AssertIfNull(nameof(_questPhaseViewModel)))
                return;

            _questPhaseViewModel.name = _questPhaseViewModelPrefab.name;
        }

        private void OnTrackedQuestChanged(QuestItemViewModel questItemViewModel)
        {
            // PZ-15197 // Logger.IfError()?.Message($"$$$$$$$$$ QuestPhaseIndicator.OnTrackedQuestChanged({questItemViewModel}) // {questItemViewModel?.questState?.QuestReactive} => {questItemViewModel?.questState?.QuestReactive.StreamState()}").Write();
            _shownQuestViewModel.Target = questItemViewModel;
            IsVisible = _shownQuestViewModel.Target != null;

            // PZ-15197 // if (questItemViewModel != null && questItemViewModel.questState != null && questItemViewModel.questState.QuestReactive != null)
            // PZ-15197 //     questItemViewModel.questState.QuestReactive.Action(D, qsr =>
            // PZ-15197 //         Logger.IfError()?.Message($"$$$$$$$$$$ questItemViewModel.questState.QuestReactive.Action({questItemViewModel})")).Write();
        }
    }
}