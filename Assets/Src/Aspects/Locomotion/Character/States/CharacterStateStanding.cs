using SharedCode.Utils;
using static Src.Locomotion.LocomotionGroundPhysics;
using static Src.Locomotion.CommonInputs;

namespace Src.Locomotion.States
{
    internal class CharacterStateStanding : StateBase<CharacterStateMachineContext>, ILocomotionState<CharacterStateMachineContext>
    {
        public override void Execute(CharacterStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(0)
                .ApplyAccel(
                    CalcSlipAccel(
                        slopeDirection: ctx.Environment.SlopeDirection,
                        slopeFactor: ctx.Environment.SlopeFactor(),
                        slipAccelFn: ctx.Stats.SlipAccel,
                        slipSpeedFn: ctx.Stats.SlipSpeed))
                .ApplyFriction(
                    friction: ctx.Stats.Decel)
                .ApplyGravity(
                    gravity: ctx.Environment.Gravity)
//                Rotation by camera:
                .ApplyOrientation(
                    direction: ctx.Input[Guide])
                ;
        }
    }
}