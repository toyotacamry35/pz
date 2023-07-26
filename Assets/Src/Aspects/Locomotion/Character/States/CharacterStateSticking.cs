using ColonyShared.SharedCode.Aspects.Locomotion;
using static Src.Locomotion.CommonInputs;

namespace Src.Locomotion.States
{
    public class CharacterStateSticking : StateBase<CommonStateMachineContext>
    {
        public override void Execute(CommonStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(LocomotionFlags.Direct)
                .SetVelocity(LocomotionVector.Zero)
                .ApplyOrientation(
                    ctx.Input[DirectOrientation])
                .ApplyGravity(
                    gravity: ctx.Environment.Gravity)
                ;
        }
    }
}