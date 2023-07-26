using L10n;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class ConstructionButton : BindingViewModel
    {
        //=== Props ===========================================================

        public int ButtonIndex { get; set; }

        private HotkeyListener _hotkey;

        public HotkeyListener Hotkey
        {
            get => _hotkey;
            set
            {
                if (_hotkey != value)
                {
                    var oldIsInteractable = IsInteractable;
                    _hotkey = value;
                    HotkeyName = _hotkey == null ? "" : _hotkey.GetFirstHotkeyName();
                    if (oldIsInteractable != IsInteractable)
                        NotifyPropertyChanged(nameof(IsInteractable));
                }
            }
        }

        private string _hotkeyName = "";

        [Binding]
        public string HotkeyName
        {
            get => _hotkeyName;
            private set
            {
                if (_hotkeyName != value)
                {
                    _hotkeyName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _hasDef;

        public bool HasDef
        {
            get => _hasDef;
            set
            {
                if (_hasDef != value)
                {
                    var oldIsInteractable = IsInteractable;
                    _hasDef = value;
                    if (oldIsInteractable != IsInteractable)
                        NotifyPropertyChanged(nameof(IsInteractable));
                }
            }
        }

        [Binding]
        public bool IsInteractable => Hotkey != null && HasDef;

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

        private Sprite _icon;

        [Binding]
        public Sprite Icon
        {
            get => _icon;
            set
            {
                if (_icon != value)
                {
                    _icon = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}