using System;
using ColonyShared.SharedCode.InputActions;
using Core.Environment.Logging.Extension;
using NLog;
using Src.Locomotion;
using static UnityQueueHelper;

namespace Src.InputActions
{
    public class InputActionHandlerLocomotionTrigger : IInputActionHandlerLocomotion, IInputActionTriggerHandler
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly CharacterInputs Inputs = new CharacterInputs();  
        private readonly InputTrigger _trigger;
        private readonly string _triggerName;
        private readonly object _lock = new object();
        private bool _activated;
        private bool _state;
        private readonly Action<IInputActionHandlerLocomotion> _onDispose;
        private readonly int _bindingId;

        public InputActionHandlerLocomotionTrigger(string inputName, Action<IInputActionHandlerLocomotion> onDispose, int bindingId)
        {
            _trigger = Inputs.GetInputInfo(inputName).Trigger;
            _triggerName = inputName;
            _onDispose = onDispose;
            _bindingId = bindingId;
        }
        
        public bool PassThrough => false;

        public void ProcessEvent(InputActionTriggerState @event, InputActionHandlerContext ctx, bool inactive)
        {
            lock (_lock)
            {
                _state = @event.Active;
                _activated |= @event.Active;
                if(Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Process Event | Binding:#{_bindingId} Handler:{this} Event:{@event} State:{_state} Activated:{_activated}").Write();
            }
        }

        public void FetchInputValue(InputState<CharacterInputs> frame)
        {
            AssertInUnityThread();
            if (_trigger != InputTrigger.None)
            {
                lock (_lock)
                {
                    frame[_trigger] |= _activated | _state;
                    _activated = false;
                }
            }
        }

        public void Dispose()
        {
            _onDispose?.Invoke(this);
        }
        
        public override string ToString() => $"{nameof(InputActionHandlerLocomotionTrigger)}(Trigger:{_triggerName})";
    }
}