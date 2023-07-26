using JetBrains.Annotations;
using UnityEngine;

namespace Uins.Tooltips
{
    public class MachineRecipeDetailsSlotTooltipDescription : BaseTooltipDescription
    {
        [UsedImplicitly]
        [SerializeField]
        private SidePanelRecipeSlotViewModel _sidePanelRecipeSlotViewModel;

        public override bool HasDescription => _sidePanelRecipeSlotViewModel != null && !_sidePanelRecipeSlotViewModel.IsEmpty;

        public override object Description => HasDescription ? _sidePanelRecipeSlotViewModel : null;

        private void Awake()
        {
            _sidePanelRecipeSlotViewModel.AssertIfNull(nameof(_sidePanelRecipeSlotViewModel), gameObject);
        }
    }
}