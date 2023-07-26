using ColonyShared.SharedCode.Aspects.Locomotion;
using SharedCode.Utils;
using Mathf = Assets.ColonyShared.SharedCode.Utils.SharedHelpers;
using static Src.Locomotion.LocomotionGroundPhysics;
using static Src.Locomotion.CommonInputs;
using static Src.Locomotion.CharacterInputs;

namespace Src.Locomotion.States
{
    internal class CharacterStateWalking : StateBase<CharacterStateMachineContext>
    {
        private Vector2 _lastAccelDir;
        
        public override void Execute(CharacterStateMachineContext ctx, VariablesPipeline pipeline)
        {
            var moveAxes = ctx.Input[Move];
            var speedFn = ctx.Input[Block] || ctx.Input[Aim] ? ctx.Stats.BlockingSpeed : ctx.Stats.WalkingSpeed;
            
            pipeline.ApplyFlags(LocomotionFlags.Moving)
                .ApplyAccel(
                    CalcWalkAccel(
                        input: moveAxes,
                        guide: ctx.Input[Guide],
                        forward: ctx.Body_Deprecated.Forward,
                        accel: ctx.Stats.WalkingAccel,
                        accelFn: ctx.Stats.AccelBySlope,
                        speedFn: speedFn,
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
                .ApplyFriction(
                    friction: ctx.Stats.Decel)
                .ApplyOrientation(
                    direction: ctx.Input[Guide],
                    maxAngularVelocity: ctx.Stats.WalkingYawSpeed)
                .ApplyGravity(
                    gravity: ctx.Environment.Gravity)
                ;
        }
    }
}