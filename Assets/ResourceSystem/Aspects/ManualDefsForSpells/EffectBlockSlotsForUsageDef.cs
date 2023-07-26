using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Wizardry;

namespace ResourceSystem.Aspects.ManualDefsForSpells
{
    public class EffectBlockSlotsForUsageDef : SpellEffectDef
    {
        public ResourceArray<SlotDef> Slots { get; [UsedImplicitly] set; } 
        public ResourceRef<SlotsListDef> SlotsList { get; [UsedImplicitly] set; } 
        public ResourceRef<SpellEntityDef> Target { get; set; }
    }
}