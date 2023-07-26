using System;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.ColonyShared.SharedCode.Interfaces;

namespace Src.Locomotion
{
    internal class ConstantDelayedPredicate<TStateMachineContext> : Predicate<TStateMachineContext>, IResettable where TStateMachineContext : StateMachineContext
    {
        private readonly PredicateDelegate<TStateMachineContext> _predicate;
        private readonly float _delay;
        private bool _value;
        private float _elapsed;

        public ConstantDelayedPredicate(PredicateDelegate<TStateMachineContext> predicate, float delay)
        {
            _predicate = predicate;
            _delay = delay;
        }

        public override void Execute(TStateMachineContext ctx)
        {
            if (_predicate(ctx))
                _elapsed += ctx.Clock.DeltaTime;
            else
                _elapsed = 0;
            _value = _elapsed >= _delay;
        }

        public override bool Evaluate(TStateMachineContext ctx)
        {
            return _value;
        }

        public void Reset()
        {
            _elapsed = 0f;
        }
    }

    internal class VariableDelayedPredicate<TStateMachineContext> : Predicate<TStateMachineContext>, IResettable where TStateMachineContext : StateMachineContext
    {
        private readonly PredicateDelegate<TStateMachineContext> _predicate;
        private readonly Func<TStateMachineContext, float> _delay;
        private bool _value;
        private float _elapsed;

        public VariableDelayedPredicate(PredicateDelegate<TStateMachineContext>predicate, Func<TStateMachineContext, float> delay)
        {
            _predicate = predicate;
            _delay = delay;
        }

        public override void Execute(TStateMachineContext ctx)
        {
            if (_predicate(ctx))
                _elapsed += ctx.Clock.DeltaTime;
            else
                _elapsed = 0;
            _value = _elapsed >= _delay(ctx);
        }

        public override bool Evaluate(TStateMachineContext ctx)
        {
            return _value;
        }

        public void Reset()
        {
            _elapsed = 0f;
        }
    }
}