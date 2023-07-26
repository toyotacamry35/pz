using ColonyShared.SharedCode.Aspects.Locomotion;
using Mathf = Assets.ColonyShared.SharedCode.Utils.SharedHelpers;
using static Src.Locomotion.LocomotionGroundPhysics;
using static Src.Locomotion.CommonInputs;

namespace Src.Locomotion.States
{
    internal class CharacterStateSlipping : StateBase<CharacterStateMachineContext>, ILocomotionState<CharacterStateMachineContext>
    {
        public override void Execute(CharacterStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(LocomotionFlags.Moving | LocomotionFlags.Slipping)
                .ApplyAccel(
                    CalcSlippingAccel(
                        inputAxes: ctx.Input[Move],
                        guide: ctx.Input[Guide],
                        forward: ctx.Environment.SlopeDirection,
                        accel: ctx.Stats.SlippingAccel,
                        speedFn: ctx.Stats.SlippingSpeed,
                        slopeFactorFn: ctx.Environment.SlopeFactorAlongDirection))
                .ApplyAccel(
                    CalcSlipAccel(
                        slopeDirection: ctx.Environment.SlopeDirection,
                        slopeFactor: ctx.Environment.SlopeFactor(),
                        slipAccelFn: ctx.Stats.SlipAccel,
                        slipSpeedFn: ctx.Stats.SlipSpeed))
                .ApplyFriction(
                    friction: ctx.Stats.SlippingDecel)
                .ApplyGravity(
                    gravity: ctx.Environment.Gravity)
//                .ApplyGravityOnGround(
//                    gravity: ctx.Environment.Gravity,
//                    hasContact: ctx.Environment.GroundContact)
                .ApplyOrientation(
                    direction: ctx.Body_Deprecated.Velocity.Horizontal.normalized,
                    maxAngularVelocity: ctx.Stats.SlipYawSpeed)
                ;
        }
    }
}