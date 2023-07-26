using Assets.ColonyShared.SharedCode.Utils;
using SharedCode.Entities;
using SharedCode.Utils;
using Src.Locomotion;

namespace GeneratedCode.DeltaObjects
{
    public class PositionHistory : IPositionHistory
    {
        private readonly RingBuffer<Frame> _history;
        private readonly Vector3 _scale;

        public PositionHistory(int capacity, Vector3 scale)
        {
            _history = new RingBuffer<Frame>(capacity);
            _scale = scale;
        }

        public Transform GetTransformAt(long timestamp)
        {
            lock (_history)
            {
                var itr = _history.GetEnumerator();
                if (!itr.MoveNext())
                    return default(Transform);

                var nextFrame = itr.Current;
                if (timestamp < nextFrame.Timestamp)
                    while (itr.MoveNext())
                    {
                        var frame = itr.Current;
                        if (timestamp >= frame.Timestamp)
                        {
                            var t = SharedHelpers.InverseLerp(frame.Timestamp, nextFrame.Timestamp, timestamp);
                            return new Transform(
                                Vector3.Lerp(frame.Position, nextFrame.Position, t),
                                SharedHelpers.Lerp(frame.Rotation, nextFrame.Rotation, t),
                                _scale
                            );
                        }
                    }
                return new Transform(nextFrame.Position, nextFrame.Rotation, _scale);
            }
        }

        public void Push(Vector3 position, float rotation, long timestamp)
        {
            lock (_history)
            {
                int i = 0;
                while (i < _history.Count && _history[i].Timestamp > timestamp) 
                    ++i;
                _history.InsertFront(i, new Frame {Position = position, Rotation = rotation, Timestamp = timestamp});
            }
        }
        
        private struct Frame
        {
            public Vector3 Position;
            public float Rotation;
            public long Timestamp;
        }
    }
}
