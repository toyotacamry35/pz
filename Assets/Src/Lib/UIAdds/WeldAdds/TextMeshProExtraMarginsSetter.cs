using TMPro;
using UnityEngine;

namespace WeldAdds
{
    public class TextMeshProExtraMarginsSetter : MonoBehaviour
    {
        public TextMeshProUGUI Target;

        /// <summary>
        /// Добавка/вычет относительно имеющегося размера, а не собственно значение
        /// </summary>
        public bool IsRelative;

        private bool _isAfterAwake;
        private Vector4 _baseMargin;


        //=== Props ===========================================================

        private float _topMargin;

        public float TopMargin
        {
            get => _topMargin;
            set
            {
                if (!Mathf.Approximately(_topMargin, value))
                {
                    _topMargin = value;
                    if (_isAfterAwake)
                        SyncIfWoken();
                }
            }
        }

        private float _bottomMargin;

        public float BottomMargin
        {
            get => _bottomMargin;
            set
            {
                if (!Mathf.Approximately(_bottomMargin, value))
                {
                    _bottomMargin = value;
                    if (_isAfterAwake)
                        SyncIfWoken();
                }
            }
        }

        private float _leftMargin;

        public float LeftMargin
        {
            get => _leftMargin;
            set
            {
                if (!Mathf.Approximately(_leftMargin, value))
                {
                    _leftMargin = value;
                    if (_isAfterAwake)
                        SyncIfWoken();
                }
            }
        }

        private float _rightMargin;

        public float RightMargin
        {
            get => _rightMargin;
            set
            {
                if (!Mathf.Approximately(_rightMargin, value))
                {
                    _rightMargin = value;
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

            _baseMargin = IsRelative ? Target.margin : new Vector4();

            _isAfterAwake = true;
            SyncIfWoken();
        }
        
        
        //=== Private =============================================================

        private void SyncIfWoken()
        {
            Target.margin = _baseMargin + new Vector4(LeftMargin, TopMargin, RightMargin, BottomMargin);
        }
    }
}