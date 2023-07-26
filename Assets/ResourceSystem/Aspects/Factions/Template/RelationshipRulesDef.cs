using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.Src.ResourcesSystem.Base;
using ResourceSystem.Aspects.AccessRights;

namespace Assets.Src.Aspects.Impl.Factions.Template
{
    public class RelationshipRulesDef : BaseResource
    {
        public ResourceRef<DamageMultiplierDef> IncomingDamageMultiplier;
        public ResourceRef<DamageMultiplierDef> ChestIncomingDamageMultiplier;
        public ResourceRef<DamageMultiplierDef> BakenIncomingDamageMultiplier;
        public ResourceRef<AccessPredicateDef> CorpseAccessPredicate;
        public ResourceRef<AccessPredicateDef> ChestAccessPredicate;
        public ResourceRef<CalcerDef<BaseResource>> KillingReward;
    }
}