using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace WeldAdds
{
    public class ButtonColorsSetter : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private Image _targetImage;

        [SerializeField, UsedImplicitly]
        private Button _targetButton;

        [SerializeField, UsedImplicitly]
        private ButtonColorsSet[] _buttonColorsSets;

        [SerializeField, UsedImplicitly]
        private ButtonColorsSet _defaultSet;

        [SerializeField, UsedImplicitly]
        private bool _useFlagNorIndex;

        private bool _isAfterAwake;


        //=== Props ===========================================================

        private bool _flag;

        public bool Flag
        {
            get => _flag;
            set
            {
                if (!enabled)
                    return;

                if (_flag != value)
                {
                    _flag = value;
                    if (_isAfterAwake)
                        SyncIfWoken();
                }
            }
        }

        private int _index;

        public int Index
        {
            get => _index;
            set
            {
                if (!enabled)
                    return;

                if (_index != value)
                {
                    _index = value;
                    if (_isAfterAwake)
                        SyncIfWoken();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            if (!_targetImage.AssertIfNull(nameof(_targetImage)))
                _targetImage.DisableSpriteOptimizations();
            
            _targetButton.AssertIfNull(nameof(_targetButton));
            _buttonColorsSets.IsNullOrEmptyOrHasNullElements(nameof(_buttonColorsSets));

            _isAfterAwake = true;
            SyncIfWoken();
        }


        //=== Private =========================================================

        private void SyncIfWoken()
        {
            var buttonColorsSet = _defaultSet;

            int index = _useFlagNorIndex ? (Flag ? 0 : -1) : Index;
            if (index >= 0 && index < _buttonColorsSets.Length)
            {
                buttonColorsSet = _buttonColorsSets[index];
            }

            if (buttonColorsSet != null)
            {
                _targetButton.targetGraphic = _targetImage;
                buttonColorsSet.SetColorsForButton(_targetButton, _targetImage);
                _targetImage.enabled = true;
            }
            else
            {
                _targetButton.targetGraphic = null;
                _targetImage.sprite = null;
                _targetImage.enabled = false;
            }
        }
    }
}