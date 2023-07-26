using Assets.ColonyShared.SharedCode.Interfaces;

namespace Src.Locomotion
{
    public class TriggerSwitchingPredicate<TStateMachineContext> : Predicate<TStateMachineContext>, IResettable where TStateMachineContext : StateMachineContext
    {
        private readonly  PredicateDelegate<TStateMachineContext> _trigger;
        private readonly float _period;
        private bool _last;
        private bool _value;
        private float _valueExpiredAt;

        public TriggerSwitchingPredicate(PredicateDelegate<TStateMachineContext> trigger, float period = 0)
        {
            _trigger = trigger;
            _period = period;
        }

        public override bool Evaluate(TStateMachineContext ctx)
        {
            return _value;
        }

        public override void Execute(TStateMachineContext ctx)
        {
            var time = ctx.Clock.Time;
            if (_valueExpiredAt < time)
                _value = false;
 
            var now = _trigger(ctx);
            if (!_last && now)
            {
                _value = true;
                _valueExpiredAt = time + _period;
            }
            _last = now;
        }

        public void Reset()
        {
            _valueExpiredAt = 0f;
            _last = default;
        }
    }
}