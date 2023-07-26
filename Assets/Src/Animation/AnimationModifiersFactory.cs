using System;
using Assets.ColonyShared.SharedCode.Entities;
using ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using NLog;
using ResourceSystem.Aspects.Misc;
using UnityEngine;
using static Src.Animation.AnimationDoerAux;
using static Src.Animation.AnimationModifiers;
using static Src.ManualDefsForSpells.EffectAnimatorDef;

namespace Src.Animation
{
    public class AnimationModifiersFactory : IAnimationModifiersFactory
    {
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

        public IAnimationModifier State(AnimationStateDef state, float length, Mode mode,
            object playId)
            => new StatePlayerBuilder(state,
                new StatePlayParams {AnimationLength = length, Mode = mode},
                playId);

        public IAnimationModifier State(AnimationStateDef state, float length, Mode mode,
            TimeRange timeRange, Func<long> clock, object playId)
            => new StatePlayerBuilder(state,
                new StatePlayParams {AnimationLength = length, TimeRange = timeRange, Clock = clock, Mode = mode }, 
                playId);


        
        #region BoolModifierBuilder

        private sealed class BoolModifierBuilder : AnimationModifiers.BoolModifierBuilder, IModifierBuilder
        {
            public BoolModifierBuilder(AnimationParameterDef param, bool value) : base(param, value)
            {}
            
            public IModifierInstance Create(IAnimationDoerInternal doer)
            {
                var parameterHash = doer.GetParameter(Param, Type).nameHash;
                return new Instance(parameterHash, Value);
            }

            public IModifierInstance CreateDefault(IAnimationDoerInternal doer)
            {
                var parameterHash = doer.GetParameter(Param, Type).nameHash;
                return new Instance(parameterHash, doer.Animator.GetBool(parameterHash));
            }

            private sealed class Instance : ParameterModifierInstance, IModifierInstance
            {
                private readonly bool _value;

                public Instance(int parameterHash, bool value) : base(parameterHash)
                {
                    _value = value;
                }

                public bool Execute(IAnimationDoerInternal doer, bool hasOwner)
                {
                    doer.Animator.SetBool(ParameterHash, _value);
                    return hasOwner;
                }
            }
        }

        #endregion


        #region BoolWithTriggerModifierBuilder

        private sealed class BoolWithTriggerModifierBuilder : AnimationModifiers.BoolWithTriggerModifierBuilder, IModifierBuilder
        {
            private const AnimatorControllerParameterType _boolType = AnimatorControllerParameterType.Bool;
            private const AnimatorControllerParameterType _triggerType = AnimatorControllerParameterType.Trigger;

            public BoolWithTriggerModifierBuilder(AnimationParameterTupleDef param, bool boolAndTriggerValue) : base(param, boolAndTriggerValue)
            {}

            public IModifierInstance Create(IAnimationDoerInternal doer)
            {
                var boolParameterHash = doer.GetParameter(Param.Name, _boolType).nameHash;
                var triggerParameterHash = doer.GetParameter(Param.Parameter2Name, _triggerType).nameHash;
                return new Instance(boolParameterHash, BoolAndTriggerValue, triggerParameterHash, BoolAndTriggerValue);
            }

            public IModifierInstance CreateDefault(IAnimationDoerInternal doer)
            {
                var boolParameterHash = doer.GetParameter(Param.Name, _boolType).nameHash;
                var triggerParameterHash = doer.GetParameter(Param.Parameter2Name, _triggerType).nameHash;
                return new Instance(boolParameterHash, doer.Animator.GetBool(boolParameterHash), triggerParameterHash, doer.Animator.GetBool(triggerParameterHash));
            }

            private sealed class Instance : IModifierInstance
            {
                private readonly int _parameter1Hash;
                private readonly int _parameter2Hash;
                private readonly bool _boolValue;
                private readonly bool _triggerValue;
                private bool _executed;

                public Instance(int boolParameterHash, bool boolValue, int triggerParameterHash, bool triggerValue)
                {
                    _parameter1Hash = boolParameterHash;
                    _boolValue = boolValue;
                    _parameter2Hash = triggerParameterHash;
                    _triggerValue = triggerValue;
                }

                public bool Execute(IAnimationDoerInternal doer, bool hasOwner)
                {
                    if (_triggerValue && !_executed)
                    {
                        _executed = true;
                        doer.Animator.SetTrigger(_parameter2Hash);
                    }
                    // else
                    //   doer.Animator.ResetTrigger(Parameter2Hash);

                    doer.Animator.SetBool(_parameter1Hash, _boolValue);
                    return hasOwner;
                }

                public void OnPop(IAnimationDoerInternal _)
                {
                }
                
                public void OnPull(IModifierInstance _)
                {
                }
            }
        }

        #endregion



        #region TriggerModifierBuilder

        private sealed class TriggerModifierBuilder : AnimationModifiers.TriggerModifierBuilder, IModifierBuilder
        {
            public TriggerModifierBuilder(AnimationParameterDef param) : base(param)
            {}

            public IModifierInstance Create(IAnimationDoerInternal doer)
            {
                SharedCode.Logging.Log.AttackStopwatch.Milestone("Anim Create");
                var parameterHash = doer.GetParameter(Param, Type).nameHash;
                return new Instance(parameterHash, true);
            }

            public IModifierInstance CreateDefault(IAnimationDoerInternal doer)
            {
                var parameterHash = doer.GetParameter(Param, Type).nameHash;
                return new Instance(parameterHash, false);
            }

            private class Instance : ParameterModifierInstance, IModifierInstance
            {
                private readonly bool _value;
                private bool _executed;

                public Instance(int parameterHash, bool value) : base(parameterHash)
                {
                    _value = value;
                }

                public bool Execute(IAnimationDoerInternal doer, bool hasOwner)
                {
                    if (_value && !_executed)
                    {
                        SharedCode.Logging.Log.AttackStopwatch.Milestone("Anim Do");
                        _executed = true;
                        doer.Animator.SetTrigger(ParameterHash);
                    }

                    //   else
                    //     doer.Animator.ResetTrigger(ParameterHash);
                    return false;
                }
            }
        }

        #endregion



        #region IntModifierBuilder

        private sealed class IntModifierBuilder :  AnimationModifiers.IntModifierBuilder, IModifierBuilder
        {
            public IntModifierBuilder(AnimationParameterDef param, int value) : base(param, value)
            {}

            public IModifierInstance Create(IAnimationDoerInternal doer)
            {
                var parameterHash = doer.GetParameter(Param, Type).nameHash;
                return new Instance(parameterHash, Value);
            }

            public IModifierInstance CreateDefault(IAnimationDoerInternal doer)
            {
                var parameterHash = doer.GetParameter(Param, Type).nameHash;
                return new Instance(parameterHash, doer.Animator.GetInteger(parameterHash));
            }

            private class Instance : ParameterModifierInstance, IModifierInstance
            {
                private readonly int _value;

                public Instance(int parameterHash, int value) : base(parameterHash)
                {
                    _value = value;
                }

                public bool Execute(IAnimationDoerInternal doer, bool hasOwner)
                {
                    doer.Animator.SetInteger(ParameterHash, _value);
                    return hasOwner;
                }
            }
        }

        #endregion


        #region FloatModifierBuilder

        private sealed class FloatModifierBuilder :  AnimationModifiers.FloatModifierBuilder, IModifierBuilder
        {
            public FloatModifierBuilder(AnimationParameterDef param, float value) : base(param, value)
            {}

            public IModifierInstance Create(IAnimationDoerInternal doer)
            {
                var parameterHash = doer.GetParameter(Param, Type).nameHash;
                return new Instance(parameterHash, Value);
            }

            public IModifierInstance CreateDefault(IAnimationDoerInternal doer)
            {
                var parameterHash = doer.GetParameter(Param, Type).nameHash;
                return new Instance(parameterHash, doer.Animator.GetFloat(parameterHash));
            }

            public class Instance : ParameterModifierInstance, IModifierInstance
            {
                private readonly float _value;

                public Instance(int parameterHash, float value) : base(parameterHash)
                {
                    _value = value;
                }

                public bool Execute(IAnimationDoerInternal doer, bool hasOwner)
                {
                    doer.Animator.SetFloat(ParameterHash, _value);
                    return hasOwner;
                }
            }
        }

        #endregion


        #region SmoothFloatModifierBuilder

        private sealed class SmoothFloatModifierBuilder :  AnimationModifiers.SmoothFloatModifierBuilder, IModifierBuilder
        {
            public SmoothFloatModifierBuilder(AnimationParameterDef param, float startValue, float endValue, float time) : base(param, startValue, endValue, time)
            {}
            
            public IModifierInstance Create(IAnimationDoerInternal doer)
            {
                var parameterHash = doer.GetParameter(Param, Type).nameHash;
                return new Instance(parameterHash, this);
            }

            public IModifierInstance CreateDefault(IAnimationDoerInternal doer)
            {
                var parameterHash = doer.GetParameter(Param, Type).nameHash;
                return new FloatModifierBuilder.Instance(parameterHash, doer.Animator.GetFloat(parameterHash));
            }

            private class Instance : ParameterModifierInstance, IModifierInstance
            {
                private readonly SmoothFloatModifierBuilder _params;

                public Instance(int parameterHash, SmoothFloatModifierBuilder @params) : base(parameterHash)
                {
                    _params = @params;
                }

                public bool Execute(IAnimationDoerInternal doer, bool hasOwner)
                {
                    var currentValue = doer.Animator.GetFloat(ParameterHash);
                    var alive = Evaluate(ref currentValue, _params.StartValue, _params.EndValue, _params.Time, UnityEngine.Time.deltaTime);
                    doer.Animator.SetFloat(ParameterHash, currentValue);
                    return alive || hasOwner;
                }
            }
        }

        #endregion



        #region LayerWeightModifierBuilder

        private sealed class LayerWeightModifierBuilder :  AnimationModifiers.LayerWeightModifierBuilder, IModifierBuilder
        {
            public LayerWeightModifierBuilder(AnimationLayerDef layer, float value) : base(layer, value)
            {}
              
            public IModifierInstance Create(IAnimationDoerInternal doer)
            {
                var layerIndex = GetLayerIndex(doer.Animator, Layer);
                return new Instance(layerIndex, Value);
            }

            public IModifierInstance CreateDefault(IAnimationDoerInternal doer)
            {
                var layerIndex = GetLayerIndex(doer.Animator, Layer);
                return new Instance(layerIndex, doer.Animator.GetLayerWeight(layerIndex));
            }

            public class Instance : ParameterModifierInstance, IModifierInstance
            {
                private readonly float _value;

                public Instance(int layerIdx, float value) : base(layerIdx)
                {
                    _value = value;
                }

                public bool Execute(IAnimationDoerInternal doer, bool hasOwner)
                {
                    doer.Animator.SetLayerWeight(ParameterHash, _value);
                    return hasOwner;
                }
            }
        }

        #endregion



        #region SmoothLayerWeightModifierBuilder

        private sealed class SmoothLayerWeightModifierBuilder : AnimationModifiers.SmoothLayerWeightModifierBuilder, IModifierBuilder
        {
            public SmoothLayerWeightModifierBuilder(AnimationLayerDef layer, float startValue, float endValue, float time) : base(layer, startValue, endValue, time)
            {}

            public IModifierInstance Create(IAnimationDoerInternal doer)
            {
                var layerIndex = GetLayerIndex(doer.Animator, Layer);
                return new Instance(layerIndex, this);
            }

            public IModifierInstance CreateDefault(IAnimationDoerInternal doer)
            {
                var layerIndex = GetLayerIndex(doer.Animator, Layer);
                return new LayerWeightModifierBuilder.Instance(layerIndex, doer.Animator.GetLayerWeight(layerIndex));
            }

            private class Instance : ParameterModifierInstance, IModifierInstance
            {
                private readonly SmoothLayerWeightModifierBuilder _params;

                public Instance(int parameterHash, SmoothLayerWeightModifierBuilder @params) : base(parameterHash)
                {
                    _params = @params;
                }

                public bool Execute(IAnimationDoerInternal doer, bool hasOwner)
                {
                    var currentValue = doer.Animator.GetLayerWeight(ParameterHash);
                    var alive = Evaluate(ref currentValue, _params.StartValue, _params.EndValue, _params.Time, UnityEngine.Time.deltaTime);
                    doer.Animator.SetLayerWeight(ParameterHash, currentValue);
                    return alive || hasOwner;
                }
            }
        }

        #endregion


        #region StatePlayerBuilder

        private sealed class StatePlayerBuilder : AnimationModifiers.StatePlayerBuilder, IModifierBuilder
        {
            public StatePlayerBuilder(AnimationStateDef state, StatePlayParams @params, object playId = null) : base(state, @params, playId) {}

            public IModifierInstance Create(IAnimationDoerInternal doer)
            {
                var (info, support) = doer.GetState(State);
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
                    doer.Animator.SetTrigger(stateTrigger);
                    doer.Animator.SetBool(stayParameter, true);
                }
                
                protected override void Stop(IAnimationDoerInternal doer, int stateTrigger, int stayParameter)
                {
                    doer.Animator.ResetTrigger(stateTrigger);
                    doer.Animator.SetBool(stayParameter, false);
                }
            }
        }
        #endregion
    }
}
