using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Uins
{
    public class SpritesAnimation : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private Image _image;

        [SerializeField, UsedImplicitly]
        [Range(0.1f, 60)]
        private int _maximumFramerate = 35;

        [SerializeField, UsedImplicitly]
        private Sprite[] _sprites;

        private UpdateInterval _updateInterval = new UpdateInterval();

        private bool _isInited;
        private int _index;

        private void Awake()
        {
            _isInited = !_image.AssertIfNull(nameof(_image)) &&
                        !_sprites.IsNullOrEmptyOrHasNullElements(nameof(_sprites));

            if (!_isInited)
            {
                _image.enabled = false;
                return;
            }

            _updateInterval.Interval = 1f / _maximumFramerate;
            _index = _sprites.Length;
        }

        private void Update()
        {
            if (!_isInited || !_updateInterval.IsItTime())
                return;

            _index = ++_index >= _sprites.Length ? 0 : _index;
            _image.sprite = _sprites[_index];
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (_sprites == null || _sprites.Length == 0)
                return;

            var sortedSprites = _sprites.ToList();
            sortedSprites.Sort(new SpriteComparer());
            _sprites = sortedSprites.ToArray();
#endif
        }

        private class SpriteComparer : IComparer<Sprite>
        {
            public int Compare(Sprite sprite1, Sprite sprite2)
            {
                return string.Compare(sprite1?.name, sprite2?.name, StringComparison.InvariantCulture);
            }
        }
    }
}