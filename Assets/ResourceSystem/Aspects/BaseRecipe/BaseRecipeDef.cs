using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.Aspects.Counters.Template;
using L10n;
using SharedCode.Aspects.Item.Templates;
using UnityEngine;

namespace Assets.ColonyShared.SharedCode.Aspects
{
    public abstract class BaseRecipeDef : SaveableBaseResource, IRecipeRewardSource
    {
        public LocalizedString NameLs { get; set; }

        public LocalizedString DescriptionLs { get; set; }

        public int Tier { get; set; }

        public int Level { get; set; }

        public UnityRef<Sprite> BlueprintIcon { get; set; }

        public ResourceRef<InventoryFiltrableTypeDef> InventoryFiltrableType { get; set; }

        public  int SortingIndex { get; set; }

        BaseRecipeDef IRecipeRewardSource.Recipe => this;
    }
}