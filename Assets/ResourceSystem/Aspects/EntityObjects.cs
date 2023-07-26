using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Aspects.Statictic;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using SharedCode.Aspects.Science;
using SharedCode.Wizardry;
using System;
using System.Collections.Generic;
using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using ResourceSystem.Entities;
using UnityEngine;
using SharedCode.AI;
using Assets.Src.RubiconAI;
using ResourceSystem.Aspects.Misc;
using SharedCode.Aspects.Item.Templates;


namespace SharedCode.Entities.GameObjectEntities
{

    public class DebugSpellDrawerDef : ComponentDef
    {

    }
    public class EntityGameObjectDef : ComponentDef
    {


    }

    public interface IEntityObjectDef : ISaveableResource
    {
        UnityRef<GameObject> Prefab { get; set; }
        ResourceRef<UnityGameObjectDef> PrefabDef { get; set; }
        LocalizedString NameLs { get; set; }
        List<ResourceRef<ShapeDef>> Colliders { get; set; }
        ResourceRef<WeaponColliderDef> AttackCollider { get; set; }
    }

    public class WeaponColliderDef : BaseResource
    {
        public ResourceRef<BoxShapeDef> DefaultCollider { get; set; }
        public Dictionary<ResourceRef<GameObjectMarkerDef>, ResourceRef<BoxShapeDef>> CustomColliders { get; set; }
    }

    public interface ILimitedLifetimeDef : IResource
    {
        ResourceRef<LimitedLifetimeDef> LimitedLifetimeDef { get; set; }
    }
    [Localized]
    public class MineableEntityDef : SaveableBaseResource, IEntityObjectDef, IHasStatsDef, IBruteDef, IMortalObjectDef, IHasBoundsDef, IHasLifespanDef, IComputableStateMachineOwnerDef, IHasKnowledgeDef, IIsDummyLegionaryDef, IHasDefaultStatsDef
    {
        public bool KnockedDownOnZeroHealth { get; set; }
        public ResourceRef<SpellDef> KnockDownSpell { get; set; }
        public UnityRef<GameObject> Prefab { get; set; }
        public ResourceRef<UnityGameObjectDef> PrefabDef { get; set; }
        public ResourceRef<CapsuleDef> Bounds { get; set; } = new CapsuleDef{ Radius = 0.5f, Height = 1f };
        public bool SelfRegistry => false;
        public float Health { get; set; }
        public ResourceRef<DamageTypeDef> DefaultDamageType { get; set; }
        public ResourceRef<LootTableBaseDef> LootTable { get; set; } //#TC-4602: #TODO: rename to DefaultLootTable
        public ItemResourceRefPack FillerResourcePack { get; set; }

        public float DestructionPowerRequired { get; set; } = 100;
        public ResourceRef<ComputableStateMachineDef> ComputableStateMachine { get; set; }

        public float CurrProgressActualTime
        {
            get;
            set;
        }

        // --- IBruteDef: --------------------------------------------------
        public ResourceRef<ItemSpecificStats> DefaultStats { get; set; }
        public float ForwardDamageMultiplier { get; set; } = 1f;
        public float SideDamageMultiplier { get; set; } = 1f;
        public float BackwardDamageMultiplier { get; set; } = 1f;
        public ResourceRef<StatisticType> ObjectType { get; set; }
        // --- IHasLifespanDef: --------------------------------------------
        public float LifeSpanSec { get; set; }
        public OnLifespanExpired DoOnExpired { get; set; }
        // --- IHasStatsDef: --------------------------------------------
        public ResourceRef<StatsDef> Stats { get; set; }
        // --- IMortalObjectDef: --------------------------------------------
        public bool DisablePreDeathHandlerAutoDeathByTimeout { get; set; }
        public ResourceRef<SpellDef> OnZeroHealthSpell { get; set; }
        public bool IsHpRegenAllowedInPreDeathState { get; set; }
        // --- IHasKnowledgeDef: --------------------------------------------
        public ResourceRef<KnowledgeSelectorDef>[] KnowledgeSelectors { get; set; }
        public TechPointCountDef[] RewardPoints { get; set; } = Array.Empty<TechPointCountDef>();
        public LocalizedString NameLs { get; set; }
        public List<ResourceRef<ShapeDef>> Colliders { get; set; }
        public ResourceRef<WeaponColliderDef> AttackCollider { get; set; }
        CapsuleDef IHasBoundsDef.Bounds => Bounds;

        public ResourceRef<LegionDef> LegionType { get; set; }
        public ResourceRef<SpellDef>[] SpellsOnResurrect { get; set; } = System.Array.Empty<ResourceRef<SpellDef>>();
    }

    public interface ILootableDef : IResource
    {
        ResourceRef<LootTableBaseDef> DefaultLootTable { get; set; }
    }

    //This type is needed only for mechanism suggesting "entity of what type to instantiate? by def type" works (see DefToType.GetEntityType usings: e.g. see CorpseSpawner.SpawnCorpseImpl)
    public class CorpseInteractiveEntityDef : InteractiveEntityDef
    {
    }

    [Localized]
    public class InteractiveEntityDef : SaveableBaseResource, IEntityObjectDef, IMortalObjectDef, IHasLifespanDef, IComputableStateMachineOwnerDef, IHasKnowledgeDef, ILootableDef, IIsDummyLegionaryDef, IHasDefaultStatsDef
    {
        public bool KnockedDownOnZeroHealth { get; set; }
        public ResourceRef<SpellDef> KnockDownSpell { get; set; }
        public UnityRef<GameObject> Prefab { get; set; }
        public ResourceRef<UnityGameObjectDef> PrefabDef { get; set; }
        public ResourceRef<LootTableBaseDef> DefaultLootTable { get; set; }
        public bool SelfRegistry => false;
        public ResourceRef<ComputableStateMachineDef> ComputableStateMachine { get; set; }
        // --- IHasLifespanDef: -------------------------------------------------
        public float LifeSpanSec { get; set; }
        public OnLifespanExpired DoOnExpired { get; set; }
        // --- IMortalObjectDef: --------------------------------------------
        public bool DisablePreDeathHandlerAutoDeathByTimeout { get; set; }
        public ResourceRef<SpellDef> OnZeroHealthSpell { get; set; }
        public bool IsHpRegenAllowedInPreDeathState { get; set; }
        // --- IHasKnowledgeDef: --------------------------------------------
        public ResourceRef<KnowledgeSelectorDef>[] KnowledgeSelectors { get; set; }
        public TechPointCountDef[] RewardPoints { get; set; } = Array.Empty<TechPointCountDef>();
        public LocalizedString NameLs { get; set; }
        public List<ResourceRef<ShapeDef>> Colliders { get; set; }
        public ResourceRef<WeaponColliderDef> AttackCollider { get; set; }
        public ResourceRef<LegionDef> LegionType { get; set; }
        public ResourceRef<ItemSpecificStats> DefaultStats { get; set; }
        public ResourceRef<SpellDef>[] SpellsOnResurrect { get; set; } = System.Array.Empty<ResourceRef<SpellDef>>();
    }

}
