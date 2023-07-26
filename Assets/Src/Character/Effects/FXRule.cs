using Assets.Src.Arithmetic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Src.Character.Effects
{
    public interface FXRule
    {
        Task CalcShouldDo(CalcerContext ctx, FXRuleDef def);
        void UpdateValue(FXRuleDef def, Animator animator);
    }

    public abstract class FXRuleBase<TDef, TRes> : FXRule where TDef : FXRuleDef where TRes : struct
    {
        private readonly object _lock = new object();
        private TRes _res;
        private bool _hasValue;

        public async Task CalcShouldDo(CalcerContext ctx, FXRuleDef def)
        {
            var res = await CalcShouldDoImpl(ctx, (TDef) def);
            lock (_lock)
            {
                _res = res;
                _hasValue = true;
            }
        }

        public void UpdateValue(FXRuleDef def, Animator animator)
        {
            if (_hasValue)
                lock (_lock)
                {
                    _hasValue = false;
                    UpdateValueImpl((TDef) def, animator, _res);
                }
        }

        protected abstract ValueTask<TRes> CalcShouldDoImpl(CalcerContext ent, TDef def);

        protected abstract void UpdateValueImpl(TDef def, Animator animator, TRes value);
    }
    
    public class FloatFXRule : FXRuleBase<FloatFXRuleDef,float>
    {
        protected override ValueTask<float> CalcShouldDoImpl(CalcerContext ctx, FloatFXRuleDef def)
        {
            return def.Calcer.Target.CalcAsync(ctx);
        }

        protected override void UpdateValueImpl(FloatFXRuleDef def, Animator animator, float value)
        {
            var animatorPropName = def.AnimatedProp;
            animator.SetFloat(animatorPropName, value);
        }
    }

    public class IntFXRule : FXRuleBase<IntFXRuleDef,int>
    {
        protected override async ValueTask<int> CalcShouldDoImpl(CalcerContext ctx, IntFXRuleDef def)
        {
            var res = await def.Calcer.Target.CalcAsync(ctx);
            return (int)res;
        }

        protected override void UpdateValueImpl(IntFXRuleDef def, Animator animator, int value)
        {
            var animatorPropName = def.AnimatedProp;
            animator.SetInteger(animatorPropName, value);
        }
    }

    public class BoolFXRule : FXRuleBase<BoolFXRuleDef,bool>
    {
        protected override ValueTask<bool> CalcShouldDoImpl(CalcerContext ctx, BoolFXRuleDef def)
        {
            return def.Predicate.Target.CalcAsync(ctx);
        }

        protected override void UpdateValueImpl(BoolFXRuleDef def, Animator animator, bool value)
        {
            var animatorPropName = def.AnimatedProp;
            animator.SetBool(animatorPropName, value);
        }
    }
}
