using ColonyShared.SharedCode.Aspects.Locomotion;
using static Src.Locomotion.CommonInputs;

namespace Src.Locomotion.States
{
    internal class MobStateJumpingFromSpotDelay : StateBase<CommonStateMachineContext>
    {
        public override void Execute(CommonStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(LocomotionFlags.Jumping)
                .ApplyGravityOnGround(
                    gravity: ctx.Environment.Gravity,
                    hasContact: ctx.Environment.HasGroundContact)
                .ApplyOrientation(
                    direction: ctx.Input[Guide],
                    maxAngularVelocity: ctx.Stats.StandingYawSpeed)
                ;
        }
    }
}