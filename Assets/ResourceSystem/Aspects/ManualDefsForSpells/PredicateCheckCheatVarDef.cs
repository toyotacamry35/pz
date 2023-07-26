using SharedCode.Wizardry;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;

namespace Shared.ManualDefsForSpells
{

    public class PredicateCheckCheatVarDef : SpellPredicateDef
    {
        public ResourceRef<BaseResource> CheatVar { get; set; }
        public string Value { get; set; }
    }

    public class CheatVarDef : BaseResource
    {

    }
}
