using Core.Environment.Logging.Extension;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

namespace Uins.Tooltips
{
    public class TooltipSelector : MonoBehaviour
    {
        [UsedImplicitly]
        [SerializeField]
        private float _tooltipsAlpha = 0.7f;

        [UsedImplicitly]
        [SerializeField]
        private float _delayBeforeShow = 0.2f;

        [UsedImplicitly]
        [SerializeField]
        private float _fadeTime = 0.2f;

        [UsedImplicitly]
        [SerializeField]
        private BaseTooltip _textTooltipPrefab;

        [UsedImplicitly]
        [SerializeField]
        private BaseTooltip _slotTooltipPrefab;

        [UsedImplicitly]
        [SerializeField]
        private BaseTooltip _craftElemTooltipPrefab;

        [UsedImplicitly]
        [SerializeField]
        private BaseTooltip _machineRecipeTooltipPrefab;

        [UsedImplicitly]
        [SerializeField]
        private BaseTooltip _technoContextConstrTooltipPrefab;

        [UsedImplicitly]
        [SerializeField]
        private BaseTooltip _machineRecipeDetailsSlotTooltipPrefab;

        [UsedImplicitly]
        [SerializeField]
        private CanvasGroup _mainCanvasGroup;

        private BaseTooltip _textTooltip;
        private BaseTooltip _slotTooltip;
        private BaseTooltip _craftElemTooltip;
        private BaseTooltip _machineRecipeTooltip;
        private BaseTooltip _technoContextConstrTooltip;
        private BaseTooltip _machineRecipeDetailsSlotTooltip;

        private State _tooltipState;
        private MonoBehaviour _currentItem;
        private MonoBehaviour _nextItem;
        private BaseTooltip _currentTooltip;
        private Tweener _tooltipAlphaTweener;

        private float _lastClickTime;


        //=== Enums ===============================================================

        private enum State
        {
            Hidden,
            HiddenWaitForShow,
            FadeIn,
            Shown,
            FadeOut,
        }


        //=== Props ===============================================================

        public static TooltipSelector Instance { get; private set; }

        private bool ShouldBeVisible
        {
            get
            {
                bool isMouseButtonPressed = Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1);
                if (isMouseButtonPressed)
                    _lastClickTime = Time.time;
                return !(isMouseButtonPressed || Time.time < _lastClickTime + _fadeTime);
            }
        }

        private bool _isVisible;

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (value != _isVisible)
                {
                    _isVisible = value;
                    OnSwitchVisibility(_isVisible);
                }
            }
        }


        //=== Unity ===============================================================

        private void Awake()
        {
            _mainCanvasGroup.AssertIfNull(nameof(_mainCanvasGroup));

            _textTooltipPrefab.AssertIfNull(nameof(_textTooltipPrefab));
            _slotTooltipPrefab.AssertIfNull(nameof(_slotTooltipPrefab));
            _craftElemTooltipPrefab.AssertIfNull(nameof(_craftElemTooltipPrefab));
            _machineRecipeTooltipPrefab.AssertIfNull(nameof(_machineRecipeTooltipPrefab));
            _technoContextConstrTooltipPrefab.AssertIfNull(nameof(_technoContextConstrTooltipPrefab));
            _machineRecipeDetailsSlotTooltipPrefab.AssertIfNull(nameof(_machineRecipeDetailsSlotTooltipPrefab));

            Instance = SingletonOps.TrySetInstance(this, Instance);
            CreateTooltipInstances();
            SetHiddenState();
        }

        private void Start()
        {
            _textTooltip.MoveOut();
        }

        private void Update()
        {
            IsVisible = ShouldBeVisible;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }


        //=== Public ==============================================================

        public void ShowTooltip(MonoBehaviour monoBehaviour = null)
        {
            bool isEnter = monoBehaviour != null;

            switch (_tooltipState)
            {
                case State.Hidden:
                    if (isEnter && ShouldBeVisible)
                    {
                        _currentTooltip = GetTooltipForItem(monoBehaviour);
                        if (_currentTooltip == null)
                        {
                            UI.Logger.IfError()?.Message($"ShowTooltip() Unable to get TooltipForItem({monoBehaviour.GetType().NiceName()})").Write();
                            SetHiddenState();
                            return;
                        }

                        StartWaitForShow(monoBehaviour);
                    }

                    break;

                case State.HiddenWaitForShow:
                    CancelInvoke(nameof(AfterWait));
                    if (isEnter && ShouldBeVisible)
                    {
                        _currentTooltip = GetTooltipForItem(monoBehaviour);
                        if (_currentTooltip == null)
                        {
                            UI.Logger.IfError()?.Message($"ShowTooltip() Unable to get TooltipForItem({monoBehaviour.GetType().NiceName()})").Write();
                            SetHiddenState();
                            return;
                        }

                        StartWaitForShow(monoBehaviour);
                    }
                    else
                    {
                        SetHiddenState();
                    }

                    break;

                case State.FadeIn:
                case State.Shown:
                    StartFadeOut(isEnter && ShouldBeVisible ? monoBehaviour : null);
                    break;

                case State.FadeOut:
                    _nextItem = monoBehaviour;
                    break;

                default:
                    UI.Logger.IfError()?.Message($"ShowTooltip() Unhandled state {_tooltipState}").Write();
                    break;
            }
        }


        //=== Private =============================================================

        private void CreateTooltipInstances()
        {
            if (_mainCanvasGroup == null)
                return;

            _textTooltip = GetTooltipInstance(_textTooltipPrefab, _mainCanvasGroup?.transform);
            _slotTooltip = GetTooltipInstance(_slotTooltipPrefab, _mainCanvasGroup?.transform);
            _craftElemTooltip = GetTooltipInstance(_craftElemTooltipPrefab, _mainCanvasGroup?.transform);
            _machineRecipeTooltip = GetTooltipInstance(_machineRecipeTooltipPrefab, _mainCanvasGroup?.transform);
            _technoContextConstrTooltip = GetTooltipInstance(_technoContextConstrTooltipPrefab, _mainCanvasGroup?.transform);
            _machineRecipeDetailsSlotTooltip = GetTooltipInstance(_machineRecipeDetailsSlotTooltipPrefab, _mainCanvasGroup?.transform);
        }

        private T GetTooltipInstance<T>(T tooltipPrefab, Transform parentTransform) where T : BaseTooltip
        {
            return tooltipPrefab != null && parentTransform != null ? Instantiate(tooltipPrefab, parentTransform) : null;
        }

        private BaseTooltip GetTooltipForItem(MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour.AssertIfNull(nameof(monoBehaviour)))
                return null;


            var tooltipDescription = monoBehaviour as ITooltipDescription;
            if (!tooltipDescription.AssertIfNull(nameof(tooltipDescription)))
            {
                if (tooltipDescription is SlotTooltipDescription)
                    return _slotTooltip;

                if (tooltipDescription is CraftElemTooltipDescription)
                    return _craftElemTooltip;

                if (tooltipDescription is TechnoContextCraftRecipeTooltipDescr)
                    return _machineRecipeTooltip;

                if (tooltipDescription is TechnosSideContrTooltipDescr)
                    return _technoContextConstrTooltip;

                if (tooltipDescription is MachineRecipeDetailsSlotTooltipDescription)
                    return _machineRecipeDetailsSlotTooltip;

                return _textTooltip;
            }

            UI.Logger.IfDebug()?.Message($"{nameof(GetTooltipForItem)}() Unhandled type: {monoBehaviour.GetType().NiceName()}").Write();
            return null;
        }

        private void OnSwitchVisibility(bool isVisible)
        {
            _mainCanvasGroup.alpha = isVisible ? _tooltipsAlpha : 0;

            if (isVisible || _tooltipState == State.Hidden || _tooltipState == State.FadeOut)
                return;

            ShowTooltip();
        }

        private void StartWaitForShow(MonoBehaviour monoBehaviour)
        {
            _currentItem = monoBehaviour;
            _nextItem = null;
            _tooltipState = State.HiddenWaitForShow;
            Invoke(nameof(AfterWait), _delayBeforeShow);
        }

        private void AfterWait()
        {
            if (_tooltipState != State.HiddenWaitForShow)
            {
                UI.Logger.IfError()?.Message($"{nameof(AfterWait)}() Unexpected {nameof(_tooltipState)}={_tooltipState}").Write();
                Reset();
                return;
            }

            if (!ShouldBeVisible)
            {
                SetHiddenState();
                return;
            }

            StartFadeIn();
        }

        /// <summary>
        /// Плавное появление тултипа
        /// </summary>
        private void StartFadeIn()
        {
            if (_currentTooltip.AssertIfNull(nameof(_currentTooltip)) ||
                _currentItem.AssertIfNull(nameof(_currentItem)))
            {
                Reset();
                return;
            }

            _nextItem = null;
            _tooltipState = State.FadeIn;
            _currentTooltip.Setup(_currentItem);
            _tooltipAlphaTweener.KillIfExistsAndActive();
            _tooltipAlphaTweener = _currentTooltip.CanvasGroup.TweenAlpha(
                1, GetFadeTime(_currentTooltip.CanvasGroup.alpha, true), AfterFadeIn);
        }

        private void AfterFadeIn()
        {
            if (_tooltipState != State.FadeIn)
            {
                UI.Logger.IfError()?.Message($"AfterFadeIn() Unexpected _tooltipState={_tooltipState}").Write();
                Reset();
                return;
            }

            if (!ShouldBeVisible)
            {
                StartFadeOut(_nextItem);
                return;
            }

            _tooltipState = State.Shown;
        }

        /// <summary>
        /// Плавное исчезновение тултипа
        /// </summary>
        private void StartFadeOut(MonoBehaviour nextMonoBehaviour = null)
        {
            if (_currentTooltip.AssertIfNull(nameof(_currentTooltip)) ||
                _currentItem.AssertIfNull(nameof(_currentItem)))
            {
                Reset();
                return;
            }

            _nextItem = nextMonoBehaviour;
            if (_tooltipState == State.FadeOut)
                return;

            _tooltipState = State.FadeOut;
            _tooltipAlphaTweener.KillIfExistsAndActive();
            _tooltipAlphaTweener = _currentTooltip.CanvasGroup.TweenAlpha(
                0, GetFadeTime(_currentTooltip.CanvasGroup.alpha, false), AfterFadeOut);
        }

        private void AfterFadeOut()
        {
            if (_tooltipState != State.FadeOut)
            {
                UI.Logger.IfError()?.Message($"AfterFadeOut() Unexpected _tooltipState={_tooltipState}").Write();
                Reset();
                return;
            }

            if (_nextItem == null || !ShouldBeVisible)
            {
                _currentTooltip.MoveOut();
                SetHiddenState();
                return;
            }

            _currentItem = _nextItem;
            _nextItem = null;
            var newCurrentTooltip = GetTooltipForItem(_currentItem);
            if (newCurrentTooltip != _currentTooltip)
            {
                _currentTooltip.MoveOut();
                _currentTooltip = newCurrentTooltip;
            }

            if (_currentTooltip == null)
            {
                UI.Logger.IfError()?.Message($"AfterFadeOut() Unable to get TooltipForItem({_currentItem.GetType().NiceName()})").Write();
                SetHiddenState();
                return;
            }

            StartFadeIn();
        }

        private void Reset()
        {
            _tooltipAlphaTweener.KillIfExistsAndActive();
            if (_currentTooltip != null)
                _currentTooltip.MoveOut();
            SetHiddenState();
        }

        private void SetHiddenState()
        {
            _currentItem = null;
            _nextItem = null;
            _currentTooltip = null;
            _tooltipState = State.Hidden;
        }

        private float GetFadeTime(float fromAlpha, bool isFadeIn)
        {
            return isFadeIn
                ? (1 - fromAlpha) * _fadeTime
                : fromAlpha * _fadeTime;
        }
    }
}