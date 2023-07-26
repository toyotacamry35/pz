using System.Collections.Generic;
using SharedCode.Utils;

namespace Src.Locomotion
{
    public class AverageVector2
    {
        private readonly float _period;
        private readonly Queue<Frame> _buffer;
        private Vector2 _sum;
        private Vector2 _value;
        private float _time;

        public int Dbg_Count => _buffer.Count;

        public AverageVector2(float period)
        {
            _period = period;
            _buffer = new Queue<Frame>(16);
            _buffer.Enqueue(new Frame());
        }

        public Vector2 Value => _value;

        public static implicit operator Vector2(AverageVector2 sv)
        {
            return sv.Value;
        }
        
        public AverageVector2 Update(Vector2 value, float deltaTime)
        {
            _time += deltaTime;
            _sum += value;
            while (_time > _period && _buffer.Count>0)
            {
                var f = _buffer.Dequeue();
                _sum -= f.Value;
                _time -= f.DeltaTime;
            }
            _buffer.Enqueue(new Frame(value, deltaTime));
            _value = _sum / _buffer.Count;
            return this;
        }

        readonly struct Frame
        {
            public readonly Vector2 Value;
            public readonly float DeltaTime;

            public Frame(Vector2 value, float deltaTime)
            {
                Value = value;
                DeltaTime = deltaTime;
            }
        }
    }
}