using System;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using L10n;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class ContextActionViewModel : BindingViewModel
    {
        private Action<object[]> _action;
        private object[] _actionParams;


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

        private bool _isDefaultAction;

        [Binding]
        public bool IsDefaultAction
        {
            get => _isDefaultAction;
            set
            {
                if (_isDefaultAction != value)
                {
                    _isDefaultAction = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isUsed;

        [Binding]
        public bool IsUsed
        {
            get => _isUsed;
            set
            {
                if (_isUsed != value)
                {
                    _isUsed = value;
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

            try
            {
                _action?.Invoke(_actionParams);
            }
            catch (Exception e)
            {
                UI.Logger.Error(
                    $"<{GetType()}> {nameof(Execute)}(action={_action}, params={_actionParams.ItemsToString()}): {e}");
            }
        }

        public override string ToString()
        {
            return $"[{GetType()}: '{Title}' {nameof(IsDisabled)}{IsDisabled.AsSign()}]";
        }
    }
}