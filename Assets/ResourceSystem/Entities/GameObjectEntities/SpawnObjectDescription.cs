using Assets.ResourceSystem.Aspects.Links;
using Assets.Src.ResourcesSystem.Base;
using ResourceSystem.ContentKeys;

namespace SharedCode.Entities.GameObjectEntities
{
    public class SpawnObjectDescription
    {
        public ResourceRef<IEntityObjectDef> Object { get; set; }
        public int Amount { get; set; }
        public ResourceRef<LinkTypeDef> RememberAsLink { get; set; }
        public ResourceRef<SpawnPointTypeDef> SpawnOnPoint { get; set; }

        public ContentKeyRequirement ContentKey { get; set; }
    }
}
