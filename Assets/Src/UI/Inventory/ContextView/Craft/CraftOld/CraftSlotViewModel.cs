using Assets.ColonyShared.SharedCode.Aspects.Craft;
using Assets.Src.Aspects;
using Assets.Src.ContainerApis;
using Assets.Tools;
using ColonyShared.SharedCode.Utils;
using ReactivePropsNs;
using SharedCode.Serializers;
using Uins.Sound;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins.Inventory
{
    [Binding]
    public class CraftSlotViewModel : BindingViewModel
    {
        public int QuequeElementIndex;

        private bool _isSubscribed;
        private int _keyIndex;
        private int _selectedVariantIndex;
        private CraftQueueSlots _craftQueueSlots;
        private bool _isAfterInit;
        private bool _isAfterAwake;
        private IStream<((EntityApiWrapper<CraftEngineQueueFullApi>, bool), (EntityApiWrapper<CraftEngineQueueFullApi>, bool))> _needToSubscribePrevCurrStream;


        //=== Props ===========================================================

        [Binding]
        public bool IsVisible { get; private set; }

        public ReactiveProperty<bool> IsVisibleRp { get; } = new ReactiveProperty<bool>() {Value = true};

        private CraftRecipeDef _craftRecipe;

        private CraftRecipeDef CraftRecipe
        {
            get => _craftRecipe;
            set
            {
                if (_craftRecipe != value)
                {
                    var oldIsEmpty = IsEmpty;
                    _craftRecipe = value;
                    OnCraftRecipeChanged();
                    if (oldIsEmpty != IsEmpty)
                        NotifyPropertyChanged(nameof(IsEmpty));
                }
            }
        }

        private Sprite _productIcon;

        [Binding]
        public Sprite ProductIcon
        {
            get => _productIcon;
            set
            {
                if (_productIcon != value)
                {
                    var oldHasProductIcon = HasProductIcon;
                    _productIcon = value;
                    NotifyPropertyChanged();
                    if (HasProductIcon != oldHasProductIcon)
                        NotifyPropertyChanged(nameof(HasProductIcon));
                }
            }
        }

        [Binding]
        public bool HasProductIcon => ProductIcon != null;

        [Binding]
        public bool IsEmpty => CraftRecipe == null;

        private bool _isCraftStarted;

        [Binding]
        public bool IsCraftStarted
        {
            get => _isCraftStarted;
            set
            {
                if (_isCraftStarted != value)
                {
                    var oldIsCraftPaused = IsCraftPaused;
                    _isCraftStarted = value;
                    NotifyPropertyChanged();
                    if (oldIsCraftPaused != IsCraftPaused)
                        NotifyPropertyChanged(nameof(IsCraftPaused));
                }
            }
        }

        [Binding]
        public bool IsCraftPaused => IsCraftStarted && !IsCraftActive;

        private bool _isCraftActive;

        [Binding]
        public bool IsCraftActive
        {
            get => _isCraftActive;
            set
            {
                if (_isCraftActive != value)
                {
                    var oldIsCraftPaused = IsCraftPaused;
                    _isCraftActive = value;
                    NotifyPropertyChanged();
                    if (oldIsCraftPaused != IsCraftPaused)
                        NotifyPropertyChanged(nameof(IsCraftPaused));
                }
            }
        }

        private float _commonRemainingTime;

        [Binding]
        public float CommonRemainingTime
        {
            get => _commonRemainingTime;
            set
            {
                if (!Mathf.Approximately(_commonRemainingTime, value))
                {
                    _commonRemainingTime = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private float _currentItemRemainingTimeRatio;

        [Binding]
        public float CurrentItemRemainingTimeRatio
        {
            get => _currentItemRemainingTimeRatio;
            set
            {
                if (!Mathf.Approximately(_currentItemRemainingTimeRatio, value))
                {
                    _currentItemRemainingTimeRatio = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _craftingItemsCount;

        [Binding]
        public int CraftingItemsCount
        {
            get => _craftingItemsCount;
            set
            {
                if (!Mathf.Approximately(_craftingItemsCount, value))
                {
                    _craftingItemsCount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private float _itemCraftTime;

        [Binding]
        public float ItemCraftTime
        {
            get => _itemCraftTime;
            set
            {
                if (!Mathf.Approximately(_itemCraftTime, value))
                {
                    _itemCraftTime = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            _isAfterAwake = true;
            if (_isAfterInit)
                AfterInitAndAwake();
        }

        private void Update()
        {
            if (!_isSubscribed)
                return;

            if (IsCraftStarted && IsCraftActive)
            {
                CommonRemainingTime = UpdateCommonRemainingTime(CommonRemainingTime, Time.deltaTime);
                CurrentItemRemainingTimeRatio = GetCurrentItemRemainingTimeRatio();
            }
        }


        //=== Public ==========================================================

        public void Init(CraftQueueSlots craftQueueSlots)
        {
            _isAfterInit = true;
            _craftQueueSlots = craftQueueSlots;
            _craftQueueSlots.AssertIfNull(nameof(_craftQueueSlots));

            _needToSubscribePrevCurrStream = _craftQueueSlots.CraftEngineQueueFullApiWrapperRp
                .Zip(D, IsVisibleRp)
                .PrevAndCurrent(D);

            if (_isAfterAwake)
                AfterInitAndAwake();
        }

        public void OnClick()
        {
            if (!_isSubscribed || IsEmpty)
                return;

            SoundControl.Instance.CraftTaskCancelEvent?.Post(transform.root.gameObject);
            AsyncUtils.RunAsyncTask(() => CraftEngineCommands.CancelCraftAsync(_craftQueueSlots.CraftEngineOuterRef, _keyIndex));
        }


        //=== Private =========================================================

        private void AfterInitAndAwake()
        {
            _needToSubscribePrevCurrStream.Action(
                D,
                (prev, curr) =>
                {
                    if (prev.Item1 != null)
                    {
                        Unsubscribe(prev.Item1.EntityApi);
                    }

                    if (curr.Item1 != null && curr.Item2)
                    {
                        Subscribe(curr.Item1.EntityApi);
                    }
                });
            Bind(IsVisibleRp, () => IsVisible);
        }

        private void Subscribe(CraftEngineQueueFullApi craftEngineQueueFullApi)
        {
            if (_isSubscribed)
                return;

            //UI.CallerLog($"<{GetType()}> [{QuequeElementIndex}] {craftEngineQueueFullApi}"); //DEBUG
            craftEngineQueueFullApi.SubscribeToCraftQueueSlot(QuequeElementIndex, OnQueueSlotChanged);
            _isSubscribed = true;
        }

        private void Unsubscribe(CraftEngineQueueFullApi craftEngineQueueFullApi)
        {
            if (!_isSubscribed)
                return;

            //UI.CallerLog($"<{GetType()}> [{QuequeElementIndex}] {craftEngineQueueFullApi}"); //DEBUG
            craftEngineQueueFullApi.UnsubscribeFromCraftQueueSlot(QuequeElementIndex, OnQueueSlotChanged);
            _isSubscribed = false;
        }

        private void OnQueueSlotChanged(int slotIndex, CraftQueueSlot craftQueueSlot)
        {
            // UI.CallerLog($"slotIndex={slotIndex}, craftQueueSlot='{craftQueueSlot}'"); //DEBUG
            if (craftQueueSlot.AssertIfNull(nameof(craftQueueSlot)))
                return;

            _keyIndex = craftQueueSlot.KeyIndex;
            IsCraftStarted = !craftQueueSlot.IsEmpty && QuequeElementIndex == 0;
            IsCraftActive = craftQueueSlot.IsActive;
            _selectedVariantIndex = craftQueueSlot.SelectedVariantIndex;
            CraftRecipe = craftQueueSlot.CraftRecipe;
            CraftingItemsCount = craftQueueSlot.Count;
            OnCraftRecipeChanged();
            long craftEndTime = craftQueueSlot.CraftStartTime + Mathf.RoundToInt(1000 * ItemCraftTime * CraftingItemsCount);
            long syncTimeNow = SyncTime.Now;
            CommonRemainingTime = Mathf.Max(0, (craftEndTime - syncTimeNow) / 1000f);
            // UI.CallerLog($"CommonRemainingTime={CommonRemainingTime}s, syncDelta={syncTimeNow - craftQueueSlot.CraftStartTime}ms " +
            //              $"craftEndTime={craftEndTime}, stn={syncTimeNow}"); //DEBUG
        }

        private void OnCraftRecipeChanged()
        {
            if (_craftRecipe != null)
            {
                var variants = _craftRecipe.Variants; //все возможные варианты крафта
                var product = variants[_selectedVariantIndex].Product; //создаваемый предмет
                ProductIcon = product.Item.Target?.Icon?.Target;
                ItemCraftTime = variants[_selectedVariantIndex].CraftingTime; // общее время для создания в секундах
            }
            else
            {
                ProductIcon = null;
                ItemCraftTime = 0;
            }
        }

        private float UpdateCommonRemainingTime(float prevTime, float deltaTime)
        {
            return CraftingItemsCount <= 0 ? 0 : Mathf.Max(0, prevTime - deltaTime);
        }

        private float GetCurrentItemRemainingTimeRatio()
        {
            return 1 - (CommonRemainingTime % ItemCraftTime) / ItemCraftTime;
        }
    }
}