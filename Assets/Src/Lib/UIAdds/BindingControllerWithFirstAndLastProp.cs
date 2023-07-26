using ReactivePropsNs;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public abstract class BindingControllerWithFirstAndLastProp<T> : BindingControllerWithUsingProp<T>, IHasFirstAndLastRp
    {
        //=== Props ===========================================================

        public ReactiveProperty<bool> IsFirstRp { get; } = new ReactiveProperty<bool>() {Value = false};
        public ReactiveProperty<bool> IsLastRp { get; } = new ReactiveProperty<bool>() {Value = false};

        [Binding]
        public bool IsFirst { get; protected set; }

        [Binding]
        public bool IsLast { get; protected set; }


        //=== Unity ===========================================================

        protected override void Awake()
        {
            base.Awake();
            Bind(IsFirstRp, () => IsFirst);
            Bind(IsLastRp, () => IsLast);
        }
    }
}