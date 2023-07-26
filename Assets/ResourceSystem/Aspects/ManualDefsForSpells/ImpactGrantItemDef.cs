using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactGrantItemDef : SpellImpactDef
    {
        public bool GrantRandom { get; set; }
        //public List<ItemStack> GrantingItems { get; set; }
        public ResourceRef<SpellEntityDef> Receiver { get; set; }
        public ResourceRef<SpellEntityDef> InteractiveGranter { get; set; }
    }
}
