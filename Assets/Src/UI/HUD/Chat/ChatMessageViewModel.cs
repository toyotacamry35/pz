using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class ChatMessageViewModel : BindingViewModel
    {
        //=== Props ===============================================================

        private bool _isVisible;

        [Binding]
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value != _isVisible)
                {
                    _isVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _orderIndex;

        public int OrderIndex
        {
            get => _orderIndex;
            set
            {
                if (value != _orderIndex)
                {
                    _orderIndex = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(VisibilityRatio));
                }
            }
        }

        [Binding]
        public float VisibilityRatio => GetVisibilityRatio();

        private string _message;

        [Binding]
        public string Message
        {
            get => _message;
            private set
            {
                if (value != _message)
                {
                    _message = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Public ==========================================================

        public void Set(string senderName, string message)
        {
            Message = $"<color=\"white\">{senderName}:</color>  {message}";
        }


        //=== Private =========================================================

        private float GetVisibilityRatio()
        {
            return 1 - OrderIndex * .3f;
        }
    }
}