using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ContainerApis;
using Assets.Src.Inventory;
using L10n;
using ReactivePropsNs;
using SharedCode.Aspects.Item.Templates;
using Uins.Inventory;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins.Slots
{
    public delegate void SlotChangedDelegate(SlotViewModel slotViewModel, int stackDelta);

    [Binding]
    public abstract class SlotViewModel : BindingViewModel, IContextViewTargetWithParams
    {
        public enum CollectionType
        {
            Inventory,
            Doll,
            Output,
            Fuel,
        }

        private const float ChangeSelectionIgnoreDelay = 0.2f;

        public event Action ItemBecomeSelected;

        public event SlotChangedDelegate SlotChanged;

        private int _inventorySlotId;

        protected IHasContextStream HasContextStream;
        private DisposableComposite _tempoD = new DisposableComposite();
        private float _lastChangeSelectionTime;

        private readonly ContextViewParams _contextViewParams = new ContextViewParams();


        //=== Props ===========================================================

        /// <summary>
        /// Решатель: можно ли и сколько именно предметов перенести в данный слот
        /// </summary>
        public ISlotAcceptanceResolver SlotAcceptanceResolver { get; private set; }

        public int BaseSortIndex { get; private set; }

        public SlotItem SelfSlotItem { get; protected set; } = new SlotItem()
        {
            Count = 123
        };

        public SlotItemFrequentStats FrequentStats { get; protected set; } = new SlotItemFrequentStats();

        public SlotsCollectionApi SlotsCollectionApi { get; protected set; }

        public virtual CollectionType SlotCollectionType { get; }

        public virtual bool IsOurPlayerSlot => false;

        public IContextActionsSource ContextActionsSource { get; private set; }

        [Binding]
        public bool IsInventorySlot => SlotCollectionType == CollectionType.Inventory;

        public virtual int SlotId => _inventorySlotId;

        [Binding]
        public LocalizedString ItemName => SelfSlotItem.ItemResource?.ItemNameLs ?? LsExtensions.Empty;

        [Binding]
        public int Stack => (int) SelfSlotItem.Count;

        public BaseItemResource ItemResource => SelfSlotItem.ItemResource;

        private bool _isEmpty = true;

        [Binding]
        public bool IsEmpty //зависит от Stack
        {
            get => _isEmpty;
            private set
            {
                if (_isEmpty != value)
                {
                    _isEmpty = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [Binding]
        public bool ItemIsNullOrDefault => ItemResource == null;

        private bool _isMany = true;

        [Binding]
        public bool IsMany
        {
            get => _isMany;
            private set
            {
                if (_isMany != value)
                {
                    _isMany = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [Binding]
        public Sprite Icon => ItemResource?.Icon?.Target;

        [Binding]
        public ItemTypeResource ItemType => SelfSlotItem.ItemResource?.ItemType;

        public virtual ItemTypeResource[] ItemTypes => new[] {ItemType};

        public virtual bool CanBeEquippedOrUnequipped => ItemType != null && (ItemResource as ItemResource)?.MountingData == null && !(ItemResource is DollItemResource);

        public virtual bool DoEquipSound => false;


        private bool _isSelected;

        [Binding]
        public virtual bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    NotifyPropertyChanged();
                    if (_isSelected)
                    {
                        if (Time.time - _lastChangeSelectionTime > ChangeSelectionIgnoreDelay)
                        {
                            ItemBecomeSelected?.Invoke();
                        }
                    }

                    _lastChangeSelectionTime = Time.time;
                }
            }
        }

        private int _ammoCount;

        [Binding]
        public int AmmoCount
        {
            get => _ammoCount;
            private set
            {
                if (_ammoCount != value)
                {
                    _ammoCount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _ammoMaxCount;

        [Binding]
        public int AmmoMaxCount
        {
            get => _ammoMaxCount;
            private set
            {
                if (_ammoMaxCount != value)
                {
                    _ammoMaxCount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _hasAmmoContainer;

        [Binding]
        public bool HasAmmoContainer
        {
            get => _hasAmmoContainer;
            private set
            {
                if (_hasAmmoContainer != value)
                {
                    _hasAmmoContainer = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _hasAmmo;

        [Binding]
        public bool HasAmmo
        {
            get => _hasAmmo;
            private set
            {
                if (_hasAmmo != value)
                {
                    _hasAmmo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _containerItemName;

        [Binding]
        public string ContainerItemName
        {
            get => _containerItemName;
            private set
            {
                if (_containerItemName != value)
                {
                    _containerItemName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [Binding]
        public Sprite FiltrableTypeBgSign => (SelfSlotItem.ItemResource as ItemResource)?.InventoryFiltrableType.Target?.InventoryBgSign?.Target;

        [Binding]
        public bool HasFiltrableTypeBgSign => FiltrableTypeBgSign != null;

        [Binding]
        public float Durability => FrequentStats.Durability;

        [Binding]
        public float DurabilityRatio => FrequentStats.DurabilityRatio;

        [Binding]
        public float DurabilityAbsRatio => FrequentStats.DurabilityAbsRatio;

        [Binding]
        public float DurabilityMaxCurrent => FrequentStats.DurabilityMaxCurrent;

        [Binding]
        public float DurabilityMaxCurrentRatio => FrequentStats.DurabilityMaxCurrentRatio;

        [Binding]
        public float DurabilityMaxCurrentInvertRatio => FrequentStats.DurabilityMaxCurrentInvertRatio;

        [Binding]
        public float DurabilityMaxAbsolute => FrequentStats.DurabilityMaxAbsolute;

        [Binding]
        public bool HasDurability => FrequentStats.HasDurability;

        [Binding]
        public bool IsBroken => FrequentStats.IsBroken;

        [Binding]
        public bool IsAlmostBroken => FrequentStats.IsAlmostBroken;

        public bool HasSortingPriority { get; protected set; }

        public InventoryTabType? TabType => null;


        //=== Unity ===========================================================

        protected virtual void Awake()
        {
            FrequentStats.PropertyChanged += OnSlotItemFrequentStatsPropertyChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            FrequentStats.PropertyChanged -= OnSlotItemFrequentStatsPropertyChanged;
        }


        //=== Public ==========================================================

        public ContextViewParams GetContextViewParamsForOpening()
        {
            return _contextViewParams;
        }        
        
        public void SetAcceptanceResolver(ISlotAcceptanceResolver slotAcceptanceResolver)
        {
            SlotAcceptanceResolver = slotAcceptanceResolver;
        }

        public void SetSlotId(int inventorySlotId)
        {
            _inventorySlotId = inventorySlotId;
        }

        public void SetContextActionsSource(IContextActionsSource contextActionsSource)
        {
            if (contextActionsSource.AssertIfNull(nameof(contextActionsSource)))
                return;

            ContextActionsSource = contextActionsSource;
        }

        public void BaseSubscribe(SlotsCollectionApi slotsCollectionApi, IHasContextStream hasContextStream = null)
        {
            Reset();

            SlotsCollectionApi = slotsCollectionApi;
            if (hasContextStream != null) //вполне м.б. пустым, например для слотов станка
            {
                HasContextStream = hasContextStream;

                hasContextStream.CurrentContext.Subscribe(_tempoD,
                    target =>
                    {
                        if (target != this)
                            IsSelected = false;
                    },
                    () =>
                    {
                        if (hasContextStream.ContextValue == this)
                            TakeContext(true);
                        hasContextStream = null;
                    }
                );
            }
        }

        public void BaseUnsubscribe()
        {
            SlotsCollectionApi = null;
            _tempoD.Clear();
        }

        public void Reset()
        {
            OnItemChanged(SlotId, new SlotItem(), 0);
            IsSelected = false;
        }

        public override string ToString()
        {
            var sb = ToStringWithoutEndPart();
            sb.Append("/svm]");
            return sb.ToString();
        }

        protected StringBuilder ToStringWithoutEndPart()
        {
            var sb = new StringBuilder($"[{GetType()}: [{SlotId}]");

            sb.Append(IsEmpty ? " empty" : $" {nameof(Stack)}={Stack}");

            if (ItemType != null)
                sb.Append($" {nameof(ItemType)}={ItemType}");

            if (IsSelected)
                sb.Append($" {nameof(IsSelected)}{IsSelected.AsSign()}");

            return sb;
        }

        public bool CanMoveToThis(SlotViewModel fromSlotViewModel)
        {
            return SlotAcceptanceResolver != null && SlotAcceptanceResolver.TryMoveTo(fromSlotViewModel, this) > 0;
        }

        public void OnDragToThis(SlotViewModel fromSlotViewModel)
        {
            SlotAcceptanceResolver?.TryMoveTo(fromSlotViewModel, this, true);
        }

        public bool OnThrowAwayFromThis()
        {
            return SlotAcceptanceResolver?.TryToDropFrom(this) ?? false;
        }

        public bool HasUnfilledStack(BaseItemResource forItemResource)
        {
            return UnfilledStack(forItemResource) > 0;
        }

        /// <summary>
        /// Недозаполненность стека айтемов в слоте
        /// </summary>
        /// <returns></returns>
        public virtual int UnfilledStack(BaseItemResource forItemResource)
        {
            if (forItemResource.AssertIfNull(nameof(forItemResource)) || (!IsEmpty && ItemResource != forItemResource))
                return 0;

            int maxStack = forItemResource.MaxStack;
            return maxStack - Stack;
        }

        public virtual void OnClick()
        {
            IsSelected = true; //безусловно выполняем

            if (HasContextStream != null) //если есть контекст - берем
                TakeContext();
        }

        public void SetBaseSortIndex(int baseSortIndex)
        {
            BaseSortIndex = baseSortIndex;
        }

        public int GetSortIndex(int sortOrder, ISortByItemTypeResolver sortByItemTypeResolver)
        {
            return BaseSortIndex + sortByItemTypeResolver.GetAdditionalSortingIndex(HasSortingPriority, sortOrder, ItemTypes);
        }


        //=== Protected =======================================================

        protected void TakeContext(bool withNull = false)
        {
            if (HasContextStream is IContextViewWithParams contextViewWithParams)
            {
                contextViewWithParams.SetContext(withNull ? null : this);
            }
            else if (HasContextStream is IContextView contextView)
            {
                contextView.TakeContext(withNull ? null : this);
            }
        }

        protected virtual void OnItemChanged(int slotIndex, SlotItem slotItem, int stackDelta)
        {
            if (slotItem.AssertIfNull(nameof(slotItem)))
                return;

            try
            {
                var oldItemIsNullOrDefault = ItemIsNullOrDefault;

                if (SelfSlotItem.Count != slotItem.Count)
                {
                    SelfSlotItem.Count = slotItem.Count;
                    NotifyPropertyChanged(nameof(Stack));
                    IsEmpty = GetIsEmpty();
                    IsMany = Stack > 1;
                }

                if (SelfSlotItem.ItemGuid != slotItem.ItemGuid)
                {
                    var oldItemName = ItemName;
                    var oldIcon = Icon;
                    var oldItemType = ItemType;
                    var oldFiltrableTypeBgSign = FiltrableTypeBgSign;
                    var oldHasFiltrableTypeBgSign = HasFiltrableTypeBgSign;
                    SelfSlotItem.ItemGuid = slotItem.ItemGuid;
                    SelfSlotItem.ItemResource = slotItem.ItemResource;
                    if (!oldItemName.Equals(ItemName))
                        NotifyPropertyChanged(nameof(ItemName));
                    if (oldIcon != Icon)
                        NotifyPropertyChanged(nameof(Icon));
                    if (oldItemType != ItemType)
                        NotifyPropertyChanged(nameof(ItemType));
                    if (oldFiltrableTypeBgSign != FiltrableTypeBgSign)
                        NotifyPropertyChanged(nameof(FiltrableTypeBgSign));
                    if (oldHasFiltrableTypeBgSign != HasFiltrableTypeBgSign)
                        NotifyPropertyChanged(nameof(HasFiltrableTypeBgSign));
                }

                if (ItemIsNullOrDefault != oldItemIsNullOrDefault)
                    NotifyPropertyChanged(nameof(ItemIsNullOrDefault));

                if (!StatModifier.Equals(SelfSlotItem.GeneralStats, slotItem.GeneralStats))
                    SlotItem.StatsClone(slotItem.GeneralStats, ref SelfSlotItem.GeneralStats);

                if (!StatModifier.Equals(SelfSlotItem.SpecificStats, slotItem.SpecificStats))
                    SlotItem.StatsClone(slotItem.SpecificStats, ref SelfSlotItem.SpecificStats);

                SlotItem.AmmoContainersClone(slotItem.InnerContainers, SelfSlotItem.InnerContainers);
                HasAmmoContainer = GetHasAmmoContainer();
                AmmoCount = GetAmmoCount();
                AmmoMaxCount = GetAmmoMaxCount();
                HasAmmo = GetHasAmmo();
                ContainerItemName = GetContainerItemName();

                FrequentStats.Reset();

                if (IsSelected && IsEmpty)
                {
                    IsSelected = false;
                    if (HasContextStream != null)
                        TakeContext(true);
                }

                SlotChanged?.Invoke(this, stackDelta);
            }
            catch (Exception e)
            {
                UI.Logger.Error($"{name}: {nameof(OnItemChanged)}([{slotIndex}], {slotItem}, delta={stackDelta}) " +
                                $"exception: {e.Message}\n{e.StackTrace}");
            }
        }

        protected virtual void OnFrequentStatsChanged(int slotIndex, List<KeyValuePair<StatResource, float>> stats)
        {
            FrequentStats.OnStatsChanged(stats);
        }

        protected bool GetIsEmpty()
        {
            return Stack == 0;
        }

        protected virtual void OnContextViewTargetChanged(IContextViewTarget target)
        {
            var asSlotViewModel = target as SlotViewModel;
            if (asSlotViewModel == null || asSlotViewModel != this)
                IsSelected = false;
        }


        //=== Private =========================================================

        private void OnSlotItemFrequentStatsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }

        private string GetAmmoInfo()
        {
            return $"HasAmmoContainer{HasAmmoContainer.AsSign()} {AmmoCount}/{AmmoMaxCount}";
        }

        private bool GetHasAmmoContainer()
        {
            return !IsEmpty && SelfSlotItem != null && SelfSlotItem.HasInnerContainer;
        }

        private bool GetHasAmmo()
        {
            return HasAmmoContainer && AmmoCount > 0;
        }

        private int GetAmmoCount()
        {
            return SelfSlotItem?.InnerItemsCount ?? 0;
        }

        private int GetAmmoMaxCount()
        {
            return SelfSlotItem?.AmmoMaxCount ?? 0;
        }

        private string GetContainerItemName()
        {
            var innerItemResource = SelfSlotItem.InnerContainer?.ItemResource;
            return innerItemResource != null ? $"({innerItemResource.ItemNameLs.GetText()})" : "";
        }
    }
}