namespace Src.Locomotion
{
    public class AndPredicate<TStateMachineContext> : Predicate<TStateMachineContext> where TStateMachineContext : StateMachineContext
    {
        private readonly ILocomotionPredicate<TStateMachineContext> _a;
        private readonly ILocomotionPredicate<TStateMachineContext> _b;

        public AndPredicate(ILocomotionPredicate<TStateMachineContext> a, ILocomotionPredicate<TStateMachineContext> b)
        {
            _a = a;
            _b = b;
        }

        public override bool Evaluate(TStateMachineContext ctx)
        {
            return _a.Evaluate(ctx) && _b.Evaluate(ctx);
        }

        public override void Execute(TStateMachineContext ctx)
        {
            _a.Execute(ctx);
            _b.Execute(ctx);
        }
    }

    public class OrPredicate<TStateMachineContext> : Predicate<TStateMachineContext> where TStateMachineContext : StateMachineContext
    {
        private readonly ILocomotionPredicate<TStateMachineContext> _a;
        private readonly ILocomotionPredicate<TStateMachineContext> _b;

        public OrPredicate(ILocomotionPredicate<TStateMachineContext> a, ILocomotionPredicate<TStateMachineContext> b)
        {
            _a = a;
            _b = b;
        }

        public override bool Evaluate(TStateMachineContext ctx)
        {
            return _a.Evaluate(ctx) || _b.Evaluate(ctx);
        }

        public override void Execute(TStateMachineContext ctx)
        {
            _a.Execute(ctx);
            _b.Execute(ctx);
        }
    }

    public class NotPredicate<TStateMachineContext> : Predicate<TStateMachineContext> where TStateMachineContext : StateMachineContext
    {
        private readonly ILocomotionPredicate<TStateMachineContext> _a;

        public NotPredicate(ILocomotionPredicate<TStateMachineContext> a)
        {
            _a = a;
        }

        public override bool Evaluate(TStateMachineContext ctx)
        {
            return !_a.Evaluate(ctx);
        }

        public override void Execute(TStateMachineContext ctx)
        {
            _a.Execute(ctx);
        }
    }
}