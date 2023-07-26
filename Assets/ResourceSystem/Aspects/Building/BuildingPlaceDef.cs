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
    public class BuildingPlaceDef : SaveableBaseResource, IEntityObjectDef
    {
        public bool Square { get; set; }
        public int Size { get; set; }
        public int Height { get; set; }
        public float BlockSize { get; set; }
        public float Thickness { get; set; }

        public ResourceRef<BuildTimerDef> TimerDef { get; set; }

        public UnityRef<GameObject> Prefab { get; set; }
        public ResourceRef<UnityGameObjectDef> PrefabDef { get; set; }


        public LocalizedString NameLs { get; set; }
        public List<ResourceRef<ShapeDef>> Colliders { get; set; }
        public ResourceRef<WeaponColliderDef> AttackCollider { get; set; }
    }
}
