namespace Src.Locomotion.States
{
    internal class CommonStateNope : StateBase<StateMachineContext>
    {
        public override void Execute(StateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(0)
                .SetVelocity(LocomotionVector.Zero);
        }
    }
}