using SharedCode.Aspects.Item.Templates;
using Uins.Slots;

namespace Assets.Src.Inventory
{
    public interface ICharacterItemsNotifier
    {
        event SlotChangedDelegate CharacterItemsChanged;
        int GetItemResourceCount(BaseItemResource itemResource);
    }
}
