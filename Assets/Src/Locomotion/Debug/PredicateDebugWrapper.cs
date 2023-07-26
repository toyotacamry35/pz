namespace Src.Locomotion
{
    internal class PredicateDebugWrapper<TStateMachineContext> : Predicate<TStateMachineContext> where TStateMachineContext : StateMachineContext
    {
        private readonly ILocomotionPredicate<TStateMachineContext> _predicate;
        private readonly DebugTag _tag;
        private bool _value;

        public PredicateDebugWrapper(ILocomotionPredicate<TStateMachineContext> predicate, DebugTag tag)
        {
            _predicate = predicate;
            _tag = tag;
        }

        public override bool Evaluate(TStateMachineContext ctx)
        {
            return _value;
        }

        public override void Execute(TStateMachineContext ctx)
        {
            _predicate.Execute(ctx);
            _value = _predicate.Evaluate(ctx);
            if (LocomotionDebug.DebugAgent != null)
                LocomotionDebug.DebugAgent.Set(_tag, _value);
        }
    }
}