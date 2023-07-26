using System;
using L10n;
using ReactivePropsNs;
using SharedCode.Aspects.Science;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding, Obsolete("Use CurrencyPointsVmodel instead this")]
    public class TechPointVmodel : BindingVmodel
    {
        private ReactiveProperty<int> _countRp = new ReactiveProperty<int>();


        //=== Props ===========================================================

        public ReactiveProperty<CurrencyResource> CurrencyResourceRp { get; } = new ReactiveProperty<CurrencyResource>();

        [Binding]
        public int Count { get; private set; }

        [Binding]
        public LocalizedString NameLs { get; private set; }

        [Binding]
        public LocalizedString ShortNameLs { get; private set; }

        [Binding]
        public LocalizedString DescriptionLs { get; private set; }

        [Binding]
        public Sprite TechPointIcon { get; private set; }

        [Binding]
        public Sprite TechPointMiniIcon { get; private set; }


        //=== Ctor ============================================================

        public TechPointVmodel(CurrencyResource currencyResource, int count)
        {
            Bind(CurrencyResourceRp.Func(D, cr => cr?.ItemNameLs ?? LsExtensions.Empty), () => NameLs);
            Bind(CurrencyResourceRp.Func(D, cr => cr?.ShortName ?? LsExtensions.Empty), () => ShortNameLs);
            Bind(CurrencyResourceRp.Func(D, cr => cr?.DescriptionLs ?? LsExtensions.Empty), () => DescriptionLs);
            Bind(CurrencyResourceRp.Func(D, cr => cr?.Icon?.Target), () => TechPointMiniIcon);
            Bind(CurrencyResourceRp.Func(D, cr => cr?.BigIcon?.Target), () => TechPointIcon);
            Bind(_countRp, () => Count);

            _countRp.Value = count;
            CurrencyResourceRp.Value = currencyResource;
        }

        public TechPointVmodel(TechPointCount techPointCount) : this(techPointCount?.TechPoint, techPointCount?.Count ?? 0)
        {
        }
    }
}