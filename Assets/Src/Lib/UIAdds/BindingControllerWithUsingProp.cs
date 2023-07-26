using ReactivePropsNs;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public abstract class BindingControllerWithUsingProp<T> : BindingController<T>
    {
        //=== Props ===========================================================

        public ReactiveProperty<bool> IsInUsingRp { get; } = new ReactiveProperty<bool>() {Value = false};

        [Binding]
        public bool IsInUsing { get; protected set; }


        //=== Unity ===========================================================

        protected virtual void Awake()
        {
            Bind(IsInUsingRp, () => IsInUsing);
        }
    }
}