namespace Src.Locomotion.States
{
    internal class MobStateStanding : StateBase<MobStateMachineContext>
    {
        public override void Execute(MobStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(0)
                .ApplyFriction(
                    friction: ctx.Stats.Decel)
                .ApplyGravityOnGround(
                    gravity: ctx.Environment.Gravity,
                    hasContact: ctx.Environment.HasGroundContact)
                ;
        }
    }
}