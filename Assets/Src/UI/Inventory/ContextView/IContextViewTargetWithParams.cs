namespace Uins.Inventory
{
    public interface IContextViewTargetWithParams : IContextViewTarget
    {
        ContextViewParams GetContextViewParamsForOpening();
        InventoryTabType? TabType { get; }
    }
}