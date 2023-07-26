using System;
using Assets.ColonyShared.SharedCode.Utils;
using Src.Locomotion;
using UnityEngine;

namespace Assets.Src.NetworkedMovement.MoveActions
{
    class LookAtAction : MoveAction
    {
        private readonly IMoveActionPosition _self;
        private readonly IMoveActionPosition _target;
        private Vector3 _guideNorm;

#if UNITY_EDITOR
        private readonly Color _debugColor = Color.magenta;
#endif

        public LookAtAction(IMoveActionPosition self, IMoveActionPosition target, Guid entityId) : base(entityId)
        {
            _self = self;
            _target = target;

#if UNITY_EDITOR
            if (DbgMode)
                _target?.DebugDrawTargetPosition(_debugColor);
#endif
        }

        public override bool Init()
        {
            return true;
        }

        public override MoveActionResult Tick(Pawn.SimulationLevel simulationLevel)
        {
            _guideNorm = Vector3.zero;
            if (_target == null)
                return MoveActionResult.Running;

#if UNITY_EDITOR
            if (DbgMode)
                _target?.DebugDrawTargetPosition(_debugColor);
#endif

            var vecToTarget = _target.Position - _self.Position;
            vecToTarget.y = 0;
            if (vecToTarget.sqrMagnitude < Mathf.Epsilon || _target.IsSameObject(_self))
                return MoveActionResult.Running;

            _guideNorm = vecToTarget.normalized;

            return MoveActionResult.Running;
        }

        public override void GetLocomotionInput(InputState<MobInputs> input)
        {
            var guide = LocomotionHelpers.WorldToLocomotionVector(_guideNorm);
            input[CommonInputs.Guide] = guide.Horizontal;
        }
    
    //      internal override float DesiredSendIntervalFromServerBest => 0.07f;
    //     protected override float DesiredSendIntervalFromServerWorst => 0.1f;
    }

}
