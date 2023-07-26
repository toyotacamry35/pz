using ReactivePropsNs;
using Uins.Inventory;

namespace Uins
{
    public interface IHasContextStream
    {
        IStream<IContextViewTarget> CurrentContext { get; }
        IContextViewTarget ContextValue { get; }
    }
}