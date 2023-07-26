using Assets.Src.ResourcesSystem.Base;
using System.Collections.Generic;

namespace SharedCode.Entities.GameObjectEntities
{
    public class SpawnTemplatesMapDef : BaseResource
    {
        public List<SpawnTemplateDef> Templates { get; set; }
        public string SceneName { get; set; }
    }
}
