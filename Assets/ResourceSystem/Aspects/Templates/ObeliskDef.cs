using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Aspects.Statictic;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.ResourceSystem.Entities;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using ResourceSystem.Entities;
using ResourceSystem.Reactions;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Wizardry;
using System.Collections.Generic;
using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using UnityEngine;
using SharedCode.AI;
using Assets.Src.RubiconAI;
using Assets.Src.Aspects.Impl.Factions.Template;
using SharedCode.Aspects.Item.Templates;

namespace ResourceSystem.Aspects.Templates
{
 
    public interface IHasInitialSpells
    {
        List<ResourceRef<SpellDef>> InitialSpells { get; set; }
    }

    public class ObeliskDef : SaveableBaseResource, IEntityObjectDef, IHasStatsDef, IBruteDef, IMortalObjectDef, IHasBoundsDef, IHasReactionsDef, ICorpseSpawnerDef, IHasInitialSpells, IHasSpatialDataHandlersDef, IIsDummyLegionaryDef, IHasDefaultStatsDef
    {
        public bool KnockedDownOnZeroHealth { get; set; }
        public UnityRef<GameObject> Prefab { get; set; }
        public ResourceRef<SpellDef> KnockDownSpell { get; set; }
        public ResourceRef<UnityGameObjectDef> PrefabDef { get; set; }
        public ResourceRef<CapsuleDef> Bounds { get; set; } = new CapsuleDef { Radius = 0.5f, Height = 1f };
        public ResourceRef<IEntityObjectDef> CorpseEntityDef { get; set; }
        public bool SelfRegistry { get; set; } = false;
        public List<ResourceRef<SpellDef>> InitialSpells { get; set; } = new List<ResourceRef<SpellDef>>();
        // --- IBruteDef: --------------------------------------------------
        public ResourceRef<StatsDef> Stats { get; set; }
        public ResourceRef<ItemSpecificStats> DefaultStats { get; set; }
        public float ForwardDamageMultiplier { get; set; } = 1f;
        public float SideDamageMultiplier { get; set; } = 1f;
        public float BackwardDamageMultiplier { get; set; } = 1f;
        public ResourceRef<StatisticType> ObjectType { get; set; }

        // --- IMortalObjectDef: --------------------------------------------
        public bool DisablePreDeathHandlerAutoDeathByTimeout { get; set; }
        public ResourceRef<SpellDef> OnZeroHealthSpell { get; set; }
        public bool IsHpRegenAllowedInPreDeathState { get; set; }
        public ResourceRef<ReactionsDef> ReactionHandlers { get; set; }
        public LocalizedString NameLs { get; set; }

        public List<ResourceRef<ShapeDef>> Colliders { get; set; }
        public ResourceRef<WeaponColliderDef> AttackCollider { get; set; }

        // --- IHasSpatialDataHandlers: -------------------------------------
        public bool QuerySpatialData { get; set; }

        public float DestructionPowerRequired { get; set; } = 100;
        CapsuleDef IHasBoundsDef.Bounds => Bounds;

        public ResourceRef<LegionDef> LegionType { get; set; }

        public ResourceRef<SpellDef>[] SpellsOnResurrect { get; set; } = System.Array.Empty<ResourceRef<SpellDef>>();

    }

    public interface IAlwaysBuffed : IResource
    {
        List<ResourceRef<BuffDef>> AlwaysBuffs { get; set; }
    }
    public struct AltarSactifices
    {
        public ResourceRef<QuestDef> Quest { get; set; }
        public ResourceRef<IEntityObjectDef> ObjectOnQuestFinish { get; set; }
        public ResourceRef<BuffDef> BuffObjectOnStart { get; set; }
        public bool ObjectAndBuffAreInterDependent { get; set; }
    }
    public class AltarDef : ObeliskDef
    {
        public List<AltarSactifices> Sacrifices { get; set; } = new List<AltarSactifices>();
        public ResourceRef<BuffDef> OnMadeOffering { get; set; }
    }

    
}
