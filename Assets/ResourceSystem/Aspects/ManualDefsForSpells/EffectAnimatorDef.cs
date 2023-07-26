using System;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ResourceSystem.Aspects.Misc;
using SharedCode.Wizardry;

namespace Src.ManualDefsForSpells
{
    public class EffectAnimatorDef : SpellEffectDef
    {
        [UsedImplicitly] public ResourceRef<SpellEntityDef> Target;
        [UsedImplicitly] public ResourceRef<AnimatorModifierDef>[] Actions;

        [Obsolete] public ResourceRef<AnimatorModifierDef>[] Parameters { set { Actions = value; } } // для совместимости со старыми спеллами

        public abstract class AnimatorModifierDef : BaseResource
        {
            /// Указание когда выполняется
            public abstract When __When { get; }

            /// Если true, то модификатор не снимается при окончании эффекта (имеет смысл только при "When": "Start" и для модификатора имеющего длительность) 
            public abstract bool __Detached { get; }
        }

        public abstract class AnimatorParameterModifierDef : AnimatorModifierDef
        {
            [UsedImplicitly, JsonProperty(Required = Required.Always)] public ResourceRef<AnimationParameterDef> Parameter;
            [UsedImplicitly] public When When;
            [UsedImplicitly] public bool Detached;

            public override When __When => When;
            public override bool __Detached => Detached;
        }
        
        public class IntParameterDef : AnimatorParameterModifierDef
        {
            [UsedImplicitly, JsonProperty(Required = Required.Always)] public int Value;
        }

        public class BoolParameterDef : AnimatorParameterModifierDef
        {
            [UsedImplicitly] public bool Value = true;
        }

        public class FloatParameterDef : AnimatorParameterModifierDef
        {
            [UsedImplicitly, JsonProperty(Required = Required.Always)] public float Value;
        }

        public class CalcerFloatParameterDef : AnimatorParameterModifierDef
        {
            [UsedImplicitly, JsonProperty(Required = Required.Always)] public ResourceRef<CalcerDef<float>> Calcer;
        }
        
        public class RandomFloatParameterDef : AnimatorParameterModifierDef
        {
            [UsedImplicitly] public float MinValue = 0;
            [UsedImplicitly] public float MaxValue = 1;
        }

        public class SmoothFloatParameterDef : AnimatorParameterModifierDef
        {
            [UsedImplicitly, JsonProperty(Required = Required.Always)] public float StartValue;
            [UsedImplicitly, JsonProperty(Required = Required.Always)] public float EndValue;
            [UsedImplicitly, JsonProperty(Required = Required.Always)] public float Time;
            
            public override bool __Detached => Detached;
        }

        public class TriggerParameterDef : AnimatorParameterModifierDef
        {
        }
        
        // Trigger to cause transition & bool named <trigger_name> + "_b" to show a state after trigger is consumed, 
        //.. but action is still in process. (`false` val. is used (f.e.) to interrupt animation).
        public class BoolWithTriggerParameterDef : AnimatorModifierDef
        {
            [UsedImplicitly, JsonProperty(Required = Required.Always)] public ResourceRef<AnimationParameterTupleDef> Parameter;
            [UsedImplicitly] public bool Value { get; set; } = true;
            [UsedImplicitly] public When When;

            public override When __When => When;
            public override bool __Detached => false;
        }
        
        public class Vector2ParameterDef : AnimatorModifierDef
        {
            [UsedImplicitly, JsonProperty(Required = Required.Always)] public ResourceRef<AnimationParameterDef> ParameterX;
            [UsedImplicitly, JsonProperty(Required = Required.Always)] public ResourceRef<AnimationParameterDef> ParameterY;
            [UsedImplicitly, JsonProperty(Required = Required.Always)] public ResourceRef<SpellVector2Def> Vector;
            [UsedImplicitly] public When When;
            [UsedImplicitly] public bool Detached;

            public override When __When => When;
            public override bool __Detached => Detached;
        }

        public class Vector3ParameterDef : AnimatorModifierDef
        {
            [UsedImplicitly, JsonProperty(Required = Required.Always)] public ResourceRef<AnimationParameterDef> ParameterX;
            [UsedImplicitly, JsonProperty(Required = Required.Always)] public ResourceRef<AnimationParameterDef> ParameterY;
            [UsedImplicitly, JsonProperty(Required = Required.Always)] public ResourceRef<AnimationParameterDef> ParameterZ;
            [UsedImplicitly, JsonProperty(Required = Required.Always)] public ResourceRef<SpellVector3Def> Vector;
            [UsedImplicitly] public When When;
            [UsedImplicitly] public bool Detached;

            public override When __When => When;
            public override bool __Detached => Detached;
        }
        
        public abstract class AnimatorLayerModifierDef : AnimatorModifierDef
        {

            [UsedImplicitly, JsonProperty(Required = Required.Always)] public ResourceRef<AnimationLayerDef> Layer;
            [UsedImplicitly] public When When;
            
            public override When __When => When;
            public override bool __Detached => false;
        }

        public class LayerWeightDef : AnimatorLayerModifierDef
        {
            [UsedImplicitly, JsonProperty(Required = Required.Always)] public float Value = 1;
        }

        public class SmoothLayerWeightDef : AnimatorLayerModifierDef
        {
            [UsedImplicitly, JsonProperty(Required = Required.Always)] public float StartValue;
            [UsedImplicitly, JsonProperty(Required = Required.Always)] public float EndValue;
            [UsedImplicitly, JsonProperty(Required = Required.Always)] public float Time;
            [UsedImplicitly] public bool Detached;
            public override bool __Detached => Detached;
        }
        
        public class StateDef : AnimatorModifierDef
        {
            public const float DurationOriginal = 0; 
            public const float DurationAdjusted = -1; 

            [UsedImplicitly, JsonProperty(Required = Required.Always)] public ResourceRef<AnimationStateDef> State;
            [UsedImplicitly] public When When;
            [UsedImplicitly] public bool Detached; // продолжать проигрывание стейта после завершения эффекта 
            [UsedImplicitly] public float Length = -1;
            [UsedImplicitly] public Mode Mode = Mode.Once;
            [UsedImplicitly] public float Duration = DurationAdjusted;
            public override When __When => When;
            public override bool __Detached => Detached;
        }        
        
        public enum When { OnStart, OnFinish }
        
        public enum Mode { Once, Loop, ClampForever }
    }
}
