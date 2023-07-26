using Assets.Src.ResourcesSystem.Base;
using Newtonsoft.Json;

namespace ResourceSystem.Entities
{
    public class CapsuleDef : BaseResource
    {
        /// <summary>
        /// Радиус
        /// </summary>
        [JsonProperty(Required = Required.Always)] public float Radius { get; set; }
        
        /// <summary>
        /// Высота - расстояние между "вершинами" полусфер
        /// </summary>
        [JsonProperty(Required = Required.Always)] public float Height { get; set; }

        /// <summary>
        /// Смещение центра капсулы по вертикали
        /// </summary>
        public float OffsetY { get; set; }
    }
}