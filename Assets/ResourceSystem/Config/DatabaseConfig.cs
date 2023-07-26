using Assets.Src.ResourcesSystem.Base;

namespace SharedCode.Config
{
    public class DatabaseConfig : BaseResource
    {
        public string ConfigId { get; set; }

        public string Type { get; set; }

        public string SerializerConfigId { get; set; }
    }
}
