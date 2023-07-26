using System;
using Assets.ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using Src.Locomotion;
using UnityEngine;

namespace Assets.Src.NetworkedMovement.MoveActions
{
    class JumpToTargetPositionAction : MoveAction
    {
        private readonly IMoveActionPosition _self;
        private readonly IMoveActionPosition _target;
        private readonly MobLocomotionReactions _reactions;
        private Vector3 _targetPoint;
        private bool _jumpStarted;
        private bool _jumpFinished;
        private Vector3 _unityWorldGuideNorm;

#if UNITY_EDITOR
        private readonly Color _debugColor = Color.red;
#endif

        public JumpToTargetPositionAction(
            [NotNull] IMoveActionPosition self, 
            [NotNull] IMoveActionPosition target,
            MobLocomotionReactions reactions, 
            Guid entityId) 
            : base(entityId)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (target == null) throw new ArgumentNullException(nameof(target));
            _self = self;
            _target = target;
            _reactions = reactions;

#if UNITY_EDITOR
            if (DbgMode)
                _target?.DebugDrawTargetPosition(_debugColor);
#endif
        }

        public override bool Init()
        {
            _reactions.Jumped += OnJumpStarted;
            _reactions.Landed += OnLanded;
            return true;
        }

        public override void Dispose()
        {
            _reactions.Jumped -= OnJumpStarted;
            _reactions.Landed -= OnLanded;
        }

        public override MoveActionResult Tick(Pawn.SimulationLevel simulationLevel)
        {
            _unityWorldGuideNorm = Vector3.zero;

            // While didn't start jump, update target pos.
            if (!_jumpStarted && _target != null)
                _targetPoint = _target.Position;
            var toTarget = _targetPoint - _self.Position;
            toTarget.y = 0;

            if (toTarget.sqrMagnitude <= Mathf.Epsilon)
                toTarget = UnityEngine.Random.onUnitSphere.SetY(0);

            _unityWorldGuideNorm = toTarget.normalized;

#if UNITY_EDITOR
            if (DbgMode)
                _target?.DebugDrawTargetPosition(_debugColor);
#endif

            var result = (_jumpFinished)
                ? MoveActionResult.Finished
                : MoveActionResult.Running;
            //if(result == MoveActionResult.Finished)
            //    if (DbgLog.Enabled) DbgLog.Log("JTT Finished!!");
            return result;
        }

        private void OnJumpStarted()
        {
            _jumpStarted = true;
        }

        private void OnLanded()
        {
            _jumpFinished = true;
        }

        public override void GetLocomotionInput(InputState<MobInputs> input)
        {
            var guide = LocomotionHelpers.WorldToLocomotionVector(_unityWorldGuideNorm);
            var point = LocomotionHelpers.WorldToLocomotionVector(_targetPoint);

            input[CommonInputs.Guide] = guide.Horizontal;
            input[MobInputs.JumpToTarget]  = !_jumpStarted; 
            input[MobInputs.TargetPointXY] = point.Horizontal;
            input[MobInputs.TargetPointV]  = point.Vertical;
        }
    
    }

}
