using System;
using ColonyShared.SharedCode.InputActions;
using Core.Environment.Logging.Extension;
using NLog;
using Src.Locomotion;
using static UnityQueueHelper;

namespace Src.InputActions
{
    public class InputActionHandlerLocomotionTriggerToAxis : IInputActionHandlerLocomotion, IInputActionTriggerHandler
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly CharacterInputs Inputs = new CharacterInputs();  
        private readonly InputAxis _axis;
        private readonly string _axisName;
        private readonly object _lock = new object();
        private bool _state;
        private bool _stateReaded = true;
        private readonly float _value;
        private readonly Action<IInputActionHandlerLocomotion> _onDispose;
        private readonly int _bindingId;

        public InputActionHandlerLocomotionTriggerToAxis(string inputName, float value, Action<IInputActionHandlerLocomotion> onDispose, int bindingId)
        {
            _axis = Inputs.GetInputInfo(inputName).Axis;
            _value = value;
            _axisName = inputName;
            _onDispose = onDispose;
            _bindingId = bindingId;
        }

        public bool PassThrough => false;

        public void ProcessEvent(InputActionTriggerState @event, InputActionHandlerContext ctx, bool inactive)
        {
            lock (_lock)
                if (_stateReaded)
                {
                    _state = @event.Active;
                    _stateReaded = false;
                    if(Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"New value | Binding:#{_bindingId} Handler:{this} Event:{@event} State:{_state}").Write();
                }
                else
                {
                    _state |= @event.Active;
                    if(Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Update value | Binding:#{_bindingId} Handler:{this} Event:{@event} State:{_state}").Write();
                }
        }

        public void FetchInputValue(InputState<CharacterInputs> frame)
        {
            AssertInUnityThread();
            if (_axis != InputAxis.None)
            {
                lock (_lock)
                {
                    frame[_axis] += _state ? _value : 0;
                    _stateReaded = true;
                }            
            }
        }

        public void Dispose()
        {
            _onDispose?.Invoke(this);
        }
        
        public override string ToString() => $"{nameof(InputActionHandlerLocomotionTriggerToAxis)}(Axis:{_axisName} Value:{_value})";
    }
}