using SharedCode.EntitySystem;
using System;

namespace Assets.ColonyShared.SharedCode.Aspects.Craft
{
    public interface ICraftRecipe : IDeltaObject
    {
        CraftRecipeDef Def { get; set; }

        bool IsAvailable { get; set; }

        [Obsolete]
        int Level { get; set; }
    }
}