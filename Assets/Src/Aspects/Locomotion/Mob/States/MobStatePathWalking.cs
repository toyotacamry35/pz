using ColonyShared.SharedCode.Aspects.Locomotion;
using JetBrains.Annotations;
using SharedCode.Utils;
using static Src.Locomotion.LocomotionGroundPhysics;
using static Src.Locomotion.CommonInputs;
using static Src.Locomotion.MobInputs;

namespace Src.Locomotion.States
{
    internal class MobStatePathWalking : StateBase<MobStateMachineContext>
    {
        private readonly ICurveLoggerProvider _curveLogProv;

        public MobStatePathWalking([CanBeNull] ICurveLoggerProvider curveLogProv)
        {
            _curveLogProv = curveLogProv;
        }

        public override void Execute(MobStateMachineContext ctx, VariablesPipeline pipeline)
        {
            var run = ctx.Input[Run];
            var moveAxes = ctx.Input[Move];
            
            pipeline.ApplyFlags(LocomotionFlags.Moving | LocomotionFlags.FollowPath | (run ? LocomotionFlags.Sprint : 0))
                .ApplyAccel(
                    CalcWalkAccel(
                        input: moveAxes,
                        guide: ctx.Input[Guide],
                        forward: ctx.Body_Deprecated.Forward,
                        accel: run ? ctx.Stats.RunningAccel : ctx.Stats.WalkingAccel,
                        speedFn: run ? ctx.Stats.RunningSpeed : ctx.Stats.WalkingSpeed,
                        slopeFactorFn: ctx.Environment.SlopeFactorAlongDirection,
                        speedFactor: ctx.Input[SpeedFactor],
                        _curveLogProv?.CurveLogger))
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