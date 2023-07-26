using System;
using Assets.ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using Src.Locomotion;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable IdentifierTypo

namespace Uins
{
    /// <summary>
    /// Assumptions: - All children has same width
    /// </summary>
    // Reference code: http://answers.unity.com/answers/894546/view.html, https://bitbucket.org/ddreaper/unity-ui-extensions/src/6928c4428fb3392f0e9735df44aafee3b347933c/Scripts/HorizontalScrollSnap.cs?at=default
    //[RequireComponent(typeof(ScrollRect))]
    public class HorizontalScrollSnap : MonoBehaviour//, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        ///#Dbg:
        private const bool Dbg = false;

        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        [SerializeField, UsedImplicitly]  private Button _nextButton;
        [SerializeField, UsedImplicitly]  private Button _prevButton;
        // Should new pos.be set instantly or use lerp to set it
        [SerializeField, UsedImplicitly]  private bool _useLerp;
        // Root of all elems & provider of total width:
        [SerializeField, UsedImplicitly]  private RectTransform _content;
        // Used to get spacing between elems:
        [SerializeField, UsedImplicitly]  private HorizontalLayoutGroup _horithLayoutGroup;
        // Frame by mask defines the size of "window" through witch we see subset of elems. Used to get width of this "window": 
        [SerializeField, UsedImplicitly]  private RectTransform _viewPort;

        private bool _lerp;

        private bool _isNextScreenAvailable;
        private bool _isPrevScreenAvailable;

        private const int PosTollerance = 1; //pixel
        private float _elemWidth;
        private float _spacing;
        private float ElemTotalWidth => _elemWidth + _spacing;
        private float ViewPortWidth => _viewPort.rect.width;
        private float ContentWidth => _content.rect.width;

        // 1st elem is visible (right-most pos)
        private float _anchoredPosMin;
        // last elem is visible (left-most pos)
        private float _anchoredPosMax;
        private float AnchoredPosCurr 
        {
            get => _content.anchoredPosition.x;
            set => _content.anchoredPosition = new Vector2(value, _content.anchoredPosition.y);
        }
        private float TargetAnchoredPosCurr => _lerpTargetAnchPos ?? AnchoredPosCurr;

        private float? _lerpTargetAnchPos;

        [UsedImplicitly] //U
        private void Awake()
        {
            _content.AssertIfNull(nameof(_content));
            _horithLayoutGroup.AssertIfNull(nameof(_horithLayoutGroup));
            _viewPort.AssertIfNull(nameof(_viewPort));
            _nextButton.AssertIfNull(nameof(_nextButton));
            _prevButton.AssertIfNull(nameof(_prevButton));
                
            _spacing = _horithLayoutGroup.spacing;

            _nextButton.onClick.AddListener(() => { NextScreen(); });
            _prevButton.onClick.AddListener(() => { PreviousScreen(); });

        }

        public void Recalc()
        {
            // if empty:
            if (_content.childCount < 1)
                return;

            var childRect = _content.GetChild(0).GetComponent<RectTransform>();
            if (childRect == null)
            {
                 Logger.IfError()?.Message("All children of content are considered as elements of scroll-list & should has RectTransform component!").Write();;
                return;
            }

            _elemWidth = childRect.rect.width;

            _anchoredPosMax = _content.anchoredPosition.x - _content.localPosition.x;
            _anchoredPosMin = _anchoredPosMax - (ContentWidth - ViewPortWidth);

            RecalcIsPrevAndNextScreensAvailable();

            if (Dbg && DbgLog.Enabled) DbgLog.Log(      $"##scrol.  Recalced: _elemWidth:{_elemWidth}, _viewPortW:{ViewPortWidth}, _contentW:{ContentWidth}, _aPMin:{_anchoredPosMax}, _aPMax:{_anchoredPosMin}, aPCurr:{AnchoredPosCurr}");
        }

        private float _anchoredPosLastFrame;
        [UsedImplicitly] //U
        void Update()
        {
            //#Tmp solution: вместо этого нужно прикрутить понимание, что скрол-лист таскают не кнопками, а ещё как-то (мышкой или колёсиком) 
            // и пересчитывать в этом случае
            if (!AnchoredPosCurr.ApproximatelyEqual(_anchoredPosLastFrame, PosTollerance))
            {
                RecalcIsPrevAndNextScreensAvailable();
                _anchoredPosLastFrame = AnchoredPosCurr;
            }

            //if (AnchoredPosCurr.ApproximatelyEqual(_lerpTargetAnchPos.Value, PosTollerance))
            //    return;

            if (_lerpTargetAnchPos.HasValue)
            { 
                if (_useLerp) // A) Lerp to new val.:
                {
                    if (_lerp)
                    {
                        AnchoredPosCurr = Mathf.Lerp(AnchoredPosCurr, _lerpTargetAnchPos.Value, 7.5f * Time.deltaTime);
                        if (Mathf.Abs(AnchoredPosCurr - _lerpTargetAnchPos.Value) < PosTollerance)
                        {
                            AnchoredPosCurr = _lerpTargetAnchPos.Value;
                            _lerp = false;
                            _lerpTargetAnchPos = null;
                            RecalcIsPrevAndNextScreensAvailable();
                        }
                    }
                }
                else // B) Set new val instantly:
                {
                    AnchoredPosCurr = _lerpTargetAnchPos.Value;
                    _lerpTargetAnchPos = null;
                    RecalcIsPrevAndNextScreensAvailable();
                }
            }
        }

        float CalcTargetAnchPos(bool nxtOrPrev)
        {
            float result;
            if (nxtOrPrev) //Next
            {
                var rightConsole = _anchoredPosMin - TargetAnchoredPosCurr;
                float targetConsole = CalcTargetAnchPos_SubCalcTargetConsoleByConsole(rightConsole);
                result = _anchoredPosMin + targetConsole;
            }
            else // Prev
            {
                var leftConsole = TargetAnchoredPosCurr - _anchoredPosMax;
                float targetConsole = CalcTargetAnchPos_SubCalcTargetConsoleByConsole(leftConsole);
                result = _anchoredPosMax - targetConsole;
            }
            return Mathf.Clamp(result, _anchoredPosMin, _anchoredPosMax);
        }
        float CalcTargetAnchPos_SubCalcTargetConsoleByConsole(float console)
        {
            var elemsInConsoleAbs = Math.Abs(console / ElemTotalWidth);
            var closestInt = Convert.ToInt32(elemsInConsoleAbs);
            if ((elemsInConsoleAbs - closestInt)*ElemTotalWidth > PosTollerance) // So elem is showed, but partly & we should show it fully
                 return ((int)elemsInConsoleAbs) * ElemTotalWidth;
            else // needed elem is totally hidden.- so we should just shift by 1-elem-width + spacing:
                return (closestInt - 1) * ElemTotalWidth;
        }

        //Function for switching screens with buttons
        public void NextScreen()
        {
            //Recalc();

            if (!_isNextScreenAvailable)
            {
                _lerpTargetAnchPos = _anchoredPosMin; //just for safe
                return;
            }

            _lerp = true;
            _lerpTargetAnchPos = CalcTargetAnchPos(true);

            if (Dbg && DbgLog.Enabled) DbgLog.Log($"##scrol.  NextScreen: aPCurr:{AnchoredPosCurr} --> _lerpTargetAnchPos:{_lerpTargetAnchPos}" +
                                                  $"\n_elemWidth:{_elemWidth}, _viewPortW:{ViewPortWidth}, _contentW:{ContentWidth}, _aPMin:{_anchoredPosMax}, _aPMax:{_anchoredPosMin}.");
        }

        private void RecalcIsPrevAndNextScreensAvailable()
        {
            _isNextScreenAvailable = Math.Abs(_anchoredPosMin - TargetAnchoredPosCurr) > PosTollerance;
            _isPrevScreenAvailable = Math.Abs(_anchoredPosMax - TargetAnchoredPosCurr) > PosTollerance;
            _nextButton.interactable = _isNextScreenAvailable;
            _prevButton.interactable = _isPrevScreenAvailable;
        }

        //Function for switching screens with buttons
        public void PreviousScreen()
        {
            //Recalc();

            if (!_isPrevScreenAvailable)
            {
                _lerpTargetAnchPos = _anchoredPosMax; //just for safe
                return;
            }

            _lerp = true;
            _lerpTargetAnchPos = CalcTargetAnchPos(false);

            if (Dbg && DbgLog.Enabled) DbgLog.Log($"##scrol.  PreviousScreen: aPCurr:{AnchoredPosCurr} --> _lerpTargetAnchPos:{_lerpTargetAnchPos}" +
                                                  $"\n_elemWidth:{_elemWidth}, _viewPortW:{ViewPortWidth}, _contentW:{ContentWidth}, _aPMin:{_anchoredPosMax}, _aPMax:{_anchoredPosMin}.");
        }

    }
}