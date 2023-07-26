using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Item.Templates;

namespace ResourceSystem.Aspects.Item.Templates
{
    public class ItemsCollectionDef : BaseResource
    {
        public Dictionary<string,ResourceArray<ItemResource>> Items;
    }
}