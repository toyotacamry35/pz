using ReactivePropsNs;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class TextWithFormatModContr : BindingController<(string, FormatMod)>
    {
        //=== Props ===========================================================

        [Binding]
        public string Text { get; protected set; }

        [Binding]
        public bool HasFrame { get; protected set; }

        [Binding]
        public float TitleVerticalPaddingTop { get; protected set; }

        [Binding]
        public float TitleVerticalPaddingBottom { get; protected set; }

        [Binding]
        public float TitleFontSizeDelta { get; protected set; }


        //=== Unity ===========================================================

        protected virtual void Awake()
        {
            var textStream = Vmodel.Func(D, (text, fm) => text ?? "");
            Bind(textStream, () => Text);
            var fmStream = Vmodel.Func(D, (text, fm) => fm);
            Bind(fmStream.Func(D, fm => fm.HasFrame), () => HasFrame);
            Bind(fmStream.Func(D, fm => fm.VerticalPadding.X), () => TitleVerticalPaddingTop);
            Bind(fmStream.Func(D, fm => fm.VerticalPadding.Y), () => TitleVerticalPaddingBottom);
            Bind(fmStream.Func(D, fm => fm.FontSizeDelta), () => TitleFontSizeDelta);
        }
    }
}