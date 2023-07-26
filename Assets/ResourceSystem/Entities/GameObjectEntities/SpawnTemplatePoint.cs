using ResourcesSystem.Loader;
using Newtonsoft.Json;
using SharedCode.Utils;

namespace SharedCode.Entities.GameObjectEntities
{
    [KnownToGameResources]
    [JsonConverter(typeof(SPConverter))]
    public struct SpawnTemplatePoint
    {
        public Vector3 SpatialPoint { get; set; }
        public Quaternion Rotation { get; set; }
    }
}
