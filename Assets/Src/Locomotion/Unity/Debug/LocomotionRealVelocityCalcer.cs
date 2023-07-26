using UnityEngine;
using static Src.Locomotion.DebugTag;

namespace Src.Locomotion.Unity
{
    public static partial class LocomotionDebugUnity
    {
        public static ILocomotionDebugable GatherRealVelocity(Transform transform) => new LocomotionRealVelocityCalcer(transform);

        private class LocomotionRealVelocityCalcer : ILocomotionDebugable
        {
            private readonly Transform _transform;
            private float _prevTime;
            private Vector3 _prevBodyPosition;

            public LocomotionRealVelocityCalcer(Transform transform)
            {
                _transform = transform;
            }

            public void GatherDebug(ILocomotionDebugAgent agent)
            {
                if (agent == null || !_transform)
                    return;

                var time = Time.time;
                var deltaTime = time - _prevTime;
                if (deltaTime > 0)
                    agent.Set(RealVelocity, (_transform.position - _prevBodyPosition).ToShared() / deltaTime);
                _prevBodyPosition = _transform.position;
                _prevTime = time;
            }
        }
    }
}