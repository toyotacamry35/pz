using System;
using Assets.ColonyShared.SharedCode.Entities;
using ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using NLog;
using ResourceSystem.Aspects.Misc;
using SharedCode.Utils.DebugCollector;
using static Src.Animation.AnimationDoerAux;
using static Src.Animation.AnimationModifiers;
using static Src.ManualDefsForSpells.EffectAnimatorDef;

namespace Src.Animation
{
    public class AnimationModifiersFactoryMock : IAnimationModifiersFactory
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(AnimationDoer));

        public IAnimationModifier BoolModifier(AnimationParameterDef parameter, bool value) => new BoolModifierBuilder(parameter, value);

        public IAnimationModifier BoolWithTriggerModifier(AnimationParameterTupleDef parameter, bool boolAndTriggerValue) =>
            new BoolWithTriggerModifierBuilder(parameter, boolAndTriggerValue);

        public IAnimationModifier IntModifier(AnimationParameterDef parameter, int value) => new IntModifierBuilder(parameter, value);

        public IAnimationModifier FloatModifier(AnimationParameterDef parameter, float value) => new FloatModifierBuilder(parameter, value);

        public IAnimationModifier FloatModifier(AnimationParameterDef parameter, float startValue, float endValue, float time) =>
            new SmoothFloatModifierBuilder(parameter, startValue, endValue, time);

        public IAnimationModifier LayerWeightModifier(AnimationLayerDef layer, float value) => new LayerWeightModifierBuilder(layer, value);

        public IAnimationModifier LayerWeightModifier(AnimationLayerDef layer, float startValue, float endValue, float time) =>
            new SmoothLayerWeightModifierBuilder(layer, startValue, endValue, time);

        public IAnimationModifier Trigger(AnimationParameterDef parameter) => new TriggerModifierBuilder(parameter);

        public IAnimationModifier State(AnimationStateDef state, Mode mode, object playId) =>
            new StatePlayerBuilder(state, new StatePlayParams {Mode = mode}, playId);

        public IAnimationModifier State(AnimationStateDef state, float length, Mode mode, object playId)
            => new StatePlayerBuilder(state, new StatePlayParams {AnimationLength = length, Mode = mode}, playId);

        public IAnimationModifier State(AnimationStateDef state, float length, Mode mode, TimeRange timeRange, Func<long> clock, object playId)
            => new StatePlayerBuilder(state, new StatePlayParams {AnimationLength = length, TimeRange = timeRange, Clock = clock, Mode = mode }, playId);


        private abstract class ModifierInstance : IModifierInstance
        {
            public bool Execute(IAnimationDoerInternal doer, bool hasOwner) => hasOwner;

            public virtual void OnPull(IModifierInstance prev) {}
            public virtual void OnPop(IAnimationDoerInternal _) {}
        }

        
        #region BoolModifierBuilder
        private sealed class BoolModifierBuilder : AnimationModifiers.BoolModifierBuilder, IModifierBuilder
        {
            public BoolModifierBuilder(AnimationParameterDef param, bool value) : base(param, value) {}
            public IModifierInstance Create(IAnimationDoerInternal doer) => new Instance();
            public IModifierInstance CreateDefault(IAnimationDoerInternal doer) => new Instance();
            private sealed class Instance : ModifierInstance {}
        }
        #endregion


        #region BoolWithTriggerModifierBuilder
        private sealed class BoolWithTriggerModifierBuilder : AnimationModifiers.BoolWithTriggerModifierBuilder, IModifierBuilder
        {
            public BoolWithTriggerModifierBuilder(AnimationParameterTupleDef param, bool boolAndTriggerValue) : base(param, boolAndTriggerValue) {}
            public IModifierInstance Create(IAnimationDoerInternal doer) => new Instance();
            public IModifierInstance CreateDefault(IAnimationDoerInternal doer) => new Instance();
            private sealed class Instance : ModifierInstance {}
        }
        #endregion


        #region TriggerModifierBuilder
        private sealed class TriggerModifierBuilder : AnimationModifiers.TriggerModifierBuilder, IModifierBuilder
        {
            public TriggerModifierBuilder(AnimationParameterDef param) : base(param) {}
            public IModifierInstance Create(IAnimationDoerInternal doer) => new Instance();
            public IModifierInstance CreateDefault(IAnimationDoerInternal doer) => new Instance();
            private sealed class Instance : ModifierInstance {}
        }
        #endregion


        #region IntModifierBuilder
        private sealed class IntModifierBuilder :  AnimationModifiers.IntModifierBuilder, IModifierBuilder
        {
            public IntModifierBuilder(AnimationParameterDef param, int value) : base(param, value) {}
            public IModifierInstance Create(IAnimationDoerInternal doer) => new Instance();
            public IModifierInstance CreateDefault(IAnimationDoerInternal doer) => new Instance();
            private sealed class Instance : ModifierInstance {}
        }
        #endregion


        #region FloatModifierBuilder
        private sealed class FloatModifierBuilder :  AnimationModifiers.FloatModifierBuilder, IModifierBuilder
        {
            public FloatModifierBuilder(AnimationParameterDef param, float value) : base(param, value) {}
            public IModifierInstance Create(IAnimationDoerInternal doer) => new Instance();
            public IModifierInstance CreateDefault(IAnimationDoerInternal doer) => new Instance();
            private sealed class Instance : ModifierInstance {}
        }
        #endregion


        #region SmoothFloatModifierBuilder
        private sealed class SmoothFloatModifierBuilder :  AnimationModifiers.SmoothFloatModifierBuilder, IModifierBuilder
        {
            public SmoothFloatModifierBuilder(AnimationParameterDef param, float startValue, float endValue, float time) : base(param, startValue, endValue, time) {}
            public IModifierInstance Create(IAnimationDoerInternal doer) => new Instance();
            public IModifierInstance CreateDefault(IAnimationDoerInternal doer) => new Instance();
            private sealed class Instance : ModifierInstance {}
        }
        #endregion


        #region LayerWeightModifierBuilder
        private sealed class LayerWeightModifierBuilder :  AnimationModifiers.LayerWeightModifierBuilder, IModifierBuilder
        {
            public LayerWeightModifierBuilder(AnimationLayerDef layer, float value) : base(layer, value) {}
              
            public IModifierInstance Create(IAnimationDoerInternal doer) => new Instance();
            public IModifierInstance CreateDefault(IAnimationDoerInternal doer) => new Instance();
            private sealed class Instance : ModifierInstance {}
        }
        #endregion



        #region SmoothLayerWeightModifierBuilder
        private sealed class SmoothLayerWeightModifierBuilder : AnimationModifiers.SmoothLayerWeightModifierBuilder, IModifierBuilder
        {
            public SmoothLayerWeightModifierBuilder(AnimationLayerDef layer, float startValue, float endValue, float time) : base(layer, startValue, endValue, time) {}
            public IModifierInstance Create(IAnimationDoerInternal doer) => new Instance();
            public IModifierInstance CreateDefault(IAnimationDoerInternal doer) => new Instance();
            private sealed class Instance : ModifierInstance {}
        }
        #endregion


        #region StatePlayerBuilder
        private sealed class StatePlayerBuilder : AnimationModifiers.StatePlayerBuilder, IModifierBuilder
        {
            public StatePlayerBuilder(AnimationStateDef state, StatePlayParams @params, object playId = null) : base(state, @params, playId) {}

            public IModifierInstance Create(IAnimationDoerInternal doer)
            {
                var (info,support) = doer.GetState(State);
                Collect.IfActive?.EventBgn($"AnimationDoer.State.{info.Name}", this);
                return new Instance(Params, new AnimationStateRuntimeInfo(info, doer.Animator), support.Parameters, support.StateDef, PlayId);
            }

            public IModifierInstance CreateDefault(IAnimationDoerInternal doer)
            {
                return null;
//                var defaultState = doer.GetState(State).DefaultState;
//                return new Instance(Params, new AnimationStateRuntimeInfo(defaultState.Info, doer.Animator));
            }

            private new class Instance : AnimationModifiers.StatePlayerBuilder.Instance
            {
                public Instance(
                    StatePlayParams @params,
                    in AnimationStateRuntimeInfo stateInfo,
                    in AnimationStateParameters stateParams,
                    [CanBeNull] AnimationStateDef stateDef = null,
                    [CanBeNull] object playId = null
                ) : base(@params, stateInfo, stateParams, stateDef, playId) {}

                protected override void Play(IAnimationDoerInternal doer, int stateTrigger, int stayParameter)
                {
                }
                
                protected override void Stop(IAnimationDoerInternal doer, int stateTrigger, int stayParameter)
                {
                }
            }
        }
        #endregion
    }
}
