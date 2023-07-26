using System.Collections.Generic;

namespace Src.Locomotion
{
    public class AverageValue
    {
        private readonly float _period;
        private readonly Queue<Frame> _buffer;
        private float _sum;
        private float _value;
        private float _time;

        public int Dbg_Count => _buffer.Count;

        public AverageValue(float period)
        {
            _period = period;
            _buffer = new Queue<Frame>(16);
            _buffer.Enqueue(new Frame());
        }
        
        public float Value => _value;

        public static implicit operator float(AverageValue sv)
        {
            return sv.Value;
        }

        public AverageValue Update(float value, float deltaTime)
        {
            _time += deltaTime;
            _sum += value;
            while (_time > _period && _buffer.Count>0)
            {
                var f = _buffer.Dequeue();
                _sum -= f.Value;
                _time -= f.DeltaTime;
            }
            _buffer.Enqueue(new Frame ( value, deltaTime ));
            _value = _sum / _buffer.Count;
            return this;
        }

        readonly struct Frame
        {
            public readonly float Value;
            public readonly float DeltaTime;

            public Frame(float value, float deltaTime)
            {
                Value = value;
                DeltaTime = deltaTime;
            }
        }
    }
}