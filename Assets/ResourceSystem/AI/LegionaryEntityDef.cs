using System;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Aspects.Statictic;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI;
using ColonyShared.SharedCode.Aspects.Locomotion;
using ColonyShared.SharedCode.InputActions;
using L10n;
using ResourceSystem.Entities;
using ResourceSystem.Reactions;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Wizardry;
using UnityEngine;
using SharedCode.Aspects.Item.Templates;
using Assets.ResourceSystem.Aspects.Banks;
using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using Assets.Src.Aspects.Impl.Factions.Template;
using ResourceSystem.Aspects.Misc;

namespace SharedCode.AI
{
    [Localized]
    public class LegionaryEntityDef : SaveableBaseResource, 
        IEntityObjectDef, IHasStatsDef, IBruteDef, IMortalObjectDef, 
        IHasBoundsDef, IHasReactionsDef, ICorpseSpawnerDef, IHasDollDef, IBankDef,
        IHasInputActionHandlersDef, IHasDefaultStatsDef
    {
        public bool KnockedDownOnZeroHealth { get; set; }
        public ResourceRef<SpellDef> KnockDownSpell { get; set; }
        public UnityRef<GameObject> Prefab { get; set; }
        public ResourceRef<UnityGameObjectDef> PrefabDef { get; set; }
        public ResourceRef<CapsuleDef> Bounds { get; set; } = new CapsuleDef{ Radius = 0.5f, Height = 1f };
        public ResourceRef<IEntityObjectDef> CorpseEntityDef { get; set; }
        public bool SelfRegistry { get; set; } = false;
        public bool HasNpcMarker { get; set; }

        public ResourceRef<MobLocomotionDef> MobLocomotion { get; set; }

        // --- IBruteDef: --------------------------------------------------
        public ResourceRef<StatsDef> Stats { get; set; }
        public ResourceRef<ItemSpecificStats> DefaultStats { get; set; }
        public float ForwardDamageMultiplier  { get; set; } = 1f;
        public float SideDamageMultiplier     { get; set; } = 1f;
        public float BackwardDamageMultiplier { get; set; } = 1f;
        public ResourceRef<StatisticType> ObjectType { get; set; }
        
        // --- IMortalObjectDef: --------------------------------------------
        public bool DisablePreDeathHandlerAutoDeathByTimeout { get; set; }
        public ResourceRef<SpellDef> OnZeroHealthSpell       { get; set; }
        public bool IsHpRegenAllowedInPreDeathState          { get; set; }
        public ResourceRef<SpatialLegionaryDef> LegionaryDef { get; set; }
        public List<ResourceRef<KnowledgeSourceDef>> CommonSenses { get; set; } = new List<ResourceRef<KnowledgeSourceDef>>();
        public ResourceRef<ReactionsDef> ReactionHandlers { get; set; }
        public LocalizedString NameLs { get; set; }
        public List<ResourceRef<ShapeDef>> Colliders { get; set; }
        public ResourceRef<WeaponColliderDef> AttackCollider { get; set; }
        public float DestructionPowerRequired { get; set; } = 100;
        public DefaultContainer DefaultDoll { get; set; }
        public ResourceRef<VisualDollDef> BodyType { get; set; }
        public List<DefaultItemsStack> FirstRunDoll { get; set; }

        public ResourceRef<BankDef> BankDef { get; set; }
        public Dictionary<ResourceRef<InputActionHandlersLayerDef>, Dictionary<ResourceRef<InputActionDef>, ResourceRef<InputActionHandlerDef>>> InputActionHandlers { get; set; }

        CapsuleDef IHasBoundsDef.Bounds => Bounds;
        public ResourceRef<SpellDef>[] SpellsOnResurrect { get; set; } = System.Array.Empty<ResourceRef<SpellDef>>();
        public ResourceRef<FactionDef> Faction;
    }

    public interface IIsDummyLegionaryDef : IResource
    {
        ResourceRef<LegionDef> LegionType { get; set; }
    }
}
