using Uins.Slots;

namespace Uins
{
    public enum DropTargetKind
    {
        None,
        SlotViewModel,
        ThrowAwayTarget
    }

    public interface IDropTarget
    {
        DropTargetKind Kind { get; }

        SlotViewModel Target { get; }
    }
}