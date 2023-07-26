using ColonyShared.SharedCode.Input;
using L10n;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class DoomDialog : BaseGuiWindow
    {
        private LocalizedString _description;
        public override InputBindingsDef InputBindings => UI.BlockedActionsMovementAndCamera;
        public override bool IsUnclosable => false;

        [Binding]
        public LocalizedString Description
        {
            get => _description;
            set
            {
                _description = value;
                NotifyPropertyChanged();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            State.Dispose();
        }

        public override void OnOpen(object args)
        {
            base.OnOpen(args);

            if (args is DoomDialogParams doomDialogParams)
                Description = doomDialogParams.Description;
            else
                UI.Logger.Error($"No Params Of Type {typeof(ConfirmationDialogParams)} Passed {args}");
        }
    }
}