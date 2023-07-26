using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Wizardry;

namespace ColonyShared.ManualDefsForSpells
{
    public class ImpactRemoveItemDef : SpellImpactDef
    {
        public ResourceRef<BaseItemResource> Item { get; set; }
        public int Count { get; set; } = 1;
        public ResourceRef<SpellEntityDef> Target { get; set; }
    }
}