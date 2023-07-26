using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.GeneratedCode.Shared;
using Core.Environment.Logging.Extension;
using NLog;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace Uins.Slots
{
    public class DraggableItem : DropTargetBase, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerClickHandler,
        IPointerEnterHandler, IPointerExitHandler
    {
        public static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(DraggableItem));

        public SlotViewModel SlotViewModel;

        private RectTransform _selfRectTransform;
        private Vector3 _orgPosition;

        private bool _hovered;

        private bool _isImGuiInited;
        private GUIStyle _debugInfoBoxStyle;

        private DraggingHandler _draggingHandler;
        private IContextActionsSource _contextActionsSource;
        private Transform _dragTopTransform;
        private Transform _orgParentTransform;
        private GameObject[] _dragIgnoreGameObjects;

        public Image SelfImage;
        public Image HighlightingImage;


        //=== Props ===============================================================

        public override SlotViewModel Target => SlotViewModel;

        private bool _isDragged;

        [Binding]
        public bool IsDragged
        {
            get => _isDragged;
            set
            {
                if (value != _isDragged)
                {
                    _isDragged = value;
                    NotifyPropertyChanged();
                    OnIsDraggedChanged();
                }
            }
        }

        private bool _isHighlighted;

        [Binding]
        public bool IsHighlighted
        {
            get => _isHighlighted;
            set
            {
                if (value != _isHighlighted)
                {
                    _isHighlighted = value;
                    NotifyPropertyChanged();
                    OnIsHighlightedChanged();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            _orgParentTransform = transform.parent;
            SlotViewModel.AssertIfNull(nameof(SlotViewModel));
            _selfRectTransform = transform as RectTransform;
            _orgPosition = _selfRectTransform.localPosition;
            SelfImage.AssertIfNull(nameof(SelfImage));
            HighlightingImage.AssertIfNull(nameof(HighlightingImage));

            HighlightingImage.enabled = false;
            _dragIgnoreGameObjects = new[] {gameObject, SlotViewModel.gameObject};
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_draggingHandler != null)
                _draggingHandler.Unregister(this);
        }


        //=== Public ==========================================================

        public void Init(DraggingHandler draggingHandler, IContextActionsSource contextActionsSource,
            Transform dragTopTransform = null)
        {
            _draggingHandler = draggingHandler;
            _contextActionsSource = contextActionsSource;
            _dragTopTransform = dragTopTransform;
            draggingHandler.Register(this);
        }

        public void Release()
        {
            _draggingHandler.Unregister(this);
            _draggingHandler = null;
            _contextActionsSource = null;
            _dragTopTransform = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            try
            {
                var shouldContextMenuToBeClosed = false;
                if (!SlotViewModel.IsEmpty && !SlotViewModel.ItemIsNullOrDefault)
                    switch (eventData.pointerId)
                    {
                        case -1: //ЛКМ
                            shouldContextMenuToBeClosed = true;
                            SlotViewModel.OnClick();
                            if (eventData.clickCount % 2 == 0)
                                _contextActionsSource?.ExecuteDefaultAction(SlotViewModel, false);
                            break;
                        case -2: //ПКМ
                            _contextActionsSource?.OnContextMenuRequest(SlotViewModel);
                            break;
                    }
                else
                    shouldContextMenuToBeClosed = true;

                if (_contextActionsSource != null && shouldContextMenuToBeClosed)
                    _contextActionsSource.CloseContextMenuRequest();
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message($"<{GetType()}> {nameof(OnPointerClick)}() [pt.1] {e}\n{nameof(SlotViewModel)}={SlotViewModel}").Write();
            }
        }

        public void OnBeginDrag(PointerEventData pointerEventData)
        {
            if (pointerEventData == null || SlotViewModel.IsEmpty)
                return;

            if (_dragTopTransform != null)
                transform.parent = _dragTopTransform;

            IsDragged = true;
            _draggingHandler.OnItemDragBegin(this);
            _contextActionsSource?.CloseContextMenuRequest();
        }

        public void OnDrag(PointerEventData pointerEventData)
        {
            if (pointerEventData == null || SlotViewModel.IsEmpty)
                return;

            _selfRectTransform.position = pointerEventData.position;
        }

        public void OnEndDrag(PointerEventData pointerEventData)
        {
            SlotViewModel toSvm = null;
            DropTargetKind dropTargetKind = DropTargetKind.SlotViewModel;
            try
            {
                dropTargetKind = GetSlotViewModelOnDropTarget(pointerEventData, out toSvm);
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message($"{nameof(OnEndDrag)}: {e}").Write();
            }

            if (transform.parent != _orgParentTransform)
                transform.parent = _orgParentTransform;

            ResetFromDraggingState();
            _draggingHandler.OnItemDragEnd(this);
            _contextActionsSource?.CloseContextMenuRequest();

            if (pointerEventData == null || SlotViewModel.IsEmpty)
            {
                Logger.IfDebug()?.Message($"Empty {nameof(pointerEventData)} or {nameof(SlotViewModel)}={SlotViewModel}").Write();
                return;
            }

            switch (dropTargetKind)
            {
                case DropTargetKind.SlotViewModel:
                    if (!toSvm.AssertIfNull(nameof(toSvm)))
                        toSvm.OnDragToThis(SlotViewModel);
                    break;

                case DropTargetKind.ThrowAwayTarget:
                    if (!SlotViewModel.OnThrowAwayFromThis())
                        Logger.IfDebug()?.Message($"Failed throw away on {SlotViewModel}").Write();
                    break;

                default:
                    Logger.IfWarn()?.Message($"Unhandled {nameof(DropTargetKind)}: {dropTargetKind}").Write();
                    break;
            }
        }

        public void HighlightEnd()
        {
            IsHighlighted = false;
        }

        public void HighlightBegin(DraggableItem currentDraggingItem)
        {
            IsHighlighted = SlotViewModel.CanMoveToThis(currentDraggingItem.SlotViewModel);
        }

        public override string ToString()
        {
            return $"{nameof(DraggableItem)}: svm={SlotViewModel}";
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hovered = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hovered = true;
        }

        public void ResetFromDraggingState()
        {
            _selfRectTransform.localPosition = _orgPosition;
            IsDragged = false;
        }


        //=== Private =============================================================

        protected void OnIsDraggedChanged()
        {
            SelfImage.raycastTarget = !IsDragged; //чтобы не загораживал других
        }

        protected void OnIsHighlightedChanged()
        {
            HighlightingImage.enabled = IsHighlighted;
        }

        private DropTargetKind GetSlotViewModelOnDropTarget(PointerEventData pointerEventData, out SlotViewModel slotViewModel)
        {
            slotViewModel = null;
            var dropTargetBase = GetTargetScript<DropTargetBase>(pointerEventData, _dragIgnoreGameObjects);
            if (dropTargetBase != null)
            {
                slotViewModel = dropTargetBase.Target;
                return dropTargetBase.Kind;
            }

            slotViewModel = GetTargetScript<SlotViewModel>(pointerEventData, _dragIgnoreGameObjects);
            if (slotViewModel != null)
                return DropTargetKind.SlotViewModel;

            return DropTargetKind.None;
        }

        private T GetTargetScript<T>(PointerEventData pointerEventData, GameObject[] ignoreGameObjects) where T : MonoBehaviour
        {
            if (pointerEventData == null || pointerEventData.hovered == null || pointerEventData.hovered.Count == 0)
                return null;

            List<T> suitableComponents = new List<T>();
            for (int i = 0, len = pointerEventData.hovered.Count; i < len; i++)
            {
                var hoveredObject = pointerEventData.hovered[len - 1 - i]; //ищем с конца, важно!
                if (ignoreGameObjects != null && ignoreGameObjects.Contains(hoveredObject))
                    continue;

                var component = hoveredObject?.GetComponent<T>();

                if (component != null)
                    suitableComponents.Add(component);
            }

            if (suitableComponents.Count == 0)
                return null;

            T closestComponent = null;
            float closestDist = float.MaxValue;
            foreach (var suitableComponent in suitableComponents)
            {
                var dist = Vector2.SqrMagnitude(pointerEventData.position - (Vector2) suitableComponent.transform.position);
                //UI.Logger.IfDebug()?.Message($"dist={dist}  {suitableComponent.transform.FullName()}").Write();
                if (closestComponent == null || closestDist > dist)
                {
                    closestDist = dist;
                    closestComponent = suitableComponent;
                }
            }

            return closestComponent;
        }
    }
}