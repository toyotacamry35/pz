using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using UnityEngine;

namespace SharedCode.Aspects.Item.Templates
{
    public interface IHasStatsResource
    {
        ResourceRef<ItemGeneralStats> GeneralStats { get; set; }

        ResourceRef<ItemSpecificStats> SpecificStats { get; set; }
    }
}
