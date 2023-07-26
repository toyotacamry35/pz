using System;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using L10n;
using UnityWeld.Binding;

namespace Uins
{
    //старая версия, перенести на ContextActionViewModel
    [Binding]
    public class ContextMenuItemViewModel : BindingViewModel
    {
        private Action<object[]> _action;
        private object[] _actionParams;

        public InventoryContextMenuViewModel ParentViewModel { get; set; }


        //=== Props ===============================================================

        private LocalizedString _title;

        [Binding]
        public LocalizedString Title
        {
            get => _title;
            set
            {
                if (!_title.Equals(value))
                {
                    _title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isDisabled;

        [Binding]
        public bool IsDisabled
        {
            get => _isDisabled;
            set
            {
                if (_isDisabled != value)
                {
                    _isDisabled = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Public ==============================================================

        public void SetAction(Action<object[]> action, object[] actionParams)
        {
            _action = action;
            _actionParams = actionParams;
        }

        public void Execute()
        {
            if (IsDisabled)
            {
                UI.Logger.IfError()?.Message($"{ToString()}.{nameof(Execute)}() Unexpected state: IsDisabled").Write();
                return;
            }
            ParentViewModel.IsShown = false;
            try
            {
                _action?.Invoke(_actionParams);
            }
            catch (Exception e)
            {
                UI.Logger.IfError()?.Message($"<{GetType()}> {nameof(Execute)}(action={_action}, params={_actionParams.ItemsToString()}): {e}").Write();
            }
        }

        public override string ToString()
        {
            return $"[{GetType()}: '{Title.GetText()}' {nameof(IsDisabled)}{IsDisabled.AsSign()}]";
        }
    }
}