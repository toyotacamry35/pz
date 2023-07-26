using ColonyShared.SharedCode.Aspects.Locomotion;
using SharedCode.Utils;
using static Src.Locomotion.LocomotionAirbornePhysics;
using static Src.Locomotion.CommonInputs;

namespace Src.Locomotion.States
{
    /// <summary>
    /// Прыжок на месте
    /// </summary>
    internal class MobStateJumpingOnSpot : StateBase<CommonStateMachineContext>
    {
        private Vector2 _jumpDirection;
        
        public override void OnEnter(CommonStateMachineContext ctx, VariablesPipeline pipeline)
        {
            _jumpDirection = ctx.Input[Guide];
            
            pipeline.ApplyVerticalImpulse(
                impulse: ctx.Stats.JumpVerticalImpulse(Vector2.zero));
        }
        
        public override void Execute(CommonStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(LocomotionFlags.Jumping | LocomotionFlags.Airborne)
                .ApplyOrientation(
                    direction: _jumpDirection,
                    maxAngularVelocity: ctx.Stats.JumpYawSpeed)
                .ApplyAccel(
                    CalcAirControlAccel(
                        input: ctx.Input[Move],
                        guide: ctx.Input[Guide],
                        forward: ctx.Body_Deprecated.Forward,
                        accelFn: ctx.Stats.AirControlAccel,
                        speedFn: ctx.Stats.AirControlSpeed,
                        airborneTime: ctx.History.AirborneTime))
                .ApplyGravity(gravity: ctx.Environment.Gravity)
                ;
        }
    }
}