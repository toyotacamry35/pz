using Uins.Slots;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class SlotTooltipViewModel : SlotPropsBaseViewModel
    {
        //=== Props ===========================================================

        //Обертка для TargetSlotViewModel под передачу через TooltipDescription
        public override object TargetDescription
        {
            get => TargetSlotViewModel;
            set
            {
                var slotViewModel = value as SlotViewModel;
                if (slotViewModel.AssertIfNull(nameof(slotViewModel)))
                    return;

                TargetSlotViewModel = slotViewModel;
            }
        }


        //=== Protected =======================================================

        protected override void OnAwake()
        {
        }
    }
}