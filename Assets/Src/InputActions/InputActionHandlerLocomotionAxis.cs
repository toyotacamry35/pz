using System;
using ColonyShared.SharedCode.InputActions;
using Core.Environment.Logging.Extension;
using NLog;
using Src.Locomotion;
using UnityEngine;
using static UnityQueueHelper;

namespace Src.InputActions
{
    public class InputActionHandlerLocomotionAxis : IInputActionHandlerLocomotion, IInputActionValueHandler
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        private static readonly CharacterInputs Inputs = new CharacterInputs();  
        private readonly InputAxis _axis;
        private readonly string _axisName;
        private readonly float _inv = 1;
        private readonly Action<IInputActionHandlerLocomotion> _onDispose;
        private readonly int _bindingId;
        private float _value;
        private float _lastValue;

        public InputActionHandlerLocomotionAxis(string inputName, Action<IInputActionHandlerLocomotion> onDispose, int bindingId)
        {
            _axisName = inputName;
            if (inputName.EndsWith("+"))
            {
                inputName = inputName.Substring(0, inputName.Length - 1);
            }
            else
            if (inputName.EndsWith("-"))
            {
                _inv = -1;
                inputName = inputName.Substring(0, inputName.Length - 1);
            }
            
            _axis = Inputs.GetInputInfo(inputName).Axis;
            _onDispose = onDispose;
            _bindingId = bindingId;
        }
        
        public bool PassThrough => false;

        public void ProcessEvent(InputActionValueState @event, InputActionHandlerContext ctx, bool inactive)
        {
            _value = @event.Value;
            if(Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"New value | Binding:#{_bindingId} Handler:{this} Event:{@event} Value:{_value}").Write();
        }

        public void FetchInputValue(InputState<CharacterInputs> frame)
        {
            AssertInUnityThread();
            if (_axis != InputAxis.None)
            {
                frame[_axis] += _value * _inv;
                if (_value !=  _lastValue) if(Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Fetch value | Binding:#{_bindingId} Handler:{this} Value:{_value}").Write();
                _lastValue = _value;
            }
        }

        public void Dispose()
        {
            _onDispose?.Invoke(this);
        }
        
        public override string ToString() => $"{nameof(InputActionHandlerLocomotionAxis)}(Axis:{_axisName})";
    }
}