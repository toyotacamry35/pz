using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;

namespace SharedCode.Aspects.Item.Templates
{
    public class DollSlotTypeResource : BaseResource
    {
        public string DollSlotTypeName { get; set; }

        public List<ResourceRef<ItemTypeResource>> AllowedItemTypes { get; set; }
    }
}
