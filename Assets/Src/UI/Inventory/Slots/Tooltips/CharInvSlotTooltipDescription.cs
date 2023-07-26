using Uins.Slots;

namespace Uins.Tooltips
{
    public class CharInvSlotTooltipDescription : SlotTooltipDescription
    {
        private CharInvSlotViewModel CharInvSlotViewModel => _slotViewModel as CharInvSlotViewModel;

        public override bool HasDescription => CharInvSlotViewModel != null && !CharInvSlotViewModel.IsEmpty;

        public override object Description => HasDescription ? CharInvSlotViewModel : null;

        private void Awake()
        {
            CharInvSlotViewModel.AssertIfNull(nameof(CharInvSlotViewModel), gameObject);
        }
    }
}