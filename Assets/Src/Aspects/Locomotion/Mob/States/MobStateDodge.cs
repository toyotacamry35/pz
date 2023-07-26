using ColonyShared.SharedCode.Aspects.Locomotion;
using SharedCode.Utils;
using Mathf = Assets.ColonyShared.SharedCode.Utils.SharedHelpers;
using static Src.Locomotion.CommonInputs;
using static Src.Locomotion.MobInputs;

namespace Src.Locomotion.States
{
    internal class MobStateDodge : StateBase<MobStateMachineContext>
    {
        private Vector2 _inputOnStart;

        public override void OnEnter(MobStateMachineContext ctx, VariablesPipeline pipeline)
        {
            _inputOnStart = ctx.Input[Move];
            if(_inputOnStart.Shorter(ctx.Constants.InputMoveThreshold))
                _inputOnStart = new Vector2(-1, 0);
        }
        
        public override void Execute(MobStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(LocomotionFlags.Moving | LocomotionFlags.Dodge)
                .SetHorizontalVelocity(
                    LocomotionHelpers.TransformMovementInputAxes(_inputOnStart, ctx.Input[Guide]).normalized *
                    ctx.Stats.DodgeVelocity(ctx.StateElapsedTime) * ctx.Input[SpeedFactor])
                .SnapToGround(
                    distanceToGround: ctx.Environment.DistanceToGround,
                    maxDistance: ctx.Stats.JumpOffDistance,
                    slopeFactor: ctx.Environment.SlopeFactorAlongDirection(ctx.Body_Deprecated.Velocity.Horizontal))
                ;
        }
        
        public override void OnExit(MobStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.SetHorizontalVelocity(Vector2.zero);
        }
    }
}