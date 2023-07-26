using ColonyShared.SharedCode.Aspects.Locomotion;
using SharedCode.Utils;
using static Src.Locomotion.CommonInputs;

namespace Src.Locomotion.States
{
    public class CharacterStateAirborneAttackHit : StateBase<CommonStateMachineContext>
    {
        public override void Execute(CommonStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(LocomotionFlags.NoCollideWithActors | LocomotionFlags.Landing);
            if (!ctx.Environment.Airborne)
                pipeline.ApplyFriction(ctx.Stats.Decel);
            if (ctx.Input[Direct])
                pipeline.ApplyOrientation(ctx.Input[DirectOrientation]);
            pipeline.ApplyGravity(gravity: ctx.Environment.Gravity * 10);

//                .SetHorizontalVelocity(Vector2.zero)
                //.SnapToGround(Max(ctx.Environment.DistanceToGround - 0.25f, 0), 0, float.MaxValue)
                ;
        }
    }
}