using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class ImageBlockContr : CreditsBlockContr<ImageBlock>
    {
        //=== Props ===========================================================

        [Binding]
        public Sprite Sprite { get; private set; }

        [Binding]
        public float Width { get; private set; }

        [Binding]
        public float Height { get; private set; }

        [Binding]
        public float VerticalPaddingTop { get; private set; }

        [Binding]
        public float VerticalPaddingBottom { get; private set; }


        //=== Unity ===========================================================

        protected override void Awake()
        {
            base.Awake();
            Bind(Vmodel.Func(D, block => block?.Image?.Target), () => Sprite);
            Bind(Vmodel.Func(D, block => block?.Size.X ?? 0), () => Width);
            Bind(Vmodel.Func(D, block => block?.Size.Y ?? 0), () => Height);
            //Воспользуемся TitleFormatMod.VerticalPadding для установки VerticalPadding всего элемента
            Bind(Vmodel.Func(D, block => block?.TitleFormatMod.VerticalPadding.X ?? 0), () => VerticalPaddingTop);
            Bind(Vmodel.Func(D, block => block?.TitleFormatMod.VerticalPadding.Y ?? 0), () => VerticalPaddingBottom);
        }
    }
}