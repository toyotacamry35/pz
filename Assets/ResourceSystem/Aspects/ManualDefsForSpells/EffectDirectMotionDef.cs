using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SharedCode.Wizardry;

namespace ColonyShared.ManualDefsForSpells
{
    public class EffectDirectMotionDef : SpellEffectDef
    {
       [UsedImplicitly] public ResourceRef<MoverDef> Mover;
       [UsedImplicitly] public ResourceRef<RotatorDef> Rotator;

       public abstract class MoverDef : BaseResource { }

        public class NullMoverDef : MoverDef { }

        public class AnimatorMoverDef : MoverDef
        {
            [UsedImplicitly] public float Factor = 1;
        }

        public class CurveMoverDef : MoverDef
        {
            [UsedImplicitly, JsonProperty(Required = Required.Always)] public UnityRef<Curve> Curve;
            [UsedImplicitly, CanBeNull] public UnityRef<Curve> VerticalCurve;
            [UsedImplicitly] public ResourceRef<SpellVector2Def> Direction = new ResourceRef<SpellVector2Def>(new SpellExplicitVector2Def{ x = 1, y = 0});
            [UsedImplicitly] public bool AdjustTime = true;
            [UsedImplicitly] public float Factor = 1;
        }
        
        public abstract class RotatorDef : BaseResource { }

        public class NullRotatorDef : RotatorDef { }

        public class HardBindToCameraRotatorDef : RotatorDef { }

        public class BindToCameraRotatorDef : RotatorDef
        {
            [UsedImplicitly] public float Speed; // градусы/сек
        }
        
        public class LookAtRotatorDef : RotatorDef
        {
            [UsedImplicitly][JsonProperty(Required = Required.Always)] public ResourceRef<SpellEntityDef> Target;
            [UsedImplicitly] public float Time; // сек
        }
    }
}