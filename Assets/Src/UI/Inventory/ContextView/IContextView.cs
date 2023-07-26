using Uins.Inventory;

namespace Uins
{
    public interface IContextView : IHasContextStream
    {
        void TakeContext(IContextViewTarget contextViewTarget);
    }
}