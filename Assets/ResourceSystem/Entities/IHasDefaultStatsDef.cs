using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.Src.ResourcesSystem.Base;

namespace SharedCode.Aspects.Item.Templates
{
    public interface IHasDefaultStatsDef
    {
        ResourceRef<ItemSpecificStats> DefaultStats { get; set; }
    }
}