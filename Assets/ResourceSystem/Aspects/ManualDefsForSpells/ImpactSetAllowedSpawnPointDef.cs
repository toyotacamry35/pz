using Assets.Src.ResourcesSystem.Base;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Wizardry;

namespace ColonyShared.ManualDefsForSpells
{
    public class ImpactSetAllowedSpawnPointDef : SpellImpactDef
    {
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public ResourceRef<SpawnPointTypeDef> SpawnPointType { get; set; }
    }
}
