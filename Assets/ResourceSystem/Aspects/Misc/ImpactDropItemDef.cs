using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace ColonyShared.ManualDefsForSpells
{
    public class ImpactDropItemDef : SpellImpactDef
    {
        public ContainerType Container { get; set; } = ContainerType.Inventory;
        public int Slot { get; set; } = 0;
        public int Count { get; set; } = 1;
        public ResourceRef<SpellEntityDef> Target { get; set; }
    }
}