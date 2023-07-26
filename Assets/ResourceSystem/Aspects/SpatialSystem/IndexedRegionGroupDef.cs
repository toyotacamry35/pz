using System;
using System.ComponentModel;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using SharedCode.Aspects.Regions;

namespace Assets.Src.SpatialSystem
{
    [Localized]
    public class IndexedRegionGroupDef : ARegionDef, ISaveableResource
    {
        public Guid Id { get; set; }

        public ResourceRef<LocalizationKeysDef> TitleJdbRef { get; set; }

        public string TitleJdbKey { get; set; }
    }
}