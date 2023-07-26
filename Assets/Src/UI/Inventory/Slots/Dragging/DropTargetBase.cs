using Uins.Slots;

namespace Uins
{
    public class DropTargetBase : BindingViewModel, IDropTarget
    {
        public virtual DropTargetKind Kind => DropTargetKind.SlotViewModel;

        public virtual SlotViewModel Target => null;
    }
}