using TMPro;
using UnityEngine;

namespace WeldAdds
{
    public class TextMeshProFontSizeSetter : MonoBehaviour
    {
        public TextMeshProUGUI Target;

        /// <summary>
        /// Добавка/вычет относительно имеющегося размера, а не собственно значение
        /// </summary>
        public bool IsRelative;

        private bool _isAfterAwake;
        private float _baseFontSize;


        //=== Props ===========================================================

        private float _fontSize;

        public float FontSize
        {
            get => _fontSize;
            set
            {
                if (!Mathf.Approximately(_fontSize, value))
                {
                    _fontSize = value;
                    if (_isAfterAwake)
                        SyncIfWoken();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            if (Target.AssertIfNull(nameof(Target)))
            {
                enabled = false;
                return;
            }

            _baseFontSize = IsRelative ? Target.fontSize : 0;

            _isAfterAwake = true;
            SyncIfWoken();
        }
        
        
        //=== Private =============================================================

        private void SyncIfWoken()
        {
            Target.fontSize = _baseFontSize + FontSize;
        }
    }
}