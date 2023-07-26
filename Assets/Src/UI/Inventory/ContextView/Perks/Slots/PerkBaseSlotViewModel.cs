using Assets.Src.ContainerApis;
using Assets.Src.Inventory;
using SharedCode.Aspects.Item.Templates;
using UnityWeld.Binding;

namespace Uins.Slots
{
    [Binding]
    public abstract class PerkBaseSlotViewModel : SlotViewModel
    {
        public enum PerksCollection
        {
            Temporary,
            Permanent,
            Saved
        }

        public PerksCollection Collection;

        public DraggableItem DraggableItem;

        public event SlotItemChangedDelegate ItemChanged;


        //=== Props ===========================================================

        public SlotsCollectionApi PerkSlotsCollectionApi { get; protected set; }

        public PerkSlotTypes PerkSlotTypes { get; set; }

        public ItemTypeResource PerkSlotType { get; private set; }

        public bool NeedForPerkSlotTypeSubscribe { get; set; }

        private bool _isLocked;

        [Binding]
        public bool IsLocked
        {
            get => _isLocked;
            protected set
            {
                if (_isLocked != value)
                {
                    _isLocked = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _itemTypeIndex;

        [Binding]
        public int ItemTypeIndex
        {
            get => _itemTypeIndex;
            protected set
            {
                if (_itemTypeIndex != value)
                {
                    _itemTypeIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _perkSlotTypeIndex;

        [Binding]
        public virtual int PerkSlotTypeIndex
        {
            get => _perkSlotTypeIndex;
            set
            {
                if (value != _perkSlotTypeIndex)
                {
                    _perkSlotTypeIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _screenedSlotTypeIndex;

        [Binding]
        public int ScreenedSlotTypeIndex
        {
            get => _screenedSlotTypeIndex;
            protected set
            {
                if (_screenedSlotTypeIndex != value)
                {
                    _screenedSlotTypeIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override ItemTypeResource[] ItemTypes => new[] {ItemType, PerkSlotType};


        //=== Unity ===========================================================

        protected override void Awake()
        {
            base.Awake();
            DraggableItem.AssertIfNull(nameof(DraggableItem));
        }


        //=== Public ==========================================================

        public override string ToString()
        {
            var sb = ToStringWithoutEndPart();
            sb.Append($" {nameof(IsLocked)}{IsLocked.AsSign()}, P/S-indices={ItemTypeIndex}/{PerkSlotTypeIndex} ");
            sb.Append("/svm]");
            return sb.ToString();
        }

        //=== Protected =======================================================

        protected override void OnItemChanged(int slotIndex, SlotItem slotItem, int stackDelta)
        {
            base.OnItemChanged(slotIndex, slotItem, stackDelta);

            ItemTypeIndex = PerkSlotTypes.GetTypeIndex(ItemType);
            PerkSlotTypeIndex = PerkSlotTypes.GetTypeIndex(PerkSlotType);
            ScreenedSlotTypeIndex = GetScreenedSlotTypeIndex();
            IsLocked = GetIsLocked();

            UpdateContextViewIsSelected();

            ItemChanged?.Invoke(slotIndex, SelfSlotItem, stackDelta);
        }

        protected virtual void OnPerkSlotTypeChanged(int slotIndex, ItemTypeResource perkSlotType)
        {
            PerkSlotType = perkSlotType;
            PerkSlotTypeIndex = PerkSlotTypes.GetTypeIndex(PerkSlotType);
            ScreenedSlotTypeIndex = GetScreenedSlotTypeIndex();
            IsLocked = GetIsLocked();

            UpdateContextViewIsSelected();
            ItemChanged?.Invoke(slotIndex, SelfSlotItem, 0);
        }

        protected virtual bool GetIsLocked()
        {
            return PerkSlotTypeIndex < 0;
        }

        //Базовый функционал: для выделенных, развыделять если VM стала пустой, перевыделять если изменилась
        protected virtual void UpdateContextViewIsSelected()
        {
            if (!IsSelected || HasContextStream == null)
                return;

            TakeContext(true);
            if (!IsEmpty)
            {
                TakeContext();
                IsSelected = true;
            }
        }


        //=== Private =========================================================

        private int GetScreenedSlotTypeIndex()
        {
            return SelfSlotItem.IsEmpty ? PerkSlotTypeIndex : -1;
        }
    }
}