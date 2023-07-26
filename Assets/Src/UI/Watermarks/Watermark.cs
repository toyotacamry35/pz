using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class Watermark : BindingViewModel
    {
        private bool _isVisible;

        [Binding]
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _text;

        [Binding]
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _text2;

        [Binding]
        public string Text2
        {
            get { return _text2; }
            set
            {
                if (_text2 != value)
                {
                    _text2 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [Binding]
        public bool HasSprite => Sprite != null;

        private Sprite _sprite;

        [Binding]
        public Sprite Sprite
        {
            get { return _sprite; }
            set
            {
                if (_sprite != value)
                {
                    var oldHasSprite = HasSprite;
                    _sprite = value;
                    NotifyPropertyChanged();
                    if (HasSprite != oldHasSprite)
                        NotifyPropertyChanged(nameof(HasSprite));
                }
            }
        }

        private Color _textColor;

        [Binding]
        public Color TextColor
        {
            get { return _textColor; }
            set
            {
                if (_textColor != value)
                {
                    _textColor = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}