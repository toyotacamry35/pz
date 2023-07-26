using ColonyShared.SharedCode.Aspects.Locomotion;
using static Src.Locomotion.LocomotionAirbornePhysics;
using static Src.Locomotion.CommonInputs;

namespace Src.Locomotion.States
{
    internal class MobStatePathJumpingOff : StateBase<CommonStateMachineContext>
    {
        public override void Execute(CommonStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(LocomotionFlags.Airborne)
                .ApplyAccel(
                    CalcAirControlAccel(
                        input: ctx.Input[Move],
                        guide: ctx.Input[Guide],
                        forward: ctx.Body_Deprecated.Forward,
                        accelFn: ctx.Stats.AirControlAccel,
                        speedFn: ctx.Stats.AirControlSpeed,
                        airborneTime: ctx.History.AirborneTime))
//                .ApplyOrientation(
//                    guide: ctx.Input[Guide],
//                    maxAngularVelocity: ctx.Stats.JumpYawSpeed)
                .ApplyGravity(gravity: ctx.Environment.Gravity)
                ;
        }
    }
}