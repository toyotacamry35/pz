using System;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Locomotion.Debug;
using ColonyShared.SharedCode.Aspects.Locomotion;
using Core.Environment.Logging.Extension;
using NLog;
using Src.Locomotion.Unity;
using UnityEngine;

using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;
using static Src.Locomotion.LocomotionHelpers;
using static Src.Locomotion.Unity.VectorConvert;

namespace Src.Locomotion
{
    public class LocomotionCharacterBody : ILocomotionBody, ILocomotionPipelinePassNode, ILocomotionPipelineFetchNode
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly CharacterController _body;
        private LocomotionVector _velocity;

        public LocomotionCharacterBody(
            ISettings settings, 
            CharacterController body)
        {
            _body = body ?? throw new ArgumentNullException(nameof(body));
            _body.enabled = true;
            _body.slopeLimit = settings.MaxWalkingSlopeAngle + 0.001f;
            _body.detectCollisions = false;
            _body.enableOverlapRecovery = true; // если выставить flase, то персонаж начинает проходить через стены во время прыжка (при ресайзе коллайдера) и даже проваливаться под землю  
        }

        public LocomotionVector Velocity => _velocity;
        public LocomotionVector Position => _body.transform.position.ToLocomotion();
        public float Orientation => WorldToLocomotionOrientation(_body.transform.rotation.ToShared());
        public SharedCode.Utils.Vector2 Forward => WorldToLocomotionVector(_body.transform.forward).Horizontal;
        public bool IsReady => true;

        public LocomotionVariables Fetch(float dt)
        {
            return new LocomotionVariables(0, Position, Velocity, Orientation);
            //#Dbg:
            // var res = new LocomotionVariables(0, Position, Velocity, Orientation);
            // if (_loggerProvider?.LogBackCounter > 0)
            //     _loggerProvider?.DLogger?.IfActive?.Log(_thisType, _loggerProvider?.LogBackCounter ?? -999, Consts.DbgTagIn, res);
            // return res;
        }

        public LocomotionVariables Pass(LocomotionVariables inVars, float dt)
        {
            if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## Loco Commit: c)LocomotionCharacterBody");
            if (inVars.Flags.Any(LocomotionFlags.Teleport))
            {
                var enabledStatus = _body.enabled;
                _body.enabled = false;
                _body.transform.position = inVars.Position.ToWorld().ToUnity();
                _body.enabled = enabledStatus;

                inVars.Velocity = _velocity = inVars.Velocity;
                //DbgLog.Log($". . . LocoCharBody:: Pass. {_body.transform.position}, Rot:{_body.transform.rotation.eulerAngles}");
            }
            else
            {
                var prevPos = _body.transform.position;
                var newPos = inVars.Position.ToWorld().ToUnity();
                var delta = newPos - prevPos;
                _body.Move(delta);
                Vector3 realPos;
                LocomotionVector realVelocity;
                if (!delta.ApproximatelyEqual(Vector3.zero, _body.minMoveDistance))
                {
                    realPos = _body.transform.position;
                    var realDelta = realPos - prevPos;
                    realVelocity = WorldToLocomotionVector(realDelta / dt);
                }
                else
                {
                    realPos = newPos;
                    realVelocity = inVars.Velocity;
                }
                
                //_velocity.Horizontal = realVelocity.Horizontal;
                //_velocity.Horizontal = velocity.Horizontal.Clamp(SharedCode.Utils.Vector2.Dot(velocity.Horizontal.Normalized, realVelocity.Horizontal)); //velocity.Horizontal;

                _velocity = new LocomotionVector(
                    realVelocity.Horizontal.Clamp(SharedCode.Utils.Vector2.Dot(realVelocity.Horizontal.Normalized, inVars.Velocity.Horizontal)),
                    Max(inVars.Velocity.Vertical, Min(realVelocity.Vertical, 0))
                ); // что-бы не подбрасывало вверх на ступеньках 
                //   _velocity.Horizontal = velocity.Horizontal.Clamp(SharedCode.Utils.Vector2.Dot(velocity.Horizontal.Normalized, WorldToLocomotionVector(_body.velocity).Horizontal)); //velocity.Horizontal;
                //  _velocity.Vertical = Max(velocity.Vertical, Min(_body.velocity.y, 0));
                inVars.Position = realPos.ToLocomotion();
                inVars.Velocity = realVelocity;
            }
            _body.transform.rotation = LocomotionToWorldOrientationAndToUnity(inVars.Orientation);
            if (LocomotionProfiler.EnableProfile) LocomotionProfiler.EndSample();

            if (Logger.IsTraceEnabled)
            {
                var bodyForward = _body.transform.forward;
                var forward = new Vector2(bodyForward.x, bodyForward.z).normalized;
                Logger.IfTrace()?.Message($"Commit | Frame:{Time.frameCount} Forward:({forward.x:F4}, {forward.y:F4})").Write();
            }
            return inVars;
        }
        
        public interface ISettings
        {
            float MaxWalkingSlopeAngle { get; } // градусы
        }
    }
}