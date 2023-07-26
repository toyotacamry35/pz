namespace Src.Locomotion
{
    public interface ILocomotionStateInfo
    {
        string Name { get; }
    }
    
    public interface ILocomotionState<in TStateMachineContext> : ILocomotionStateInfo where TStateMachineContext : StateMachineContext
    {
        void Execute(TStateMachineContext ctx, VariablesPipeline pipeline);

        void OnEnter(TStateMachineContext ctx, VariablesPipeline pipeline);
        
        void OnExit(TStateMachineContext ctx, VariablesPipeline pipeline); 
    }
}
