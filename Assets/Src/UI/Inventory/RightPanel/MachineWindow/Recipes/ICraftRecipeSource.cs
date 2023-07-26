using Assets.ColonyShared.SharedCode.Aspects.Craft;

namespace Uins.Inventory
{
    public interface ICraftRecipeSource
    {
        CraftRecipeDef CraftRecipeDef { get; }
    }
}