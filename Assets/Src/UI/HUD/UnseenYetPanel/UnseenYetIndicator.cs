using Assets.ColonyShared.SharedCode.Aspects.UI;
using L10n;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class UnseenYetIndicator : BindingViewModel
    {
        //=== Props ===========================================================

        private int _count;

        [Binding]
        public int Count
        {
            get => _count;
            private set
            {
                if (_count != value)
                {
                    _count = value;
                    NotifyPropertyChanged();
                    IsVisible = _count != 0;
                }
            }
        }

        private bool _isVisible;

        [Binding]
        public bool IsVisible
        {
            get => _isVisible;
            private set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private LocalizedString _title;

        [Binding]
        public LocalizedString Title
        {
            get => _title;
            private set
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
            private set
            {
                if (_icon != value)
                {
                    _icon = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Sprite _shortcutIcon;

        [Binding]
        public Sprite ShortcutIcon
        {
            get => _shortcutIcon;
            private set
            {
                if (_shortcutIcon != value)
                {
                    _shortcutIcon = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Public ==========================================================

        public void Init(UnseenYetIndicatorDef def)
        {
            if (def.AssertIfNull(nameof(def)))
                return;

            Icon = def.Icon?.Target;
            ShortcutIcon = def.ShortcutIcon?.Target;
            Title = def.Title;
        }

        public void SetCount(int newCount)
        {
            Count = newCount;
        }
    }
}