using ColonyShared.SharedCode.Aspects.Locomotion;
using static Src.Locomotion.LocomotionGroundPhysics;
using static Src.Locomotion.CommonInputs;

namespace Src.Locomotion.States
{
    internal class CharacterStatePostLandingStun : StateBase<CharacterStateMachineContext>, ILocomotionState<CharacterStateMachineContext>
    {
        public override void Execute(CharacterStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(LocomotionFlags.Falling)
                .ApplyFriction(
                    friction: ctx.Stats.Decel)
                .ApplyGravity(
                    gravity: ctx.Environment.Gravity)
                ;
        }
    }
}