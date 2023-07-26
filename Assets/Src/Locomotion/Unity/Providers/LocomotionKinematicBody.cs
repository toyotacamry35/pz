using System;
using Assets.ColonyShared.SharedCode.Interfaces;
using Assets.Src.Locomotion.Debug;
using Assets.Src.Locomotion.Utils;
using ColonyShared.SharedCode.Aspects.Locomotion;
using ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using NLog;
using SharedCode.Utils;
using Src.Locomotion.Unity;
using UnityEngine;

using static Src.Locomotion.LocomotionHelpers;
using static Src.Locomotion.LocomotionDebug;

using Vector3 = UnityEngine.Vector3;

namespace Src.Locomotion
{
    public class LocomotionKinematicBody : ILocomotionBody, ILocomotionPipelineCommitNode, ILocomotionPipelineFetchNode, IResettable
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(LocomotionKinematicBody));

        private readonly Rigidbody _body;
        private readonly ICurveLoggerProvider _curveLogProv;
        private readonly IFrameIdNormalizer _frameIdNormalizer;
        private LocomotionVector _lastPosition;
        // Unlike `_lastPosition` this one == inVars.Position + inVars.ExtraPosition
        private LocomotionVector _lastTotalPosition;
        private float _lastOrientation;
        private SharedCode.Utils.Vector2 _lastForward;
        private LocomotionVector _lastVelocity;

        public LocomotionKinematicBody(
            Rigidbody body,
            [CanBeNull] ILocoCurveLoggerProvider curveLogProv = null)
        {
            _body = body;
            _curveLogProv = curveLogProv;
            _frameIdNormalizer = (IFrameIdNormalizer)curveLogProv ?? DefaultFrameIdNormalizer.Instance;

            _body.isKinematic = true;
            _body.interpolation = RigidbodyInterpolation.Interpolate;
            _lastPosition      = WorldToLocomotionVector(_body.position);
            _lastTotalPosition = WorldToLocomotionVector(_body.position);
            _lastOrientation = WorldToLocomotionOrientation(_body.rotation);
            _lastForward = LocomotionOrientationToForwardVector(_lastOrientation);
        }

        public void Reset()
        {
            _lastPosition      = WorldToLocomotionVector(_body.position);
            _lastTotalPosition = WorldToLocomotionVector(_body.position);
            _lastOrientation = WorldToLocomotionOrientation(_body.rotation);
            _lastForward = LocomotionOrientationToForwardVector(_lastOrientation);
            _lastGoPos = _body.transform.position;
            _lastGoPos = Vector3.zero;
        }

        public LocomotionVector Velocity => _lastVelocity;

        public LocomotionVector Position => _lastPosition;

        public float Orientation => _lastOrientation;

        public SharedCode.Utils.Vector2 Forward => _lastForward;
        
        public bool IsReady => true;

        LocomotionVariables ILocomotionPipelineFetchNode.Fetch(float dt)
        {
            return new LocomotionVariables(0, Position, _lastVelocity, Orientation);            
        }

        //#Important!: @param `inVars`: 'ref' === 'in' here - i.e. "const-ref" (only to avoid struct copying)
        // So DO NOT change `inVars`!
        void ILocomotionPipelineCommitNode.Commit(ref LocomotionVariables inVars, float dt)
        {
            if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## Loco Commit: d)LocoKinematicBody");

            if (!inVars.Position.ApproximatelyEqual(_lastPosition))
            {
                var v = inVars.Position + inVars.ExtraPosition;
                var newPosition = VectorConvert.LocomotionToWorldVectorAndToUnity(ref v);

                if ((inVars.Flags & LocomotionFlags.Teleport) != 0)
                    _body.position = newPosition;
                else
                    _body.MovePosition(newPosition);

                _lastPosition = inVars.Position;
                _lastTotalPosition = v;

                //DebugAgent.Set(LocoConsts.ClAppliedToGoPosition, v);
            }
            if (!inVars.Orientation.ApproximatelyEqual(_lastOrientation))
            {
                var newRotation = VectorConvert.LocomotionToWorldOrientationAndToUnity(inVars.Orientation);
                if ((inVars.Flags & LocomotionFlags.Teleport) != 0)
                    _body.rotation = newRotation;
                else
                    _body.MoveRotation(newRotation);
                _lastOrientation = inVars.Orientation;
                _lastForward = LocomotionOrientationToForwardVector(inVars.Orientation);
            }
            _lastVelocity = inVars.Velocity;
            //_body.velocity = Vector3.zero; //@Andrey: it's no need.

            if (DebugAgent.IsNotNullAndActive())
            {
                _curveLogProv?.CurveLogger?.IfActive?.AddData("5)Cl_Appl-edToGo.Pos",        SyncTime.Now, _lastTotalPosition);
                _curveLogProv?.CurveLogger?.IfActive?.AddData("5)Cl_Appl-edToGo.Velo",       SyncTime.Now, _lastVelocity);
                _curveLogProv?.CurveLogger?.IfActive?.AddData("5)Cl_Appl-edToGo.VeloAsDPos", SyncTime.Now, CurveLoggerExt.VeloAsDltPos(_lastTotalPosition, _lastVelocity));
                if(false)_curveLogProv?.CurveLogger?.IfActive?.AddData("5)Cl_Appl-edToGo.FrameId", SyncTime.Now, _frameIdNormalizer.NormalizeFrameId(inVars.Timestamp));

                var goPos = _body.transform.position;
                ///var utcNow = DateTime.UtcNow;
                ///if (_lastVeloCalcTime != DateTime.MinValue && _lastGoPos != Vector3.zero) // if not defaults
                ///     _currGoVelo = (goPos - _lastGoPos) / (float)(utcNow - _lastVeloCalcTime).TotalSeconds;
                if (_lastGoPos != Vector3.zero) // if not defaults
                    _currGoVelo = (goPos - _lastGoPos) / dt;

                ///_lastVeloCalcTime = utcNow;
                _lastGoPos = goPos;

                _curveLogProv?.CurveLogger?.IfActive?.AddData("6)Cl_GoFactual.Pos",        SyncTime.Now, goPos.ToShared()); //_body.position.ToLocomotion());
                _curveLogProv?.CurveLogger?.IfActive?.AddData("6)Cl_GoFactual.Velo",       SyncTime.Now, _currGoVelo.ToShared()); //_body.velocity.ToLocomotion());
                _curveLogProv?.CurveLogger?.IfActive?.AddData("6)Cl_GoFactual.VeloAsDPos", SyncTime.Now, CurveLogger.VeloAsDltPos(goPos.ToShared(), _currGoVelo.ToShared()));
                _curveLogProv?.CurveLogger?.IfActive?.AddData("6)Cl_GoFactual.FrameId",    SyncTime.Now, _frameIdNormalizer.NormalizeFrameId(inVars.Timestamp));
            }

            LocomotionProfiler.EndSample();
        }

        // --- LocomotionPositionLogAgent state data ------------------------------------
        private DateTime _lastVeloCalcTime = DateTime.MinValue;
        private Vector3 _lastGoPos = Vector3.zero;
        private Vector3 _currGoVelo = Vector3.zero;
    }
}