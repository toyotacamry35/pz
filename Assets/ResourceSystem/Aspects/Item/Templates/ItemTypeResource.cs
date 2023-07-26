using Assets.Src.ResourcesSystem.Base;
using L10n;

namespace SharedCode.Aspects.Item.Templates
{
    [Localized]
    public class ItemTypeResource : SaveableBaseResource
    {
        public ResourceRef<ItemTypeResource> Parent { get; set; }
        public bool HideVisualAtSlot { get; set; }
        public LocalizedString DescriptionLs { get; set; }
    }
}
