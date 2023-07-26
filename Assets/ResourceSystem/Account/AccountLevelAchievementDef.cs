using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using SharedCode.Wizardry;
using UnityEngine;

namespace Assets.ResourceSystem.Account
{
    [Localized]
    public class AccountLevelAchievementDef : BaseResource
    {
        public LocalizedString Name { get; set; }
        public UnityRef<Sprite> Icon;
        public LocalizedString Description { get; set; }

        // These spells 'll be autocasted on resurrect, birth, ... for character of this or greater lvl:
        public ResourceRef<SpellDef>[] SpellsOnResurrect  { get; set; } = System.Array.Empty<ResourceRef<SpellDef>>();
        public ResourceRef<SpellDef>[] SpellsOnBirth      { get; set; } = System.Array.Empty<ResourceRef<SpellDef>>();
        public ResourceRef<SpellDef>[] SpellsOnEnterWorld { get; set; } = System.Array.Empty<ResourceRef<SpellDef>>();

    }
}
