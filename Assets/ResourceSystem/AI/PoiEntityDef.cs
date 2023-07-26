using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI;
using L10n;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using UnityEngine;

namespace SharedCode.AI
{
    public class PoiEntityDef : SaveableBaseResource, IEntityObjectDef, IHasStatsDef, IIsDummyLegionaryDef
    {
        public UnityRef<GameObject> Prefab { get; set; }
        public ResourceRef<UnityGameObjectDef> PrefabDef { get; set; }
        public bool SelfRegistry { get; set; } = true;
        // IHasStatsDef: 
        public ResourceRef<StatsDef> Stats { get; set; }
        public LocalizedString NameLs { get; set; }
        public List<ResourceRef<ShapeDef>> Colliders { get; set; }
        public ResourceRef<WeaponColliderDef> AttackCollider { get; set; }
        public ResourceRef<LegionDef> LegionType { get; set; }
    }
}