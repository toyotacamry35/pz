using SharedCode.Utils;

namespace Src.Locomotion
{
    internal struct InputAxisEnumerator
    {
        private readonly ILocomotionInputProvider _input;
        private readonly InputAxis _axis;
        private readonly int _cnt;
        private float _time;
        private float _extra;
        private int _i;

        public float Current;

        public InputAxisEnumerator(ILocomotionInputProvider input, InputAxis axis, float time, int extra)
        {
            _input = input;
            _axis = axis;
            _i = 0;
            _time = time;
            _extra = extra;
            _cnt = input.HistoryCount;
            Current = 0;
        }

        public bool MoveNext()
        {
            if (_i < _cnt && _time >= 0)
            {
                var frame = _input.History(_i);
                _time -= frame.DeltaTime;
                Current = frame[_axis];
                ++_i;
                return true;
            }            
            if (_extra > 0)
            {
                Current = _i < _cnt ? _input.History(_i)[_axis] : 0;
                --_extra;
                ++_i;
                return true;
            }
            return false;
        }
    }
    
    
    internal struct InputAxesEnumerator
    {
        private readonly ILocomotionInputProvider _input;
        private readonly InputAxes _axes;
        private readonly int _cnt;
        private float _time;
        private float _extra;
        private int _i;

        public Vector2 Current;
        
        public InputAxesEnumerator(ILocomotionInputProvider input, InputAxes axes, float time, int extra)
        {
            _input = input;
            _axes = axes;
            _i = 0;
            _time = time;
            _extra = extra;
            _cnt = input.HistoryCount;
            Current = Vector2.zero;
        }

        public bool MoveNext()
        {
            if (_i < _cnt && _time >= 0)
            {
                var frame = _input.History(_i);
                _time -= frame.DeltaTime;
                Current.x = frame[_axes.First];
                Current.y = frame[_axes.Second];
                ++_i;
                return true;
            }            
            if (_extra > 0)
            {
                if (_i < _cnt)
                {
                    var frame = _input.History(_i);
                    Current.x = frame[_axes.First];
                    Current.y = frame[_axes.Second];
                }
                else
                    Current = Vector2.zero;
                --_extra;
                ++_i;
                return true;
            }
            return false;
        }
    }
    
    
    internal struct InputTriggerEnumerator
    {
        public const float TriggerThreshold = 0.01f;
        private readonly ILocomotionInputProvider _input;
        private readonly InputTrigger _trigger;
        private readonly int _cnt;
        private float _time;
        private float _extra;
        private int _i;

        public bool Current;
        
        public InputTriggerEnumerator(ILocomotionInputProvider input, InputTrigger trigger, float time, int extra)
        {
            _input = input;
            _trigger = trigger;
            _i = 0;
            _time = time;
            _extra = extra;
            _cnt = input.HistoryCount;
            Current = false;
        }

        public bool MoveNext()
        {
            if (_i < _cnt && _time >= 0)
            {
                var frame = _input.History(_i);
                _time -= frame.DeltaTime;
                Current = frame[_trigger];
                ++_i;
                return true;
            }            
            if (_extra > 0)
            {
                Current = _i < _cnt && _input.History(_i)[_trigger];
                --_extra;
                ++_i;
                return true;
            }
            return false;
        }
    }
}