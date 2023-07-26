using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResourceSystem.ContentKeys
{
    public class ContentKeyDef : BaseResource
    {
    }
    [KnownToGameResources]
    public struct ContentKeyRequirement
    {
        public ResourceRef<ContentKeyDef> Key { get; set; }
        public bool NotIncludedByDefault { get; set; }
    }
}
