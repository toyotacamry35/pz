namespace Uins.Inventory
{
    public class PerksSideContextViewTarget : BaseSideContextViewTarget
    {
        protected override ContextViewParams.LayoutType Layout => ContextViewParams.LayoutType.ExtraSpaceWithPointsPanels;
        public override InventoryTabType? TabType => InventoryTabType.Perks;
    }
}