using System;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using GeneratedDefsForSpells;
using Src.Locomotion;
using UnityEngine;

namespace Assets.Src.NetworkedMovement.MoveActions
{
    class StrafeAction : MoveAction
    {
        private readonly IMoveActionPosition _self;
        private readonly Vector3 _direction;
        private readonly IMoveActionPosition _target;
        private readonly float _speedFactor;
        private readonly MoveEffectDef.MoveModifier _moveModifier;
        private readonly MoveEffectDef.RotationType _rotationType;

#if UNITY_EDITOR
        private readonly Color _debugColor = Color.yellow;
#endif

        public StrafeAction([NotNull] IMoveActionPosition self, [NotNull] IMoveActionPosition target, Vector3 direction, MoveEffectDef.RotationType rotationType, MoveEffectDef.MoveModifier moveModifier, float speedFactor, Guid entityId) : base(entityId)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (target == null) throw new ArgumentNullException(nameof(target));
            _self = self;
            _moveModifier = moveModifier;
            _rotationType = rotationType;
            _direction = (direction != Vector3.zero) 
                ? direction.normalized
                : direction;
            _target = target;
            _speedFactor = speedFactor;

#if UNITY_EDITOR
            if (DbgMode)
                _target?.DebugDrawTargetPosition(_debugColor);
#endif
        }

        public override bool Init() => true;

        private Vector3 _unityWorldGuideNorm;
        private Vector3 _unityWorldMove;

        public override MoveActionResult Tick(Pawn.SimulationLevel simulationLevel)
        {
            _unityWorldGuideNorm = Vector3.zero;

            if (_target == null || !_target.Valid)
                return MoveActionResult.Running; // FIXME: почему не Finished или Failed?
            if (_target == _self)
                return MoveActionResult.Running; // FIXME: почему не Finished или Failed?

            //#Dbg:
            if (_self == null)
            {
                 Logger.IfError()?.Message("_self == null").Write();;
                return MoveActionResult.Running; // FIXME: почему не Finished или Failed?
            }

            switch (_rotationType)
            {
                case MoveEffectDef.RotationType.LookAtTarget:        TickDo_LookAtTarget(); break;
                case MoveEffectDef.RotationType.LookAtMoveDirection: TickDo_LookAtMoveDirection(); break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

#if UNITY_EDITOR
            if (DbgMode)
                _target?.DebugDrawTargetPosition(_debugColor);
#endif

            return MoveActionResult.Running;
        }

        private void TickDo_LookAtMoveDirection()
        {
            var vecToTarget = _target.Position - _self.Position;
            vecToTarget.y = 0;

//            if (vecToTarget.sqrMagnitude <= Mathf.Epsilon)
//                vecToTarget = _self.transform.forward;
            //#wrong:
            // _unityWorldGuideNorm = (vecToTarget.sqrMagnitude > Mathf.Epsilon)
            //     ? vecToTarget.normalized 
            //     : _pawn.transform.forward;
            // 
            // var rot = Quaternion.LookRotation(_unityWorldGuideNorm);
            // _unityWorldMove = rot * Direction * Speed;
            if (vecToTarget != Vector3.zero)
            {
                var rot = Quaternion.LookRotation(vecToTarget);
                _unityWorldGuideNorm = rot * _direction;
            }

            _unityWorldMove = Vector3.forward;
        }
        
        private void TickDo_LookAtTarget()
        {
            var vecToTarget = _target.Position - _self.Position;
            vecToTarget.y = 0;

            if (vecToTarget.sqrMagnitude > Mathf.Epsilon)
                _unityWorldGuideNorm = vecToTarget.normalized;
            // else leave it Zero
                
            _unityWorldMove = _direction;

            //if (DrawDebug && DbgCounter % 2 == 0)
            //{
            //    var p0 = _pawn.transform.position;
            //    Debug.DrawLine(p0, p0 + (worldDirection * 2), Color.green, 5f, false);
            //    Debug.DrawLine(p0, p0 + (Quaternion.AngleAxis(-90, Vector3.up) * vecToTarget.normalized * 2), Color.gray, 5f, false);
            //    Debug.DrawLine(p0, p0 + vecToTarget, Color.gray, 5f, false);
            //}
        }

        public override void GetLocomotionInput(InputState<MobInputs> input)
        {
            var guide = LocomotionHelpers.WorldToLocomotionVector(_unityWorldGuideNorm);
            var move = LocomotionHelpers.WorldToLocomotionVector(_unityWorldMove);
            
            input[CommonInputs.Guide] = guide.Horizontal;
            input[CommonInputs.Move]  = move.Horizontal;
            input[MobInputs.Run] = (_moveModifier & MoveEffectDef.MoveModifier.Run) != 0;
            input[MobInputs.SpeedFactor] = _speedFactor;
        }
    
    }

}
