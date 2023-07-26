using Assets.Src.ResourcesSystem.Base;
using L10n;
using SharedCode.Wizardry;
using System.Collections.Generic;

namespace ResourceSystem.Aspects.Dialog
{
    [Localized]
    public class DialogDef : BaseResource
    {
        public ResourceRef<SpellDef> Spell { get; set; }
        public LocalizedString Phrase { get; set; }
        public DialogTransition[] Next { get; set; }
    }

    [Localized]
    public struct DialogTransition
    {
        public LocalizedString Answer1 { get; set; }
        //public LocalizedString[] Answer { get; set; }
        public LocalizedString OverwritePhrase { get; set; }
        public ResourceRef<DialogDef> Dialog { get; set; }
    }
}
