using ColonyShared.SharedCode.Aspects.Locomotion;
using static Src.Locomotion.LocomotionAirbornePhysics;
using static Src.Locomotion.CommonInputs;

namespace Src.Locomotion.States
{
    internal class CharacterStateFalling : StateBase<CharacterStateMachineContext>, ILocomotionState<CharacterStateMachineContext>
    {
        public override void Execute(CharacterStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(LocomotionFlags.Falling | LocomotionFlags.Airborne)
                .ApplyAccel(
                    CalcAirControlAccel(
                        input: ctx.Input[Move],
                        guide: ctx.Input[Guide],
                        forward: ctx.Body_Deprecated.Forward,
                        accelFn: ctx.Stats.AirControlAccel,
                        speedFn: ctx.Stats.AirControlSpeed,
                        airborneTime: ctx.History.AirborneTime))
                .ApplyGravity(
                    gravity: ctx.Environment.Gravity)
                .ApplyOrientation(
                    direction: ctx.Input[Guide],
                    maxAngularVelocity: ctx.Stats.JumpYawSpeed)
                ;
        }
    }
}
