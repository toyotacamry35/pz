namespace Uins.Inventory
{
    public class CharStatsSideContextViewTarget : BaseSideContextViewTarget
    {
        public override InventoryTabType? TabType => InventoryTabType.PlayerStats;
        protected override ContextViewParams.LayoutType Layout => ContextViewParams.LayoutType.NoTitleAndBackground;
    }
}