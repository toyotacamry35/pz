using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class WatermarkScreen : BindingViewModel
    {
        private const int RandomPositionsCount = 100;
        private const string ConstantTextPart = "Betatest ";

        [SerializeField, UsedImplicitly]
        private Watermark _watermarkPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _watermarksRoot;

        [SerializeField, UsedImplicitly]
        private int _columnCount = 3;

        [SerializeField, UsedImplicitly]
        private int _rowCount = 3;

        [SerializeField, UsedImplicitly]
        private bool _useOnlyOdd;

        [SerializeField, UsedImplicitly]
        private GridLayoutGroup _gridLayoutGroup;

        [SerializeField, UsedImplicitly]
        private UpdateInterval _moveUpdateInterval;

        [SerializeField, UsedImplicitly]
        private float _moveDelta;

        [SerializeField, UsedImplicitly]
        private Color _textColor;

        [SerializeField, UsedImplicitly]
        private UpdateInterval _updateInterval;

        private List<Watermark> _watermarks = new List<Watermark>();
        private string _text = "";
        private string _text2 = "";
        private Sprite _sprite = null;
        private RectTransform _rootRectTransform;
        private Vector2[] _randomPositions = new Vector2[RandomPositionsCount];
        private int _randomPosIndex = -1;
        private int _randomPosIndexDelta = 1;
        private int _lastScreenWidth, _lastScreenHeight;


        //=== Props ===========================================================

        public static WatermarkScreen Instance { get; private set; }

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
                    if (_isVisible && _watermarks.Count == 0)
                        CreateWatermarks();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            Instance = SingletonOps.TrySetInstance(this, Instance);

            _watermarksRoot.AssertIfNull(nameof(_watermarksRoot));
            _watermarkPrefab.AssertIfNull(nameof(_watermarkPrefab));

            SetDefaultParams();
            _rootRectTransform = _watermarksRoot.GetRectTransform();
            CheckGridSize();

            IsVisible = GameState.Instance?.GameOptions?.ShowWatermarks ?? false;

            _moveDelta = Mathf.Abs(_moveDelta / 2);
            Vector2 prev = Vector2.zero;
            for (int i = 0; i < RandomPositionsCount; i++)
            {
                _randomPositions[i] = prev + new Vector2(Random.Range(-_moveDelta, _moveDelta), Random.Range(-_moveDelta, _moveDelta));
                prev = _randomPositions[i];
            }
        }

        private void Update()
        {
            if (!_updateInterval.IsItTime())
                return;

            IsVisible = GameState.Instance?.GameOptions?.ShowWatermarks ?? false;
            if (!IsVisible)
                return;

            if (_moveUpdateInterval.IsItTime())
            {
                CheckGridSize();
                _rootRectTransform.anchoredPosition = _randomPositions[GetNextRandomIndex()];
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Instance == this)
                Instance = null;
        }

        private void OnEnable()
        {
            UpdateTextColor();
        }


        //=== Public ==========================================================

        public void SetDefaultParams()
        {
            SetParams("w/o login");
        }

        public void SetParams(string text, string text2 = null, Sprite sprite = null)
        {
            _sprite = sprite;
            _text = ConstantTextPart + text;
            _text2 = text2;
            if (_watermarks.Count > 0)
                foreach (var watermark in _watermarks)
                {
                    watermark.Text = _text;
                    watermark.Text2 = _text2;
                    watermark.Sprite = _sprite;
                }
        }


        //=== Private =========================================================

        private void CheckGridSize()
        {
            var newScreenWidth = Mathf.Max(Screen.width, 800);
            var newScreenHeight = Mathf.Max(Screen.height, 600);
            if (_lastScreenWidth == newScreenWidth && _lastScreenHeight == newScreenHeight)
                return;

            _lastScreenHeight = newScreenHeight;
            _lastScreenWidth = newScreenWidth;

            _rootRectTransform.sizeDelta = new Vector2(_lastScreenWidth, _lastScreenHeight);
            var windowWidth = _rootRectTransform.rect.width;
            var windowHeight = _rootRectTransform.rect.height;
            if (!_gridLayoutGroup.AssertIfNull(nameof(_gridLayoutGroup)))
                _gridLayoutGroup.cellSize = new Vector2(windowWidth / _columnCount, windowHeight / _rowCount);
        }

        private void CreateWatermarks()
        {
            for (int i = 0, len = _columnCount * _rowCount; i < len; i++)
            {
                var watermark = Instantiate(_watermarkPrefab, _watermarksRoot);
                watermark.name = _watermarkPrefab.name + _watermarks.Count;
                watermark.Text = _text;
                watermark.Sprite = _sprite;
                watermark.TextColor = _textColor;
                watermark.IsVisible = !(_useOnlyOdd && i % 2 != 0);
                _watermarks.Add(watermark);
            }
        }

        private int GetNextRandomIndex()
        {
            _randomPosIndex += _randomPosIndexDelta;

            if (_randomPosIndex < 0)
            {
                _randomPosIndex = 1;
                _randomPosIndexDelta = 1;
            }
            else
            {
                if (_randomPosIndex >= RandomPositionsCount)
                {
                    _randomPosIndexDelta = -1;
                    _randomPosIndex = RandomPositionsCount - 2;
                }
            }

            return _randomPosIndex;
        }

        private void UpdateTextColor()
        {
            foreach (var watermark in _watermarks)
                watermark.TextColor = _textColor;
        }
    }
}