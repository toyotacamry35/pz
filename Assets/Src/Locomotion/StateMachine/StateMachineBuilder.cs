using System;
using System.Collections.Generic;
using Assets.Src.Locomotion.Debug;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using SharedCode.Utils;
using UnityEngine;

namespace Src.Locomotion
{
    public partial class StateMachineBuilder<TStateMachineContext> where TStateMachineContext : StateMachineContext
    {
        // ReSharper disable once StaticMemberInGenericType - It's ok - there are only 2 concrete types
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<ILocomotionState<TStateMachineContext>, Entry> _states;

        // ReSharper disable once InconsistentNaming
        private readonly int StatesInitialCapacity_forDbg;
        /// [#Optimization]: `statesCapacityForOptimization` is needed to avoid dictionary resize, which allocates 4.7KB each time.
        public StateMachineBuilder(int statesCapacityForOptimization)
        {
            _states = new Dictionary<ILocomotionState<TStateMachineContext>, Entry>(statesCapacityForOptimization);
            StatesInitialCapacity_forDbg = statesCapacityForOptimization;
        }

        public StateMachineBuilder<TStateMachineContext> State(ILocomotionState<TStateMachineContext> state, Action<StateBuilder> statebuilder)
        {
            statebuilder(new StateBuilder(state, this));
            return this;
        }

        public StateMachineBuilder<TStateMachineContext> AnyState(Action<StateBuilder> statebuilder)
        {
            statebuilder(new StateBuilder(__AnyState, this));
            return this;
        }

        public struct StateBuilder
        {
            private readonly Entry _entry;

            public TransitionBuilder If(ILocomotionPredicate<TStateMachineContext> predicate)
            {
                return new TransitionBuilder(predicate, this);
            }

            public TransitionBuilder If(PredicateDelegate<TStateMachineContext> predicate)
            {
                return new TransitionBuilder(Predicates<TStateMachineContext>.IsTrue(predicate), this);
            }

            public void Otherwise(ILocomotionState<TStateMachineContext> state)
            {
                new TransitionBuilder(Predicates<TStateMachineContext>.True, this).State(state);
            }

            public StateBuilder OnEnter(Action reaction)
            {
                _entry.Node.OnEnter += _ => reaction();
                return this;
            }

            public StateBuilder OnEnter(Action<TStateMachineContext> reaction)
            {
                if (reaction != null)
                    _entry.Node.OnEnter += reaction;
                return this;
            }

            public StateBuilder OnExit(Action reaction)
            {
                if (reaction != null)
                    _entry.Node.OnExit += _ => reaction();
                return this;
            }

            public StateBuilder OnExit(Action<TStateMachineContext> reaction)
            {
                _entry.Node.OnExit += reaction;
                return this;
            }

            internal StateBuilder(ILocomotionState<TStateMachineContext> state, StateMachineBuilder<TStateMachineContext> parent)
            {
                if(state == null) throw new ArgumentNullException(nameof(state));
                if (Debug.isDebugBuild)
                {
                    if (parent._states.ContainsKey(state))
                        throw new Exception($"Locomotion state machine has a duplicate state: {state.GetType().Name}");
                    if (parent._states.Count + 1 > parent.StatesInitialCapacity_forDbg)
                        Logger.IfError()?.Message($"[#Optimization]: `_states` increased initial capacity: {parent.StatesInitialCapacity_forDbg}. Value is hardcoded & should be actualized");
                }
                _entry = new Entry(new StateMachine.Node(state));
                parent._states.Add(state, _entry);
            }

            public class TransitionBuilder
            {
                /// [#Optimization]: is used as initial capacity for dict. (hashset) to avoid allocation on resizing
                // Rounded up value calculated manually by counting places, where `_predicate` instantiated
                // ReSharper disable once InconsistentNaming
                internal const int PredicatesInitialCapacity_forOptimization = 85; //2020.05 there are 82 for character & 66 for mob

                private readonly ILocomotionPredicate<TStateMachineContext> _predicate;
                private readonly StateBuilder _parent;

                public StateBuilder State(ILocomotionState<TStateMachineContext> state)
                {
                    if(state == null) throw new ArgumentNullException(nameof(state));
                    _parent._entry.Transitions.Add(new Entry.Transition {Predicate = _predicate, State = state});
                    return _parent;
                }

                public StateBuilder Stay()
                {
                    _parent._entry.Transitions.Add(new Entry.Transition {Predicate = _predicate, State = __SameState});
                    return _parent;
                }

                public StateBuilder Do(Action action)
                {
                    _parent._entry.Transitions.Add(new Entry.Transition {Predicate = _predicate, Action = c => action(), State = __NopeState});
                    return _parent;
                }

                public StateBuilder Do(Action<TStateMachineContext> action)
                {
                    _parent._entry.Transitions.Add(new Entry.Transition {Predicate = _predicate, Action = action, State = __NopeState});
                    return _parent;
                }

                internal TransitionBuilder(ILocomotionPredicate<TStateMachineContext> predicate, StateBuilder parent)
                {
                    if(predicate == null) throw new ArgumentNullException(nameof(predicate));
                    _predicate = predicate;
                    _parent = parent;
                }
            }  
        }

        public ILocomotionStateMachine Build(TStateMachineContext ctx, [CanBeNull] ICurveLoggerProvider curveLogProv = null, [CanBeNull] IDumpingLoggerProvider dumpLoggerProvider = null, 
            Func<bool> shouldSaveLocoVars = null, Action<LocomotionVariables, Type> saveLocoVarsCallback = null)
        {
            StateMachine.Node startNode = null;
            StateMachine.Transition[] anyStateTransactions = null;
            // [#Optimization]: use dict. instead of hashset, 'cos last haven't a ctor with capacity arg. & so allocates on resizing
            //      I understand, the cost is ~8.5 KB waste memory on client (for unused bools) (85 Byte 8 ~ 100 mobs existing simultaneously on Cl) - is price for avoiding resize HashSet
            //      & allocating array (when was .ToArray - when passed as arg to `StateMachine` ctor.
            //      Why I need Dictionary with waste bool values instead of HashSet - 'cos HashSet hasn't ctor with capacity arg (until .NET Standard 2.1.): https://stackoverflow.com/a/6771986/3241228
            var predicates = new Dictionary<ILocomotionPredicate<TStateMachineContext>, /*not used*/bool>(StateBuilder.TransitionBuilder.PredicatesInitialCapacity_forOptimization);//new HashSet<ILocomotionPredicate<TStateMachineContext>>();
            foreach (var entry in _states)
            {
                StateMachine.Transition[] transitions = new StateMachine.Transition[entry.Value.Transitions.Count];
                for (int i = 0;  i < entry.Value.Transitions.Count;  ++i)
                {
                    var transition = entry.Value.Transitions[i];
                    Entry target;
                    if (!_states.TryGetValue(transition.State, out target) && !__AuxStates.TryGetValue(transition.State, out target))
                        throw new Exception($"Locomotion state machine has a transition to undefined state: {transition.State}");
                    transitions[i] = new StateMachine.Transition(transition.Predicate, target.Node, transition.Action);

                    //Use `.TryAdd` when .Net 2.1
                    if (!predicates.ContainsKey(transition.Predicate))
                        predicates.Add(transition.Predicate, default);
                }

                if (entry.Key == __AnyState)
                    anyStateTransactions = transitions;
                else
                {
                    if(transitions.Length == 0)
                        throw new Exception($"Locomotion state machine has a state without transactions: {entry.Key.GetType().Name}");
                    entry.Value.Node.Transitions = transitions;
                    if (startNode == null)
                        startNode = entry.Value.Node;
                }
            }
            if (startNode == null)
                throw new Exception("Locomotion state machine without states");

            return new StateMachine(startNode, anyStateTransactions, predicates.Keys, ctx, curveLogProv, dumpLoggerProvider, shouldSaveLocoVars, saveLocoVarsCallback);
        }

        private struct Entry
        {
            public Entry(StateMachine.Node node)
            {
                if(node==null ) throw new ArgumentNullException(nameof(node));
                Node = node;
                Transitions = new List<Transition>();
            }
            
            public readonly StateMachine.Node Node;
            public readonly List<Transition> Transitions;

            public struct Transition
            {
                public ILocomotionPredicate<TStateMachineContext> Predicate;
                public ILocomotionState<TStateMachineContext> State;
                public Action<TStateMachineContext> Action;
            }
        }

        private class __NullState : ILocomotionState<TStateMachineContext>
        {
            public __NullState(string name) { Name = name; }
            public string Name { get; }
            public void FixedUpdate(TStateMachineContext ctx, VariablesPipeline pipeline) { throw new InvalidOperationException(Name);}
            public void Execute(TStateMachineContext ctx, VariablesPipeline pipeline) { throw new NotImplementedException(); }
            public void OnEnter(TStateMachineContext ctx, VariablesPipeline pipeline) { throw new InvalidOperationException(Name); }
            public void OnExit(TStateMachineContext ctx, VariablesPipeline pipeline) { throw new InvalidOperationException(Name); }
            
        }
        
        private readonly Dictionary<ILocomotionState<TStateMachineContext>, Entry> __AuxStates = new Dictionary<ILocomotionState<TStateMachineContext>, Entry>
        {
            {__SameState, new Entry(new StateMachine.Node(__SameState))},
            {__NopeState, new Entry(new StateMachine.Node(__NopeState))},
        };

        private static readonly ILocomotionState<TStateMachineContext> __AnyState = new __NullState(nameof(__AnyState));
        private static readonly ILocomotionState<TStateMachineContext> __SameState = new __NullState(nameof(__SameState));
        private static readonly ILocomotionState<TStateMachineContext> __NopeState = new __NullState(nameof(__NopeState));        
    }
}