using UnityEngine;

[CreateAssetMenu(menuName = "GuiWindows/WindowId")]
public class WindowId : ScriptableObject
{
    public WindowStackId PrefferedStackId;

    public override string ToString()
    {
        return name;
    }
}
