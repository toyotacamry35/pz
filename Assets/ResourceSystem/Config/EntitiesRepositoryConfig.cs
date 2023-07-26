using System;
using Assets.Src.ResourcesSystem.Base;
using GeneratedCode.Custom.Config;
using SharedCode.Aspects.Sessions;

namespace SharedCode.Config
{
    public class RepositoryPortConfig : BaseResource
    {
        public int InternalBindPort { get; set; }
        public int ExternalBindPort { get; set; }
        public int InternalAnnouncePort { get; set; }
        public int ExternalAnnouncePort { get; set; }
    }

    public class EntitiesRepositoryConfig : BaseResource
    {
        public bool NotifiesOtherRepositoriesOfIncommingConnections { get; set; } = false;
        public string ConfigId { get; set; }

        public ResourceRef<RepositoryPortConfig> Ports { get; set; }

        public ResourceRef<ServerServicesConfigDef> Addresses { get; set; }

        public int Count { get; set; } = 1;

        public bool SuppressEntityInitialization { get; set; }

        public ResourceRef<CloudEntityConfig>[] ServiceEntities { get; set; } = Array.Empty<ResourceRef<CloudEntityConfig>>();
    }

    public class CloudEntityConfig : BaseResource
    {
        public string CloudEntityType { get; set; }

        public ResourceRef<CustomConfig> CustomConfig { get; set; }
    }

    public abstract class CustomConfig : BaseResource
    {
    }
    public class MapBootstrapperDef : BaseResource
    {
        public ResourceRef<RealmRulesQueryDef> RealmRulesQuery { get; set; }
        //public ResourceRef<MapDef> Map { get; set; }
        public Guid MapId { get; set; }
    }

    public class WorldCoordinatorConfig : CustomConfig
    {
        public Guid RealmsCollectionId { get; set; }
        public ResourceRef<MapBootstrapperDef>[] PreloadedMaps { get; set; } = Array.Empty<ResourceRef<MapBootstrapperDef>>();
    }
}

