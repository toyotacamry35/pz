using ColonyShared.SharedCode.Aspects.Locomotion;
using SharedCode.Utils;
using static Src.Locomotion.LocomotionGroundPhysics;
using static Src.Locomotion.CommonInputs;

namespace Src.Locomotion.States
{
    internal class CharacterStateSprint : StateBase<CharacterStateMachineContext>, ILocomotionState<CharacterStateMachineContext>
    {
        private Vector2 _lastAccelDir;

        public override void Execute(CharacterStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(LocomotionFlags.Moving | LocomotionFlags.Sprint)
                .ApplyAccel(
                    CalcWalkAccel(
                        input: ctx.Input[Move],
                        guide: ctx.Input[Guide],
                        forward: ctx.Body_Deprecated.Forward,
                        accel: ctx.Stats.SprintAccel,
                        accelFn: ctx.Stats.AccelBySlope,
                        speedFn: ctx.Stats.SprintSpeed,
                        slopeFactorFn: ctx.Environment.SlopeFactorAlongDirection,
                        minSpeed: ctx.Stats.StandingSpeedThreshold - 0.001f,
                        elapsedTime: ctx.StateElapsedTime,
                        lastAccelDir: ref _lastAccelDir))
                .ApplyAccel(
                    CalcSlipAccel(
                        slopeDirection: ctx.Environment.SlopeDirection,
                        slopeFactor: ctx.Environment.SlopeFactor(),
                        slipAccelFn: ctx.Stats.SlipAccel,
                        slipSpeedFn: ctx.Stats.SlipSpeed))
//                .ApplyStepUp(
//                    CalculateStepUp(
//                        input: ctx.Input[Move],
//                        guide: ctx.Input[Guide],
//                        velocity: ctx.Body_Deprecated.Velocity,
//                        slopeFn: ctx.Environment.SlopeFactorAlongDirection,
//                        obstacleFn: ctx.Environment.DetectObstacle,
//                        lookAheadTime: ctx.Stats.StairsLookahead,
//                        maxStepUp: ctx.Stats.StairsMaxHeight,
//                        deltaTime: ctx.Clock.DeltaTime
//                    ), 
//                    limit: ctx.Stats.StairsStepUpLimit)
                .ApplyFriction(
                    friction: ctx.Stats.Decel)
                .ApplyOrientation(
                    direction: ctx.Input[Guide],
                    maxAngularVelocity: ctx.Stats.SprintYawSpeed)
                .ApplyGravity(
                    gravity: ctx.Environment.Gravity)
//                .ApplyGravityOnGround(
//                    gravity: ctx.Environment.Gravity,
//                    hasContact: ctx.Environment.GroundContact)
                ;
        }
    }
}