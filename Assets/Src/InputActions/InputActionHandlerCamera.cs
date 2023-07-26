using System;
using ColonyShared.SharedCode.InputActions;
using NLog;
using UnityEngine;
using static UnityQueueHelper;

namespace Src.InputActions
{
    public class InputActionHandlerCamera : IInputActionValueHandler
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly Action<InputActionHandlerCamera> _onDispose;
        private readonly CameraAxis _axis;
        private float _value;

        public InputActionHandlerCamera(CameraAxis axis, Action<InputActionHandlerCamera> onDispose)
        {
            _axis = axis;
            _onDispose = onDispose;
        }

        public bool PassThrough => false;

        public void ProcessEvent(InputActionValueState @event, InputActionHandlerContext ctx, bool inactive)
        {
            // Bad Thread // if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Execute | Frame:{Time.frameCount} Value:{@event.Value}").Write();
            _value = @event.Value;
        }

        public void FetchInputValue(ref Vector2 axes)
        {
            AssertInUnityThread();
            var value = _value * ControlsModifiers.MouseSensivity;
       //     if(Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Fetch | Frame:{Time.frameCount} Value:{value}").Write();
            switch (_axis)
            {
                case CameraAxis.X:
                    axes.x += value;
                    break;
                case CameraAxis.Y:
                    if (ControlsModifiers.IsMouseInverted)
                        axes.y -= value;
                    else
                        axes.y += value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public void Dispose()
        {
            _onDispose?.Invoke(this);
        }
        
        public override string ToString() => $"{nameof(InputActionHandlerCamera)}(Axis:{_axis} Value:{_value})";
    }
}