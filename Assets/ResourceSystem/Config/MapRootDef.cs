using Assets.Src.ResourcesSystem.Base;
using GeneratedCode.Custom.Config;
using SharedCode.Config;

namespace Infrastructure.Config
{
    public struct KeyDependencyScene
    {
        public string Path { get; set; }
        public bool Optional { get; set; }
    }

    public class MapRootDef : BaseResource
    {
        public ResourceRef<MapDef>[] Maps { get; set; } = { };
        public ResourceRef<ContainerConfig> HostConfig { get; set; }
        public ResourceRef<CloudSharedDataConfig> HostShared { get; set; }

        public KeyDependencyScene[] KeyDependencyScenes { get; set; } = { };

        public string[] SystemScenes { get; set; } = { };

    }
}
