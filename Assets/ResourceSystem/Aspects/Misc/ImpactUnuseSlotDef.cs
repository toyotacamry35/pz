using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Wizardry;

namespace ResourceSystem.Aspects.Misc
{
    public class ImpactUnuseSlotDef : SpellImpactDef
    {
        public ResourceArray<SlotDef> Slots { get; [UsedImplicitly] set; } 
        public ResourceRef<SlotsListDef> SlotsList { get; [UsedImplicitly] set; } 
        public ResourceRef<SpellEntityDef> Target { get; set; }
    }
}