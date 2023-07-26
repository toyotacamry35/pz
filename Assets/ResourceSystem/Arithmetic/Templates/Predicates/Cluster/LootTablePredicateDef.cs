using Assets.Src.ResourcesSystem.Base;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Aspects.Templates;
using SharedCode.Aspects.Item.Templates;
using ResourceSystem.ContentKeys;
using SharedCode.Wizardry;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates.Cluster
{
    // It's adapted copy-paste from `Predicate`.
    public abstract class LootTablePredicateDef : BaseResource
    {
    }

    // --- Logic Predicates: ----------------------------------------------

    public class LootTablePredicateTrueDef : LootTablePredicateDef
    {
    }

    public class LootTablePredicateFalseDef : LootTablePredicateDef
    {
    }

    public class LootTablePredicateInverseDef : LootTablePredicateDef
    {
        public ResourceRef<LootTablePredicateDef> Predicate { get; set; }
    }

    public class LootTablePredicateAndDef : LootTablePredicateDef
    {
        public List<ResourceRef<LootTablePredicateDef>> Predicates { get; set; }
    }

    public class LootTablePredicateOrDef : LootTablePredicateDef
    {
        public List<ResourceRef<LootTablePredicateDef>> Predicates { get; set; }
    }

    public class LootTablePredicateInGameTimeDef : LootTablePredicateDef
    {
        public ResourceRef<InGameTimeIntervalDef> TimeInterval { get; set; }
    }

    public class LootTablePredicateExpiredLifespanPercentDef : LootTablePredicateDef
    {
        public float FromIncluding { get; set; }
        public float TillExcluding { get; set; }
    }

    public class LootTablePredicatePlayerHaveNotPerkDef : LootTablePredicateDef
    {
        public ResourceRef<BaseItemResource> Perk;
    }

    // --- true LoootTables specific predicates: ----------------------------------------------

    public class LootTablePredicateIsAnyOfDamageTypesInLootRequestDef : LootTablePredicateDef
    {
        public List<ResourceRef<DamageTypeDef>> DamageTypes { get; set; }
    }

    public class LootTablePredicatePremium : LootTablePredicateDef
    {
    }

    public class LootTablePredicateContentKey : LootTablePredicateDef
    {
        public ResourceRef<ContentKeyDef> Key { get; set; }
    }

    //SpellPredicate adapter
    public class LootTablePredicateSpellPredicate : LootTablePredicateDef
    {
        public ResourceRef<SpellPredicateDef> SpellPredicate { get; set; }
    }
}
