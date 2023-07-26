using L10n;
using ReactivePropsNs;
using SharedCode.Aspects.Science;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class RequiredCurrencyCountContr : RequiredCountContr<CurrencyResource>
    {
        [Binding]
        public Sprite Icon { get; protected set; }

        [Binding]
        public LocalizedString Name { get; protected set; }

        protected override void Awake()
        {
            base.Awake();
            Bind(Vmodel.Func(D, vm => vm?.Resource?.Icon?.Target), () => Icon);
            Bind(Vmodel.Func(D, vm => vm?.Resource?.ItemNameLs ?? LsExtensions.Empty), () => Name);
        }
    }
}