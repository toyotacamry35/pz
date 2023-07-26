using System;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Config;

namespace Infrastructure.Config
{
    public class Command : BaseResource
    {
        public string Path { get; set; }
        public string Arguments { get; set; }
    }

    public class ExternalContainerConfig : BaseResource
    {
        public ResourceRef<ContainerConfig> Container { get; set; }

        public ResourceRef<Command>[] Commands { get; set; } = Array.Empty<ResourceRef<Command>>();
    }

    public class ContainerConfig : BaseResource
    {
        public string Name { get; set; } = string.Empty;        

        public bool CanDisableRepositoryGetEntitiesTimeout { get; set; } = true;

        public bool CollectStackTraces { get; set; } = false;

        public bool EnableEntityUsagesAndEventsTimeouts { get; set; } = true;

        public bool CollectOperationsLog { get; set; } = false;

        public bool CollectChainCallHistory { get; set; } = false;

        public bool CollectReplicationHistory { get; set; } = false;

        public bool CollectSubscriptionsHistory { get; set; } = false;
        
        public bool CollectEntityLifecycleHistory { get; set; } = false;

        public bool MockUnityServices { get; set; }

        public ResourceRef<EntitiesRepositoryConfig>[] EntitiesRepositories { get; set; } = Array.Empty<ResourceRef<EntitiesRepositoryConfig>>();
        public ResourceRef<ExternalContainerConfig>[] ExternalContainers { get; set; } = Array.Empty<ResourceRef<ExternalContainerConfig>>();
    }
}
