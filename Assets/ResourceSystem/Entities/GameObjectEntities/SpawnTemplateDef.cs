using Assets.Src.ResourcesSystem.Base;
using System.Collections.Generic;

namespace SharedCode.Entities.GameObjectEntities
{
    public class SpawnTemplateDef : BaseResource
    {
        public ResourceRef<SpawnTemplateExclusionTypeDef> Type { get; set; }
        public List<SpawnPoints> Points { get; set; }
        // R in which should be checked, are other active templates of same type near it, when attempts to activate it
        public float Size { get; set; }
    }
}
