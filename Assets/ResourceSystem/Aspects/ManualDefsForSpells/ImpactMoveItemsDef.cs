using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Wizardry;

namespace ColonyShared.ManualDefsForSpells
{
    public class ImpactMoveItemsDef : SpellImpactDef
    {
        public EntityWithAddress From { get; set; }
        public EntityWithAddress To { get; set; }
    }

    public class EntityWithAddress
    {
        public ResourceRef<SpellEntityDef> Entity { get; set; }
        public ContainerType Container { get; set; }
        public ResourceRef<SlotDef> Slot { get; set; }
        public int SlotId { get; set; } = -1;
        public int Count { get; set; } = 1;
    }

    public enum ContainerType
    {
        Doll, 
        Inventory
    }
}