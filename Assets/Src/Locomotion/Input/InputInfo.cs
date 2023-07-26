using System;

namespace Src.Locomotion
{
    public enum InputType { Axis, Trigger }
    
    public class InputInfo
    {
        private readonly InputAxis _inputAxis;
        private readonly InputTrigger _inputTrigger;
        
        public readonly string Name;
        
        public readonly InputType Type;

        public InputAxis Axis
        {
            get
            {
                CheckType(InputType.Axis);
                return _inputAxis;
            }
        }
        
        public InputTrigger Trigger
        {
            get
            {
                CheckType(InputType.Trigger);
                return _inputTrigger;
            }
        }

        public InputInfo(string name, InputAxis axis)
        {
            Name = name;
            Type = InputType.Axis;
            _inputAxis = axis;
        }

        public InputInfo(string name, InputTrigger trigger)
        {
            Name = name;
            Type = InputType.Trigger;
            _inputTrigger = trigger;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case InputType.Axis:
                    return $"Axis.{Name}[{_inputAxis.Index}]";
                case InputType.Trigger:
                    return $"Trigger.{Name}[{_inputTrigger.Index}]";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CheckType(InputType type)
        {
            if(type != Type)
                throw new InvalidOperationException($"Trying to get input as {type} but it is {Type}");
        }
    }
}