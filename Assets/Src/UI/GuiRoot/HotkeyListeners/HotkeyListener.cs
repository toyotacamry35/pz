using System.Linq;
using EnumerableExtensions;
using UnityEngine;

[CreateAssetMenu(menuName = "GuiWindows/HotkeyListener")]
public class HotkeyListener : ScriptableObject
{
    public string UseComponentSpell;
    public HotkeySettings[] Hotkeys;
    public AxisAsHotkeySettings[] AxesAsHotkeys;

    /// <summary>
    /// Аналог GetKeyDown или GetKeyUp в зависимости от настроек в Hotkeys
    /// </summary>
    public bool IsFired(bool keyDown = true)
    {
        if (Hotkeys != null)
            for (int i = 0, len = Hotkeys.Length; i < len; ++i)
                if (Hotkeys[i] != null && Hotkeys[i].IsFired(keyDown))
                    return true;
        if (AxesAsHotkeys != null)
            for (int i = 0, len = AxesAsHotkeys.Length; i < len; i++)
                if (AxesAsHotkeys[i] != null && AxesAsHotkeys[i].IsFired())
                    return true;
        return false;
    }

    /// <summary>
    /// Аналог GetKey
    /// </summary>
    public bool IsPressed()
    {
        if (Hotkeys != null)
            for (int i = 0, len = Hotkeys.Length; i < len; i++)
                if (Hotkeys[i] != null && Hotkeys[i].IsPressed())
                    return true;
        if (AxesAsHotkeys != null)
            for (int i = 0, len = AxesAsHotkeys.Length; i < len; i++)
                if (AxesAsHotkeys[i] != null && AxesAsHotkeys[i].IsPressed())
                    return true;
        return false;
    }

    private const string AlphaPrefix = "Alpha";

    public string GetFirstHotkeyName()
    {
        if (Hotkeys == null || Hotkeys.Length == 0)
            return "";

        var firstHotkey = Hotkeys[0];
        var shortcutText = KeycodeToString(firstHotkey.Key);
        if (firstHotkey.Modifiers != null && firstHotkey.Modifiers.Length > 0)
            foreach (var modifierKeyCode in firstHotkey.Modifiers)
                shortcutText = $"{KeycodeToString(modifierKeyCode)}+{shortcutText}";
        return shortcutText;
    }

    private string KeycodeToString(KeyCode keyCode)
    {
        var keyToString = keyCode.ToString();
        if (keyToString.StartsWith(AlphaPrefix))
            keyToString = keyToString.Substring(AlphaPrefix.Length);
        return keyToString;
    }

    public override string ToString()
    {
        return Hotkeys.Select(x => x.ToString()).Concat(AxesAsHotkeys.Select(x => x.ToString())).ItemsToString(false);
    }
}