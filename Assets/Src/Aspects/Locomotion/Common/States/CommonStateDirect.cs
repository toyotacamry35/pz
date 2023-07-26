using ColonyShared.SharedCode.Aspects.Locomotion;
using static Src.Locomotion.CommonInputs;

namespace Src.Locomotion.States
{
    internal class CommonStateDirect : StateBase<CommonStateMachineContext>
    {
        public override void Execute(CommonStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(LocomotionFlags.Direct)
                .SetHorizontalVelocity(
                    ctx.Input[DirectVelocity])
                .ApplyOrientation(
                    ctx.Input[DirectOrientation])
                .SnapToGround(
                    distanceToGround: ctx.Environment.DistanceToGround,
                    maxDistance: ctx.Stats.JumpOffDistance,
                    slopeFactor: ctx.Environment.SlopeFactorAlongDirection(ctx.Body_Deprecated.Velocity.Horizontal))
                .ApplyGravity(
                    gravity: ctx.Environment.Gravity)
                ;
        }

        public override void OnExit(CommonStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.SetVelocity(LocomotionVector.Zero);
        }
    }
}