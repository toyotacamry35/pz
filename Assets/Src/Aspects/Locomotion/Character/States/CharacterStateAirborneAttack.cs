using ColonyShared.SharedCode.Aspects.Locomotion;
using SharedCode.Utils;
using static Src.Locomotion.CommonInputs;
using static Src.Locomotion.LocomotionAirbornePhysics;

namespace Src.Locomotion.States
{
    public class CharacterStateAirborneAttack : StateBase<CharacterStateMachineContext>
    {
        private Vector2 _controlVelocity;

        public override void OnEnter(CharacterStateMachineContext ctx, VariablesPipeline pipeline)
        {
            _controlVelocity = Vector2.zero;
        }

        public override void Execute(CharacterStateMachineContext ctx, VariablesPipeline pipeline)
        {
            var controlAccel = CalcAirControlAccel(
                input: ctx.Input[Move],
                guide: ctx.Input[Guide],
                forward: ctx.Body_Deprecated.Forward,
                accelFn: ctx.Stats.AirborneAttackControlAccel,
                speedFn: ctx.Stats.AirborneAttackControlSpeed,
                airborneTime: ctx.StateElapsedTime);
            _controlVelocity = LocomotionPhysics.ApplyAccel(_controlVelocity, controlAccel, ctx.Clock.DeltaTime);
            
            pipeline.ApplyFlags(LocomotionFlags.Direct | LocomotionFlags.NoCollideWithActors)
                .ApplyHorizontalImpulse(_controlVelocity);
            if (ctx.Input[Direct])
                pipeline
                    .SetHorizontalVelocity(ctx.Input[DirectVelocity])
                    .SetVerticalVelocity(
                        ctx.Input[DirectVelocityVertical])
                    .ApplyOrientation(
                        ctx.Input[DirectOrientation]);
        }

        public override void OnExit(CharacterStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.SetHorizontalVelocity(Vector2.zero);
        }
    }
}