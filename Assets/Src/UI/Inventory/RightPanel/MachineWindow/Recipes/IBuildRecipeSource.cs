using SharedCode.Aspects.Building;

namespace Uins.Inventory
{
    public interface IBuildRecipeSource
    {
        BuildRecipeDef BuildRecipeDef { get; }
    }
}