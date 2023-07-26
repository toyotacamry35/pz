using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Uins
{
    public class ToggleInvertedTick : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private Toggle _toggle;

        [SerializeField, UsedImplicitly]
        private Image _tickImage;

        [SerializeField, UsedImplicitly]
        private bool _switchGameObjectNorImage = false;


        //=== Unity ===========================================================

        private void Awake()
        {
            if (_toggle.AssertIfNull(nameof(_toggle)) ||
                _tickImage.AssertIfNull(nameof(_tickImage)))
                return;

            _tickImage.DisableSpriteOptimizations();
            _toggle.onValueChanged.AddListener(OnToggleChanged);
            SwitchImage(!_toggle.isOn);
        }

        private void OnToggleChanged(bool isOn)
        {
            SwitchImage(!isOn);
        }

        private void SwitchImage(bool isOn)
        {
            if (_switchGameObjectNorImage)
            {
                _tickImage.gameObject.SetActive(isOn);
            }
            else
            {
                _tickImage.enabled = isOn;
            }
        }
    }
}