using System.Collections.Generic;
using Uins.Slots;

namespace Uins
{
    public delegate void ShownContextMenuEvent(List<ContextMenuItemData> contextMenuItems);

    public interface IContextActionsSource
    {
        List<ContextMenuItemData> OnContextButtonsRequest(SlotViewModel slotViewModel);

        event ShownContextMenuEvent ShowContextMenu;
        void OnContextMenuRequest(SlotViewModel slotViewModel);
        void CloseContextMenuRequest();

        void ExecuteDefaultAction(SlotViewModel slotViewModel, bool isLimitedSet);
    }
}