using System.Collections.Generic;
using Assets.Src.ContainerApis;
using Assets.Src.Inventory;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using Assets.Src.ResourceSystem;
using SharedCode.Aspects.Item.Templates;
using UnityEngine;
using UnityWeld.Binding;
using Uins.Sound;

namespace Uins.Slots
{
    [Binding]
    public class CharDollSlotViewModel : SlotViewModel, ICanBeInaccessible
    {
        public SlotDefRef SlotDefRef;

        protected IWeaponSelector WeaponSelector;


        //=== Props ===========================================================

        public SlotDef SlotDef => SlotDefRef != null ? SlotDefRef.Target : null;

        public override CollectionType SlotCollectionType => CollectionType.Doll;

        public override bool IsOurPlayerSlot => true;

        public override int SlotId
        {
            get
            {
                var id = SlotDef != null ? SlotDef.SlotId : -1;
                return id;
            }
        }

        public ResourceIDFull SlotResourceIDFull => GameResourcesHolder.Instance.GetID(SlotDef);

        public override bool DoEquipSound => SlotId < SoundControl.MaxDollSlotIndexForSound;

        private Sprite _slotBackgroundIcon;

        [Binding]
        public Sprite SlotBackgroundIcon
        {
            get
            {
                if (_slotBackgroundIcon != SlotDef.BackgroundIcon?.Target)
                {
                    _slotBackgroundIcon = SlotDef.BackgroundIcon?.Target;
                    //NotifyPropertyChanged();
                }

                return _slotBackgroundIcon;
            }
        }

        private bool _isSlotBackgroundIconVisible;

        [Binding]
        public bool IsSlotBackgroundIconVisible
        {
            get => _isSlotBackgroundIconVisible;
            private set
            {
                if (_isSlotBackgroundIconVisible != value)
                {
                    _isSlotBackgroundIconVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isWeaponSelected;

        [Binding]
        public bool IsWeaponSelected
        {
            get => _isWeaponSelected;
            set
            {
                if (_isWeaponSelected != value)
                {
                    _isWeaponSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isInUse;

        [Binding]
        public bool IsInUse
        {
            get => _isInUse;
            set
            {
                if (_isInUse != value)
                {
                    _isInUse = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isInaccessible;

        [Binding]
        public bool IsInaccessible
        {
            get => _isInaccessible;
            set
            {
                if (_isInaccessible != value)
                {
                    _isInaccessible = value;
                    NotifyPropertyChanged();
                    if (_isInaccessible && IsSelected)
                    {
                        IsSelected = false;
                        if (HasContextStream != null)
                            TakeContext(true);
                    }
                }
            }
        }


        //=== Unity ===========================================================

        protected override void Awake()
        {
            base.Awake();
            SlotDefRef.Target.AssertIfNull(nameof(SlotDefRef));
        }


        //=== Public ==========================================================

        public void Subscribe(HasDollBroadcastApi hasDollBroadcastApi, HasDollFullApi hasDollFullApi, IHasContextStream hasContextStream = null)
        {
            BaseSubscribe(hasDollBroadcastApi, hasContextStream);
            hasDollBroadcastApi.SubscribeToUsedSlotsChanged(OnUsedSlotsChanged);
            hasDollFullApi.SubscribeToSlot(SlotId, OnItemChanged);
            hasDollFullApi.SubscribeToSlotFrequentStats(SlotId, OnFrequentStatsChanged);
            hasDollFullApi.SubscribeToInaccessibleSlots(OnInaccessibleSlotsChanged);
        }

        public void Unsubscribe(HasDollBroadcastApi hasDollBroadcastApi, HasDollFullApi hasDollFullApi)
        {
            BaseUnsubscribe();
            hasDollFullApi.UnsubscribeFromSlot(SlotId, OnItemChanged);
            hasDollFullApi.UnsubscribeFromSlotFrequentStats(SlotId, OnFrequentStatsChanged);
            hasDollFullApi.UnsubscribeFromInaccessibleSlots(OnInaccessibleSlotsChanged);
            hasDollBroadcastApi.UnsubscribeFromUsedSlotsChanged(OnUsedSlotsChanged);
            if (WeaponSelector != null)
            {
                WeaponSelector.SelectedSlotChanged -= OnSelectedWeaponSlotIndex;
                WeaponSelector = null;
            }

            IsInUse = false;
            IsWeaponSelected = false;
            IsInaccessible = false;
            Reset();
        }

        public void SetWeaponSelector(IWeaponSelector weaponSelector)
        {
            if (weaponSelector.AssertIfNull(nameof(weaponSelector)))
                return;

            WeaponSelector = weaponSelector;
            weaponSelector.SelectedSlotChanged += OnSelectedWeaponSlotIndex;
        }

        public override int UnfilledStack(BaseItemResource forItemResource)
        {
            if (forItemResource.AssertIfNull(nameof(forItemResource)) || (!IsEmpty && ItemResource != forItemResource))
                return 0;

            int maxStack = Mathf.Min(forItemResource.MaxStack, SlotDef.MaxStack);
            return maxStack - Stack;
        }

        public override string ToString()
        {
            return $"{base.ToString()} {nameof(SlotDef)}={SlotDef}, {nameof(IsInUse)}{IsInUse.AsSign()}, " +
                   $"{nameof(IsWeaponSelected)}{IsWeaponSelected.AsSign()}";
        }


        //=== Protected =======================================================

        protected override void OnItemChanged(int slotIndex, SlotItem slotItem, int stackDelta)
        {
            base.OnItemChanged(slotIndex, slotItem, stackDelta);
            IsSlotBackgroundIconVisible = SlotBackgroundIcon != null && Stack == 0;
        }

        protected virtual void OnSelectedWeaponSlotIndex(int selectedWeaponSlotIndex)
        {
            IsWeaponSelected = selectedWeaponSlotIndex == SlotId;
        }

        protected virtual void OnUsedSlotsChanged(IList<ResourceIDFull> usedSlotsIndices)
        {
            IsInUse = usedSlotsIndices.Contains(SlotResourceIDFull);
        }


        //=== Private =========================================================

        private void OnInaccessibleSlotsChanged(int slotId, bool isAdded)
        {
            if (slotId != SlotId)
                return;

            IsInaccessible = isAdded;
        }
    }
}