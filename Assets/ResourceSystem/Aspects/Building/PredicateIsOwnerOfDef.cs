using SharedCode.Wizardry;

namespace Assets.Src.BuildingSystem
{
    public class PredicateIsOwnerOfDef : SpellPredicateDef
    {
        public bool Override { get; set; } = false;
        public bool Result { get; set; } = false;
        public bool Negate { get; set; } = false;
    }
}
