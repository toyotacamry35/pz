using L10n;
using ReactivePropsNs;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Aspects.Science;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class ScienceVmodel : BindingVmodel
    {
        private ReactiveProperty<int> _countRp = new ReactiveProperty<int>();


        //=== Props ===========================================================

        public ReactiveProperty<ScienceDef> ScienceDefRp { get; } = new ReactiveProperty<ScienceDef>();

        [Binding]
        public int Count { get; private set; }

        [Binding]
        public LocalizedString NameLs { get; private set; }

        [Binding]
        public LocalizedString DescriptionLs { get; private set; }

        [Binding]
        public Sprite Sprite { get; private set; }

        [Binding]
        public Sprite MiniSprite { get; private set; }

        [Binding]
        public Sprite InactiveMiniSprite { get; private set; }


        //=== Ctor ============================================================

        public ScienceVmodel(ScienceDef scienceDef, int count)
        {
            Bind(ScienceDefRp.Func(D, s => s?.NameLs ?? LsExtensions.Empty), () => NameLs);
            Bind(ScienceDefRp.Func(D, s => s?.DescriptionLs ?? LsExtensions.Empty), () => DescriptionLs);
            Bind(ScienceDefRp.Func(D, s => s?.Sprite?.Target), () => Sprite);
            Bind(ScienceDefRp.Func(D, s => s?.MiniSprite?.Target), () => MiniSprite);
            Bind(ScienceDefRp.Func(D, s => s?.InactiveMiniSprite?.Target), () => InactiveMiniSprite);
            Bind(_countRp, () => Count);

            _countRp.Value = count;
            ScienceDefRp.Value = scienceDef;
        }

        public ScienceVmodel(ScienceCount scienceCount) : this(scienceCount?.Science, scienceCount?.Count ?? 0)
        {
        }
    }
}