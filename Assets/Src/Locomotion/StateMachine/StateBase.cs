using Mathf = Assets.ColonyShared.SharedCode.Utils.SharedHelpers;

namespace Src.Locomotion
{
    public abstract class StateBase<TStateMachineContext> : ILocomotionState<TStateMachineContext> where TStateMachineContext : StateMachineContext
    {
        public string Name { get; private set; }

        public ILocomotionState<TStateMachineContext> SetName(string name)
        {
            Name = name;
            return this;
        }

        public abstract void Execute(TStateMachineContext ctx, VariablesPipeline pipeline);

        public virtual void OnEnter(TStateMachineContext ctx, VariablesPipeline pipeline)
        {}
        
        public virtual void OnExit(TStateMachineContext ctx, VariablesPipeline pipeline)
        {}

        protected StateBase()
        {
            Name = GetType().Name.Replace("State", "");
        }
    }
}
