using Assets.Src.ContainerApis;
using Assets.Src.Inventory;
using SharedCode.Aspects.Item.Templates;
using UnityWeld.Binding;

namespace Uins.Slots
{
    [Binding]
    public abstract class MayBeFactionPerkSlotViewModel<T, U> : PerkCollectionApiSlotViewModel<T, U>
        where T : PerksBaseFullApi
        where U : PerkSlotsBaseFullApi
    {
        private IFactionStagePerksResolver _factionStagePerksResolver;


        //=== Props ===========================================================

        private bool _isFactionStagePerk;

        [Binding]
        public bool IsFactionStagePerk
        {
            get => _isFactionStagePerk;
            private set
            {
                if (_isFactionStagePerk != value)
                {
                    _isFactionStagePerk = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isNormalPerk;

        [Binding]
        public bool IsNormalPerk
        {
            get => _isNormalPerk;
            protected set
            {
                if (_isNormalPerk != value)
                {
                    _isNormalPerk = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Public ==========================================================

        public void SetFactionStagePerksResolver(IFactionStagePerksResolver factionStagePerksResolver)
        {
            _factionStagePerksResolver = factionStagePerksResolver;
        }


        //=== Protected =======================================================

        protected override void OnItemChanged(int slotIndex, SlotItem slotItem, int stackDelta)
        {
            base.OnItemChanged(slotIndex, slotItem, stackDelta);
            CustomPropsCheck();
        }

        protected override void OnPerkSlotTypeChanged(int slotIndex, ItemTypeResource perkSlotType)
        {
            base.OnPerkSlotTypeChanged(slotIndex, perkSlotType);
            CustomPropsCheck();
        }


        //=== Private =========================================================

        private void CustomPropsCheck()
        {
            HasSortingPriority = IsFactionStagePerk = _factionStagePerksResolver?.GetIsFactionStagePerk(ItemResource) ?? false;
            IsNormalPerk = !IsLocked && !IsFactionStagePerk;
        }
    }
}