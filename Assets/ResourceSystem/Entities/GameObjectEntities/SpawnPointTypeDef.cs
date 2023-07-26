using System;
using Assets.Src.ResourcesSystem.Base;

namespace SharedCode.Entities.GameObjectEntities
{
    public class SpawnPointTypeDef : SaveableBaseResource
    {
        public float CooldownSec { get; set; } = 60f;
    }
}
