using ColonyShared.SharedCode.Aspects.Locomotion;
using SharedCode.Utils;
using static Src.Locomotion.LocomotionAirbornePhysics;
using static Src.Locomotion.CommonInputs;

namespace Src.Locomotion.States
{
    /// <summary>
    /// Прыжок с разбега
    /// </summary>
    internal class CharacterStateJumpingFromRun : StateBase<CommonStateMachineContext>, ILocomotionState<CommonStateMachineContext>
    {
        public override void OnEnter(CommonStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline
                .SetVerticalVelocity(0)
                .ApplyImpulse(
                    CalcJumpImpulse(
                        input: ctx.Input[Move].Step(ctx.Constants.InputMoveThreshold),
                        //input: ctx.Input.AxesMax(InputAxes.Move, 0.1f).normalized,
                        guide: ctx.Input[Guide],
                        forward: ctx.Body_Deprecated.Forward,
                        velocity: ctx.Body_Deprecated.Velocity.Horizontal,
                        verticalImpulseFn: ctx.Stats.JumpVerticalImpulse,
                        horizontalImpulseFn: ctx.Stats.JumpHorizontalImpulse));
        }

        public override void Execute(CommonStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(LocomotionFlags.Jumping | LocomotionFlags.Moving | LocomotionFlags.Airborne)
                .ApplyOrientation(
                    direction: ctx.Input[Guide],
                    maxAngularVelocity: ctx.Stats.JumpYawSpeed)
                .ApplyAccel(
                    CalcAirControlAccel(
                        input: ctx.Input[Move],
                        guide: ctx.Input[Guide],
                        forward: ctx.Body_Deprecated.Forward,
                        accelFn: ctx.Stats.AirControlAccel,
                        speedFn: ctx.Stats.AirControlSpeed,
                        airborneTime: ctx.History.AirborneTime))
                .ApplyGravity(
                    ctx.Environment.Gravity)
                ;
        }
    }
}