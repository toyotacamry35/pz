using System.Linq;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using SharedCode.Wizardry;

namespace SharedCode.Aspects.Item.Templates
{
    [Localized]
    public class ConsumDef : BaseResource
    {
        public SpellsGroup[] SpellsGroups { get; set; } = System.Array.Empty<SpellsGroup>();

        public bool HasSpells => SpellsGroups != null && SpellsGroups.SelectMany(group => group.Spells).Select(sd => sd.Target).Any();
    }

    [Localized]
    public struct SpellsGroup
    {
        public LocalizedString ActionTitleLs { get; set; }

        public bool DontRemoveWhenUse { get; set; }
        public ResourceRef<SpellDef>[] Spells { get; set; }
    }
}