using SharedCode.Aspects.Item.Templates;

namespace Uins.Slots
{
    public interface ISortByItemTypeResolver
    {
        int GetAdditionalSortingIndex(bool hasSortingPriority, int sortingOrder, ItemTypeResource[] itemTypes);
    }
}