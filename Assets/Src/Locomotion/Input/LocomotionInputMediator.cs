using System;
using System.Collections.Generic;
using SharedCode.Utils;

namespace Src.Locomotion
{
    public class LocomotionInputMediator<T> : ILocomotionInputReceiver where T : Inputs, new()
    {
        private readonly InputState<T> _inputs = new InputState<T>();
        private readonly List<CauserHolder> _causers = new List<CauserHolder>();

        public void SetInput(InputAxis it, float value)
        {
            _inputs[it] = value;
        }

        public void SetInput(InputAxes it, Vector2 value)
        {
            _inputs[it] = value;
        }

        public void SetInput(InputTrigger it, bool value)
        {
            _inputs[it] = value;
        }

        public void PushInput(object causer, string inputName, float value)
        {
            _causers.Add(new CauserHolder{ Causer = causer, Input = inputName, Value = value });
        }

        public void PopInput(object causer, string inputName)
        {
            for (int i = _causers.Count - 1; i >= 0 ; --i)
            {
                var x = _causers[i];
                if (x.Causer.Equals(causer) && StringComparer.OrdinalIgnoreCase.Equals(x.Input, inputName))
                {
                    _causers.RemoveAt(i);
                    break;
                }
            }
        }

        public void ApplyTo(InputState<T> inputs)
        {
            inputs.ApplyFrom(_inputs);
            _inputs.Clear();
            foreach (var causer in _causers)
                inputs[causer.Input] = causer.Value;
        }

        public void Clean()
        {
            _inputs.Clear();
            _causers.Clear();
        }

        //#Dbg
        public bool DBG_IsCLear()
        {
            return _causers.Count == 0 && _inputs.DBG_IsClear();
        }

        private struct CauserHolder
        {
            public object Causer;
            public string Input;
            public float Value;
        }
    }
}