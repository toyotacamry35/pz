namespace Src.Locomotion
{
    public class SimplePredicate<TStateMachineContext> : Predicate<TStateMachineContext> where TStateMachineContext : StateMachineContext
    {
        private readonly PredicateDelegate<TStateMachineContext> _predicate;
        private int _value;

        public SimplePredicate(PredicateDelegate<TStateMachineContext> predicate)
        {
            _predicate = predicate;
        }

        public override bool Evaluate(TStateMachineContext ctx)
        {
            if (_value == -1)
                _value = _predicate(ctx) ? 1 : 0;
            return _value == 1;
        }
        
        public override void Execute(TStateMachineContext ctx)
        {
            _value = -1;
        }        
    }
}