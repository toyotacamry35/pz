using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Assets.Src.Inventory;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using ProcessSourceNamespace;
using SharedCode.Entities;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public abstract class InteractionIndicator : BindingViewModel
    {
        public ProgressPanelBase ProgressPanelBase;
        public PanelAppearing ProgressPanelAppearing;
        public float ProgressPanelResourceMoveTime = 0.3f;
        public float DefaultProgressDuration = 0.5f;
        public BreakingExtractionIndicatorPanel BreakingPanelPrefab;
        public ExtractedResourcesPanel ExtractedResourcesPanelPrefab;

        protected ExtractedResourcesPanel ExtractedResourcesPanel;
        protected BreakingExtractionIndicatorPanel BreakingPanel;

        protected LinearRelation CurrentProgressRelation;


        //=== Enums ===============================================================

        public enum EndType
        {
            None,
            SuccessEnding,
            CancelEnding,
            FailEnding,
        }

        public enum IndicatorVisualStage
        {
            NeedForAppearing,
            Appearing,
            Appeared,
            Disappearing,
            Disappeared
        }


        //=== Props ===============================================================

        //public IInventoryStats InventoryStats { get; set; }

        public IndicatorVisualStage VisualStage { get; protected set; }

        public EndType Ending { get; protected set; }

        public IProcessSource ProcessSource { get; protected set; }

        public float AlwaysCorrectProgressDuration => ProcessSource == null || ProcessSource.ProgressDuration < 0
            ? DefaultProgressDuration
            : ProcessSource.ProgressDuration;

        private bool _isOpen;

        [Binding]
        public bool IsOpen
        {
            get
            {
                var newIsOpen = Ending == EndType.None;
                if (newIsOpen != _isOpen)
                {
                    _isOpen = newIsOpen;
                    NotifyPropertyChanged();
                }

                return _isOpen;
            }
        }

        private float _currentProgress;

        [Binding]
        public float CurrentProgress
        {
            get { return _currentProgress; }
            protected set
            {
                if (!Mathf.Approximately(_currentProgress, value))
                {
                    _currentProgress = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _firstResourceExpectedCount;

        [Binding]
        public int FirstResourceExpectedCount
        {
            get { return _firstResourceExpectedCount; }
            protected set
            {
                if (_firstResourceExpectedCount != value)
                {
                    _firstResourceExpectedCount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public float LastChangesTime { get; protected set; }

        public List<ItemResourcePack> AchievedItems { get; set; }

        public bool IsEndProgressChanged { get; set; }
        public bool IsStartProgressChanged { get; set; }
        public bool IsProgressDurationChanged { get; set; }
        public bool IsExpectedItemsChanged { get; set; }

        public bool IsChanged => IsEndProgressChanged || IsStartProgressChanged || IsProgressDurationChanged || IsExpectedItemsChanged;


        //=== Unity ===============================================================

        protected virtual void Awake()
        {
            VisualStage = IndicatorVisualStage.NeedForAppearing;
            Ending = EndType.None;
            ProgressPanelBase.AssertIfNull(nameof(ProgressPanelBase));

            if (!ExtractedResourcesPanelPrefab.AssertIfNull(nameof(ExtractedResourcesPanelPrefab)))
            {
                ExtractedResourcesPanel = Instantiate(ExtractedResourcesPanelPrefab, transform);
                ExtractedResourcesPanel.AssertIfNull(nameof(ExtractedResourcesPanel));
            }

            if (!BreakingPanelPrefab.AssertIfNull(nameof(BreakingPanelPrefab)))
            {
                BreakingPanel = Instantiate(BreakingPanelPrefab, transform);
                BreakingPanel.AssertIfNull(nameof(BreakingPanel));
            }
        }


        //=== Public ==============================================================

        public virtual void Init(IProcessSource processSource)
        {
            ProcessSource = processSource;
            IsProgressDurationChanged = IsStartProgressChanged = IsEndProgressChanged = IsExpectedItemsChanged = true;

            ProcessSource.StateChanged += OnStateChanged;
            ProcessSource.ItemsAchieved += OnItemsAchieved;
            ProcessSource.FailOrCancelEnding += OnFailOrCancelEnding;
            FirstResourceExpectedCountUpdate();
        }

        public void InstantHideAndReset()
        {
            VisualStage = IndicatorVisualStage.NeedForAppearing;
            Ending = EndType.None;
            CurrentProgress = 0f;
            FirstResourceExpectedCount = 0;
            ProcessSource = null;
            AchievedItems = null;
            _isOpen = false;
            IsEndProgressChanged = IsStartProgressChanged = IsProgressDurationChanged = IsExpectedItemsChanged = false;
            ProgressPanelBase.HideAndReset();
            ProgressPanelAppearing.HideAndReset();
            BreakingPanel.HideAndReset();
            //ExtractedResourcesPanel.HideAndReset();
        }

        /// <summary>
        /// Общая логика смены визуальной составляющей. Запуская методы Visual_...() ожидаем, что
        /// отработает визуализация состояния и по окончании VisualStage будет изменено на следующую стадию
        /// </summary>
        public void VisualUpdate()
        {
            Visual_AnyUpdate();
            switch (VisualStage)
            {
                case IndicatorVisualStage.NeedForAppearing:
                    TryChangeStage(IndicatorVisualStage.Appearing);
                    ProgressPanelBase.SetupResources(this);
                    ProgressPanelAppearing.Appearing(true, () => VisualStage = IndicatorVisualStage.Appeared);
                    Visual_AppearingStart();
                    break;

                case IndicatorVisualStage.Appearing:
                    if (IsChanged)
                    {
                        if (IsExpectedItemsChanged)
                            ProgressPanelBase.SetupResources(this);
                        Visual_ChangingStart();
                    }

                    break;

                case IndicatorVisualStage.Appeared:
                    //Visual_AppearedUpdate();

                    if (IsChanged)
                    {
                        if (IsExpectedItemsChanged)
                            ProgressPanelBase.SetupResources(this);
                        Visual_ChangingStart();
                    }

                    if (!IsOpen)
                    {
                        TryChangeStage(IndicatorVisualStage.Disappearing);
                        switch (Ending)
                        {
                            case EndType.SuccessEnding:
                                ProgressPanelBase.HideResourcesButOne();
                                ProgressPanelBase.BringOneResourceToCenterAndThenHideIt(ProgressPanelSilentDisappearing);
                                ExtractedResourcesPanel.Appearing(
                                    ProgressPanelResourceMoveTime,
                                    AchievedItems,
                                    () => VisualStage = IndicatorVisualStage.Disappeared);
                                //Visual_SuccessDisappearingStart();
                                break;

                            case EndType.FailEnding:
                                ProgressPanelBase.HideAndReset();
                                BreakingPanel.Appearing(() => VisualStage = IndicatorVisualStage.Disappeared);
                                //Visual_FailDisappearingStart();
                                break;

                            default:
                                ProgressPanelAppearing.Appearing(false, () => VisualStage = IndicatorVisualStage.Disappeared);
                                //Visual_CancelledDisappearingStart();
                                break;
                        }
                    }

                    break;
            }
        }

        public override string ToString()
        {
            var detailIsChanged = "";
            if (IsChanged)
            {
                detailIsChanged = $" (SP{IsStartProgressChanged.AsSign()}, EP{IsEndProgressChanged.AsSign()}, " +
                                  $"PD{IsProgressDurationChanged.AsSign()}, EI{IsExpectedItemsChanged.AsSign()})";
            }

            return $"[<{GetType()}> {nameof(IsChanged)}{IsChanged.AsSign()}{detailIsChanged}, " +
                   $"{nameof(Ending)}={Ending}, {nameof(VisualStage)}={VisualStage}, {nameof(ProcessSource)}={ProcessSource}]";
        }


        //=== Protected ===========================================================

        //--- Регистрация изменениий наблюдаемого объекта, с визуальным отображением не связана

        protected virtual void OnStateChanged([NotNull] IProcessSource processSource, bool isEndProgressChanged,
            bool isStartProgressChanged, bool isProgressDurationChanged, bool isExpectedItemsChanged)
        {
            if (processSource.AssertIfNull(nameof(processSource)))
                return;

            if ((int) VisualStage > (int) IndicatorVisualStage.Appeared)
            {
                UI.Logger.IfError()?.Message($"{nameof(OnStateChanged)}() Unaccepted on Stage {VisualStage}").Write();
                return;
            }

            if (isEndProgressChanged)
                IsEndProgressChanged = true;

            if (isStartProgressChanged)
                IsStartProgressChanged = true;

            if (isProgressDurationChanged)
                IsProgressDurationChanged = true;

            if (isExpectedItemsChanged)
            {
                IsExpectedItemsChanged = true;
                FirstResourceExpectedCountUpdate();
            }
        }

        protected virtual void OnItemsAchieved(IProcessSource processSource, IList<ItemResourcePack> achievedItems,
            IList<uint> inventoryCounts, bool isEnded)
        {
            if (isEnded)
            {
                if (!TryChangeEnding(EndType.SuccessEnding))
                    return;

                UnsubscribeFromProcessSource(processSource);
            }

            AchievedItems = new List<ItemResourcePack>();
            for (int i = 0, len = achievedItems.Count; i < len; i++)
                AchievedItems.Add(new ItemResourcePack(achievedItems[i].ItemResource, inventoryCounts[i]));
        }

        protected virtual void OnFailOrCancelEnding(IProcessSource processSource, bool isFail)
        {
            if (!TryChangeEnding(isFail ? EndType.FailEnding : EndType.CancelEnding))
                return;

            UnsubscribeFromProcessSource(processSource);
        }

        protected virtual void Visual_AnyUpdate()
        {
            if ((int) VisualStage >= (int) IndicatorVisualStage.Appearing)
                CurrentProgress = CurrentProgressRelation?.GetClampedY(Time.time) ?? 0;
        }

        //--- Update для индикаторов, исчезающих не по внешним событиям, а собственному таймеру
//        protected virtual void Visual_AppearedUpdate()
//        {
//        }

        protected virtual void Visual_AppearingStart()
        {
        }

        protected virtual void Visual_ChangingStart()
        {
        }

//        protected virtual void Visual_SuccessDisappearingStart()
//        {
//        }

//        protected virtual void Visual_FailDisappearingStart()
//        {
//        }

//        protected virtual void Visual_CancelledDisappearingStart()
//        {
//        }

        protected bool TryChangeEnding(EndType newEnding)
        {
            if ((int) VisualStage >= (int) IndicatorVisualStage.Disappearing)
            {
                UI.Logger.IfError()?.Message($"{nameof(TryChangeEnding)}() Unable to change {Ending} to {newEnding}").Write();
                return false;
            }

            Ending = newEnding;
            return true;
        }

        protected bool TryChangeStage(IndicatorVisualStage newVisualStage)
        {
            if ((int) newVisualStage <= (int) VisualStage)
            {
                UI.Logger.IfError()?.Message($"{nameof(TryChangeStage)}() Unable to change {VisualStage} to {newVisualStage}").Write();
                return false;
            }

            VisualStage = newVisualStage;
            return true;
        }

        protected void ProgressSetup(bool canOverrideStartProgressOfSource = false)
        {
            var startProgress = canOverrideStartProgressOfSource && !IsProgressDurationChanged && !IsStartProgressChanged
                ? CurrentProgress
                : ProcessSource.StartProgress;
            LastChangesTime = Time.time;
            CurrentProgressRelation = new LinearRelation(
                Time.time, startProgress,
                Time.time + AlwaysCorrectProgressDuration, ProcessSource.EndProgress);
            IsStartProgressChanged = IsEndProgressChanged = IsProgressDurationChanged = false;
        }

        protected void DebugLog(string message = "", [CallerMemberName] string callerMethodName = "")
        {
            UI.Logger.IfDebug()?.Message($"[{Time.time:f1}] *** {callerMethodName}() {ToString()}  {message}").Write();
        }


        //=== Private =============================================================

        private void ProgressPanelSilentDisappearing()
        {
            ProgressPanelAppearing.Appearing(false);
        }

        private void UnsubscribeFromProcessSource(IProcessSource processSource)
        {
            processSource.StateChanged -= OnStateChanged;
            processSource.ItemsAchieved -= OnItemsAchieved;
            processSource.FailOrCancelEnding -= OnFailOrCancelEnding;
        }

        private void FirstResourceExpectedCountUpdate()
        {
            FirstResourceExpectedCount = (int) (ProcessSource?.ExpectedItems?.FirstOrDefault().ItemPack.Count ?? 0);
        }
    }
}