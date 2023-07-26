using System;
using ColonyShared.SharedCode.Utils;
using NLog;
using ResourceSystem.Aspects.Misc;
using SharedCode.Utils.DebugCollector;
using SharedCode.Utils;
using UnityEngine;
using UnityEngine.Assertions;
using static UnityEngine.Mathf;
using static Src.Animation.AnimationDoerAux;
using static Src.ManualDefsForSpells.EffectAnimatorDef;

namespace Src.Animation
{
    internal static class AnimationModifiers
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(AnimationDoer));

        
        internal abstract class ParameterModifierBuilder
        {
            protected readonly AnimationParameterDef Param;

            protected ParameterModifierBuilder(AnimationParameterDef param)
            {
                Param = param ?? throw new ArgumentNullException(nameof(param));
            }

            public bool InsertDefaultOnSet => false;

            public string DebugName => Param.Name;
            
            public object GetStackId(IAnimationDoerInternal _) => Param;
        }

        internal abstract class LayerModifierBuilder
        {
            protected readonly AnimationLayerDef Layer;

            protected LayerModifierBuilder(AnimationLayerDef layer)
            {
                Layer = layer ?? throw new ArgumentNullException(nameof(layer));
            }

            public bool InsertDefaultOnSet => false;

            public string DebugName => Layer.Name;

            public object GetStackId(IAnimationDoerInternal _) => Layer;
        }
        
        
        internal abstract class ParameterModifierInstance
        {
            protected readonly int ParameterHash;

            protected ParameterModifierInstance(int parameterHash)
            {
                ParameterHash = parameterHash;
            }

            public virtual void OnPop(IAnimationDoerInternal _)
            {
            }

            public virtual void OnPull(IModifierInstance _)
            {
            }
        }
        

        #region BoolModifierBuilder

        internal abstract class BoolModifierBuilder : ParameterModifierBuilder
        {
            protected const AnimatorControllerParameterType Type = AnimatorControllerParameterType.Bool;

            protected readonly bool Value;

            protected BoolModifierBuilder(AnimationParameterDef param, bool value) : base(param)
            {
                Value = value;
            }

            public override string ToString() => StringBuildersPool.Get.Append("[").Append("Param:").Append(Param.____GetDebugAddress()).Append(" Type:").Append(Type).Append(" Value:").Append(Value).Append("]").ToStringAndReturn();

        }

        #endregion


        #region TriggerModifierBuilder

        internal abstract class TriggerModifierBuilder : ParameterModifierBuilder
        {
            protected const AnimatorControllerParameterType Type = AnimatorControllerParameterType.Trigger;

            protected TriggerModifierBuilder(AnimationParameterDef param) : base(param) 
            {}

            public override string ToString() => StringBuildersPool.Get.Append("[").Append("Param:").Append(Param.____GetDebugAddress()).Append(" Type:").Append(Type).Append("]").ToStringAndReturn();
        }

        #endregion



        #region IntModifierBuilder

        internal abstract class IntModifierBuilder : ParameterModifierBuilder
        {
            protected const AnimatorControllerParameterType Type = AnimatorControllerParameterType.Int;

            protected readonly int Value;

            protected IntModifierBuilder(AnimationParameterDef param, int value) : base(param)
            {
                Value = value;
            }

            public override string ToString() => StringBuildersPool.Get.Append("[").Append("Param:").Append(Param.____GetDebugAddress()).Append(" Type:").Append(Type).Append(" Value:").Append(Value).Append("]").ToStringAndReturn();
        }

        #endregion


        #region FloatModifierBuilder

        internal abstract class FloatModifierBuilder : ParameterModifierBuilder
        {
            protected const AnimatorControllerParameterType Type = AnimatorControllerParameterType.Float;

            protected readonly float Value;

            protected FloatModifierBuilder(AnimationParameterDef param, float value) : base(param)
            {
                Value = value;
            }

            public override string ToString() => StringBuildersPool.Get.Append("[").Append("Param:").Append(Param.____GetDebugAddress()).Append(" Type:").Append(Type).Append(" Value:").Append(Value).Append("]").ToStringAndReturn();
        }

        #endregion


        #region SmoothFloatModifierBuilder

        internal abstract class SmoothFloatModifierBuilder : ParameterModifierBuilder
        {
            protected const AnimatorControllerParameterType Type = AnimatorControllerParameterType.Float;

            protected readonly float StartValue, EndValue, Time;

            protected SmoothFloatModifierBuilder(AnimationParameterDef param, float startValue, float endValue, float time) : base(param)
            {
                StartValue = startValue;
                EndValue = endValue;
                Time = time;
            }

            public override string ToString() => StringBuildersPool.Get.Append("[").Append("Param:").Append(Param.____GetDebugAddress()).Append(" Type:").Append(Type).Append(" Value:").Append(StartValue).Append("->").Append(EndValue).Append(" Time:").Append(Time).Append("]").ToStringAndReturn();
        }

        #endregion


        #region LayerWeightModifierBuilder

        internal abstract class LayerWeightModifierBuilder : LayerModifierBuilder
        {
            protected readonly float Value;

            protected LayerWeightModifierBuilder(AnimationLayerDef layer, float value) : base(layer)
            {
                Value = value;
            }
              
            public override string ToString() => StringBuildersPool.Get.Append("[").Append("Layer:").Append(Layer.____GetDebugAddress()).Append(" Weight:").Append(Value).Append("]").ToStringAndReturn();
        }

        #endregion


        #region SmoothLayerWeightModifierBuilder

        internal abstract class SmoothLayerWeightModifierBuilder : LayerModifierBuilder
        {
            protected readonly float StartValue, EndValue, Time;

            protected SmoothLayerWeightModifierBuilder(AnimationLayerDef layer, float startValue, float endValue, float time) : base(layer)
            {
                StartValue = startValue;
                EndValue = endValue;
                Time = time;
            }

            public override string ToString() => StringBuildersPool.Get.Append("[").Append("Layer:").Append(Layer.____GetDebugAddress()).Append(" Weight:").Append(StartValue).Append("->").Append(EndValue).Append(" Time:").Append(Time).Append("]").ToStringAndReturn();
        }

        #endregion


        #region BoolWithTriggerModifierBuilder

        internal abstract class BoolWithTriggerModifierBuilder
        {
            protected readonly AnimationParameterTupleDef Param;
            protected readonly bool BoolAndTriggerValue;

            protected BoolWithTriggerModifierBuilder(AnimationParameterTupleDef param, bool boolAndTriggerValue)
            {
                Param = param ?? throw new ArgumentNullException(nameof(param));
                BoolAndTriggerValue = boolAndTriggerValue;
            }

            public bool InsertDefaultOnSet => false;

            public string DebugName => Param.Name;

            public object GetStackId(IAnimationDoerInternal _) => Param;
        }

        #endregion


        #region StatePlayerBuilder

        internal abstract class StatePlayerBuilder
        {
            protected readonly AnimationStateDef State;
            protected readonly StatePlayParams Params;
            protected readonly object PlayId;

            protected StatePlayerBuilder(AnimationStateDef state, StatePlayParams @params, object playId = null)
            {
                State = state ?? throw new ArgumentNullException(nameof(state));
                Params = @params;
                PlayId = playId;

                if (Params.TimeRange.IsValid && Params.TimeRange.Duration == Int64.MaxValue)
                    throw new ArgumentException("params.TimeRange in infinite");
            }

            public bool InsertDefaultOnSet => true;

            public string DebugName => State.____GetDebugRootName();

            // все стейты одного слоя в один стек, так как несколько стейтов не могут играть параллельно
            public object GetStackId(IAnimationDoerInternal doer) => doer.GetLayer(doer.GetState(State).Info.LayerName);

            public override string ToString() => StringBuildersPool.Get.Append("[").Append("State:").Append(State.____GetDebugAddress()).Append("]").ToStringAndReturn();
            
            protected abstract class Instance : IModifierInstance
            {
                private static readonly Func<long> DefaultClock = () => SyncTime.NowUnsynced;  
                private readonly object _playId;
                private readonly AnimationStateDef _stateDef;
                private readonly AnimationStateRuntimeInfo _stateInfo;
                private readonly AnimationStateParameters _stateParams;
                private StatePlayParams _params;
                private bool _started;

                protected Instance(
                    StatePlayParams @params,
                    AnimationStateRuntimeInfo stateInfo,
                    AnimationStateParameters stateParams,
                    AnimationStateDef stateDef,
                    object playId
                )
                {
                    _params = @params;
                    _stateInfo = stateInfo;
                    _stateDef = stateDef;
                    _stateParams = stateParams;
                    _playId = playId;

                    if (_stateInfo.Length < 0)
                        _stateInfo = new AnimationStateRuntimeInfo(_stateInfo, length:0);

                    if (_params.AnimationLength < 0)
                        _params.AnimationLength = _stateInfo.Length;

                    if (_params.Clock == null)
                        _params.Clock = DefaultClock;

                    if (!_params.TimeRange.IsValid)
                        _params.TimeRange = TimeRange.FromDuration(_params.Clock(), SyncTime.FromSeconds(_params.AnimationLength));
                }

                public bool Execute(IAnimationDoerInternal doer, bool hasOwner)
                {
                    Assert.IsTrue(_params.TimeRange.IsValid);

                    var currentTime = _params.Clock();

                    var timeOffset = SyncTime.ToSeconds(currentTime - _params.TimeRange.Start);
                    if (timeOffset < 0)
                        return true;

                    var timeRemains = SyncTime.ToSeconds(_params.TimeRange.Finish - currentTime);

                    if (!_started)
                    {
                        _started = true;

                        float speedFactor = 1;
                        if (_stateParams.SpeedParameterHash != 0)
                        {
                            speedFactor = _params.AnimationLength / Max(timeRemains, 0.001f); // or whole TimeRange ?
                            doer.Animator.SetFloat(_stateParams.SpeedParameterHash, speedFactor);
                        }

                        Collect.IfActive?.EventBgn($"AnimationDoer.State.{_stateInfo.FullName}", this);
                        
                        if (Logger.IsTraceEnabled)
                            Logger.Trace(
                                $"Play State | {_stateInfo.FullName} ({_stateInfo.FullNameHash}) | TimeRange:[{timeOffset:F2}, +{timeRemains:F2}] AnimLength:{_params.AnimationLength:F2} Speed:{speedFactor} PlayId:{_playId} Mode:{_params.Mode}");
                        
                        Play(doer, _stateParams.RunParameterHash, _stateParams.StayParameterHash);

                        if (_playId != null)
                            doer.FireAnimationPlayStarted(new AnimationPlayInfo(_playId, _stateDef, startTime: _params.TimeRange.Start, speedFactor: speedFactor, animationOffset: 0));
                    }

                    switch (_params.Mode)
                    {
                        case Mode.Once:
                            return timeRemains > 0 && hasOwner;
                        case Mode.ClampForever:
                            return hasOwner;
                        case Mode.Loop:
                            if (timeRemains <= 0)
                            {
                                _params.TimeRange = TimeRange.FromDuration(_params.Clock(), _params.TimeRange.Duration);
                                _started = false;
                            }
                            return hasOwner;
                        default:
                            throw new ArgumentOutOfRangeException($"_params.Mode:{_params.Mode}");
                    }
                }

                public void OnPop(IAnimationDoerInternal doer)
                {
                    Collect.IfActive?.EventEnd(this);
                    Stop(doer, _stateParams.RunParameterHash, _stateParams.StayParameterHash);
                }

                public void OnPull(IModifierInstance prev)
                {
                    _started = false;
                }

                protected abstract void Play(IAnimationDoerInternal doer, int triggerHash, int boolHash);
                
                protected abstract void Stop(IAnimationDoerInternal doer, int triggerHash, int boolHash);
               
            }
        }

        internal struct StatePlayParams
        {
            public float AnimationLength;
            public TimeRange TimeRange;
            public Func<long> Clock;
            public Mode Mode;
        }
 }
    #endregion
}
