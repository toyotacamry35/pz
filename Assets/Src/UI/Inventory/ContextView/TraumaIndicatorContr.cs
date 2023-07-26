using Assets.Src.Aspects.Impl.Traumas.Template;
using L10n;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class TraumaIndicatorContr : BindingController<TraumaDef>
    {
        [Binding]
        public Sprite Sprite { get; private set; }

        [Binding]
        public LocalizedString DescriptionLs { get; private set; }

        private void Awake()
        {
            var spriteStream = Vmodel.Func(D, def => def.Icon());
            Bind(spriteStream, () => Sprite);

            var descrStream = Vmodel.Func(D, def => def.Description());
            Bind(descrStream, () => DescriptionLs);
        }
    }
}