using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Wizardry;
using UnityEngine;

namespace GeneratedDefsForSpells
{
    public class ImpactSpawnObjectDef : SpellImpactDef
    {
        public ResourceRef<SpellEntityDef> At { get; set; }
        public ResourceRef<IEntityObjectDef> EntityObject { get; set; }
        public SharedCode.Utils.Vector3 Offset { get; set; }
    }
}
