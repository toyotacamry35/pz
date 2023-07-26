using System.ComponentModel;
using System.Linq;
using Uins.Slots;
using UnityEngine;
using UnityEngine.UI;
using WeldAdds;

public class SlotBackgroundSetter : MonoBehaviour
{
    public ButtonSpritesSet NormalButtonSpritesSet;
    public ButtonSpritesSet SelectedButtonSpritesSet;
    public ButtonSpritesSet InaccessibleButtonSpritesSet; //may be null
    public ButtonSpritesSet BrokenButtonSpritesSet; //may be null

    public Button TargetButton;
    public Image TargetImage;
    public SlotViewModel TargetSlotViewModel;
    private ICanBeInaccessible _asInaccessible;

    protected string[] ControlledProps;
    protected ButtonSpritesSet LastButtonSpritesSet;


    //=== Unity ===============================================================

    private void Awake()
    {
        if (NormalButtonSpritesSet.AssertIfNull(nameof(NormalButtonSpritesSet)) ||
            SelectedButtonSpritesSet.AssertIfNull(nameof(SelectedButtonSpritesSet)) ||
            TargetButton.AssertIfNull(nameof(TargetButton)) ||
            TargetImage.AssertIfNull(nameof(TargetImage)) ||
            TargetSlotViewModel.AssertIfNull(nameof(TargetSlotViewModel)))
            return;

        _asInaccessible = TargetSlotViewModel as ICanBeInaccessible;
        OnAwake();
        SetState();
        TargetSlotViewModel.PropertyChanged += OnSlotViewModelPropertyChanged;
    }

    private void OnDestroy()
    {
        if (TargetSlotViewModel != null)
            TargetSlotViewModel.PropertyChanged -= OnSlotViewModelPropertyChanged;
    }


    //=== Protected ===========================================================

    protected virtual void OnAwake()
    {
        ControlledProps = new[]
        {
            nameof(SlotViewModel.IsSelected),
            nameof(SlotViewModel.IsEmpty),
            nameof(ICanBeInaccessible.IsInaccessible),
            nameof(SlotViewModel.IsBroken)
        };
    }

    protected virtual ButtonSpritesSet GetButtonSpritesSet()
    {
        if (InaccessibleButtonSpritesSet != null && (_asInaccessible?.IsInaccessible ?? false))
            return InaccessibleButtonSpritesSet;

        if (TargetSlotViewModel.IsSelected)
            return SelectedButtonSpritesSet;

        if (BrokenButtonSpritesSet != null && TargetSlotViewModel.IsBroken)
            return BrokenButtonSpritesSet;

        return NormalButtonSpritesSet;
    }


    //=== Private =============================================================

    private void OnSlotViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (!ControlledProps.Contains(e.PropertyName))
            return;

        SetState();
    }

    private void SetState()
    {
        if (TargetSlotViewModel == null)
            return;

        var buttonSpritesSet = GetButtonSpritesSet();
        if (LastButtonSpritesSet != buttonSpritesSet)
        {
            LastButtonSpritesSet = buttonSpritesSet;
            TargetButton.targetGraphic = TargetImage;
            buttonSpritesSet.SetSpritesForButton(TargetButton, TargetImage);
        }

        TargetButton.interactable = !TargetSlotViewModel.IsEmpty && !(_asInaccessible?.IsInaccessible ?? false);
    }
}