using JetBrains.Annotations;
using Uins.Slots;
using UnityEngine;

namespace Uins.Tooltips
{
    public class SlotTooltipDescription : BaseTooltipDescription
    {
        [SerializeField, UsedImplicitly]
        protected SlotViewModel _slotViewModel;

        public override bool HasDescription => _slotViewModel != null && !_slotViewModel.IsEmpty;

        public override object Description => HasDescription ? _slotViewModel : null;

        private void Awake()
        {
            _slotViewModel.AssertIfNull(nameof(_slotViewModel), gameObject);
        }
    }
}