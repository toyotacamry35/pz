using L10n;
using ResourceSystem.Aspects.Dialog;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class DialogVariantVmodel : BindingVmodel
    {
        [Binding]
        public int Index { get; }

        [Binding]
        public LocalizedString DescriptionLs { get; }

        public LocalizedString Phrase { get; }
        public DialogDef Dialog { get; set; }

        public DialogVariantVmodel(DialogDef dialogDef, LocalizedString answer, LocalizedString phrase, int index = 0)
        {
            Dialog = dialogDef;
            DescriptionLs = answer;
            Phrase = phrase;
            Index = index;
        }
    }
}