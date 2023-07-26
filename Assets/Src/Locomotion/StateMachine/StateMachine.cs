using System;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Interfaces;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Locomotion.Debug;
using JetBrains.Annotations;
using SharedCode.Utils;
using UnityEngine.Assertions;
using static Src.Locomotion.DebugTag;

namespace Src.Locomotion
{
    public partial class StateMachineBuilder<TStateMachineContext> where TStateMachineContext : StateMachineContext
    {
        class StateMachine : ILocomotionStateMachine, ILocomotionDebugable, IResettable
        {
            public string CurrentStateName => _currentNode.State.Name;

            private readonly Node _anyStateNode;
            private readonly IEnumerable<ILocomotionPredicate<TStateMachineContext>> _predicates;
            private Node _currentNode;
            private float _currentStateElapsedTime;
            private readonly TStateMachineContext _context;
            private readonly VariablesBuilder _variablesBuilder = new VariablesBuilder();
            private readonly ICurveLoggerProvider _curveLogProv;
            private readonly IDumpingLoggerProvider _loggerProvider;
            private readonly Type _thisType = typeof(StateMachine);

            public event Action OnStartNewStateEvent;
            public event Action OnFinishCurrStateEvent;

            ///#PZ-13568: #Dbg:
            protected Func<bool> ShouldSaveLocoVars;
            protected Action<LocomotionVariables, Type> SaveLocoVarsCallback;

            private Node _startNode;
            bool ILocomotionPipelinePassNode.IsReady => _context.IsReady;
            private static readonly Node __SameStateNode = new Node(__SameState);

            public StateMachine(
                Node startNode, 
                Transition[] anyStateTransitions, 
                IEnumerable<ILocomotionPredicate<TStateMachineContext>> predicates, 
                TStateMachineContext ctx,
                [CanBeNull] ICurveLoggerProvider curveLogProv,
                IDumpingLoggerProvider loggerProvider, 
                Func<bool> shouldSaveLocoVars, Action<LocomotionVariables, Type> saveLocoVarsCallback)
            {
                Assert.IsNotNull(startNode);
                _startNode = startNode;
                _currentNode = startNode;
                _predicates = predicates;
                _context = ctx;
                _anyStateNode = new Node(__AnyState) {Transitions = anyStateTransitions};
                _curveLogProv = curveLogProv;
                _loggerProvider = loggerProvider;
                ShouldSaveLocoVars = shouldSaveLocoVars;
                SaveLocoVarsCallback = saveLocoVarsCallback;
            }

            public void Reset()
            {
                _currentNode = _startNode;
                _currentStateElapsedTime = 0f;
                // Reset not-stateless predicates:
                foreach (var predicate in _predicates)
                    if (predicate is IResettable resettable)
                        resettable.Reset();
            }

            LocomotionVariables ILocomotionPipelinePassNode.Pass(LocomotionVariables vars, float dt)
            {
                //#Dbg:
                if (_loggerProvider?.LogBackCounter > 0)
                    _loggerProvider?.DLogger?.IfActive?.Log(_thisType, _loggerProvider.LogBackCounter, Consts.DbgTagIn, vars);

                if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## StateMachine.Pass 1) _var-sBuilder.Init(..)");
                _variablesBuilder.Init(vars, deltaTime: dt);
                LocomotionProfiler.EndSample();

                if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## StateMachine.Execute 2) ExecuteImpl(..)");
                ExecuteImpl(_variablesBuilder, in vars);
                LocomotionProfiler.EndSample();

                //#Tmp_Off: return _variablesBuilder.Build();
                if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## StateMachine.Execute 3) _var-sBuilder.Build(..)");

                //Dbg:
                // var velocityHorizontal_0 = _variablesBuilder.Dbg_GetVeloByAccel(vars);
                // _curveLogProv?.CurveLogger?.IfActive?.AddData("0.20) SmPass.VeloFromAccel", SyncTime.Now, velocityHorizontal_0.x, velocityHorizontal_0.y);

                var resultVars = _variablesBuilder.Build();

                //Dbg:
                // var velocityHorizontal_1 = _variablesBuilder.Dbg_GetVeloByAccel(resultVars);
                // _curveLogProv?.CurveLogger?.IfActive?.AddData("0.22) SmPass.VeloFromAccel", SyncTime.Now, velocityHorizontal_1.x, velocityHorizontal_1.y);
                // _curveLogProv?.CurveLogger?.IfActive?.AddData("0.22b) SmPass.Velo", SyncTime.Now, resultVars.Velocity);

                LocomotionProfiler.EndSample();

                //#Dbg:
                if (_loggerProvider?.LogBackCounter > 0)
                    _loggerProvider?.DLogger?.IfActive?.Log(_thisType, _loggerProvider.LogBackCounter, Consts.DbgTagOut, resultVars);

                //#Dbg:
                if (ShouldSaveLocoVars?.Invoke() ?? false)
                    SaveLocoVarsCallback(resultVars, this.GetType());

                return resultVars;
            }

            private void ExecuteImpl(VariablesPipeline pipeline, in LocomotionVariables vars)
            {
                float dt = _context.Clock.DeltaTime;
                _context.SetCurrentState(_currentNode.State);
                _context.SetStateElapsedTime(_currentStateElapsedTime);
                _context.SetCurrentVars(in vars);

                if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## StateMachine.Execute 1) predicate.Execute(..)");
                foreach (var predicate in _predicates)
                    predicate.Execute(_context);
                LocomotionProfiler.EndSample();

                if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## StateMachine.Execute 2.1) ProcessTransitions(..)");
                var newNode = ProcessTransitions(_anyStateNode, _context) ?? ProcessTransitions(_currentNode, _context) ?? __SameStateNode;
                LocomotionProfiler.EndSample();

                if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## StateMachine.Execute 2.2) \"if (newNode.State != __SameState)\"");
                if (newNode.State != __SameState)
                {
                    FinishCurrState(pipeline);
                    StartNewState(newNode, pipeline);
                }
                LocomotionProfiler.EndSample();

                if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample($"## StateMachine.Execute 3) State.Execute(..) [[`{_currentNode.State}`]]");
                _currentNode.State.Execute(_context, pipeline);

                //Dbg:
                // var velocityHorizontal = pipeline.Dbg_GetVeloByAccel(vars);
                // _curveLogProv?.CurveLogger?.IfActive?.AddData("0.21) SmExec.VeloFromAccel", SyncTime.Now, velocityHorizontal.x, velocityHorizontal.y);

                LocomotionProfiler.EndSample();

                _currentStateElapsedTime += dt;
            }

            private void FinishCurrState(VariablesPipeline pipeline)
            {
                OnFinishCurrStateEvent?.Invoke();
                _currentNode.State.OnExit(_context, pipeline);
                _currentNode.OnExit?.Invoke(_context);
                _currentStateElapsedTime = 0;
            }

            private void StartNewState(Node newNode, VariablesPipeline pipeline)
            {
                _currentNode = newNode;
                _context.SetCurrentState(_currentNode.State);
                _context.SetStateElapsedTime(_currentStateElapsedTime);
                _currentNode.State.OnEnter(_context, pipeline);
                _currentNode.OnEnter?.Invoke(_context);
                OnStartNewStateEvent?.Invoke();
            }

            private Node ProcessTransitions(Node node, TStateMachineContext ctx)
            {
                if (node.Transitions != null)
                    foreach (var transition in node.Transitions)
                        if (transition.Predicate.Evaluate(ctx))
                        {
                            transition.Action?.Invoke(ctx);
                            if(transition.Node.State != __NopeState)
                                return transition.Node;
                        }
                return null;
            }

            public class Node
            {
                public readonly ILocomotionState<TStateMachineContext> State;
                public Action<TStateMachineContext> OnEnter;
                public Action<TStateMachineContext> OnExit;
                public Transition[] Transitions;

                public Node(ILocomotionState<TStateMachineContext> state)
                {
                    if(state==null) throw new ArgumentNullException(nameof(state));
                    State = state;
                }
            }

            public struct Transition
            {
                public readonly ILocomotionPredicate<TStateMachineContext> Predicate;
                public readonly Node Node;
                public readonly Action<TStateMachineContext> Action;

                public Transition(ILocomotionPredicate<TStateMachineContext> predicate, Node node, Action<TStateMachineContext> action)
                {
                    Predicate = predicate;
                    Node = node;
                    Action = action;
                }
            }
            
            public void GatherDebug(ILocomotionDebugAgent agent)
            {
                agent.Set(StateMachineStateName, CurrentStateName);
            }
            
        }
    }
}