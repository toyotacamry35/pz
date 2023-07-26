using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using SharedCode.Aspects.Cartographer;
using SharedCode.Entities.GameObjectEntities;
using UnityEngine;

namespace GeneratedCode.Custom.Config
{
    public class GatewayDef : SaveableBaseResource, IEntityObjectDef
    {
        public UnityRef<GameObject> Prefab { get; set; }
        public ResourceRef<UnityGameObjectDef> PrefabDef { get; set; }
        public LocalizedString NameLs { get; set; }
        public List<ResourceRef<ShapeDef>> Colliders { get; set; }
        public ResourceRef<WeaponColliderDef> AttackCollider { get; set; }
        public ResourceRef<MapDef> ToMap { get; set; }
        
    }
}
