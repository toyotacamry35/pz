using System.Collections.Generic;
using JetBrains.Annotations;
using Uins;
using UnityEngine;
using UnityEngine.UI;
using WeldAdds;

public class InventoryContextMenu : MonoBehaviour
{
    public ButtonSpritesSet ActiveButtonSpritesSet;
    public ButtonSpritesSet NormalButtonSpritesSet;
    public Transform ItemsParentTransform;
    public ContextMenuItemViewModel ContextMenuItemViewModelPrefab;
    public InventoryContextMenuViewModel ContextMenuViewModel;

    [SerializeField, UsedImplicitly]
    private Image _backgroundImage;

    private RectTransform _menuParentRectTransform;

    private List<ContextMenuItemViewModel> _items = new List<ContextMenuItemViewModel>();


    //=== Unity ===============================================================

    private void Awake()
    {
        ActiveButtonSpritesSet.AssertIfNull(nameof(ActiveButtonSpritesSet));
        NormalButtonSpritesSet.AssertIfNull(nameof(NormalButtonSpritesSet));
        ItemsParentTransform.AssertIfNull(nameof(ItemsParentTransform));
        ContextMenuItemViewModelPrefab.AssertIfNull(nameof(ContextMenuItemViewModelPrefab));
        ContextMenuViewModel.AssertIfNull(nameof(ContextMenuViewModel));
        _backgroundImage.AssertIfNull(nameof(_backgroundImage));

        _menuParentRectTransform = transform.parent.GetRectTransform();
    }

    private void Start()
    {
        if (!ContextMenuViewModel.AssertIfNull(nameof(ContextMenuViewModel)))
            ContextMenuViewModel.ShowContextMenu += OnShowContextMenu;
    }

    private void Destroy()
    {
        if (ContextMenuViewModel != null)
            ContextMenuViewModel.ShowContextMenu -= OnShowContextMenu;
    }


    //=== Public ==============================================================

    public static void SetPositionWithGuaranteedFullDisplayability(RectTransform targetRectTransform, RectTransform parentRectTransform)
    {
        //ориентация: относительно левого нижнего угла parentRectTransform
        var posX = Input.mousePosition.x / Screen.width * parentRectTransform.rect.width;
        var posY = Input.mousePosition.y / Screen.height * parentRectTransform.rect.height;
        var targetSize = targetRectTransform.sizeDelta;
        var parentRect = parentRectTransform.rect;
        var finalPosX = posX;
        var finalPosY = posY;
        if (targetRectTransform.pivot.y > 0.5 && posY - targetSize.y < 0)
        {
            //targetRectTransform - вниз от позиции, поднимаем позицию, если уходит за нижний край
            finalPosY = Mathf.Min(posY + targetSize.y, parentRect.height - targetSize.y);
        }
        else if (targetRectTransform.pivot.y < 0.5 && posY + targetSize.y > parentRect.height)
        {
            //targetRectTransform - вверх от позиции, опускаем позицию, если уходит за верхний край
            finalPosY = Mathf.Max(0, posY - targetSize.y);
        }

        if (targetRectTransform.pivot.x < 0.5 && posX + targetSize.x > parentRect.width)
        {
            //targetRectTransform - вправо от позиции, сдвигаем позицию влево, если уходит за правый край
            finalPosX = Mathf.Max(0, posX - targetSize.x);
        }
        else if (targetRectTransform.pivot.x > 0.5 && posX - targetSize.x < 0)
        {
            //targetRectTransform - влево от позиции, сдвигаем позицию вправо, если уходит за левый край
            finalPosX = Mathf.Min(posX + targetSize.x, parentRect.width - targetSize.x);
        }
        targetRectTransform.anchoredPosition = new Vector2(finalPosX, finalPosY);
    }


    //=== Private =============================================================

    private void OnShowContextMenu([NotNull] List<ContextMenuItemData> contextMenuItems)
    {
        _backgroundImage.enabled = false;
        //UI.Logger.IfDebug()?.Message($"[{Time.time:f2}] OnShowContextMenu() {contextMenuItems.VarDump(nameof(contextMenuItems))}").Write(); //DEBUG
        ClearOldItems();

        transform.position = Input.mousePosition;
        bool isActiveItemHasBeenAdded = false;
        foreach (var contextMenuItemData in contextMenuItems)
        {
            var contextMenuItem = Instantiate(ContextMenuItemViewModelPrefab, ItemsParentTransform);
            if (contextMenuItem.AssertIfNull(nameof(contextMenuItem)))
                break;

            isActiveItemHasBeenAdded = contextMenuItemData.IsActive;
            if (isActiveItemHasBeenAdded)
            {
                //TODOM добавлять разделитель после активного
            }

            contextMenuItem.name = $"{nameof(contextMenuItem)}{_items.Count}";
            contextMenuItem.Title = contextMenuItemData.Title;
            contextMenuItem.IsDisabled = contextMenuItemData.IsDisabled;
            contextMenuItem.SetAction(contextMenuItemData.Action, contextMenuItemData.ActionParams);
            SetButtonSprites(contextMenuItemData.IsActive, contextMenuItem.gameObject);
            contextMenuItem.ParentViewModel = ContextMenuViewModel;
            _items.Add(contextMenuItem);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_backgroundImage.rectTransform);
        _backgroundImage.enabled = true;
        SetPositionWithGuaranteedFullDisplayability(_backgroundImage.rectTransform, _menuParentRectTransform);
    }

    private void ClearOldItems()
    {
        while (_items.Count > 0)
        {
            var removedContextMenuItemViewModel = _items[0];
            _items.Remove(removedContextMenuItemViewModel);
            Destroy(removedContextMenuItemViewModel.gameObject);
        }
    }

    private void SetButtonSprites(bool isActive, GameObject gameObject)
    {
        var buttonSpritesSet = isActive ? ActiveButtonSpritesSet : NormalButtonSpritesSet;
        buttonSpritesSet.SetSpritesForButton(gameObject.GetComponent<Button>());
    }
}