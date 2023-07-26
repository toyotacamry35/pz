using ColonyShared.SharedCode.Aspects.Locomotion;
using static Src.Locomotion.LocomotionGroundPhysics;
using static Src.Locomotion.CommonInputs;

namespace Src.Locomotion.States
{
    internal class CharacterStateJumpingFromSpotDelay : StateBase<CommonStateMachineContext>, ILocomotionState<CommonStateMachineContext>
    {
        public override void Execute(CommonStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(LocomotionFlags.Jumping)
//                .ApplyGravityOnGround(
//                    gravity: ctx.Environment.Gravity,
//                    hasContact: ctx.Environment.GroundContact)
                .ApplyGravity(
                    gravity: ctx.Environment.Gravity)
                .ApplyOrientation(
                    direction: ctx.Input[Guide],
                    maxAngularVelocity: ctx.Stats.JumpYawSpeed)
                ;
        }
    }
}