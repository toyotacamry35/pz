using System;
using System.Collections.Generic;
using Assets.Src.Tools;
using ColonyShared.SharedCode.Input;
using ColonyShared.SharedCode.InputActions;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using ReactivePropsNs.ThreadSafe;
using Src.Locomotion;
using static UnityQueueHelper;

namespace Src.Input
{
    public class InputDispatchersStack : IInputActionStatesSource, IDisposable
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly NLog.Logger LoggerTriggers = LogManager.GetLogger(Logger.Name + ".Triggers");
        private static readonly NLog.Logger LoggerValues = LogManager.GetLogger(Logger.Name + ".Values");
        
        private readonly List<ProcessorHolder> _stack = new List<ProcessorHolder>();
        private readonly Dictionary<InputActionDef, IPropertyWrapper> _actions = new Dictionary<InputActionDef, IPropertyWrapper>();
        private readonly Dictionary<InputActionDef, InputActionState> _states = new Dictionary<InputActionDef, InputActionState>();
        
        public IStream<InputActionTriggerState> Stream(InputActionTriggerDef action)
        {
            lock (_actions)
                return _actions.GetOrCreate(action, a => new TriggerPropertyWrapper(action)).Stream;
        }

        public IStream<InputActionValueState> Stream(InputActionValueDef action)
        {
            lock (_actions)
                return _actions.GetOrCreate(action, a => new ValuePropertyWrapper(action)).Stream;
        }
        
        public void PushDispatcher([NotNull] object causer, [NotNull] IInputDispatcher dispatcher)
        {
            if (dispatcher == null) throw new ArgumentNullException(nameof(dispatcher));
            if (causer == null) throw new ArgumentNullException(nameof(causer));
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Push Input Dispatcher | Dispatcher:{dispatcher} Causer:{causer}").Write();
            lock(_stack)
                _stack.Add(new ProcessorHolder{ Dispatcher = dispatcher, Causer = causer });
        }

        public IInputDispatcher PopDispatcher([NotNull] object causer)
        {
            if (causer == null) throw new ArgumentNullException(nameof(causer));
            lock (_stack)
            {
                var idx = _stack.FindLastIndex(x => x.Causer == causer);
                if (idx != -1)
                {
                    var dispatcher = _stack[idx].Dispatcher;
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Pop Input Dispatcher | Dispatcher:{dispatcher} Causer:{causer}").Write();
                    _stack.RemoveAt(idx);
                    _stack.ForEach(x => x.Dispatcher.ResetStates());
                    return dispatcher;
                }
            }
            Logger.IfWarn()?.Message($"Pop Input Dispatcher | No input dispatcher for Causer:{causer}").Write();
            return null;
        }
        
        public void UpdateStates()
        {
            AssertInUnityThread();
            lock (_stack)
            {
                _states.Clear();
                for (var i = _stack.Count - 1; i >= 0; i--)
                    _stack[i].Dispatcher.UpdateStates(_states);
            }
            lock (_actions)
                foreach (var state in _states)
                    if (_actions.TryGetValue(state.Key, out var proxy))
                        proxy.Update(state.Value);
        }

        public void ResetStates()
        {
            lock (_stack)
                foreach (var holder in _stack)
                    holder.Dispatcher.ResetStates();
        }
        
        public IEnumerable<InputSourceDef> GetBindingForAction(InputActionDef action)
        {
            lock (_stack)
                for (var i = _stack.Count - 1; i >= 0; --i)
                {
                    var binding = _stack[i].Dispatcher.GetBindingForAction(action);
                    if (binding != null)
                        return binding;
                }
            return null;
        }

        public InputSlotDef GetSlotForAction(InputActionDef action)
        {
            lock (_stack)
                for (var i = _stack.Count - 1; i >= 0; --i)
                {
                    var slot = _stack[i].Dispatcher.GetSlotForAction(action);
                    if (slot != null)
                        return slot;
                }
            return null;
        }

        public bool IsActionBlocked(InputActionDef action)
        {
            lock (_stack)
                for (var i = _stack.Count - 1; i >= 0; --i)
                    if (_stack[i].Dispatcher.IsActionBlocked(action))
                        return true;
            return false;
            
        }
        
        public void Dispose()
        {
            foreach (var proxy in _actions.Values)
                proxy.Dispose();
        }
        
        private struct ProcessorHolder
        {
            public IInputDispatcher Dispatcher;
            public object Causer;
        }

        private interface IPropertyWrapper : IDisposable
        {
            void Update(InputActionState state);
        }

        private class ValuePropertyWrapper : IPropertyWrapper
        {
            private static readonly float ValueChangingThreshold = 0.001f;
            private readonly ReactiveProperty<InputActionValueState> _property;
            public ValuePropertyWrapper(InputActionDef action) => _property = new ReactiveProperty<InputActionValueState>(default, new Comparer(action));
            public IStream<InputActionValueState> Stream => _property;
            public void Update(InputActionState state) => _property.Value = state.IsValid ? state.ValueState : default;
            public void Dispose() => _property.Dispose();
            private class Comparer : IEqualityComparer<InputActionValueState>
            {
                private readonly InputActionDef _action;
                public Comparer(InputActionDef action) => _action = action;
                public bool Equals(InputActionValueState x, InputActionValueState y)
                {
                    if (!x.Value.ApproximatelyEqual(y.Value, ValueChangingThreshold))
                    {
                        if (LoggerValues.IsDebugEnabled) LoggerValues.IfDebug()?.Message($"{_action.____GetDebugAddress()} | {x.Value:F2}").Write();
                        return false;
                    }
                    return true;
                }
                public int GetHashCode(InputActionValueState x) => x.Value.GetHashCode();
            }
        }
        
        private class TriggerPropertyWrapper : IPropertyWrapper
        {
            private readonly ReactiveProperty<InputActionTriggerState> _property;
            public TriggerPropertyWrapper(InputActionDef action) => _property = new ReactiveProperty<InputActionTriggerState>(default, new Comparer(action));
            public IStream<InputActionTriggerState> Stream => _property;
            public void Update(InputActionState state) => _property.Value = state.IsValid ? state.TriggerState : default;
            public void Dispose() => _property.Dispose();
            private class Comparer : IEqualityComparer<InputActionTriggerState>
            {
                private readonly InputActionDef _action;
                public Comparer(InputActionDef action) => _action = action;
                public bool Equals(InputActionTriggerState x, InputActionTriggerState y)
                {
                    if (x != y)
                    {
                        if (LoggerTriggers.IsDebugEnabled && (y.Activated || y.Deactivated)) LoggerTriggers.IfDebug()?.Message($"{_action.____GetDebugAddress()} | {(y.Activated ? "ACTIVATED" : "deactivated")}").Write();
                        return false;
                    }
                    return true;
                }
                public int GetHashCode(InputActionTriggerState x) => x.GetHashCode();
            }
        }
    }
}