using System;
using Assets.Src.ResourceSystem;
using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class CreditsContr : BindingController<CreditsDef>
    {
        private const int DefaultShowTime = 3;
        private const float OutOfScreenHeightRatio = 1.2f;

        [SerializeField, UsedImplicitly]
        private bool _isPreviewMode;

        [SerializeField, UsedImplicitly]
        private Transform _movingTransform;

        [SerializeField, UsedImplicitly]
        private CreditsDefRefCreditsDefRef _creditsDefRef;

        [SerializeField, UsedImplicitly]
        private DivisionBlockContr _divisionBlockContrPrefab;

        [SerializeField, UsedImplicitly]
        private LabelBlockContr _labelBlockContrPrefab;

        [SerializeField, UsedImplicitly]
        private ImageBlockContr _imageBlockContrPrefab;

        [SerializeField, UsedImplicitly]
        private HotkeyListener _escListener;

        private Vector2 _moveSpeedV2;
        private RectTransform _movingRectTransform;
        private float _maxPosition;


        //=== Props ===========================================================

        [Binding]
        public bool IsVisible { get; private set; }

        public ReactiveProperty<bool> IsVisibleRp { get; } = new ReactiveProperty<bool>() {Value = false};

        public bool IsEnded => _movingRectTransform.anchoredPosition.y > _maxPosition;


        //=== Unity ===========================================================

        private void Start()
        {
            if (_movingTransform.AssertIfNull(nameof(_movingTransform)) ||
                _creditsDefRef.Target.AssertIfNull(nameof(_creditsDefRef)) ||
                _divisionBlockContrPrefab.AssertIfNull(nameof(_divisionBlockContrPrefab)) ||
                _labelBlockContrPrefab.AssertIfNull(nameof(_labelBlockContrPrefab)) ||
                _imageBlockContrPrefab.AssertIfNull(nameof(_imageBlockContrPrefab)) ||
                _escListener.AssertIfNull(nameof(_escListener)))
                return;

            _movingRectTransform = _movingTransform.GetRectTransform();

            Vmodel.Action(D, OnVmodelChanged);
            Bind(IsVisibleRp, () => IsVisible);
            Vmodel.Func(D, def => def != null).Bind(D, IsVisibleRp);
        }

        private void Update()
        {
            if (!IsVisible || _isPreviewMode)
                return;

            DoMoving(Time.deltaTime);

            if (_escListener.IsFired() || IsEnded)
                Vmodel.Value = null;
        }


        //=== Public ==========================================================

        [UsedImplicitly]
        public void OnShowCredits()
        {
            Vmodel.Value = _creditsDefRef.Target;
        }


        //=== Private =========================================================

        private void OnVmodelChanged(CreditsDef creditsDef)
        {
            _movingTransform.DestroyAllChildren();
            _movingRectTransform.anchoredPosition = Vector2.zero;
            if (creditsDef?.Credits == null)
                return;

            float offset = 0;
            //var startDt = DateTime.Now;
            var credits = creditsDef.Credits;
            for (int i = 0; i < credits.Length; i++)
            {
                var creditsBlock = credits[i].Target;
                if (creditsBlock == null)
                {
                    UI.Logger.Error($"Null {i} element in {creditsDef}");
                    continue;
                }

                var creditsBlockContrPrefab = GetCreditsBlockContrPrefab(creditsBlock);
                if (creditsBlockContrPrefab == null)
                {
                    UI.Logger.Error($"{creditsDef} [{i}] Unhandled block type: {creditsBlock.GetType()}");
                    continue;
                }

                var creditsBlockContr = Instantiate(creditsBlockContrPrefab, _movingTransform);
                SetCreditsBlockContrVmodel(creditsBlockContr, creditsBlock);
                var rectTransform = creditsBlockContr.transform.GetRectTransform();
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
                rectTransform.anchoredPosition = new Vector2(0, offset);

                offset -= rectTransform.rect.height;
            }

            _maxPosition = _movingTransform.parent.GetRectTransform().rect.height * OutOfScreenHeightRatio - offset;
            //UI.CallerLog($"Creation time: {(DateTime.Now - startDt).TotalSeconds:f2}, offset {offset}, maxPosition {_maxPosition}"); //DEBUG

            var showTime = _creditsDefRef.Target.ShowTimeInSeconds;
            if (showTime < 1)
            {
                showTime = DefaultShowTime;
                UI.Logger.Error($"Wrong {nameof(_creditsDefRef.Target.ShowTimeInSeconds)} value. Corrected to {showTime}");
            }

            var moveSpeed = _maxPosition / showTime;
            //UI.CallerLog($"{nameof(moveSpeed)}={moveSpeed}"); //DEBUG
            _moveSpeedV2 = new Vector2(0, moveSpeed);
        }

        private void SetCreditsBlockContrVmodel(MonoBehaviour monoBehaviour, CreditsBlock creditsBlock)
        {
            if (monoBehaviour.AssertIfNull(nameof(monoBehaviour)) ||
                creditsBlock.AssertIfNull(nameof(creditsBlock)))
                return;

            if (creditsBlock is DivisionBlock divisionBlock && monoBehaviour is DivisionBlockContr divisionBlockContr)
                divisionBlockContr.SetVmodel(divisionBlock);
            else if (creditsBlock is LabelBlock labelBlock && monoBehaviour is LabelBlockContr labelBlockContr)
                labelBlockContr.SetVmodel(labelBlock);
            else if (creditsBlock is ImageBlock imageBlock && monoBehaviour is ImageBlockContr imageBlockContr)
                imageBlockContr.SetVmodel(imageBlock);
            else
                UI.Logger.Error($"Inconsistent types: {monoBehaviour.GetType().NiceName()} AND {creditsBlock.GetType().NiceName()}");
        }

        private MonoBehaviour GetCreditsBlockContrPrefab(CreditsBlock creditsBlock)
        {
            if (creditsBlock is DivisionBlock)
                return _divisionBlockContrPrefab;

            if (creditsBlock is LabelBlock)
                return _labelBlockContrPrefab;

            if (creditsBlock is ImageBlock)
                return _imageBlockContrPrefab;

            return null;
        }

        private void DoMoving(float deltaTime)
        {
            _movingRectTransform.anchoredPosition += _moveSpeedV2 * deltaTime;
        }
    }
}