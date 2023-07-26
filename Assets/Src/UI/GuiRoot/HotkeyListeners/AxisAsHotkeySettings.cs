using System;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class AxisAsHotkeySettings
{
    [SerializeField, UsedImplicitly]
    private string _axisName;

    [SerializeField, UsedImplicitly]
    private bool _isNegative;

    [SerializeField, UsedImplicitly]
    private KeyCode[] _modifiers;

    public bool IsFired()
    {
        var isKeyFired = Input.GetAxis(_axisName) * (_isNegative ? -1 : 1) > 0;
        return isKeyFired && IsModifiersPressed;
    }

    public bool IsPressed()
    {
        return IsFired();
    }

    public bool IsModifiersPressed
    {
        get
        {
            var isModifiersPressed = true;
            if (_modifiers != null && _modifiers.Length > 0)
            {
                for (int j = 0, modifiersLen = _modifiers.Length; j < modifiersLen; j++)
                {
                    if (!Input.GetKey(_modifiers[j]))
                    {
                        isModifiersPressed = false;
                        break;
                    }
                }
            }

            return isModifiersPressed;
        }
    }
}