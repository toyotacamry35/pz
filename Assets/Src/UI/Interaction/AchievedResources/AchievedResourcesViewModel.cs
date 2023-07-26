using Assets.Src.Inventory;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using Uins.Slots;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class AchievedResourcesViewModel : BindingViewModel
    {
        private const float AfterPawnAppearingSilencePeriod = 2;

        [SerializeField, UsedImplicitly]
        private WindowId _containerWindowId;

        [SerializeField, UsedImplicitly]
        private WindowId _inventoryWindowId;

        [SerializeField, UsedImplicitly]
        private AchievedItemContr _achievedItemContrPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _achievedItemsTransform;

        private bool _hasPawn;
        private float _lastPawnAppearingTime;
        private int _id = 1;

        private ICharacterItemsNotifier _characterItemsNotifier;
        private ListStream<AchievedItemVmodel> _itemViewModels = new ListStream<AchievedItemVmodel>();


        //=== Props ===========================================================

        [Binding]
        public bool IsVisible { get; private set; }

        private bool IsPawnAppearingSilence => Time.time - _lastPawnAppearingTime < AfterPawnAppearingSilencePeriod;


        //=== Public ==============================================================

        public void Init(IPawnSource pawnSource, ICharacterItemsNotifier characterItemsNotifier, WindowsManager windowsManager)
        {
            if (_achievedItemContrPrefab.AssertIfNull(nameof(_achievedItemContrPrefab)) ||
                _achievedItemsTransform.AssertIfNull(nameof(_achievedItemsTransform)) ||
                _containerWindowId.AssertIfNull(nameof(_containerWindowId)) ||
                _inventoryWindowId.AssertIfNull(nameof(_inventoryWindowId)) ||
                characterItemsNotifier.AssertIfNull(nameof(characterItemsNotifier)) ||
                windowsManager.AssertIfNull(nameof(windowsManager)) ||
                pawnSource.AssertIfNull(nameof(pawnSource)))
                return;

            _characterItemsNotifier = characterItemsNotifier;
            pawnSource.PawnChangesStream.Action(
                D,
                (prevEgo, newEgo) =>
                {
                    _hasPawn = newEgo != null;
                    _lastPawnAppearingTime = Time.time;
                });
            var containerGuiWindow = windowsManager.GetWindow(_containerWindowId);
            var inventoryGuiWindow = windowsManager.GetWindow(_inventoryWindowId);
            if (inventoryGuiWindow.AssertIfNull(nameof(inventoryGuiWindow)) ||
                containerGuiWindow.AssertIfNull(nameof(containerGuiWindow)))
                return;

            characterItemsNotifier.CharacterItemsChanged += OnCharacterItemsChanged;
            var isVisibleStream = containerGuiWindow.State
                .Zip(D, inventoryGuiWindow.State)
                .Zip(D, pawnSource.PawnChangesStream)
                .Func(
                    D,
                    (containerWndState, inventoryWndState, egoTuple) =>
                        containerWndState == GuiWindowState.Closed && inventoryWndState == GuiWindowState.Closed && egoTuple.Item2 != null);
            Bind(isVisibleStream, () => IsVisible);

            var pool = new BindingControllersPoolWithUsingProp<AchievedItemVmodel>(_achievedItemsTransform, _achievedItemContrPrefab);
            pool.Connect(_itemViewModels);
        }


        //=== Private ==============================================================

        private void OnCharacterItemsChanged(SlotViewModel slotViewModel, int stackDelta)
        {
            if (slotViewModel.AssertIfNull(nameof(slotViewModel)))
                return;

            //UI.CallerLog($"[{slotIndex}] INV{isInventoryNorDoll.AsSign()}, delta={stackDelta}, {slotItem}"); //2del

            if (stackDelta <= 0 || !_hasPawn || IsPawnAppearingSilence || !IsVisible)
                return;

            _itemViewModels.Add(
                new AchievedItemVmodel()
                {
                    Description = slotViewModel.ItemName,
                    ResourceSprite = slotViewModel.Icon,
                    AchievedItemsCount = stackDelta,
                    TotalItemsCount = _characterItemsNotifier.GetItemResourceCount(slotViewModel.ItemResource),
                    ShownVmodels = _itemViewModels,
                    Id = _id++
                });

            if (_id > 1000)
                _id = 1;
        }
    }
}