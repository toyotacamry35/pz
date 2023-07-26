using System.Linq;
using L10n;
using SharedCode.Aspects.Building;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class MainConstructionButton : ConstructionButton
    {
        public IGrouping<BuildRecipeGroupDef, BuildRecipeDef> Group { get; private set; }

        public void Init(IGrouping<BuildRecipeGroupDef, BuildRecipeDef> group)
        {
            if (group != null && group.Any())
            {
                HasDef = true;
                var recipeGroupDef = group.Key;
                Icon = recipeGroupDef.Icon?.Target;
                Title = recipeGroupDef.NameLs;
                Group = group;
            }
            else
            {
                HasDef = false;
                Icon = null;
                Title = LsExtensions.Empty;
                Group = null;
            }
        }
    }
}