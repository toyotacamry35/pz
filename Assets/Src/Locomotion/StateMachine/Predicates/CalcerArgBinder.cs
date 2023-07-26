using System;

namespace Src.Locomotion
{
    public class CalcerArgBinder<TStateMachineContext>
    {
        private readonly Func<TStateMachineContext, float> _func;

        public CalcerArgBinder(Func<TStateMachineContext, float> func)
        {
            _func = func;
        }

        public float Value { get; private set; }

        public void Execute(TStateMachineContext ctx)
        {
            Value = _func.Invoke(ctx);
        }
    }
}