using System.ComponentModel;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourceSystem.L10n;
using JetBrains.Annotations;
using L10n;
using NLog;
using ReactivePropsNs;
using Uins.Inventory;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class QuestContextView : BindingViewModel, IContextView, IQuestTrackingContext
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        [SerializeField, UsedImplicitly]
        private ShownQuestViewModel _shownQuestViewModel;

        [SerializeField, UsedImplicitly]
        private QuestProgressDescriptions _questProgressDescriptions;

        [SerializeField, UsedImplicitly]
        private QuestPhases _questPhases;

        [SerializeField, UsedImplicitly]
        private QuestRewards _questRewards;

        [SerializeField, UsedImplicitly]
        private LocalizationKeyPairsDefRef _moreLessDefRef;


        //=== Props ===========================================================

        public ReactiveProperty<QuestItemViewModel> SelectedQuestRp { get; } = new ReactiveProperty<QuestItemViewModel>();

        public ReactiveProperty<QuestItemViewModel> TrackedQuestRp { get; } = new ReactiveProperty<QuestItemViewModel>();

        public bool IsAlreadyTrackedAnyQuest { get; private set; }

        public bool IsNoneTracked => IsAlreadyTrackedAnyQuest && TrackedQuestRp.Value == null;

        public ReactiveProperty<PointOfInterestDef[]> TrackedQuestPoiList { get; } = new ReactiveProperty<PointOfInterestDef[]>();
        //if (!_trackedQuestPoiList.NullableSequenceEqual(value)) //TODOM реализовать в RP сравнение

        public QuestDef LastTrackedQuestDef { get; set; }

        private ReactiveProperty<bool> _isVisibleDetailsRp = new ReactiveProperty<bool>();

        [Binding]
        public bool IsOpen { get; private set; }

        [Binding]
        public LocalizedString OpenButtonText { get; private set; }

        public IStream<IContextViewTarget> CurrentContext => SelectedQuestRp;

        public IContextViewTarget ContextValue => SelectedQuestRp.HasValue ? SelectedQuestRp.Value : null;

        private ReactiveProperty<Sprite> _questMainSpriteRp = new ReactiveProperty<Sprite>();

        [Binding]
        public Sprite QuestMainSprite { get; private set; }

        [Binding]
        public bool HasQuestMainSprite { get; private set; }


        //=== Unity ===========================================================

        private void Start()
        {
            if (!_shownQuestViewModel.AssertIfNull(nameof(_shownQuestViewModel)))
                _shownQuestViewModel.PropertyChanged += OnShownQuestViewModelPropertyChanged;

            _questProgressDescriptions.AssertIfNull(nameof(_questProgressDescriptions));
            _questPhases.AssertIfNull(nameof(_questPhases));
            _questRewards.AssertIfNull(nameof(_questRewards));
            _moreLessDefRef.Target.AssertIfNull(nameof(_moreLessDefRef));

            _isVisibleDetailsRp.Value = false;
            Bind(_isVisibleDetailsRp, () => IsOpen);
            var moreLessTextLsStream = _isVisibleDetailsRp.Func(D, b => b ? _moreLessDefRef.Target.Ls2 : _moreLessDefRef.Target.Ls1); //less/more
            Bind(moreLessTextLsStream, () => OpenButtonText);

            Bind(_questMainSpriteRp, () => QuestMainSprite);
            var hasQuestMainSpriteStream = _questMainSpriteRp.Func(D, spr => spr != null);
            Bind(hasQuestMainSpriteStream, () => HasQuestMainSprite);

            SelectedQuestRp.Value = null;
            TrackedQuestRp.Value = null;

            SelectedQuestRp.Action(D, questItemViewModel => _isVisibleDetailsRp.Value = false);
            SelectedQuestRp.Action(D, questItemViewModel => _shownQuestViewModel.Target = questItemViewModel);

            TrackedQuestRp.Action(D, vm =>
            {
                if (vm != null)
                    IsAlreadyTrackedAnyQuest = true;
            });
            var prevAndCurrentTrackedQuest = TrackedQuestRp.PrevAndCurrent(D);
            prevAndCurrentTrackedQuest.Action(D, (prev, curr) =>
            {
                if (prev != null)
                    UnsubscribeFromTrackedQuestItemViewModel(prev);

                if (curr != null)
                {
                    SubscribeToTrackedQuestItemViewModel(curr);
                    LastTrackedQuestDef = curr.QuestDef;
                }
            });

            var removedQuestPoiList = RemovedQuestPoiList.Instance;
            if (!removedQuestPoiList.AssertIfNull(nameof(removedQuestPoiList)))
            {
                prevAndCurrentTrackedQuest
                    .Zip(D, removedQuestPoiList.RemovedQuestPoints.CountStream)
                    .Action(D,
                        (prev, curr, count) =>
                        {
                            TrackedQuestPoiList.Value = curr != null
                                ? GetTrackedQuestPoiList(curr, RemovedQuestPoiList.Instance.RemovedQuestPoints)
                                : null;
                        });
            }
        }


        //=== Public ==========================================================

        [UsedImplicitly]
        public void OnOpenChange()
        {
            _isVisibleDetailsRp.Value = !_isVisibleDetailsRp.Value;
        }

        public void TakeContext(IContextViewTarget contextViewTarget)
        {
            SelectedQuestRp.Value = contextViewTarget as QuestItemViewModel;
        }


        //=== Private =========================================================

        private void OnShownQuestViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propName = e.PropertyName;
            switch (propName)
            {
                case nameof(ShownQuestViewModel.PhaseIndex):
                    UpdatePhases();
                    break;

                case nameof(ShownQuestViewModel.QuestDef):
                    _questMainSpriteRp.Value = _shownQuestViewModel.QuestDef?.Image?.Target;
                    UpdatePhases();
                    UpdateRewards();
                    break;
            }
        }

        private void UpdatePhases()
        {
            if (_shownQuestViewModel.CorrectLastPhaseIndex < 0)
            {
                _questProgressDescriptions.ClearCollection();
                _questPhases.ClearCollection();
            }
            else
            {
                _questProgressDescriptions.FillCollection(GetAllDescriptionsUpToPhase(_shownQuestViewModel.CorrectLastPhaseIndex));
                _questPhases.FillCollection(GetPhases(_shownQuestViewModel.CorrectLastPhaseIndex, _shownQuestViewModel.IsDone));
            }
        }

        private void UpdateRewards()
        {
            _questRewards.FillCollection(_shownQuestViewModel.Rewards);
        }

        private LocalizedStringContainer[] GetAllDescriptionsUpToPhase(int phaseIndex)
        {
            if (_shownQuestViewModel.Target == null)
                return null;

            var descrs = new LocalizedStringContainer[phaseIndex + 1];
            for (int i = 0; i < descrs.Length; i++)
            {
                var descriptionLs = LsExtensions.Empty;
                if (_shownQuestViewModel.Target.QuestDef.Phases[i].Target != null)
                {
                    descriptionLs = _shownQuestViewModel.Target.QuestDef.Phases[i].Target.DescriptionLs;
                    if (descriptionLs.IsEmpty())
                        descriptionLs = LsExtensions.EmptyWarning;
                }

                descrs[i] = new LocalizedStringContainer(descriptionLs);
            }

            return descrs;
        }

        private QuestPhaseData[] GetPhases(int lastPhaseIndex, bool isDone)
        {
            if (_shownQuestViewModel.Target == null || _shownQuestViewModel.Target.QuestDef == null)
                return null;

            // PZ-15197 // Logger.IfError()?.Message($"$$$$$$$$$$ GetPhases({lastPhaseIndex}, {isDone})").Write();

            var questStateReactiveSource = _shownQuestViewModel.Target.questState.QuestReactive;

            var phases = new QuestPhaseData[lastPhaseIndex + 1];
            for (int i = 0; i < phases.Length; i++)
            {
                // PZ-15197 // Logger.IfError()?.Message($"$$$$$$$$$$ phases[{i}]").Write();
                phases[i] = new QuestPhaseData(questStateReactiveSource)
                {
                    PhaseDef = _shownQuestViewModel.Target.QuestDef.Phases[i].Target,
                    IsCurrent = !isDone && lastPhaseIndex == i,
                    IsDone = isDone || lastPhaseIndex > i,
                    IsDelimiterHidden = i == 0
                };
            }

            return phases;
        }

        private void SubscribeToTrackedQuestItemViewModel(QuestItemViewModel trackedQuest)
        {
            if (trackedQuest != null)
            {
                trackedQuest.PropertyChanged += OnTrackedQuestItemViewModelPropertyChanged;
            }
        }

        private void UnsubscribeFromTrackedQuestItemViewModel(QuestItemViewModel trackedQuest)
        {
            trackedQuest.PropertyChanged -= OnTrackedQuestItemViewModelPropertyChanged;
        }

        private void OnTrackedQuestItemViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propName = e.PropertyName;

            switch (propName)
            {
                case nameof(QuestItemViewModel.PoiDefs):
                    TrackedQuestPoiList.Value = GetTrackedQuestPoiList(TrackedQuestRp.Value, RemovedQuestPoiList.Instance.RemovedQuestPoints);
                    break;
            }
        }

        private PointOfInterestDef[] GetTrackedQuestPoiList(QuestItemViewModel questItemViewModel, ListStream<PointOfInterestDef> removedQuestPoints)
        {
            if (questItemViewModel.AssertIfNull(nameof(questItemViewModel)) ||
                questItemViewModel.PoiDefs == null)
                return null;

            return questItemViewModel.PoiDefs.Where(poi => !removedQuestPoints.Contains(poi)).ToArray();
        }
    }
}