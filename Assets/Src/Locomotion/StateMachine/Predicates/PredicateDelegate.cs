namespace Src.Locomotion
{
    public delegate bool PredicateDelegate<in TStateMachineContext>(TStateMachineContext ctx) where TStateMachineContext : StateMachineContext;
}