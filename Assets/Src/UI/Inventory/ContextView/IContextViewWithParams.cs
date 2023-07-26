using Uins.Inventory;

namespace Uins
{
    public interface IContextViewWithParams : IHasContextStream
    {
        void SetContext(IContextViewTargetWithParams target);
    }
}