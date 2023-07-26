using SharedCode.Entities;

namespace GeneratedCode.DeltaObjects
{
    public class TrivialPositionHistory : IPositionHistory
    {
        private Transform _transform;
        private readonly object _transformLock = new object();

        public void Update(Transform transform)
        {
            lock(_transformLock)
                _transform = transform;
        }
        
        public Transform GetTransformAt(long timestamp) => _transform;
    }
}