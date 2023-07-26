using Assets.Src.ResourcesSystem.Base;

namespace SharedCode.Aspects.Science
{
    public class TechnologyAtlasDef : BaseResource
    {
        public ResourceRef<TechnologyTabDef>[] Tabs { get; set; }
    }
}