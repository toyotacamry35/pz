using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Locomotion.Utils;
using ColonyShared.SharedCode.Aspects.Locomotion;
using ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using NLog;
using SharedCode.Utils;
using UnityEngine;

using static UnityEngine.Mathf;
using static Src.Locomotion.LocomotionDebug;

using SVector2 = SharedCode.Utils.Vector2;


namespace Src.Locomotion.Unity
{
    public class MobCommitToAnimator : ILocomotionPipelineCommitNode
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(MobCommitToAnimator));

        const bool EnableCurveLogger = false; //true;
        private readonly ICurveLoggerProvider _curveLogProv;

        private static readonly int MovementSpeed           = Animator.StringToHash(nameof(MovementSpeed));
        private static readonly int LngMovementSpeed        = Animator.StringToHash(nameof(LngMovementSpeed));
        private static readonly int LatMovementSpeed        = Animator.StringToHash(nameof(LatMovementSpeed));
        private static readonly int HorizontalMovementSpeed = Animator.StringToHash(nameof(HorizontalMovementSpeed));
        private static readonly int VerticalMovementSpeed   = Animator.StringToHash(nameof(VerticalMovementSpeed));
        private static readonly int LngMovementDirection    = Animator.StringToHash(nameof(LngMovementDirection));
        private static readonly int LatMovementDirection    = Animator.StringToHash(nameof(LatMovementDirection));
        private static readonly int AngularVelocity         = Animator.StringToHash(nameof(AngularVelocity));
        private static readonly int MovementAngle           = Animator.StringToHash(nameof(MovementAngle));
        private static readonly int MovementExpression      = Animator.StringToHash(nameof(MovementExpression));
        private static readonly int MovementTwist           = Animator.StringToHash(nameof(MovementTwist));
        private static readonly int TurningDirection        = Animator.StringToHash(nameof(TurningDirection));

        private static readonly KeyValuePair<LocomotionFlags, int>[] Flags = new[]
        {
            LocomotionFlags.Moving,
            LocomotionFlags.Sprint,
            LocomotionFlags.Falling,
            LocomotionFlags.Jumping,
            LocomotionFlags.Landing,
            LocomotionFlags.Airborne,
            //#todo: when we 'll be ripe to make "dodge" separate loco. state, uncomment it & switch to state by additional input. 
            //.. (Now it's just strafe move action with dodge animation)
            // LocomotionFlags.Dodge, 
            LocomotionFlags.Turning,
        }.Select(x => new KeyValuePair<LocomotionFlags, int>(x, Animator.StringToHash(x.ToString()))).ToArray();

        private readonly Animator _animator;
        private readonly ISettings _settings;
        private readonly AverageVector2 _movementDirection;
        private readonly AverageVector2 _movementSpeedHorizontal;
        private readonly AverageValue _movementSpeedVertical;
        private readonly AverageValue _movementExpression;
        private readonly AverageValue _angularVelocity;
        private readonly AverageValue _movementTwist;
        private readonly Transform _transform;
        private readonly Guid _entityId;
        private HashSet<int> _existingParameters = new HashSet<int>();
        public MobCommitToAnimator(Animator animator, ISettings settings, Transform transform, Guid entityId, [CanBeNull] ICurveLoggerProvider curveLogProv = null)
        {
            _animator = animator;
            _settings = settings;
            _movementDirection = new AverageVector2(_settings.MovementDirectionSmoothness);
            _movementSpeedHorizontal = new AverageVector2(_settings.MovementSpeedSmoothness);
            _movementSpeedVertical = new AverageValue(_settings.MovementSpeedSmoothness);
            _movementExpression = new AverageValue(_settings.MovementDirectionSmoothness);
            _angularVelocity = new AverageValue(_settings.AngularVelocitySmoothness);
            _movementTwist = new AverageValue(_settings.AngularVelocitySmoothness);
            _transform = transform;
            _entityId = entityId;
            _curveLogProv = EnableCurveLogger ? curveLogProv : null; 
            foreach (AnimatorControllerParameter param in _animator.parameters)
                _existingParameters.Add(param.nameHash);
        }

        bool ILocomotionPipelineCommitNode.IsReady => true;

        private LocomotionVector _lastPos = LocomotionVector.Invalid;
        private LocomotionTimestamp _lastTimeStamp = LocomotionTimestamp.None;
        private float _lastRotationRad = Single.NaN;

        //#Important!: @param `inVars`: 'ref' === 'in' here - i.e. "const-ref" (only to avoid struct copying)
        // So DO NOT change `inVars`!
        void ILocomotionPipelineCommitNode.Commit(ref LocomotionVariables inVars, float dt)
        {
            if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## Loco Commit: l.1)MobCommitToAnimator Cos,Atan");

            var lastPos = _lastPos;
            var lastRotation = _lastRotationRad;
            var lastTimeStamp = _lastTimeStamp;
            _lastPos = inVars.Position;
            _lastRotationRad = inVars.Orientation;
            _lastTimeStamp = inVars.Timestamp;

            if ( lastPos == LocomotionVector.Invalid
               || _lastPos == LocomotionVector.Invalid
               || !lastTimeStamp.Valid
               || !_lastTimeStamp.Valid
               || float.IsNaN(lastRotation)
               || float.IsNaN(_lastRotationRad)
               || inVars.Flags.Any(LocomotionFlags.Teleport) )
            { 
                //Dbg:
                if ((_lastPos == LocomotionVector.Invalid && lastPos != LocomotionVector.Invalid)
                    || (!_lastTimeStamp.Valid && lastTimeStamp.Valid)
                    || (float.IsNaN(_lastRotationRad) && !float.IsNaN(lastRotation)))
                    Logger.Error($"Invalid vars received after valid: " +
                                 $"((_lastPos == LocomotionVector.Invalid({_lastPos == LocomotionVector.Invalid} // {_lastPos}) " +
                                 $"&& lastPos != LocomotionVector.Invalid({lastPos == LocomotionVector.Invalid} // {lastPos})" +
                                 $"|| (!_lastTimeStamp.Valid({_lastTimeStamp.Valid} // {_lastTimeStamp}) " +
                                 $"&& lastTimeStamp.Valid({lastTimeStamp.Valid} // {lastTimeStamp})" +
                                 $"|| (IsNaN(_lastRotationRad)({float.IsNaN(_lastRotationRad)} // {_lastRotationRad}) " +
                                 $"&& !IsNaN(lastRotation)({float.IsNaN(lastRotation)} // {lastRotation})))");

                _curveLogProv?.CurveLogger?.IfActive?.AddData("8)Cl_MobAnim-r.Invalid", SyncTime.Now, 0);

                return; // just skip 1st 2 frames.
            }

            var forward = new SVector2(Cos(inVars.Orientation), Sin(inVars.Orientation));
            //var forward = _transform.forward.normalized;

            var deltaTimestamps = (inVars.Timestamp - lastTimeStamp).Seconds;
            if (deltaTimestamps < 0)
                Logger./*Warn*/Error($"lastTimeStamp > inVars.Timestamp (delta seconds:{dt})"); // It's excluded by logic of LocoSyncedClock (don't goes back) involved by LocoSimpleExtrpolapolNode

            //var speed = new LocomotionVector(LocomotionHelpers.InverseTransformVector(inVars.Velocity.Horizontal, forward), inVars.Velocity.Vertical);
            //// var velo = (deltaTimestamps > 0)
            ////     ? (inVars.Position - lastPos) / deltaTimestamps
            ////     : LocomotionVector.Zero;
            LocomotionVector velo = LocomotionVector.Zero;
            float angularVelo = 0f;
            if (deltaTimestamps > 0)
            {
                velo = (inVars.Position - lastPos) / deltaTimestamps;
                angularVelo = SharedHelpers.DeltaAngleRad(lastRotation, inVars.Orientation) / deltaTimestamps;
            }
            else //==0
            {
                // Do nothing - just leave them default;
            }

            var velocityHorizontal = LocomotionHelpers.InverseTransformVector(velo.Horizontal, forward).Threshold(_settings.MotionThreshold);
            var directionHorizontal = velocityHorizontal.normalized;
            
            _movementSpeedHorizontal.Update(velocityHorizontal, dt);
            _movementSpeedVertical.Update(velo.Vertical, dt);
            _movementDirection.Update(directionHorizontal, dt);
            _movementExpression.Update((inVars.Flags & LocomotionFlags.Sprint) != 0 ? 1 : 0, dt);
            float movementAngle = DeltaAngle(0, Atan2(_movementDirection.Value.y, _movementDirection.Value.x) * Rad2Deg);

            LocomotionProfiler.EndSample();

            if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## Loco Commit: l.2)MobCommitToAnimator _animator.SetXxx(..)");

            // Logger.IfInfo()?.Message($"{_entityId} LngMovementSpeed={_movementSpeedHorizontal.Value.x} speed.Horizontal.x={speed.Horizontal.x} inVars.Position={inVars.Position} lastPos={lastPos} deltaTimestamps={deltaTimestamps} velo={velo} forward={forward} inVars.Orientation={inVars.Orientation} inVars.Timestamp={inVars.Timestamp}").Write();
            
            //_animator.applyRootMotion = false;
            if (_existingParameters.Contains(MovementSpeed))           _animator.SetFloat(MovementSpeed, new LocomotionVector(_movementSpeedHorizontal.Value, _movementSpeedVertical).Magnitude);
            if (_existingParameters.Contains(LngMovementSpeed))        _animator.SetFloat(LngMovementSpeed, _movementSpeedHorizontal.Value.x);
            if (_existingParameters.Contains(LatMovementSpeed))        _animator.SetFloat(LatMovementSpeed, -_movementSpeedHorizontal.Value.y);
            if (_existingParameters.Contains(HorizontalMovementSpeed)) _animator.SetFloat(HorizontalMovementSpeed, _movementSpeedHorizontal.Value.magnitude);
            if (_existingParameters.Contains(VerticalMovementSpeed))   _animator.SetFloat(VerticalMovementSpeed, _movementSpeedVertical.Value);
            if (_existingParameters.Contains(LngMovementDirection))    _animator.SetFloat(LngMovementDirection, _movementDirection.Value.x);
            if (_existingParameters.Contains(LatMovementDirection))    _animator.SetFloat(LatMovementDirection, -_movementDirection.Value.y);
            if (_existingParameters.Contains(MovementAngle))           _animator.SetFloat(MovementAngle, movementAngle);
            if (_existingParameters.Contains(MovementExpression))      _animator.SetFloat(MovementExpression, _movementExpression.Value);
            if (_existingParameters.Contains(AngularVelocity))         _animator.SetFloat(AngularVelocity, _angularVelocity.Update(/*inVars.AngularVelocity*/angularVelo, dt).Value);
            if (_existingParameters.Contains(MovementTwist))           _animator.SetFloat(MovementTwist, _movementTwist.Update(_settings.AngularVelocityToTwist(/*inVars.AngularVelocity*/angularVelo), dt).Value);
            if (_existingParameters.Contains(TurningDirection))        _animator.SetFloat(TurningDirection, Sign(/*inVars.AngularVelocity*/angularVelo));
            for (int i = 0; i < Flags.Length; ++i)
            {
                if (_existingParameters.Contains(Flags[i].Value))
                {
                    bool newVal = (Flags[i].Key & inVars.Flags) != 0;
                    if (Flags[i].Key != LocomotionFlags.Turning || newVal == false)
                        _animator.SetBool(Flags[i].Value, newVal);
                    else
                    { // Особая обработка флага "Turning" - это временное костыльное решение (до перехода на нов. высокоур-ую систему синхрнонизации мобов) - выключаем флаг, когда ang.Velo == 0
                      //.. Это нужно, т.к. флаг держит аниматор в состоянии поворота.
                        _animator.SetBool(Flags[i].Value, !Approximately(_angularVelocity.Value, 0f));
                        ///#Dbg:
                        if (Approximately(_angularVelocity.Value, 0f))
                        {
                            //DbgLog.Log("Turning := OFF");
                            _curveLogProv?.CurveLogger?.IfActive?.AddData("8.b)Cl_MobAnim-r.TurningOFF", SyncTime.Now, -1);
                        }
                    }
                }
            }

            {//#Dbg:
                DebugAgent.Set(DebugTag.AnimationState, _animator.GetCurrentAnimatorStateInfo(0).shortNameHash);
                DebugAgent.Set(DebugTag.NextAnimationState, _animator.GetNextAnimatorStateInfo(0).shortNameHash);

                _curveLogProv?.CurveLogger?.IfActive?.AddData("8)Cl_MobAnim-r.dTStamps*10",          SyncTime.Now, deltaTimestamps*10);
                _curveLogProv?.CurveLogger?.IfActive?.AddData("8)Cl_MobAnim-r.dt*10",                SyncTime.Now, dt*10);
                if(false) _curveLogProv?.CurveLogger?.IfActive?.AddData("8.a)Cl_MobAnim-r.velo",               SyncTime.Now, velo);
                if(false) _curveLogProv?.CurveLogger?.IfActive?.AddData("8.a)Cl_MobAnim-r.speed",              SyncTime.Now, velocityHorizontal);
                if(false) _curveLogProv?.CurveLogger?.IfActive?.AddData("8.a)Cl_MobAnim-r._mvtSpeedHor.Val",   SyncTime.Now, _movementSpeedHorizontal.Value);
                if(false) _curveLogProv?.CurveLogger?.IfActive?.AddData("8.a)Cl_MobAnim-r._mvtSpeedHor.Count", SyncTime.Now, _movementSpeedHorizontal.Dbg_Count);

                _curveLogProv?.CurveLogger?.IfActive?.AddData("8.b)Cl_MobAnim-r.angVelo",            SyncTime.Now, angularVelo);
                _curveLogProv?.CurveLogger?.IfActive?.AddData("8.b)Cl_MobAnim-r.inVars.angVelo",     SyncTime.Now, inVars.AngularVelocity);
                _curveLogProv?.CurveLogger?.IfActive?.AddData("8.b)Cl_MobAnim-r._angVeloAvg.Val",    SyncTime.Now, _angularVelocity.Value);
                _curveLogProv?.CurveLogger?.IfActive?.AddData("8.b)Cl_MobAnim-r._angVeloAvg.Count",  SyncTime.Now, _angularVelocity.Dbg_Count);
            }

            LocomotionProfiler.EndSample();
        }

        public interface ISettings
        {
            float MotionThreshold { get; }

            float MovementDirectionSmoothness { get; }

            float MovementSpeedSmoothness { get; }

            float AngularVelocitySmoothness { get; }

            float AngularVelocityToTwist(float av);
        }
    }
}
