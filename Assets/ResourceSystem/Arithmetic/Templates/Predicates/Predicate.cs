using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates
{
    public abstract class PredicateDef : CalcerDef<bool>
    {
        public static readonly PredicateDef True = new PredicateTrueDef();
        public static readonly PredicateDef False = new PredicateFalseDef();
    }
}
