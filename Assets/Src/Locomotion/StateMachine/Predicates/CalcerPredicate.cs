namespace Src.Locomotion
{
    internal class CalcerPredicate<TStateMachineContext> : Predicate<TStateMachineContext> where TStateMachineContext : StateMachineContext
    {
        private readonly CalcersCache.PredicateProxy _value;
        private readonly CalcerArgBinder<TStateMachineContext>[] _argBinders;

        public CalcerPredicate(CalcersCache.PredicateProxy value)
        {
            _value = value;
        }

        public CalcerPredicate(CalcersCache.PredicateProxy value, params CalcerArgBinder<TStateMachineContext>[] argBinders)
        {
            _value = value;
            _argBinders = argBinders;
        }
        
        public override bool Evaluate(TStateMachineContext ctx)
        {
            return _value;
        }

        public override void Execute(TStateMachineContext ctx)
        {
            if (_argBinders != null)
                foreach (var argBinder in _argBinders)
                    argBinder.Execute(ctx);
        }
    }
}