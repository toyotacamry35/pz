using Assets.Src.ResourcesSystem.Base;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public abstract class RequiredCountContr<T> : BindingController<RequirementVmodel<T>> where T : BaseResource
    {
        //=== Props ===========================================================

        [Binding]
        public int Count { get; protected set; }

        [Binding]
        public bool IsEmpty { get; protected set; }

        [Binding]
        public bool IsEnough { get; protected set; }


        //=== Unity ===========================================================

        protected virtual void Awake()
        {
            var countStream = Vmodel.Func(D, vm => vm?.RequiredCount ?? 0);
            Bind(countStream, () => Count);
            Bind(Vmodel.SubStream(D, vm => vm.IsEnoughRp, true), () => IsEnough);
            Bind(Vmodel.Func(D, vm => (vm?.RequiredCount ?? 0) == 0), () => IsEmpty);
        }
    }
}