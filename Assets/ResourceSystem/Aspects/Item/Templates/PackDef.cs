using Assets.Src.ResourcesSystem.Base;

namespace SharedCode.Aspects.Item.Templates
{
    public struct PackDef
    {
        public ResourceRef<SlotDef>[] BlockSlots { get; set; }
        public ResourceRef<SlotDef>[] UnblockSlots { get; set; }
        public int ExtraInventorySlots { get; set; }
    }
}