using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class SpriteFromSlider : BindingViewModel
    {
        private const int SpritesLength = 4;

        [SerializeField, UsedImplicitly]  private float _sprite2Border = 0.33f;
        [SerializeField, UsedImplicitly]  private float _sprite3Border = 0.66f;
        [SerializeField, UsedImplicitly]  private Sprite[] _sprites;
        [SerializeField, UsedImplicitly]  private Slider _slider;

        private ReactiveProperty<Sprite> _spriteRp = new ReactiveProperty<Sprite>();


        //=== Props ===========================================================

        [Binding]
        public Sprite Sprite { get; private set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            if (_sprites == null || _sprites.Length != SpritesLength)
            {
                UI.Logger.IfError()?.Message($"{nameof(_sprites)} is empty or its length don't corresponds {SpritesLength}", gameObject).Write();
                return;
            }

            if (_slider.AssertIfNull(nameof(_slider)))
                return;

            Bind(_spriteRp, () => Sprite);
            OnSliderValueChanged(_slider.value);
            _slider.onValueChanged.AddListener(OnSliderValueChanged);
        }


        //=== Private =========================================================

        private void OnSliderValueChanged(float val)
        {
            if (Mathf.Approximately(0, val))
            {
                _spriteRp.Value = _sprites[0];
            }
            else
            {
                _spriteRp.Value = val < _sprite2Border
                    ? _sprites[1]
                    : val < _sprite3Border
                        ? _sprites[2]
                        : _sprites[3];
            }
        }
    }
}