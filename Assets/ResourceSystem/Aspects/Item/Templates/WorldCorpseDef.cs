using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using SharedCode.Entities.GameObjectEntities;
using UnityEngine;

namespace SharedCode.Aspects.Item.Templates
{
    public class WorldCorpseDef : SaveableBaseResource, IEntityObjectDef, ILimitedLifetimeDef
    {
        public UnityRef<GameObject> Prefab { get; set; } // префаб показываемый всем игрокам в PvP, и только владельцу в PvE 
        public UnityRef<GameObject> AnotherPrefab { get; set; }  // префаб показываемый всем игрокам в PvE, кроме владельца
        public ResourceRef<UnityGameObjectDef> PrefabDef { get; set; }
        public ResourceRef<LimitedLifetimeDef> LimitedLifetimeDef { get; set; }
        public bool SelfRegistry => false;

        public LocalizedString NameLs { get; set; }
        public List<ResourceRef<ShapeDef>> Colliders { get; set; }
        public ResourceRef<WeaponColliderDef> AttackCollider { get; set; }
    }
}