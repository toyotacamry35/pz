using JetBrains.Annotations;
using Uins.Slots;
using UnityEngine;

namespace Uins.Tooltips
{
    public class CharDollSlotTooltipDescription : SlotTooltipDescription
    {
        [UsedImplicitly]
        [SerializeField]
        private CharDollSlotViewModel CharDollSlotViewModel => _slotViewModel as CharDollSlotViewModel;

        public override bool HasDescription => CharDollSlotViewModel != null && !CharDollSlotViewModel.IsEmpty;

        public override object Description => HasDescription ? CharDollSlotViewModel : null;

        private void Awake()
        {
            CharDollSlotViewModel.AssertIfNull(nameof(CharDollSlotViewModel), gameObject);
        }
    }
}