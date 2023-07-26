using ColonyShared.SharedCode.Aspects.Locomotion;
using static Src.Locomotion.CommonInputs;

namespace Src.Locomotion.States
{
    internal class MobStateLanding : StateBase<CommonStateMachineContext>
    {
        public override void Execute(CommonStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(LocomotionFlags.Airborne | LocomotionFlags.Landing)
                .ApplyGravity(
                    gravity: ctx.Environment.Gravity)
//                .ApplyOrientation(
//                    guide: ctx.Input[Guide],
//                    maxAngularVelocity: ctx.Stats.JumpYawSpeed)
                ;
        }
    }
}