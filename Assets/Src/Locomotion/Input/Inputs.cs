using System;
using System.Collections.Generic;

namespace Src.Locomotion
{
    public abstract class Inputs
    {        
        public abstract int AxisCount { get; }
        
        public abstract int TriggerCount { get; }

        
        public abstract InputInfo GetInputInfo(string name);
        
        protected class Registry
        {
            private readonly string _name;
            private readonly Registry _base;
            private int _axisIndex;
            private int _triggerIndex;
            private readonly Dictionary<string, InputInfo> _inputs = new Dictionary<string, InputInfo>(StringComparer.OrdinalIgnoreCase);

            public Registry(string name, Registry @base = null)
            {
                _name = name;
                _base = @base;
                if (@base != null)
                {
                    _axisIndex = @base._axisIndex;
                    _triggerIndex = @base._triggerIndex;
                }
            }
            
            public InputAxis Axis(string name)
            {
                var axis = new InputAxis(_axisIndex++);
                Register(new InputInfo(name, axis));
                return axis;
            }

            public InputTrigger Trigger(string name)
            {
                var trigger = new InputTrigger(_triggerIndex++);
                Register(new InputInfo(name, trigger));
                return trigger;
            }

            public InputAxes Axes(InputAxis first, InputAxis second)
            {
                return new InputAxes(first, second);
            }

            public int AxisCount => _axisIndex + (_base?.AxisCount ?? 0);            
            
            public int TriggerCount => _triggerIndex + (_base?.TriggerCount ?? 0);
            

            public InputInfo GetInputInfo(string name)
            {
                InputInfo info = GetInputInfoImpl(name);
                if (info == null)
                    throw new KeyNotFoundException($"Input with name {name} not registered in {_name}");
                return info;
            } 

            private void Register(InputInfo info)
            {
                var name = info.Name.ToLower();
                if (GetInputInfoImpl(name) != null)
                    throw new InvalidOperationException($"Input with name {name} already registered in {_name}");                    
                _inputs.Add(name, info);
//                 Debug.LogFormat("{0} Register: {1}", _name, info);
            }
            
            private InputInfo GetInputInfoImpl(string name) => _inputs.TryGetValue(name, out var info) ? info : _base?.GetInputInfoImpl(name);
        }
    }
}