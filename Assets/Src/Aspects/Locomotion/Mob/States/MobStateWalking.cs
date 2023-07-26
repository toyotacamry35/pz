using ColonyShared.SharedCode.Aspects.Locomotion;
using Mathf = Assets.ColonyShared.SharedCode.Utils.SharedHelpers;
using static Src.Locomotion.LocomotionGroundPhysics;
using static Src.Locomotion.CommonInputs;
using static Src.Locomotion.MobInputs;

namespace Src.Locomotion.States
{
    internal class MobStateWalking : StateBase<MobStateMachineContext>
    {
        public override void Execute(MobStateMachineContext ctx, VariablesPipeline pipeline)
        {
            var run = ctx.Input[Run];
            var moveAxes = ctx.Input[Move];
            
            pipeline.ApplyFlags(LocomotionFlags.Moving | (run ? LocomotionFlags.Sprint : 0))
                .ApplyAccel(
                    CalcWalkAccel(
                        input: moveAxes,
                        guide: ctx.Input[Guide],
                        forward: ctx.Body_Deprecated.Forward,
                        accel: run ? ctx.Stats.RunningAccel : ctx.Stats.WalkingAccel,
                        speedFn: run ? ctx.Stats.RunningSpeed : ctx.Stats.WalkingSpeed,
                        slopeFactorFn: ctx.Environment.SlopeFactorAlongDirection,
                        speedFactor: ctx.Input[SpeedFactor]))
                .ApplyFriction(
                    friction: ctx.Stats.Decel)
                .ApplyOrientation(
                    direction: ctx.Input[Guide],
                    maxAngularVelocity: run ? ctx.Stats.RunningYawSpeed : ctx.Stats.WalkingYawSpeed)
                .ApplyGravityOnGround(
                    gravity: ctx.Environment.Gravity,
                    hasContact: ctx.Environment.HasGroundContact)
                ;
        }
    }
}