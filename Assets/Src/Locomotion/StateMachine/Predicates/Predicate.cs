using System;

namespace Src.Locomotion
{
    public abstract class Predicate<TStateMachineContext> : ILocomotionPredicate<TStateMachineContext> where TStateMachineContext : StateMachineContext
    {
        public static AndPredicate<TStateMachineContext> operator &(Predicate<TStateMachineContext> a, ILocomotionPredicate<TStateMachineContext> b)
        {
            return new AndPredicate<TStateMachineContext>(a, b);
        }

        public static AndPredicate<TStateMachineContext> operator &(Predicate<TStateMachineContext> a, PredicateDelegate<TStateMachineContext> b)
        {
            return new AndPredicate<TStateMachineContext>(a, new SimplePredicate<TStateMachineContext>(b));
        }

        public static OrPredicate<TStateMachineContext> operator |(Predicate<TStateMachineContext> a, ILocomotionPredicate<TStateMachineContext> b)
        {
            return new OrPredicate<TStateMachineContext>(a, b);
        }

        public static OrPredicate<TStateMachineContext> operator |(Predicate<TStateMachineContext> a, PredicateDelegate<TStateMachineContext> b)
        {
            return new OrPredicate<TStateMachineContext>(a, new SimplePredicate<TStateMachineContext>(b));
        }

        public static NotPredicate<TStateMachineContext> operator !(Predicate<TStateMachineContext> a)
        {
            return new NotPredicate<TStateMachineContext>(a);
        }
        
        public abstract bool Evaluate(TStateMachineContext ctx);
        public abstract void Execute(TStateMachineContext ctx);
    }
}
