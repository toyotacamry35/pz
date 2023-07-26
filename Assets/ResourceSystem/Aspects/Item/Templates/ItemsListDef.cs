using Assets.Src.ResourcesSystem.Base;

namespace SharedCode.Aspects.Item.Templates
{
    public class ItemsListDef : BaseResource
    {
        public ResourceRef<BaseItemResource>[] Items { get; set; }
    }

    public struct ItemPackDef
    {
        public ResourceRef<BaseItemResource> Item { get; set; }
        public uint Count { get; set; }
    }
}
