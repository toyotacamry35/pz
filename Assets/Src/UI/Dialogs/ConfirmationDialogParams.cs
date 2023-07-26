using System;
using L10n;

namespace Uins
{
    public class ConfirmationDialogParams
    {
        public Action OnConfirmAction;
        public Action OnCancelAction;
        public LocalizedString Description;
    }
}