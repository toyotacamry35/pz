using Assets.Src.ResourceSystem;
using ColonyShared.SharedCode.Input;
using UnityEngine;

[CreateAssetMenu(menuName = "GuiWindows/WindowStackId")]
public class WindowStackId : ScriptableObject
{
    public bool CanOverlayOtherStacks;

    [SerializeField] private JdbMetadata _inputBindings;

    public InputBindingsDef InputBindings => _inputBindings ? _inputBindings.Get<InputBindingsDef>() : null; 
    
    public override string ToString()
    {
        return $"[{name}{(CanOverlayOtherStacks ? " " + nameof(CanOverlayOtherStacks) : "")}]";
    }
}
