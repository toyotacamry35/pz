using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.ResourcesSystem.Base;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Assets.Src.Character.Effects
{
    public class FXs : BaseResource
    {
        public ResourceArray<FXRuleDef> Rules { get; [UsedImplicitly] set; }
    }

    public class FXRuleDef : BaseResource
    {
        public bool MainAnimatorProp { get; set; }
        public string AnimatedProp { get; set; }
    }

    public class FloatFXRuleDef : FXRuleDef
    {
        public ResourceRef<CalcerDef<float>> Calcer { get; set; }
    }

    public class IntFXRuleDef : FXRuleDef
    {
        public ResourceRef<CalcerDef<float>> Calcer { get; set; }
    }

    public class BoolFXRuleDef : FXRuleDef
    {
        public ResourceRef<PredicateDef> Predicate { get; set; }
    }
}
