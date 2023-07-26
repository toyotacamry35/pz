using Assets.Src.ContainerApis;
using ReactivePropsNs;
using UnityEngine;

namespace Uins
{
    public class PeriodicallyUpdatableStatValue
    {
        private float _roughness;
        private bool _isLastChangesTaken;
        private bool _isAlwaysPositive;


        //=== Props =======================================================

        public ReactiveProperty<AnyStatState> StatStateRp { get; } = new ReactiveProperty<AnyStatState>();
        public ReactiveProperty<float> ValueRp { get; } = new ReactiveProperty<float>();

        public PeriodicallyUpdatableStatValue(DisposableComposite d, float roughness, bool isAlwaysPositive = false)
        {
            _isAlwaysPositive = isAlwaysPositive;
            _roughness = roughness;
            StatStateRp.Action(d, state => RoughSetValue());
        }


        //=== Public ======================================================

        public void UpdateIfNeed()
        {
            if (!StatStateRp.HasValue)
                return;

            var neeedToBeUpdated = StatStateRp.Value.NeeedToBeUpdated;
            if (neeedToBeUpdated || !_isLastChangesTaken)
            {
                RoughSetValue();
                _isLastChangesTaken = !neeedToBeUpdated;
            }
        }

        public void RoughSetValue()
        {
            var val = _isAlwaysPositive ? Mathf.Max(0, StatStateRp.Value.Value) : StatStateRp.Value.Value;
            if (!ValueRp.HasValue || !RoughApproximately(val, ValueRp.Value))
                ValueRp.Value = val;
        }


        //=== Private =====================================================

        private bool RoughApproximately(float f1, float f2)
        {
            return Mathf.RoundToInt(f1 / _roughness) == Mathf.RoundToInt(f2 / _roughness);
        }
    }
}