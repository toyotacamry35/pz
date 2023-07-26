using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using SharedCode.Entities.GameObjectEntities;
using UnityEngine;

namespace SharedCode.Aspects.Building
{
    public class FencePlaceDef : SaveableBaseResource, IEntityObjectDef
    {
        public UnityRef<GameObject> Prefab { get; set; }
        public ResourceRef<UnityGameObjectDef> PrefabDef { get; set; }
        public bool SelfRegistry => false;

        public LocalizedString NameLs { get; set; }
        public List<ResourceRef<ShapeDef>> Colliders { get; set; }
        public ResourceRef<WeaponColliderDef> AttackCollider { get; set; }
    }
}