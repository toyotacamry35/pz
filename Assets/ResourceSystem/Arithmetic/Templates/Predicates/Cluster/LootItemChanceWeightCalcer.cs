using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers.Cluster
{
    public abstract class LootItemChanceWeightCalcerDef : BaseResource
    {
    }

    public class LootItemChanceWeightCalcerConstantDef : LootItemChanceWeightCalcerDef
    {
        public float Value { get; set; }
    }

    public class LootItemChanceWeightCalcerByLootRequestDamageTypeDef : LootItemChanceWeightCalcerDef
    {
        public ResourceRef<DamageTypeDef> DamageType { get; set; }
        public float Weight { get; set; }
    }

    public class LootItemChancePerkWeightCalcerDef : LootItemChanceWeightCalcerDef
    {
        public float Value { get; set; }
        public float PerkDestroyMult { get; set; }
    }
}
