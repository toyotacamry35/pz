using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Uins
{
    public class MapGridSetter : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private Image[] _horizontalLines;

        [SerializeField, UsedImplicitly]
        private Sprite _horizontalLineSprite;

        [SerializeField, UsedImplicitly]
        private Image[] _verticalLines;

        [SerializeField, UsedImplicitly]
        private Sprite _verticalLineSprite;

        [SerializeField, UsedImplicitly]
        private Transform _verticalRulerTransform;

        [SerializeField, UsedImplicitly]
        private Transform _horizontalRulerTransform;

        [SerializeField, UsedImplicitly]
        private CellIndex _verticalCellIndexPrefab;

        [SerializeField, UsedImplicitly]
        private CellIndex _horizontalCellIndexPrefab;


        //=== Private =========================================================

        public void SetGridPositions()
        {
            if (_verticalRulerTransform.AssertIfNull(nameof(_verticalRulerTransform)) ||
                _horizontalRulerTransform.AssertIfNull(nameof(_horizontalRulerTransform)) ||
                _verticalCellIndexPrefab.AssertIfNull(nameof(_verticalCellIndexPrefab)) ||
                _horizontalCellIndexPrefab.AssertIfNull(nameof(_horizontalCellIndexPrefab)) ||
                _horizontalLineSprite.AssertIfNull(nameof(_horizontalLineSprite)) ||
                _verticalLineSprite.AssertIfNull(nameof(_verticalLineSprite)) ||
                _horizontalLines.IsNullOrEmptyOrHasNullElements(nameof(_horizontalLines)) ||
                _verticalLines.IsNullOrEmptyOrHasNullElements(nameof(_verticalLines)))
                return;

            Debug.Log($"Set grid positions: {_horizontalLines.Length + 1}x{_verticalLines.Length + 1}");
            // SetGrid();
            SetRulers();
        }

        // private void SetGrid()
        // {
        //     //выставляем имеющиеся горизонтальные линии
        //     var imageHeightOrWidth = _horizontalLineSprite.texture?.height ?? 1;
        //     var anchorPosDelta = 1 / (float) (_horizontalLines.Length + 1);
        //     for (int i = 0; i < _horizontalLines.Length; i++)
        //     {
        //         var rectTransform = _horizontalLines[i].rectTransform;
        //         _horizontalLines[i].sprite = _horizontalLineSprite;
        //         var ratio = (i + 1) * anchorPosDelta;
        //         rectTransform.pivot = new Vector2(0.5f, 0.5f);
        //         rectTransform.anchorMin = new Vector2(0, ratio);
        //         rectTransform.anchorMax = new Vector2(1, ratio);
        //         rectTransform.anchoredPosition = Vector2.zero;
        //         rectTransform.sizeDelta = new Vector2(0, imageHeightOrWidth);
        //     }
        //
        //     //выставляем имеющиеся вертикальные линии
        //     imageHeightOrWidth = _verticalLineSprite.texture?.width ?? 1;
        //     anchorPosDelta = 1 / (float) (_verticalLines.Length + 1);
        //     for (int i = 0; i < _verticalLines.Length; i++)
        //     {
        //         var rectTransform = _verticalLines[i].rectTransform;
        //         _verticalLines[i].sprite = _verticalLineSprite;
        //         var ratio = (i + 1) * anchorPosDelta;
        //         rectTransform.pivot = new Vector2(0.5f, 0.5f);
        //         rectTransform.anchorMin = new Vector2(ratio, 0);
        //         rectTransform.anchorMax = new Vector2(ratio, 1);
        //         rectTransform.anchoredPosition = Vector2.zero;
        //         rectTransform.sizeDelta = new Vector2(imageHeightOrWidth, 0);
        //     }
        // }

        private void SetRulers()
        {
            //для вертикального рулера
            DeleteOldInstances(_verticalRulerTransform);
            var cellCount = _horizontalLines.Length + 1;
            var anchorPosDelta = 1 / (float) cellCount;
            for (int i = 0; i < cellCount; i++)
            {
                var verticalCellIndex = Instantiate(_verticalCellIndexPrefab, _verticalRulerTransform);
                verticalCellIndex.name = _verticalCellIndexPrefab.name + (i + 1);
                verticalCellIndex.Title.text = GetAlphabetSymbols(i);
                var anchorRatio = (0.5f + i) * anchorPosDelta;
                var rectTransform = verticalCellIndex.transform.GetRectTransform();
                rectTransform.anchorMin = new Vector2(0, anchorRatio);
                rectTransform.anchorMax = new Vector2(1, anchorRatio);
                rectTransform.anchoredPosition = Vector2.zero;
            }

            //для горизонтального рулера
            DeleteOldInstances(_horizontalRulerTransform);
            cellCount = _verticalLines.Length + 1;
            anchorPosDelta = 1 / (float) cellCount;
            for (int i = 0; i < cellCount; i++)
            {
                var horizontalCellIndex = Instantiate(_horizontalCellIndexPrefab, _horizontalRulerTransform);
                horizontalCellIndex.name = _horizontalCellIndexPrefab.name + (i + 1);
                horizontalCellIndex.Title.text = (i + 1).ToString();
                var anchorRatio = (0.5f + i) * anchorPosDelta;
                var rectTransform = horizontalCellIndex.transform.GetRectTransform();
                rectTransform.anchorMin = new Vector2(anchorRatio, 0);
                rectTransform.anchorMax = new Vector2(anchorRatio, 1);
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }

        private void DeleteOldInstances(Transform target)
        {
            var children = target.GetComponentsInChildren<Transform>();
            foreach (var child in children)
            {
                if (child == target)
                    continue;

                DestroyImmediate(child.gameObject);
            }
        }

        private string GetAlphabetSymbols(int i)
        {
            var str = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray().Select(ch => ch.ToString()).ToArray();
            return i > 25 ? "A" + str[i] : str[i];
        }
    }
}