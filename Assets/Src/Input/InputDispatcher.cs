using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Src.Tools;
using ColonyShared.SharedCode.Input;
using ColonyShared.SharedCode.InputActions;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using SharedCode.Utils.DebugCollector;
using Src.Locomotion;
using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;

namespace Src.Input
{
    public class InputDispatcher : IInputDispatcher
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly float ValueChangingThreshold = 0.001f;
        
        private readonly Dictionary<InputActionDef, IActionHolder> _actions = new Dictionary<InputActionDef, IActionHolder>();
        private readonly List<ListenerHolder> _listeners = new List<ListenerHolder>();
        
        public IEnumerable<InputSourceDef> GetBindingForAction(InputActionDef action)
        {
            var idx = _listeners.FindIndex(x => x.SlotDef.HasAction(action));
            return idx != -1 ? _listeners[idx].Listener.Sources : null;
        }

        public InputSlotDef GetSlotForAction(InputActionDef action)
        {
            var idx = _listeners.FindIndex(x => x.SlotDef.HasAction(action));
            return idx != -1 ? _listeners[idx].SlotDef : null;
        }
       
        public void AddBinding([NotNull] InputListener listener, [NotNull] InputSlotDef def)
        {
            if(_listeners.Any(x => x.Listener == listener)) throw new ArgumentException($"Input listener already exists in input processor");
            var holder = new ListenerHolder
            (
                slotDef: def ?? throw new ArgumentNullException(nameof(def)),
                listener: listener ?? throw new ArgumentNullException(nameof(listener)),
                value: def.Value?.Select(GetOrAddAction).Where(x => x != null).ToArray(),
                pressTrigger: def.Press?.Select(GetOrAddAction).Where(x => x != null).ToArray(),
                holdTrigger: def.Hold?.Select(GetOrAddAction).Where(x => x != null).ToArray(),
                clickTrigger: def.Click?.Select(GetOrAddAction).Where(x => x != null).ToArray(),
                releaseTrigger: def.Release?.Select(GetOrAddAction).Where(x => x != null).ToArray()
            );

            if (Logger.IsDebugEnabled)
            {
                var str = string.Empty;
                if (holder.Value != null) str += "\nValue:[" + string.Join(", ", holder.Value.Select(x => x.Action.ActionToString())) + "]";
                if (holder.PressTrigger != null) str += "\nPressTrigger:[" + string.Join(", ", holder.PressTrigger.Select(x => x.Action.ActionToString())) + "]";
                if (holder.HoldTrigger != null) str += "\nHoldTrigger:[" + string.Join(", ", holder.HoldTrigger.Select(x => x.Action.ActionToString())) + "]";
                if (holder.ClickTrigger != null) str += "\nClickTrigger:[" + string.Join(", ", holder.ClickTrigger.Select(x => x.Action.ActionToString())) + "]";
                if (holder.ReleaseTrigger != null) str += "\nReleaseTrigger:[" + string.Join(", ", holder.ReleaseTrigger.Select(x => x.Action.ActionToString())) + "]";
                Logger.IfDebug()?.Message($"Add Input Binding | {listener} -> {def.____GetDebugAddress()} | {str}").Write();
            }
            
            _listeners.Add(holder);
        }

        public void AddBlocker(InputActionDef action)
        {
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Add Input Blocker | {action}").Write();
            if(action != null)
                _actions.GetOrCreate(action, a => new Blocker(action));
        }

        public bool IsActionBlocked(InputActionDef action)
        {
            return _actions.TryGetValue(action, out var valueHolder) && valueHolder.Blocker;
        }
        
        public void UpdateStates(Dictionary<InputActionDef, InputActionState> states)
        {
            UnityQueueHelper.AssertInUnityThread();
            foreach (var holder in _listeners)
            {
                var listener = holder.Listener; 
                listener.Update();

                if (holder.Value != null)
                {
                    holder.ValueState = new InputActionValueState(listener.Value);
                    foreach (var h in holder.Value)
                        h.NewState = MergeValueState(h.NewState, holder.ValueState);
                }
                if (holder.PressTrigger != null)
                {
                    holder.PressState = new InputActionTriggerState(listener.Pressed, listener.PressEvent, listener.ReleaseEvent);
                    foreach (var h in holder.PressTrigger)
                        h.NewState = MergeTriggerState(h.NewState, holder.PressState);
                }
                if (holder.HoldTrigger != null)
                {
                    holder.HoldState = new InputActionTriggerState(listener.Holded, listener.HoldEvent, holder.HoldState.Active && !listener.Holded);
                    foreach (var h in holder.HoldTrigger)
                        h.NewState = MergeTriggerState(h.NewState, holder.HoldState);
                }
                if (holder.ClickTrigger != null)
                {
                    holder.ClickState = new InputActionTriggerState(listener.ClickEvent, listener.ClickEvent, holder.ClickState.Active && !listener.ClickEvent);
                    foreach (var h in holder.ClickTrigger)
                        h.NewState = MergeTriggerState(h.NewState, holder.ClickState);
                }
                if (holder.ReleaseTrigger != null)
                {
                    holder.ReleaseState = new InputActionTriggerState(listener.ReleaseEvent, listener.ReleaseEvent, holder.ReleaseState.Active && !listener.ReleaseEvent);
                    foreach (var h in holder.ReleaseTrigger)
                        h.NewState = MergeTriggerState(h.NewState, holder.ReleaseState);
                }
            }

            foreach (var holder in _actions.Values)
            {
                holder.UpdateState(states);
            }
        }

        public void ResetStates()
        {
            foreach (var holder in _actions.Values)
                holder.Reset();
            foreach (var holder in _listeners)
            {
                holder.Listener.Reset();
                holder.ValueState = default;
                holder.PressState = default;
                holder.ClickState = default;
                holder.HoldState = default;
                holder.ReleaseState = default;
            }
        }

        private static InputActionTriggerState MergeTriggerState(InputActionTriggerState l, InputActionTriggerState r)
        {
            return new InputActionTriggerState(
                active: l.Active || r.Active,
                activated: l.Activated && (!r.Active || r.Activated) || r.Activated && !l.Active,
                deactivated: l.Deactivated && !r.Active || r.Deactivated && !l.Active
            );
        }
        
        private InputActionValueState MergeValueState(InputActionValueState l, InputActionValueState r)
        {
            return Abs(l.Value) > Abs(r.Value) ? new InputActionValueState(l.Value) : new InputActionValueState(r.Value);
        }
        
        private ValueActionHolder GetOrAddAction(InputActionValueDef action)
        {
            return action == null ? null : _actions.GetOrCreate(action, a => new ValueActionHolder(a));
        }

        private TriggerActionHolder GetOrAddAction(InputActionTriggerDef action)
        {
            return action == null ? null : _actions.GetOrCreate(action, a => new TriggerActionHolder(a));
        }

        private interface IActionHolder
        {
            InputActionDef Action { get; }
            bool Blocker { get; }
            void UpdateState(Dictionary<InputActionDef, InputActionState> states);
            void Reset();
        }
        
        private sealed class ValueActionHolder : IActionHolder
        {
            private readonly InputActionDef _action;
            
            public ValueActionHolder(InputActionDef action) => _action = action;

            public InputActionDef Action => _action;
            
            public void UpdateState(Dictionary<InputActionDef, InputActionState> states)
            {
                if (!states.ContainsKey(_action))
                    states[_action] = new InputActionState(NewState);
                NewState = default;
            }

            public InputActionValueState NewState;
            bool IActionHolder.Blocker => false;
            public void Reset() => NewState = default;
        }
        
        private sealed class TriggerActionHolder : IActionHolder
        {
            private readonly InputActionDef _action;
            
            public TriggerActionHolder(InputActionDef action) => _action = action;

            public InputActionDef Action => _action;

            public void UpdateState(Dictionary<InputActionDef, InputActionState> states)
            {
                if (!states.ContainsKey(_action))
                    states[_action] = new InputActionState(NewState);
                NewState = default;
            }

            public InputActionTriggerState NewState;
            bool IActionHolder.Blocker => false;
            public void Reset() => NewState = default;
        }
        
        private sealed class Blocker : IActionHolder
        {
            private readonly InputActionDef _action;

            public Blocker(InputActionDef action) => _action = action;

            public InputActionDef Action => _action;
            
            public void UpdateState(Dictionary<InputActionDef, InputActionState> states)
            {
                if (!states.ContainsKey(_action))
                    states[_action] = new InputActionState();
            }
            bool IActionHolder.Blocker => true;
            public void Reset() {}
        }
        
        private class ListenerHolder
        {
            public readonly InputSlotDef SlotDef;
            public readonly InputListener Listener;
            public readonly ValueActionHolder[] Value;
            public readonly TriggerActionHolder[] PressTrigger;
            public readonly TriggerActionHolder[] HoldTrigger;
            public readonly TriggerActionHolder[] ClickTrigger;
            public readonly TriggerActionHolder[] ReleaseTrigger;

            public InputActionValueState ValueState;
            public InputActionTriggerState PressState;
            public InputActionTriggerState HoldState;
            public InputActionTriggerState ClickState;
            public InputActionTriggerState ReleaseState;

            public ListenerHolder(InputSlotDef slotDef, InputListener listener, ValueActionHolder[] value, TriggerActionHolder[] pressTrigger, TriggerActionHolder[] holdTrigger, TriggerActionHolder[] clickTrigger, TriggerActionHolder[] releaseTrigger)
            {
                SlotDef = slotDef;
                Listener = listener;
                Value = value;
                PressTrigger = pressTrigger;
                HoldTrigger = holdTrigger;
                ClickTrigger = clickTrigger;
                ReleaseTrigger = releaseTrigger;
            }
        }
    }
}