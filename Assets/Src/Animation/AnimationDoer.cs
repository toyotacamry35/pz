using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.Src.Tools;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using ResourceSystem.Aspects.Misc;
using ResourceSystem.Utils;
using SharedCode.Utils.DebugCollector;
using Src.Animation.ACS;
using Src.Aspects.Doings;
using UnityEngine;
using UnityEngine.Assertions;
using static Src.Animation.AnimationDoerAux;

namespace Src.Animation
{
    public class AnimationDoer : IAnimationDoer, IAnimationPlayProvider, IAnimationDoerInternal
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(AnimationDoer));
        private static int StackSizeLimit = 10;
        private static readonly IReadOnlyDictionary<AnimationStateDef, (AnimationStateInfo, AnimationStateDoerSupport)> EmptyStates = new Dictionary<AnimationStateDef, (AnimationStateInfo, AnimationStateDoerSupport)>();

        private List<Operation> _operations = new List<Operation>(8);
        private List<Operation> _operationsUpdate = new List<Operation>(8);
        private readonly Dictionary<object, List<Entry>> _entries = new Dictionary<object, List<Entry>>();
        private readonly IReadOnlyDictionary<AnimationStateDef, (AnimationStateInfo, AnimationStateDoerSupport)> _supportedStates;
        private readonly Animator _animator;
        private readonly Dictionary<string, AnimatorControllerParameter> _animatorParameters; // т.к. Animator.parameter генерирует тонну мусора (~10Кb за вызов), то приходится кешировать параметры
        private readonly Dictionary<string, AnimatorLayerDescriptor> _animatorLayers;
        private readonly object _selfCauser = new object();
        private readonly object _lock = new object();
        private readonly OuterRef _entityRef;

        public AnimationDoer(Animator animator, [CanBeNull] AnimationStateInfoStorage animationInfo, IAnimationModifiersFactory modifiersFactory, OuterRef entityRef)
        {
            ModifiersFactory = modifiersFactory ?? throw new ArgumentNullException(nameof(modifiersFactory));
            _animator = animator;
            _entityRef = entityRef;
            try
            {
                if (animationInfo != null)
                {
                    var states = animator.GetBehaviours<AnimationStateDoerSupport>();
                    _supportedStates = states.ToDictionary(x => x.StateDef, x =>
                    {
                        AnimationStateInfo nfo = default;
                        try
                        {
                            nfo = animationInfo.GetInfo(x.GetComponent<AnimationStateProfile>().Guid);
                        } catch (Exception e) { Logger.IfError()?.Exception(e).Write(); }
                        return (nfo, x);
                    });
                    if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Animation Doer States:\n{string.Join("\n", _supportedStates.Keys)}").Write(); 
                }
                else
                {
                    _supportedStates = EmptyStates;
                    if(Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("No Animation State Info storage provided").Write();;
                }
                _animatorParameters = animator.parameters.ToDictionary(x => x.name, x => x);
                _animatorLayers = Enumerable.Range(0, animator.layerCount).ToDictionary(animator.GetLayerName, x => new AnimatorLayerDescriptor(animator.GetLayerName(x)));
            }
            catch (Exception e)
            {
                Logger.IfError()?.Exception(e).Write();
            }
        }

        public event AnimationPlayStartedDelegate AnimationPlayStarted;

        public IAnimationModifiersFactory ModifiersFactory { get; }

        public void Set(IAnimationModifier modifier)
        {
            Assert.IsNotNull(modifier, nameof(modifier));
            lock(_lock)
                _operations.Add(new Operation(OperationType.Set, (IModifierBuilder)modifier));
            Collect.IfActive?.Event($"AnimationDoer.Set.{modifier.DebugName}", _entityRef);
        }

        public void Push(object causer, IAnimationModifier modifier)
        {
            Assert.IsNotNull(modifier, nameof(modifier)); 
            lock(_lock)
                _operations.Add(new Operation(OperationType.Push, (IModifierBuilder)modifier, causer));
            Collect.IfActive?.Event($"AnimationDoer.Push.{modifier.DebugName}", _entityRef);
        }

        public void Replace(object oldCauser, object newCauser, IAnimationModifier modifier)
        {
            Assert.IsNotNull(oldCauser, nameof(oldCauser));
            Assert.IsNotNull(modifier, nameof(modifier)); 
            lock(_lock)
                _operations.Add(new Operation(OperationType.Replace, (IModifierBuilder)modifier, newCauser, oldCauser));
            Collect.IfActive?.Event($"AnimationDoer.Replace.{modifier.DebugName}", _entityRef);
        }

        public void Pop(object causer, IAnimationModifier modifier)
        {
            Assert.IsNotNull(causer, nameof(causer));
            Assert.IsNotNull(modifier, nameof(modifier));
            lock(_lock)
                _operations.Add(new Operation(OperationType.Pop, (IModifierBuilder)modifier, causer));
            Collect.IfActive?.Event($"AnimationDoer.Pop.{modifier.DebugName}", _entityRef);
        }

        public void Detach(object causer, IAnimationModifier modifier)
        {
            Assert.IsNotNull(causer, nameof(causer));
            Assert.IsNotNull(modifier, nameof(modifier));
            lock(_lock)
                _operations.Add(new Operation(OperationType.Detach, (IModifierBuilder)modifier, causer));
            Collect.IfActive?.Event($"AnimationDoer.Detach.{modifier.DebugName}", _entityRef);
        }
        
        private void Insert(object causer, IModifierBuilder builder, int idx, bool insertDefault = true)
        {
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Insert | Causer:{causer} Modifier:{builder} Idx:{idx}").Write();
            var key = builder.GetStackId(this);
            var instance = builder.Create(this);
            var stack = _entries.GetOrCreate(key);
            if (stack.Count > StackSizeLimit && Logger.IsWarnEnabled)
                Logger.IfWarn()?.Message($"Animation Doer stack overflow for {builder} Causer:{causer}").Write();
            if (stack.Count == 0 && insertDefault)
            {
                var defaultInstance = builder.CreateDefault(this);
                if (defaultInstance != null)
                {
                    stack.Add(new Entry(null, defaultInstance, Collect.IsActive ? builder.DebugName : string.Empty));
                    Collect.IfActive?.EventBgn($"AnimationDoer.Default.{builder.DebugName}", _entityRef, (this.GetHashCode(), defaultInstance));
                }
            }
            var entry = new Entry(causer, instance, builder.DebugName);
            if (idx != -1)
                stack.Insert(idx, entry);
            else
                stack.Add(entry);
            Collect.IfActive?.EventBgn($"AnimationDoer.Insert.{builder.DebugName}", _entityRef, (this.GetHashCode(), instance));
        }

        private int Remove(object causer, IModifierBuilder builder)
        {
            int idx = -1;
            if (_entries.TryGetValue(builder.GetStackId(this), out var stack))
            {
                for (idx = stack.Count - 1; idx >= 0; --idx)
                    if (causer.Equals(stack[idx].Causer))
                        break;
                if (idx != -1)
                {
                    if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Removed | Causer:{causer} Modifier:{builder} Idx:{idx}").Write();
                    var removed = stack[idx];
                    removed.Modifier.OnPop(this);
                    stack.RemoveAt(idx);
                    Collect.IfActive?.EventEnd((this.GetHashCode(), removed.Modifier));
                    if(idx > 0)
                        stack[idx - 1].Modifier.OnPull(removed.Modifier);
                }
            }
            return idx;
        }        

        private void Detach(object causer, IModifierBuilder builder)
        {
            if (_entries.TryGetValue(builder.GetStackId(this), out var stack))
            {
                int idx;
                for (idx = stack.Count - 1; idx >= 0; --idx)
                    if (causer.Equals(stack[idx].Causer))
                        break;
                if (idx != -1)
                {
                    if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Detached | Causer:{causer} Modifier:{builder} Idx:{idx}").Write();
                    stack[idx] = new Entry(null, stack[idx].Modifier, stack[idx].DebugName);
                }
            }
        }        
 
        public void Update()
        {
            lock (_lock)
            {
                var tmp = _operationsUpdate;
                _operationsUpdate = _operations;
                _operations = tmp;
            }

            foreach (var operation in _operationsUpdate)
            {
                try
                {
                    switch (operation.Type)
                    {
                        case OperationType.Set:
                        {
                            var idx = Remove(_selfCauser, operation.Builder);
                            Insert(_selfCauser, operation.Builder, idx, operation.Builder.InsertDefaultOnSet);
                        }
                            break;
                        case OperationType.Replace:
                        {
                            var idx = Remove(operation.OldCauser, operation.Builder);
                            Insert(operation.Causer, operation.Builder, idx);
                        }
                            break;
                        case OperationType.Push:
                            Insert(operation.Causer, operation.Builder, -1);
                            break;
                        case OperationType.Pop:
                            Remove(operation.Causer, operation.Builder);
                            break;
                        case OperationType.Detach:
                            Detach(operation.Causer, operation.Builder);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException($"operation.Type={operation.Type}");
                    }
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                }
            }

            _operationsUpdate.Clear();
            
            foreach (var stack in _entries.Values)
            {
                while (stack.Count > 0)
                {
                    var idx = stack.Count - 1;
                    var top = stack[idx];
                    Collect.IfActive?.Event($"AnimationDoer.Execute.{top.DebugName}", _entityRef);
                    var alive = top.Modifier.Execute(this, !ReferenceEquals(top.Causer, null));
                    if (!alive)
                    {
                        if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Removed dead | Modifier:{top.Modifier}").Write();
                        top.Modifier.OnPop(this);
                        stack.RemoveAt(idx);
                        Collect.IfActive?.EventEnd((this.GetHashCode(), top.Modifier));
                        if (stack.Count > 0)
                            stack[stack.Count - 1].Modifier.OnPull(top.Modifier);
                    }
                    else
                        break;
                }
            }
        }

        Animator IAnimationDoerInternal.Animator => _animator;

        void IAnimationDoerInternal.FireAnimationPlayStarted(in AnimationPlayInfo info) => AnimationPlayStarted?.Invoke(info);

        AnimatorControllerParameter IAnimationDoerInternal.GetParameter(AnimationParameterDef parameter, AnimatorControllerParameterType type)
        {
            if (!_animatorParameters.TryGetValue(parameter.Name, out var param))
                throw new Exception($"Animator parameter {parameter.Name} ({parameter.____GetDebugAddress()}) not exist in animator {_animator.transform.FullName()}");
            if (param.type != type)
                throw new Exception($"Wrong type {type} of animator parameter {parameter.Name} ({parameter.____GetDebugAddress()}) in animator {_animator.transform.FullName()}");
            return param;
        }

        AnimatorControllerParameter IAnimationDoerInternal.GetParameter(string parameterName, AnimatorControllerParameterType type)
        {
            if (!_animatorParameters.TryGetValue(parameterName, out var param))
                throw new Exception($"Animator parameter {parameterName} not exist in animator {_animator.transform.FullName()}");
            if (param.type != type)
                throw new Exception($"Wrong type {type} of animator parameter {parameterName} in animator {_animator.transform.FullName()}");
            return param;
        }
        
        (AnimationStateInfo,AnimationStateDoerSupport) IAnimationDoerInternal.GetState(AnimationStateDef def)
        {
            if(!_supportedStates.TryGetValue(def, out var state))
                throw new KeyNotFoundException($"Animation state {def.____GetDebugAddress()} not exists in {_animator.transform.FullName()} or has not {nameof(AnimationStateDoerSupport)}");
            return state;
        }

        AnimatorLayerDescriptor IAnimationDoerInternal.GetLayer(string name)
        {
            if (!_animatorLayers.TryGetValue(name, out var layer))
                throw new KeyNotFoundException($"Animation layer {name} not exists in {_animator.transform.FullName()}");
            return layer;
        }
        
        private readonly struct Entry
        {
            public readonly object Causer;
            public readonly IModifierInstance Modifier;
            public readonly string DebugName;

            public Entry(object causer, IModifierInstance modifier, string debugName)
            {
                Causer = causer;
                Modifier = modifier;
                DebugName = debugName;
            }
        }

        private readonly struct Operation
        {
            public readonly OperationType Type;
            public readonly IModifierBuilder Builder;
            public readonly object Causer;
            public readonly object OldCauser;

            
            public Operation(OperationType type, IModifierBuilder builder, object causer = null, object oldCauser = null)
            {
                Type = type;
                Builder = builder;
                Causer = causer;
                OldCauser = oldCauser;
            }
            
        }
        
        enum OperationType { Set, Push, Pop, Replace, Detach }
    }
}