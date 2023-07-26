using System;
using L10n;

public class ContextMenuItemData
{
    public LocalizedString Title;
    public bool IsActive;
    public bool IsDisabled;
    public Action<object[]> Action;
    public object[] ActionParams;
}