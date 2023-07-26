using ColonyShared.SharedCode.Aspects.Locomotion;
using static Src.Locomotion.CommonInputs;

namespace Src.Locomotion.States
{
    internal class MobStateTurning : StateBase<MobStateMachineContext>
    {
        public override void Execute(MobStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(LocomotionFlags.Turning)
                .ApplyOrientation(
                    direction: ctx.Input[Guide],
                    maxAngularVelocity: ctx.Stats.StandingYawSpeed)
                .ApplyFriction(
                    friction: ctx.Stats.Decel)
                .ApplyGravityOnGround(
                    gravity: ctx.Environment.Gravity,
                    hasContact: ctx.Environment.HasGroundContact)
                ;
        }
    }
}