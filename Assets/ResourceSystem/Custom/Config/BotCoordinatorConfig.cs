using Assets.Src.ResourcesSystem.Base;
using SharedCode.Config;
using SharedCode.Entities.GameObjectEntities;

namespace Assets.ResourceSystem.Custom.Config
{
    public class BotCoordinatorConfig : CustomConfig
    {
        public ResourceRef<SpawnPointTypeDef> BotSpawnPointTypeDef { get; set; }
    }
}
