using System.Collections.Generic;
using System.Linq;
using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ContainerApis;
using Assets.Src.ResourceSystem;
using Assets.Src.ResourceSystem.L10n;
using Assets.Src.SpawnSystem;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using L10n;
using NLog;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;
using Uins.Inventory;
using Uins.Sound;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class QuestsPanelViewModel : BindingViewModel
    {
        [SerializeField, UsedImplicitly]
        private HotkeyListener _trackingListener;

        [SerializeField, UsedImplicitly]
        private QuestItemViewModel _questItemViewModelPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _mainQuestsRoot;

        [SerializeField, UsedImplicitly]
        private Transform _dailyQuestsRoot;

        [SerializeField, UsedImplicitly]
        private QuestContextView _questContextView;

        [SerializeField, UsedImplicitly]
        private UpdateInterval _questDistUpdateInterval;

        [SerializeField, UsedImplicitly]
        private UnseenYetIndicatorDefRef _indicatorDefRef;

        [SerializeField, UsedImplicitly]
        private LocalizationKeyPairsDefRef _trackingBtnTitlesDefRef;

        [SerializeField, UsedImplicitly]
        private WindowId _inventoryWindowId;

        private UnseenYetIndicator _unseenYetIndicator;

        ListStream<QuestItemViewModel> _mainQuests = new ListStream<QuestItemViewModel>();
        ListStream<QuestItemViewModel> _optionalQuests = new ListStream<QuestItemViewModel>();

        private Transform _ourPawnTransform;

        //Внутренние переменные чтобы частично сохранить совместимость со старым API
        private ITouchable<IQuestEngineClientFull> _questEngineTch;


        //=== Props ===========================================================

        private LocalizationKeyPairsDef TrackingBtnTitlesDef => _trackingBtnTitlesDefRef.Target;

        public IQuestTrackingContext QuestTrackingContext => _questContextView;

        [Binding]
        public bool IsTabOpen { get; private set; }

        private bool _isMainQuestTab = true;

        [Binding]
        public bool IsMainQuestTab
        {
            get => _isMainQuestTab;
            private set
            {
                if (_isMainQuestTab != value)
                {
                    _isMainQuestTab = value;
                    NotifyPropertyChanged();
                    IsTrackingButtonAvail = GetIsTrackingButtonAvail();
                }
            }
        }

        private bool _isTrackingButtonAvail;

        [Binding]
        public bool IsTrackingButtonAvail //TODOM Rp
        {
            get => _isTrackingButtonAvail;
            private set
            {
                if (_isTrackingButtonAvail != value)
                {
                    _isTrackingButtonAvail = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ReactiveProperty<LocalizedString> _trackingButtonTextRp = new ReactiveProperty<LocalizedString>();

        [Binding]
        public LocalizedString TrackingButtonText { get; private set; }

        private bool _isQuestsTabVisible;

        public bool IsQuestsTabVisible
        {
            get => _isQuestsTabVisible;
            private set
            {
                if (_isQuestsTabVisible != value)
                {
                    _isQuestsTabVisible = value;
                    if (_isQuestsTabVisible)
                        UnseenQuestsCount = 0;
                }
            }
        }

        private int _unseenQuestsCount;

        public int UnseenQuestsCount
        {
            get => _unseenQuestsCount;
            set
            {
                if (_isQuestsTabVisible)
                    value = 0;
                if (_unseenQuestsCount != value)
                {
                    _unseenQuestsCount = value;
                    _unseenYetIndicator?.SetCount(_unseenQuestsCount);
                }
            }
        }

        [Binding]
        public bool HasMainQuests { get; private set; }

        [Binding]
        public bool HasOptionalQuests { get; private set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            if (_trackingListener.AssertIfNull(nameof(_trackingListener)) ||
                _questItemViewModelPrefab.AssertIfNull(nameof(_questItemViewModelPrefab)) ||
                _mainQuestsRoot.AssertIfNull(nameof(_mainQuestsRoot)) ||
                _dailyQuestsRoot.AssertIfNull(nameof(_dailyQuestsRoot)) ||
                _questContextView.AssertIfNull(nameof(_questContextView)) ||
                _inventoryWindowId.AssertIfNull(nameof(_inventoryWindowId)))
                return;

            _questContextView.SelectedQuestRp.Action(D, selectedVm => IsTrackingButtonAvail = GetIsTrackingButtonAvail());

            var trackedAndSelected = _questContextView.TrackedQuestRp.Zip(D, _questContextView.SelectedQuestRp);
            trackedAndSelected.Action(
                D,
                (tracked, selected) =>
                {
                    _trackingButtonTextRp.Value =
                        selected == null
                            ? TrackingBtnTitlesDef.Ls1
                            : tracked == selected
                                ? TrackingBtnTitlesDef.Ls2 //untrack
                                : TrackingBtnTitlesDef.Ls1; //track
                });
            Bind(_trackingButtonTextRp, () => TrackingButtonText);

            _indicatorDefRef.Target.AssertIfNull(nameof(_indicatorDefRef));

            _trackingBtnTitlesDefRef.Target.AssertIfNull(nameof(_trackingBtnTitlesDefRef));

            Bind(_mainQuests.CountStream.Func(D, count => count > 0), () => HasMainQuests);
            Bind(_optionalQuests.CountStream.Func(D, count => count > 0), () => HasOptionalQuests);
        }


        //=== Public ==============================================================

        public void Init(
            IPawnSource pawnSource,
            UnseenYetPanel unseenYetPanel,
            WindowsManager windowsManager,
            IStream<ContextViewWithParamsVmodel> cvwpVmodelStream)
        {
            if (pawnSource.AssertIfNull(nameof(pawnSource)) ||
                unseenYetPanel.AssertIfNull(nameof(unseenYetPanel)) ||
                windowsManager.AssertIfNull(nameof(windowsManager)) ||
                cvwpVmodelStream.AssertIfNull(nameof(cvwpVmodelStream)))
                return;

            _questEngineTch = pawnSource.TouchableEntityProxy.Child(D, ch => ch.Quest);
            // PZ-15197 // Logger.IfError()?.Message($"$$$$$$$$$$ Init(IPawnSource, UnseenYetPanel, WindowsManager, {cvwpVmodelStream} => {cvwpVmodelStream.StreamState()}) // _questEngineTch:{_questEngineTch}").Write();
            // PZ-15197 // pawnSource.TouchableEntityProxy.Log(D, "$$$$$$$$$$ pawnSource.TouchableEntityProxy: ");
            // PZ-15197 // _questEngineTch.Log(D, "$$$$$$$$$$ QuestsPanelViewModel.Init()._questEngineTch: ");


            pawnSource.PawnChangesStream.Action(D, OnOurPawnChanged);
            _unseenYetIndicator = unseenYetPanel.GetNewIndicator(_indicatorDefRef.Target);

            var inventoryWindow = windowsManager.GetWindow(_inventoryWindowId);
            var questInventoryTabIsOpenStream = cvwpVmodelStream.SubStream(D, vm => vm.GetTabVmodel(InventoryTabType.Quests).IsOpenTabRp);
            var isWindowAndTabOpenStream = inventoryWindow.State
                .Zip(D, questInventoryTabIsOpenStream)
                .Func(D, (state, isTabOpened) => state == GuiWindowState.Opened && isTabOpened);
            Bind(isWindowAndTabOpenStream, () => IsTabOpen);
            //восстановление выбранного квеста на открытии окна
            isWindowAndTabOpenStream.Action(
                D,
                isOpen =>
                {
                    OnQuestsTabVisibilityChanged(isOpen);
                    if (!isOpen)
                        return;

                    if (GetSelectedQuestItemViewModel() != null) //есть выделенный - все Ок
                        return;

                    var trackedOrFirstQuestItem = GetTrackedOrFirst();
                    if (trackedOrFirstQuestItem != null)
                        _questContextView.TakeContext(trackedOrFirstQuestItem);
                });
        }

        public void OnUpdate()
        {
            if (!IsTabOpen)
                return;

            if (_trackingListener.IsFired())
                OnTrackingButton();

            if (_questDistUpdateInterval.IsItTime())
            {
                UpdateCollectionDistances(true);
                UpdateCollectionDistances(false);
            }
        }

        private void UpdateCollectionDistances(bool isMainCollection)
        {
            var questCollection = GetQuestCollection(!isMainCollection);
            foreach (var questItemViewModel in questCollection)
                questItemViewModel.UpdateDist(_ourPawnTransform);
        }

        [UsedImplicitly]
        public void SwitchQuestsTab(bool isMainTab)
        {
            IsMainQuestTab = isMainTab;
        }

        [UsedImplicitly]
        public void ClearQuestSelection()
        {
            _questContextView.TakeContext(null);
        }

        [UsedImplicitly] //click
        public void OnTrackingButton()
        {
            if (!IsTrackingButtonAvail)
                return;

            var selectedQuestItemViewModel = GetSelectedQuestItemViewModel();
            if (selectedQuestItemViewModel == null || selectedQuestItemViewModel.IsDone)
                return;

            // PZ-15197 // Logger.IfError()?.Message($"$$$$$$$$$$ QuestPhaseViewModel.OnTrackingButton()").Write();

            _questContextView.TrackedQuestRp.Value = selectedQuestItemViewModel.IsTracked ? null : selectedQuestItemViewModel;
            _questContextView.LastTrackedQuestDef = _questContextView.TrackedQuestRp.Value?.QuestDef; //только здесь LastTrackedQuestDef может стать null
        }


        //=== Private =========================================================

        private void OnQuestsTabVisibilityChanged(bool isVisible)
        {
            IsQuestsTabVisible = isVisible;
        }

        private EntityApiWrapper<QuestEngineFullApi> _questEngineFullApiWrapper;

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
            {
                _ourPawnTransform = null;
                _questEngineFullApiWrapper.EntityApi.UnsubscribeFromQuests(OnQuestChanged, OnQuestRemoved);
                _questContextView.TakeContext(null);
                if (_questContextView.IsAlreadyTrackedAnyQuest)
                    _questContextView.TrackedQuestRp.Value = null;
                ClearCollection(true);
                ClearCollection(false);

                _questEngineFullApiWrapper.Dispose();
                _questEngineFullApiWrapper = null;
            }

            if (newEgo != null)
            {
                _ourPawnTransform = newEgo.transform;
                _questEngineFullApiWrapper = EntityApi.GetWrapper<QuestEngineFullApi>(newEgo.OuterRef);
                _questEngineFullApiWrapper.EntityApi.SetQuestEngine(_questEngineTch);

                _questEngineFullApiWrapper.EntityApi.SubscribeToQuests(OnQuestChanged, OnQuestRemoved);
                if (_questContextView.IsNoneTracked)
                    RestoreLastTrackedQuest();
            }
        }

        private bool GetIsTrackingButtonAvail()
        {
            if (_questContextView.ContextValue == null)
                return false;

            var selectedVm = _questContextView.ContextValue as QuestItemViewModel;

            return !selectedVm.IsDone; // && (IsMainQuestTab ? selectedVm.Group == QuestGroup.Main : selectedVm.Group == QuestGroup.Daily)
        }

        /// <summary> Получаем новую модель из источника. </summary>
        private void OnQuestChanged(QuestState questState, bool isFirstTime)
        {
            if (questState.AssertIfNull(nameof(questState)))
                return;

            var questDef = questState.QuestDef;
            var questCollection = GetQuestCollection(questDef.Group == QuestGroup.Daily);
            var changedQuestItemViewModel = questCollection.FirstOrDefault(vm => vm.QuestDef == questDef);
            if (changedQuestItemViewModel == null)
            {
                changedQuestItemViewModel = Instantiate(
                    _questItemViewModelPrefab,
                    questState.QuestDef.Group == QuestGroup.Daily ? _dailyQuestsRoot : _mainQuestsRoot);
                if (changedQuestItemViewModel.AssertIfNull(nameof(changedQuestItemViewModel)))
                    return;

                changedQuestItemViewModel.Init(_questContextView, _questContextView, questState);
                questCollection.Add(changedQuestItemViewModel);
                if (!questState.IsDone)
                {
                    if (isFirstTime) //в момент возрождения, когда все квесты приходят как новые
                    {
                        if (!_questContextView.IsAlreadyTrackedAnyQuest) //первое возрождение с момента захода в игру
                        {
                            _questContextView.TrackedQuestRp.Value = changedQuestItemViewModel; //отслеживаем первый активный
                        }
                    }
                    else
                    {
                        UnseenQuestsCount++;
                        CenterNotificationQueue.Instance.SendNotification(new QuestNotificationInfo(questState.QuestDef.NameLs, true));

                        //если новый можно отслеживать И (отслеживаемого нет ИЛИ измененный - из группы Main) - меняем отслеживаемый
                        if (!changedQuestItemViewModel.IsDone &&
                            (_questContextView.TrackedQuestRp.Value == null ||
                             changedQuestItemViewModel.QuestDef.Group == QuestGroup.Main))
                            _questContextView.TrackedQuestRp.Value = changedQuestItemViewModel;
                    }
                }
            }
            else
            {
                if (!isFirstTime)
                {
                    if (questState.IsDone)
                    {
                        CenterNotificationQueue.Instance.SendNotification(new QuestNotificationInfo(questState.QuestDef.NameLs, false));
                    }
                    else
                    {
                        SoundControl.Instance?.QuestProgress?.Post(transform.root.gameObject);
                    }
                }

                bool wasTracked = changedQuestItemViewModel.IsTracked;
                changedQuestItemViewModel.UpdateState(questState);
                if (changedQuestItemViewModel.IsSelected)
                    IsTrackingButtonAvail = GetIsTrackingButtonAvail();

                //Если существующий отслеживаемый квест закончился, трекаем следующий активный
                if (changedQuestItemViewModel.IsDone && wasTracked)
                    SetFirstActiveQuestTracking(questCollection);
            }

            if (_questContextView.IsNoneTracked)
                RestoreLastTrackedQuest();
        }

        /// <summary>
        /// Трекаем следующий активный
        /// </summary>
        /// <param name="questCollection"></param>
        private void SetFirstActiveQuestTracking(ListStream<QuestItemViewModel> questCollection)
        {
            var nextQuestItemViewModel = questCollection.LastOrDefault(q => !q.IsDone);
            if (nextQuestItemViewModel != null)
                _questContextView.TrackedQuestRp.Value = nextQuestItemViewModel;
        }

        private void OnQuestRemoved(QuestDef questDef)
        {
            if (questDef.AssertIfNull(nameof(questDef)))
                return;

            var questCollection = GetQuestCollection(questDef.Group == QuestGroup.Daily);
            var removedQuestItem = questCollection.FirstOrDefault(elem => elem.QuestDef == questDef);

            if (removedQuestItem == null)
            {
                UI.Logger.Warn($"{nameof(removedQuestItem)} not found by {nameof(questDef)}");
                return;
            }

            questCollection.Remove(removedQuestItem);
            if (removedQuestItem.IsTracked)
            {
                _questContextView.TrackedQuestRp.Value = null;
                SetFirstActiveQuestTracking(questCollection);
            }

            if (_questContextView.SelectedQuestRp.Value == removedQuestItem)
            {
                _questContextView.SelectedQuestRp.Value = null;
            }

            Destroy(removedQuestItem.gameObject);
        }

        private ListStream<QuestItemViewModel> GetQuestCollection(bool isDaily)
        {
            return isDaily ? _optionalQuests : _mainQuests;
        }

        private void ClearCollection(bool isDaily)
        {
            var collection = GetQuestCollection(isDaily);
            while (collection.Count > 0)
            {
                var index = collection.Count - 1;
                Destroy(collection[index].gameObject);
                collection.RemoveAt(index);
            }
        }

        /// <summary>
        /// Восстановление из сохраненного LastTrackedQuestDef на момент возрождения
        /// </summary>
        private void RestoreLastTrackedQuest()
        {
            if (_questContextView.LastTrackedQuestDef != null)
            {
                var questItemViewModel = GetVmByQuestDef(_questContextView.LastTrackedQuestDef);
                if (questItemViewModel != null && !questItemViewModel.IsDone)
                {
                    _questContextView.TrackedQuestRp.Value = questItemViewModel;
                }
            }
        }

        private QuestItemViewModel GetVmByQuestDef(QuestDef questDef)
        {
            var vm = GetQuestCollection(true).FirstOrDefault(qivm => qivm.QuestDef == questDef);
            if (vm != null)
                return vm;

            return GetQuestCollection(false).FirstOrDefault(qivm => qivm.QuestDef == questDef);
        }

        private QuestItemViewModel GetSelectedQuestItemViewModel()
        {
            if (!IsMainQuestTab)
                return null;

            return _questContextView.SelectedQuestRp.Value;
        }

        private QuestItemViewModel GetTrackedOrFirst()
        {
            if (!IsMainQuestTab)
                return null;

            return _questContextView.TrackedQuestRp.Value ?? _mainQuests.FirstOrDefault() ?? _optionalQuests.FirstOrDefault();
        }
    }
}