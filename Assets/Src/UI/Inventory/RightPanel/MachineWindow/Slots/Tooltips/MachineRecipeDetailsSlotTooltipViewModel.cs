using L10n;
using SharedCode.Aspects.Item.Templates;
using UnityWeld.Binding;

namespace Uins
{
    /// <summary>
    /// Тултип-VM для ингредиента рецепта автостанка, показываемого в окошке детальной информации
    /// </summary>
    [Binding]
    public class MachineRecipeDetailsSlotTooltipViewModel : ItemPropsBaseViewModel
    {
        //=== Props ===========================================================

        private SidePanelRecipeSlotViewModel _targetViewModel;

        public override object TargetDescription
        {
            get => _targetViewModel;
            set
            {
                var sidePanelRecipeSlotViewModel = value as SidePanelRecipeSlotViewModel;
                if (sidePanelRecipeSlotViewModel.AssertIfNull(nameof(sidePanelRecipeSlotViewModel)))
                    return;

                _targetViewModel = sidePanelRecipeSlotViewModel;
                var itemResource = _targetViewModel?.ItemResource as ItemResource;

                BigIcon = itemResource?.BigIcon.Target;
                ItemName = itemResource?.ItemNameLs ?? LsExtensions.Empty;
                Description = itemResource?.DescriptionLs ?? LsExtensions.Empty;
                ItemTier = itemResource?.Tier ?? 0;
                ItemWeight = itemResource?.Weight ?? 0;
                InventoryFiltrableType = itemResource?.InventoryFiltrableType;

                var regularStats = GetItemRegularStats(itemResource);
                var mainStats = SlotPropsBaseViewModel.GetMainStats(itemResource, regularStats);
                StatsViewModel.SetItemStats(mainStats, regularStats);
            }
        }


        //=== Protected =======================================================

        protected override void OnAwake()
        {
        }
    }
}