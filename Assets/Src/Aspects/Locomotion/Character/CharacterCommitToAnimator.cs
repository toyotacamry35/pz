using System.Collections.Generic;
using System.Linq;
using ColonyHelpers;
using ColonyShared.SharedCode.Aspects.Locomotion;
using UnityEngine;
using static Src.Locomotion.DebugTag;
using static UnityEngine.Mathf;
using static Src.Locomotion.LocomotionDebug;
using SVector2 = SharedCode.Utils.Vector2;
using SMath = Assets.ColonyShared.SharedCode.Utils.SharedHelpers;

namespace Src.Locomotion.Unity
{
    public class CharacterCommitToAnimator : ILocomotionPipelineCommitNode
    {
        private static readonly int MovementSpeed           = Animator.StringToHash(nameof(MovementSpeed));
        private static readonly int LngMovementSpeed        = Animator.StringToHash(nameof(LngMovementSpeed));
        private static readonly int LatMovementSpeed        = Animator.StringToHash(nameof(LatMovementSpeed));
        private static readonly int LngMovementDirection    = Animator.StringToHash(nameof(LngMovementDirection));
        private static readonly int LatMovementDirection    = Animator.StringToHash(nameof(LatMovementDirection));
        private static readonly int MovementExpression      = Animator.StringToHash(nameof(MovementExpression));
        private static readonly KeyValuePair<LocomotionFlags,int>[] Flags = new []
        {
            LocomotionFlags.Moving,
            LocomotionFlags.Falling,
            LocomotionFlags.Jumping,
            LocomotionFlags.Landing,
            LocomotionFlags.Airborne,
            LocomotionFlags.Slipping,
            LocomotionFlags.CheatMode,
            
        } .Select(x => new KeyValuePair<LocomotionFlags,int>(x, Animator.StringToHash(x.ToString()))).ToArray();

        private readonly Animator _animator;
        private readonly ISettings _settings;
        private readonly AverageVector2 _movementDirection; 
        private readonly AverageVector2 _movementSpeedHorizontal; 
        private readonly AverageValue _movementSpeedVertical;
        private readonly AverageValue _movementExpression;
        private float _movementSpeed = float.MaxValue / 2;
        private float _movementSpeedX = float.MaxValue / 2;
        private float _movementSpeedY = float.MaxValue / 2;
        private float _movementDirectionX = float.MaxValue / 2;
        private float _movementDirectionY = float.MaxValue / 2;
        private float _movementExpressionValue = float.MaxValue / 2;
        private float _orientation = float.MaxValue / 2;
        private SVector2 _forward;
        private LocomotionFlags _flags;
        
        public CharacterCommitToAnimator(Animator animator, ISettings settings)
        {
            _animator = animator;
            _settings = settings;
            _movementDirection       = new AverageVector2(_settings.MovementDirectionSmoothness);
            _movementSpeedHorizontal = new AverageVector2(_settings.MovementSpeedSmoothness);
            _movementSpeedVertical = new AverageValue(_settings.MovementSpeedSmoothness);
            _movementExpression = new AverageValue(_settings.MovementDirectionSmoothness);
        }

        bool ILocomotionPipelineCommitNode.IsReady => true;

        //#Important!: @param `inVars`: 'ref' === 'in' here - i.e. "const-ref" (only to avoid struct copying)
        // So DO NOT change `inVars`!
        void ILocomotionPipelineCommitNode.Commit(ref LocomotionVariables inVars, float dt)
        {
            if (!_animator) return;
            if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## Loco Commit: j)CharCommitToAnimator");

            if (Abs(_orientation - inVars.Orientation) > 0.1 * Deg2Rad)
                _forward = new SVector2(Cos(inVars.Orientation), Sin(inVars.Orientation));

            {
                var horizontalVelocity = LocomotionHelpers.InverseTransformVector(inVars.Velocity.Horizontal, _forward).Threshold(_settings.MotionThreshold);
                var horizontalDirection = horizontalVelocity.normalized;
                _movementSpeedHorizontal.Update(horizontalVelocity, dt);
                _movementSpeedVertical.Update(inVars.Velocity.Vertical, dt);
                _movementDirection.Update(horizontalDirection, dt);
                _movementExpression.Update((inVars.Flags & LocomotionFlags.Sprint) != 0 ? 1 : 0, dt);
            }
            
            var movementSpeed = new Vector3(_movementSpeedHorizontal.Value.x, _movementSpeedHorizontal.Value.y, _movementSpeedVertical.Value).magnitude;
            if (SMath.Abs(_movementSpeed - movementSpeed) > 0.001f)
                _animator.SetFloat(MovementSpeed, _movementSpeed = movementSpeed);
            
            var horizontalMovementSpeed = _movementSpeedHorizontal.Value.magnitude;
            var factor = horizontalMovementSpeed > 0.01f ? movementSpeed / horizontalMovementSpeed : 1;
            
            var movementSpeedX = _movementSpeedHorizontal.Value.x * factor;
            if (SMath.Abs(_movementSpeedX - movementSpeedX) > 0.001f)
                _animator.SetFloat(LngMovementSpeed, _movementSpeedX = movementSpeedX);
            
            var movementSpeedY = -_movementSpeedHorizontal.Value.y * factor;
            if (SMath.Abs(_movementSpeedY - movementSpeedY) > 0.001f)
                _animator.SetFloat(LatMovementSpeed, _movementSpeedY = movementSpeedY);
            
            var movementDirectionX = _movementDirection.Value.x;
            if (SMath.Abs(_movementDirectionX - movementDirectionX) > 0.001f)
                _animator.SetFloat(LngMovementDirection, _movementDirectionX = movementDirectionX);
            
            var movementDirectionY = -_movementDirection.Value.y;
            if (SMath.Abs(_movementDirectionY - movementDirectionY) > 0.001f)
                _animator.SetFloat(LatMovementDirection, _movementDirectionY = movementDirectionY);
            
            var movementExpression = _movementExpression.Value;
            if (SMath.Abs(_movementExpressionValue - movementExpression) > 0.001f)
                _animator.SetFloat(MovementExpression, _movementExpressionValue = movementExpression);

            var changedFlags = inVars.Flags ^ _flags; 
            if (changedFlags != 0)
                for (int i = 0;  i < Flags.Length;  ++i)
                    if ((changedFlags & Flags[i].Key) != 0)
                        _animator.SetBool(Flags[i].Value, (Flags[i].Key & inVars.Flags) != 0);
            _flags = inVars.Flags;

            DebugAgent.Set(DebugTag.AnimationState, _animator.GetCurrentAnimatorStateInfo(0).shortNameHash);
            DebugAgent.Set(NextAnimationState, _animator.GetNextAnimatorStateInfo(0).shortNameHash);
            
            LocomotionProfiler.EndSample();
        }
        
        public interface ISettings
        {
            float MotionThreshold { get; }

            float MovementDirectionSmoothness { get; }

            float MovementSpeedSmoothness { get; }
        }
    }
}
