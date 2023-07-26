using System;
using System.Threading;
using SharedCode.Utils;
using SharedCode.Wizardry;

namespace ColonyShared.SharedCode.InputActions
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct InputActionTriggerState : IEquatable<InputActionTriggerState>
    {
        public readonly bool Active;
        public readonly bool Activated;
        public readonly bool Deactivated;
        public readonly AwaitableSpellDoerCast Awaiter;
        public readonly long Id;
        public InputActionTriggerState(bool active, bool activated, bool deactivated, AwaitableSpellDoerCast awaiter = default)
        {
            Active = active;
            Activated = activated;
            Deactivated = deactivated;
            Awaiter = awaiter;
            Id = Interlocked.Increment(ref _idCounter);
        }

        public static bool operator==(InputActionTriggerState lhv, InputActionTriggerState rhv) => lhv.Active == rhv.Active && lhv.Activated == rhv.Activated && lhv.Deactivated == rhv.Deactivated;

        public static bool operator !=(InputActionTriggerState lhv, InputActionTriggerState rhv) => !(lhv == rhv);

        public bool Equals(InputActionTriggerState other) => this == other;

        public override bool Equals(object obj) => throw new NotSupportedException();

        public override int GetHashCode() => throw new NotSupportedException();
        
        public override string ToString()
        {
            var sb = StringBuildersPool.Get;
            sb.Append("#").Append(Id).Append('(');
            sb.Append(Active ? "Active" : "Not Active");
            if(Activated) sb.Append(" Activated");
            if(Deactivated) sb.Append(" Deactivated");
            sb.Append(")");
            return sb.ToStringAndReturn();
        }

        private static long _idCounter = 0;
    }
    
    
    /// <summary>
    /// 
    /// </summary>
    public readonly struct InputActionValueState : IEquatable<InputActionValueState>
    {
        public readonly float Value;
        public readonly long Id;

        public InputActionValueState(float v)
        {
            Value = v;
            Id = Interlocked.Increment(ref _idCounter);
        }
        
        public override string ToString()
        {
            return StringBuildersPool.Get.Append("#").Append(Id).Append("(V:").Append(Value.ToString("F2")).Append(")").ToStringAndReturn();
        }
        
        public bool Equals(InputActionValueState other)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return Value == other.Value;
        }

        public override bool Equals(object obj) => throw new NotImplementedException();

        public override int GetHashCode() => throw new NotImplementedException();

        private static long _idCounter;
    }
    
    
    /// <summary>
    /// 
    /// </summary>
    public readonly struct InputActionState 
    {
        private readonly InputActionValueState _valueState;
        private readonly InputActionTriggerState _triggerState;
        private readonly Type _type;

        public bool IsValid => _type != Type.Invalid;
        
        public InputActionValueState ValueState
        {
            get
            {
                if (_type != Type.Value) throw new Exception("Is not a value state");
                return _valueState;
            }
        }

        public InputActionTriggerState TriggerState
        {
            get
            {
                if (_type != Type.Trigger) throw new Exception("Is not a trigger state");
                return _triggerState;
            }
        }

        public InputActionState(InputActionValueState v)
        {
            _valueState = v;
            _triggerState = default;
            _type = Type.Value;
        }

        public InputActionState(InputActionTriggerState v)
        {
            _triggerState = v;
            _valueState = default;
            _type = Type.Trigger;
        }
        
        public override string ToString()
        {
            switch (_type)
            {
                case Type.Invalid:
                    return "<invalid>";
                case Type.Trigger:
                    return TriggerState.ToString();
                case Type.Value:
                    return ValueState.ToString();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private enum Type { Invalid, Trigger, Value }
    }
}