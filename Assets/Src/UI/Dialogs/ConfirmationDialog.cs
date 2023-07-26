using System;
using ColonyShared.SharedCode.Input;
using L10n;
using Uins.Sound;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class ConfirmationDialog : BaseGuiWindow
    {
        private LocalizedString _description;
        private Action _onCancelAction;

        private Action _onConfirmAction;
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

            if (args is ConfirmationDialogParams confirmationDialogParams)
            {
                _onConfirmAction = confirmationDialogParams.OnConfirmAction;
                _onCancelAction = confirmationDialogParams.OnCancelAction;
                Description = confirmationDialogParams.Description;
            }
            else
            {
                UI.Logger.Error($"No Params Of Type {typeof(ConfirmationDialogParams)} Passed {args}");
            }
        }

        public override void OnClose()
        {
            base.OnClose();
            _onConfirmAction = _onCancelAction = null;
        }

        public void OnConfirmClick()
        {
            WindowsManager.Close(this);

            _onConfirmAction?.Invoke();
        }

        public void OnCancelClick()
        {
            SoundControl.Instance.ButtonSmall.Post(transform.root.gameObject);
            WindowsManager.Close(this);

            _onCancelAction?.Invoke();
        }
    }
}