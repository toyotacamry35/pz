using ColonyShared.SharedCode.Aspects.Locomotion;
using Src.Locomotion.Unity;
using UnityEngine;
using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;
using static Src.Locomotion.LocomotionHelpers;

namespace Src.Locomotion
{
    public class LocomotionDynamicBody : ILocomotionBody, ILocomotionPipelineCommitNode, ILocomotionPipelineFetchNode
    {
        private readonly Rigidbody _body;

        public LocomotionDynamicBody(Rigidbody body, ISettings settings)
        {
            _body = body;
            _body.isKinematic = false;
            _body.useGravity = false; 
            _body.interpolation = RigidbodyInterpolation.Interpolate;
            _body.maxDepenetrationVelocity = settings.DepenetrationSpeedLimit;
            _body.maxAngularVelocity = float.MaxValue / 10;
        }

        public LocomotionVector Velocity => WorldToLocomotionVector(_body.velocity);

        public LocomotionVector Position => WorldToLocomotionVector(_body.position);

        public float Orientation => WorldToLocomotionOrientation(_body.rotation);

        public SharedCode.Utils.Vector2 Forward => WorldToLocomotionVector(_body.rotation * Vector3.forward).Horizontal.normalized;

        public bool IsReady => true;

        LocomotionVariables ILocomotionPipelineFetchNode.Fetch(float dt)
        {
            return new LocomotionVariables(0, Position, Velocity, Orientation);
        }

        //#Important!: @param `inVars`: 'ref' === 'in' here - i.e. "const-ref" (only to avoid struct copying)
        // So DO NOT change `inVars`!
        void ILocomotionPipelineCommitNode.Commit(ref LocomotionVariables inVars, float dt)
        {
            if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## Loco Commit: c)LocoDynamicBody");

            if ((inVars.Flags & LocomotionFlags.Teleport) != 0)
            {
                var v = inVars.Position + inVars.ExtraPosition;
                _body.position = VectorConvert.LocomotionToWorldVectorAndToUnity(ref v);
                _body.rotation = VectorConvert.LocomotionToWorldOrientationAndToUnity(inVars.Orientation);
            }
            else
            {
                var orientation = WorldToLocomotionOrientation(_body.rotation);
                var delta = DeltaAngleRad(orientation, inVars.Orientation); 
                // уменьшение джиттера при повороте тела
                if (Mathf.Abs(delta) < Mathf.PI * 0.5f)
                {
                    // Выставляем ориентацию через угловую скорость, а не через MoveRotation, потому, что для dynamic body MoveRotation работает без интерполяции, и это весьма заметно  
                    //_body.MoveRotation((Quaternion)LocomotionHelpers.LocomotionToWorldOrientation(vars.Orientation));
                    _body.angularVelocity = new Vector3(0, -delta / dt, 0);
                }
                else
                {
                    _body.angularVelocity = Vector3.zero;
                    _body.MoveRotation((Quaternion)LocomotionToWorldOrientation(inVars.Orientation));
                }
                if(!inVars.ExtraPosition.ApproximatelyZero())
                    _body.MovePosition(_body.position + VectorConvert.LocomotionToWorldVectorAndToUnity(ref inVars.ExtraPosition));
            }

            var vec = inVars.Velocity + inVars.ExtraVelocity;
            _body.velocity = VectorConvert.LocomotionToWorldVectorAndToUnity(ref vec);

            if (LocomotionProfiler.EnableProfile) LocomotionProfiler.EndSample();
        }

        public interface ISettings
        {
            float DepenetrationSpeedLimit { get; }
        }
    }
}