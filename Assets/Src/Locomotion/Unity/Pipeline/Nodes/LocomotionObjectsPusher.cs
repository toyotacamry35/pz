using UnityEngine;

namespace Src.Locomotion.Unity
{
    public class LocomotionObjectsPusher : ILocomotionPipelineCommitNode
    {
        private readonly Collider _collider;
        private readonly LayerMask _layers;
        private static readonly Collider[] Buffer = new Collider[256];

        public LocomotionObjectsPusher(Collider collider, LayerMask layers)
        {
            _collider = collider;
            _layers = layers;
        }

        public bool IsReady => true;
        
        public void Commit(ref LocomotionVariables inVars, float dt)
        {
            var position = LocomotionHelpers.LocomotionToWorldVector(inVars.Position).ToUnity();
            var rotation = LocomotionHelpers.LocomotionToWorldOrientation(inVars.Orientation).ToUnity();
            int count = PhysicsUtils.OverlapColliderNonAlloc(_collider, position, rotation, Buffer, _layers, QueryTriggerInteraction.Ignore);
            for (int i=0; i<count; ++i)
            {
                var body = Buffer[i].attachedRigidbody;
                if (!body || body.isKinematic || body.transform.root == _collider.transform.root)
                    continue;

                var pushDir = (body.transform.position - position).normalized;
                var pushVel = pushDir * Mathf.Max(Vector3.Dot(pushDir, new LocomotionVector(inVars.Velocity.Horizontal).ToWorld().ToUnity()), 0.5f);
                body.AddForce(pushVel, ForceMode.VelocityChange);
            }
        }
    }
}