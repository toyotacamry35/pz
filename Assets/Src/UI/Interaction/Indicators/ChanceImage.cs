using UnityEngine;
using UnityEngine.UI;

namespace Uins
{
    public class ChanceImage : MonoBehaviour
    {
        private const float SpriteAllChancesSize = 66f;
        private const float MinDisplayedChance = .15f;
        private const float SmallChanceBorder = .25f;
        private const float MediumChanceBorder = .6f;

        [SerializeField]
        private Image _image;

        [SerializeField]
        private ColorSettings _colorSettings;

        private Color _smallChanceColor;
        private Color _mediumChanceColor;
        private Color _bigChanceColor;

        private float _imageSizeCorrectionRatioX;
        private float _imageSizeCorrectionRatioY;
        private float _constantMultiplier;
        private float _maxSquare;


        //=== Unity ===============================================================

        private void Awake()
        {
            if (!_colorSettings.AssertIfNull(nameof(_colorSettings)) && 
                _colorSettings.Colors != null && 
                _colorSettings.Colors.Count >= 3)
            {
                _smallChanceColor = _colorSettings.Colors[0];
                _mediumChanceColor = _colorSettings.Colors[1];
                _bigChanceColor = _colorSettings.Colors[2];
            }
            if (!_image.AssertIfNull(nameof(_image)))
            {
                _imageSizeCorrectionRatioX = _image.rectTransform.sizeDelta.x / SpriteAllChancesSize;
                _imageSizeCorrectionRatioY = _image.rectTransform.sizeDelta.y / SpriteAllChancesSize;
            }
            _constantMultiplier = 2 / Mathf.Sqrt(Mathf.PI);
            _maxSquare = Mathf.PI / 4;
        }


        //=== Public ==============================================================

        /// <summary>
        /// Включает
        /// </summary>
        /// <param name="chance">0...1</param>
        public void SwitchImagesByChance(float chance)
        {
            var chanceColor = _bigChanceColor;
            if (chance < MediumChanceBorder)
                chanceColor = chance < SmallChanceBorder ? _smallChanceColor : _mediumChanceColor;

            _image.enabled = true;
            _image.color = chanceColor;
            chance = Mathf.Max(chance, MinDisplayedChance);
            var size = SpriteAllChancesSize * _constantMultiplier * Mathf.Sqrt(chance * _maxSquare);
            _image.rectTransform.sizeDelta = new Vector2(size * _imageSizeCorrectionRatioX, size * _imageSizeCorrectionRatioY);
        }

        public void Hide()
        {
            _image.enabled = false;
        }
    }
}