using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Uins.Inventory;
using UnityEngine;
using UnityWeld.Binding;
using Assets.Src.Aspects.Impl.Factions.Template;
using L10n;
using SharedCode.Wizardry;
using EnumerableExtensions;
using ReactivePropsNs;

namespace Uins
{
    /// <summary>
    /// Модель итспользуется не только в индиктаторе, но и в окне квестов. Придётся постараться использовать её, а не плодить нувую.
    /// </summary>

    [Binding]
    public class QuestItemViewModel : BindingViewModel, IContextViewTarget
    {
        private IContextView _contextView;
        private IQuestTrackingContext _questTrackingContext;


        //=== Props ===========================================================

        public QuestGroup Group => QuestDef?.Group ?? QuestGroup.Main;


        private QuestDef _questDef;
        public QuestState questState;

        [Binding]
        public QuestDef QuestDef
        {
            get => _questDef;
            private set
            {
                if (_questDef != value)
                {
                    _questDef = value;
                    NotifyPropertyChanged(nameof(Name));
                    NotifyPropertyChanged(nameof(ShortDescr));
                    NotifyPropertyChanged(nameof(IsSelected));
                    NotifyPropertyChanged(nameof(IsSelectedEvaluated));
                }
            }
        }

        [Binding]
        public LocalizedString Name => QuestDef?.NameLs ?? LsExtensions.Empty;

        [Binding]
        public LocalizedString ShortDescr => QuestDef?.ShortDescriptionLs ?? LsExtensions.Empty;

        private bool _isSelected;

        [Binding]
        public bool IsSelected
        {
            get => _isSelected;
            private set
            {
                if (_isSelected != value)
                {
                    var oldTrackedIndex = TrackedIndex;
                    var oldIsSelectedEvaluated = IsSelectedEvaluated;
                    _isSelected = value;
                    NotifyPropertyChanged();
                    if (oldTrackedIndex != TrackedIndex)
                        NotifyPropertyChanged(nameof(TrackedIndex));
                    if (oldIsSelectedEvaluated != IsSelectedEvaluated)
                        NotifyPropertyChanged(nameof(IsSelectedEvaluated));
                }
            }
        }

        private int _phaseIndex = -2; //srv

        [Binding]
        public int PhaseIndex
        {
            get => _phaseIndex;
            set
            {
                if (_phaseIndex != value)
                {
                    var oldStateIndex = StateIndex;
                    var oldTrackedIndex = TrackedIndex;
                    var oldIsSelectedEvaluated = IsSelectedEvaluated;
                    _phaseIndex = value;
                    IsDone = GetQuestIsDone(QuestDef, _phaseIndex);
                    PoiDefs = GetPoiDefs();
                    NotifyPropertyChanged();
                    if (oldStateIndex != StateIndex)
                        NotifyPropertyChanged(nameof(StateIndex));
                    if (oldTrackedIndex != TrackedIndex)
                        NotifyPropertyChanged(nameof(TrackedIndex));
                    if (oldIsSelectedEvaluated != IsSelectedEvaluated)
                        NotifyPropertyChanged(nameof(IsSelectedEvaluated));
                }
            }
        }

        public int PhasesLength => QuestDef?.Phases?.Length ?? 0;

        private bool _isTracked; //srv

        [Binding]
        public bool IsTracked
        {
            get => _isTracked;
            set
            {
                if (_isTracked != value)
                {
                    var oldStateIndex = StateIndex;
                    var oldTrackedIndex = TrackedIndex;
                    _isTracked = value;
                    NotifyPropertyChanged();
                    if (oldStateIndex != StateIndex)
                        NotifyPropertyChanged(nameof(StateIndex));
                    if (oldTrackedIndex != TrackedIndex)
                        NotifyPropertyChanged(nameof(TrackedIndex));
                }
            }
        }

        [Binding]
        public bool IsSelectedEvaluated => IsSelected && !IsDone;

        private bool _isDone;

        [Binding]
        public bool IsDone
        {
            get => _isDone;
            private set
            {
                if (_isDone != value)
                {
                    var oldStateIndex = StateIndex;
                    _isDone = value;
                    NotifyPropertyChanged();
                    if (oldStateIndex != StateIndex)
                        NotifyPropertyChanged(nameof(StateIndex));
                    if (IsDone && IsTracked)
                        _questTrackingContext.TrackedQuestRp.Value = null;
                }
            }
        }

        /// <summary>
        /// 0 - undone, untracked, 1 - undone, tracked, 2 - done
        /// </summary>
        [Binding]
        public int StateIndex => IsDone ? 2 : (IsTracked ? 1 : 0);

        /// <summary>
        /// 0 [semi eye] - selected, untracked, 1 [full eye] - tracked; 2 [none] - done OR (unselected, untracked) 
        /// </summary>
        [Binding]
        public int TrackedIndex => IsDone || (!IsTracked && !IsSelected) ? 2 : (IsTracked ? 1 : 0);

        private PointOfInterestDef[] _poiDefs;

        [Binding] //для отслеживания в QuestContextView
        public PointOfInterestDef[] PoiDefs
        {
            get => _poiDefs;
            private set
            {
                if (!_poiDefs.NullableSequenceEqual(value))
                {
                    var oldHasNavPoint = HasNavPoint;
                    _poiDefs = value;
                    NotifyPropertyChanged();
                    if (oldHasNavPoint != HasNavPoint)
                        NotifyPropertyChanged(nameof(HasNavPoint));
                }
            }
        }

        [Binding]
        public bool HasNavPoint => (PoiDefs?.Length ?? 0) > 0;

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

        public PhaseDef CurrentPhase => (QuestDef == null || IsDone) ? null : QuestDef.Phases[PhaseIndex].Target;

        //=== Public ==========================================================

        public void Init(IContextView contextView, IQuestTrackingContext questTrackingContext, QuestState questState)
        {
            if (questState.AssertIfNull(nameof(questState)))
                return;

            this.questState = questState;
            _contextView = contextView;
            _questTrackingContext = questTrackingContext;
            if (!_contextView.AssertIfNull(nameof(_contextView)))
                _contextView.CurrentContext.Action(D, target => IsSelected = target == this);
            if (!_questTrackingContext.AssertIfNull(nameof(_questTrackingContext)))
            {
                _questTrackingContext.TrackedQuestRp.Action(D, OnTrackedQuestChanged);
            }

            QuestDef = questState.QuestDef;
            PhaseIndex = questState.PhaseIndex;
        }

        public void UpdateState(QuestState questState)
        {
            this.questState = questState;
            PhaseIndex = questState.PhaseIndex;
        }

        public void OnClick()
        {
            if (_contextView.AssertIfNull(nameof(_contextView)) ||
                _contextView.ContextValue == this)
                return;

            _contextView?.TakeContext(this);
        }

        public void UpdateDist(Transform pawnTransform)
        {
            if (!IsDone && HasNavPoint)
                DistToNavPoint = PointOfInterestNotifier.GetMinDistanceFromTargetToNotifier(PoiDefs, pawnTransform);
        }

        public override string ToString()
        {
            return $"[{GetType()}: {Group}, QuestDef={QuestDef}, Phase={PhaseIndex}/{PhasesLength - 1}, " +
                   $"{nameof(IsSelected)}{IsSelected.AsSign()}, {nameof(IsTracked)}{IsTracked.AsSign()}, " +
                   $"{nameof(IsDone)}{IsDone.AsSign()}, {nameof(HasNavPoint)}{HasNavPoint.AsSign()}," +
                   $"{nameof(PoiDefs)}={PoiDefs.ItemsToString()}]";
        }

        public static bool GetQuestIsDone(QuestDef questDef, int phaseIndex)
        {
            return phaseIndex >= (questDef?.Phases?.Length ?? 0) || phaseIndex < 0;
        }


        //=== Private =========================================================

        private PointOfInterestDef[] GetPoiDefs()
        {
            if (QuestDef == null || IsDone || CurrentPhase?.OnStart == null)
                return null;

            return CurrentPhase.OnStart
                .Where(imp => (imp.Target as IPointOfInterestDefSource)?.PoiDef != null)
                .Select(imp => ((IPointOfInterestDefSource) imp.Target).PoiDef)
                .ToArray();
        }

        private void OnContextViewTargetChanged(IContextViewTarget target, bool isContextViewBlocked)
        {
            IsSelected = target == this;
        }

        private void OnTrackedQuestChanged(QuestItemViewModel questItemViewModel)
        {
            IsTracked = !IsDone && questItemViewModel == this;
        }
    }
}