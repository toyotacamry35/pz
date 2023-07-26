using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;

namespace SharedCode.Entities.GameObjectEntities
{
    [KnownToGameResources]
    public struct SpawnPoints
    {
        public ResourceRef<SpawnPointTypeDef> PointType { get; set; }
        public SpawnTemplatePoint[] Points { get; set; }
    }
}
