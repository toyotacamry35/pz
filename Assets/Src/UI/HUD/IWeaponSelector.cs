using System;
using System.Threading.Tasks;
using SharedCode.Aspects.Item.Templates;

namespace Uins.Slots
{
    public delegate void SelectedSlotChangesDelegate(int slotIndex);

    public interface IWeaponSelector
    {
        event SelectedSlotChangesDelegate SelectedSlotChanged;

        int SelectedWeaponSlotIndex { get; }

        void SlotSelectionRequest(int newSelectedWeaponSlotIndex = -1);

        Task SlotUsageRequest(SlotDef slotDef, bool isInUse);
    }
}