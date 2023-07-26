using System;
using Assets.ColonyShared.SharedCode.Interfaces;
using Src.Locomotion.Unity;
using UnityEngine;
using static Src.Locomotion.LocomotionHelpers;

namespace Src.Locomotion
{
    public class LocomotionTransformBody : ILocomotionBody, ILocomotionPipelineCommitNode, ILocomotionPipelineFetchNode, IResettable
    {
        private readonly Transform _body;
        private LocomotionVector _velocity;

        public LocomotionTransformBody(Transform body)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));
            _body = body;
        }

        public void Reset()
        {
            _velocity = default;
        }

        public LocomotionVector Velocity => _velocity;

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
            if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## Loco Commit: h)LocoTransformBody");

            var v = inVars.Position + inVars.ExtraPosition;
            _body.position = VectorConvert.LocomotionToWorldVectorAndToUnity(ref v); 
            _body.rotation = VectorConvert.LocomotionToWorldOrientationAndToUnity(inVars.Orientation);
            _velocity = inVars.Velocity;

            LocomotionProfiler.EndSample();
        }
    }
}