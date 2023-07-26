using ColonyShared.SharedCode.Aspects.Locomotion;
using static Src.Locomotion.CommonInputs;
using static Src.Locomotion.CharacterInputs;
using Mathf = Assets.ColonyShared.SharedCode.Utils.SharedHelpers;

namespace Src.Locomotion.States
{
    internal class CharacterStateCheatMovement: StateBase<CharacterStateMachineContext>, ILocomotionState<CharacterStateMachineContext>
    {
        public override void Execute(CharacterStateMachineContext ctx, VariablesPipeline pipeline)
        {
            var inputHorizontal = LocomotionHelpers.TransformMovementInputAxes(ctx.Input[Move], ctx.Input[Guide]);
            var inputVertical = ctx.Input[MoveVertical];
            float speed = ctx.Stats.CheatSpeed * Mathf.Max(ctx.Input[CheatSpeed], 1);
            var velocity = new LocomotionVector(inputHorizontal, inputVertical) * speed;
            pipeline.ApplyFlags(LocomotionFlags.CheatMode)
                .SetVelocity(velocity)
                .ApplyOrientation(
                    direction: ctx.Input[Guide])
                ;
        }

        public override void OnExit(CharacterStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.SetVelocity(LocomotionVector.Zero);

            ctx.History.ResetAirborne(); // чтобы персонаж после установки на землю не умирал от падения

            if (ctx.Environment.DistanceToGround < float.MaxValue)
//                pipeline.ApplyTeleport(ctx.Body_Deprecated.Position - LocomotionVector.Up * Mathf.Max(ctx.Environment.DistanceToGround - 0.25f, 0));
                pipeline.SnapToGround(Mathf.Max(ctx.Environment.DistanceToGround - 0.25f, 0), 0, float.MaxValue);
        }
    }
    
    internal class CharacterStatePostCheatMovement: StateBase<CharacterStateMachineContext>, ILocomotionState<CharacterStateMachineContext>
    {
        public override void Execute(CharacterStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(LocomotionFlags.CheatMode)
                .SetVelocity(LocomotionVector.Zero)
                .ApplyOrientation(
                    direction: ctx.Input[Guide])
                ;
        }
    }
}