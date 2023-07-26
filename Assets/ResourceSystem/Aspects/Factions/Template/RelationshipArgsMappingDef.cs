using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using ResourceSystem.Reactions;
using ResourceSystem.Utils;

namespace Assets.Src.Aspects.Impl.Factions.Template
{
    public class RelationshipArgsMappingDef : BaseResource
    {
        public ResourceRef<ArgDef<OuterRef>> ThisEntity;
        public ResourceRef<ArgDef<OuterRef>> OtherEntity;
    }
}