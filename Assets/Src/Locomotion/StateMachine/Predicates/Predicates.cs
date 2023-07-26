using System;

namespace Src.Locomotion
{
    public class Predicates<TStateMachineContext> where TStateMachineContext : StateMachineContext
    {
        public static Predicate<TStateMachineContext> True = new SimplePredicate<TStateMachineContext>(c => true);

        public static Predicate<TStateMachineContext> False = new SimplePredicate<TStateMachineContext>(c => false);
        
        public static Predicate<TStateMachineContext> IsTrue(PredicateDelegate<TStateMachineContext> predicate)
        {
            return new SimplePredicate<TStateMachineContext>(predicate);
        }

        public static Predicate<TStateMachineContext> StayedTrue(PredicateDelegate<TStateMachineContext> predicate, float delay)
        {
            return new ConstantDelayedPredicate<TStateMachineContext>(predicate, delay);
        }

        public static Predicate<TStateMachineContext> StayedTrue(PredicateDelegate<TStateMachineContext> predicate, Func<TStateMachineContext,float> delay)
        {
            return new VariableDelayedPredicate<TStateMachineContext>(predicate, delay);
        }

        public static Predicate<TStateMachineContext> BecameTrue(PredicateDelegate<TStateMachineContext> predicate, float period = 0)
        {
            return new TriggerSwitchingPredicate<TStateMachineContext>(predicate, period);
        }

        public static Predicate<TStateMachineContext> WasTrue(PredicateDelegate<TStateMachineContext> predicate, float period)
        {
            return new TriggerStatePredicate<TStateMachineContext>(predicate, period);
        }
 
        public static Predicate<TStateMachineContext> IsTrue(CalcersCache.PredicateProxy proxy)
        {
            return new CalcerPredicate<TStateMachineContext>(proxy);
        }

        public static Predicate<TStateMachineContext> IsTrue(CalcersCache.PredicateProxy proxy, params CalcerArgBinder<TStateMachineContext>[] argBinders)
        {
            return new CalcerPredicate<TStateMachineContext>(proxy, argBinders);
        }

        public static CalcerArgBinder<TStateMachineContext> CalcerArgBinder(Func<TStateMachineContext, float> func)
        {
            return new CalcerArgBinder<TStateMachineContext>(func);
        }

        public static Predicate<TStateMachineContext> Debug(ILocomotionPredicate<TStateMachineContext> predicate, DebugTag tag)
        {
            return new PredicateDebugWrapper<TStateMachineContext>(predicate, tag);
        }
    }

    public static class PredicateExtensions
    {
        public static Predicate<T> Debug<T>(this ILocomotionPredicate<T> predicate, DebugTag tag) where T : StateMachineContext
        {
            return Predicates<T>.Debug(predicate, tag);
        }
    }
}