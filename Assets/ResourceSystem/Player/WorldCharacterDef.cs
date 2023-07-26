using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Aspects.Statictic;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Entities;
using ColonyShared.SharedCode.Aspects.Locomotion;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Wizardry;
using UnityEngine;
using ResourcesSystem.Loader;
using SharedCode.Aspects.Item.Templates;
using Assets.Src.Aspects.Impl.Factions.Template;
using ColonyShared.SharedCode.InputActions;
using Assets.Src.Aspects.Impl.Stats;
using ResourceSystem.Reactions;
using L10n;
using ResourceSystem.Entities;
using SharedCode.AI;
using Assets.Src.RubiconAI;
using Assets.Src.Aspects.Impl.Traumas.Template;
using Assets.ResourceSystem.Entities;
using ResourceSystem.Aspects.Misc;

namespace Assets.ColonyShared.SharedCode.Player
{
    public class WorldCharacterDef : SaveableBaseResource, IEntityObjectDef, IHasStatsDef, IBruteDef, IMortalObjectDef, IHasBoundsDef, IHasReactionsDef, IIsDummyLegionaryDef, IHasInputActionHandlersDef, IHasTraumasDef, IHasSpatialDataHandlersDef, IHasDefaultStatsDef
    {
        public bool KnockedDownOnZeroHealth { get; set; } = true;
        public ResourceRef<SpellDef> KnockDownSpell { get; set; }
        public bool SelfRegistry => false;
        public WorldCharacterAFKStateMachineDescription AFKStateMachine { get; set; }
        public UnityRef<GameObject> Prefab               { get; set; }
        public ResourceRef<UnityGameObjectDef> PrefabDef { get; set; }
        public ResourceRef<CapsuleDef> Bounds { get; set; } = new CapsuleDef{ Radius = 0.4f, Height = 1.8f };
        public float MaxMiningDistance                   { get; set; }
        public float DestructionPowerRequired { get; set; } = 0;
        public ResourceRef<QuestDef>[] InitialQuests { get; set; }
        public ResourceRef<CharacterLocomotionDef> Locomotion { get; set; }
        ///  TODO: Удалить!
        public ResourceRef<MobLocomotionDef> MobLocomotion { get; set; }

        // --- IBruteDef: --------------------------------------------------
        public ResourceRef<ItemSpecificStats> DefaultStats { get; set; }
        public ResourceRef<StatResource> InventoryWeightProxyStat { get; set; }
        public ResourceRef<StatResource> DollWeightProxyStat { get; set; }
        public ResourceRef<StatResource> CraftWeightProxyStat { get; set; }
        public float ForwardDamageMultiplier  { get; set; } = 1f;
        public float SideDamageMultiplier     { get; set; } = 1f;
        public float BackwardDamageMultiplier { get; set; } = 1f;
        public ResourceRef<StatisticType> ObjectType { get; set; }
        // --- IHasStatsDef: --------------------------------------------------
        public ResourceRef<StatsDef> Stats { get; set; }
        // --- IMortalObjectDef: --------------------------------------------
        public bool DisablePreDeathHandlerAutoDeathByTimeout { get; set; }
        public ResourceRef<SpellDef> OnZeroHealthSpell       { get; set; }
        public bool IsHpRegenAllowedInPreDeathState          { get; set; }
        public ResourceRef<WorldCorpseDef> CorpseDef { get; set; }
        public ResourceRef<LegionDef> LegionType { get; set; }
        public Dictionary<ResourceRef<InputActionHandlersLayerDef>,Dictionary<ResourceRef<InputActionDef>,ResourceRef<InputActionHandlerDef>>> InputActionHandlers { get; set; }
        public ResourceRef<ReactionsDef> ReactionHandlers { get; set; }
        public LocalizedString NameLs { get; set; }

        public List<ResourceRef<ShapeDef>> Colliders { get; set; }
        public ResourceRef<WeaponColliderDef> AttackCollider { get; set; }

        // --- IHasSpatialDataHandlers: -----------------------------------------
        public bool QuerySpatialData { get; set; }
        public int MaxCraftQueueSize { get; set; }

        CapsuleDef IHasBoundsDef.Bounds => Bounds;

        public ResourceRef<TraumasDef> AllTraumas { get; set; }

        public ResourceRef<SpellDef>[] SpellsOnResurrect { get; set; } = System.Array.Empty<ResourceRef<SpellDef>>();
        public ResourceRef<SpellDef>[] SpellsOnBirth { get; set; } = System.Array.Empty<ResourceRef<SpellDef>>();
        public ResourceRef<SpellDef>[] SpellsOnEnterWorld { get; set; } = System.Array.Empty<ResourceRef<SpellDef>>();

        public Dictionary<ResourceRef<VisualDollDef>, WorldCharacterBodyCondition> Bodies;
    }

    [KnownToGameResources]
    public class WorldCharacterAFKStateMachineDescription
    {
        public float TimeSinceAFKDeathToNotShowRespawnWindowAndRespawnImmediately { get; set; }
        public float TimeToBecomeIdleAndInteractive { get; set; } = 60;
        public float TimeToDieAndUnload { get; set; } = 60000;
    }
    
    [KnownToGameResources]
    public struct WorldCharacterBodyCondition
    {
        public ResourceArray<GenderDef> Genders;
        public ResourceArray<MutationStageDef> Mutations;
    }
}
