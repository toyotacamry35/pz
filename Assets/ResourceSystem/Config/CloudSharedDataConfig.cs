using Assets.Src.ResourcesSystem.Base;
using GeneratedCode.Custom.Config;
using System;

namespace SharedCode.Config
{
    public class CloudSharedDataConfig : BaseResource
    {
        public ResourceRef<CloudEntryPointConfig> CloudEntryPoint { get; set; }

        public ResourceRef<CloudEntryPointConfig> ClientEntryPoint { get; set; }

        public ResourceRef<CloudRequirement>[] CloudRequirements { get; set; } = Array.Empty<ResourceRef<CloudRequirement>>();

        public ResourceRef<ServerServicesConfigDef> WebServicesConfig { get; set; }
    }

    public class CloudEntryPointConfig : BaseResource
    {
        public string Host { get; set; }

        public int Port { get; set; }
    }

    public class CloudRequirement : BaseResource
    {
        public string RepositoryId { get; set; }

        public int Count { get; set; }
    }
}
