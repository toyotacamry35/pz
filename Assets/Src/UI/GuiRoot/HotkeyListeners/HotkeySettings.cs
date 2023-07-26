using System;
using EnumerableExtensions;
using JetBrains.Annotations;
using UnityEngine;


[Serializable]
public class HotkeySettings
{
    [SerializeField, UsedImplicitly]
    private KeyCode _key;

    [SerializeField, UsedImplicitly]
    private KeyCode[] _modifiers;

    [SerializeField, UsedImplicitly]
    private ClickType _clickType = ClickType.Click;

    [SerializeField, UsedImplicitly]
    private float _activationTime = 0.1f;

    public KeyCode Key => _key;
    public KeyCode[] Modifiers => _modifiers;
    private bool _lastValue = false;
    private float _lastInputTimes = 0;

    public bool IsFired(bool keyDown)
    {
        var isKeyFired = keyDown ? Input.GetKeyDown(_key) : Input.GetKeyUp(_key);

        switch (_clickType)
        {
            case ClickType.Click:
            {
                if (isKeyFired && IsModifiersPressed)
                    return true;
                break;
            }
            case ClickType.DoubleClick:
            {
                bool clicked = false;
                if (isKeyFired)
                {
                    if (_lastInputTimes + _activationTime > Time.time)
                        clicked = true;
                    _lastInputTimes = Time.time;
                }

                return clicked;
            }
            case ClickType.Hold:
            {
                if (isKeyFired)
                    _lastInputTimes = Time.time;
                else
                    _lastInputTimes = 0;

                return _lastInputTimes > 0 && _lastInputTimes + _activationTime > Time.time;
            }
        }

        return false;
    }

    public bool IsPressed()
    {
        if (Input.GetKey(_key) && IsModifiersPressed)
            return true;

        return false;
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

    public override string ToString()
    {
        if(_modifiers!=null && _modifiers.Length > 0)
            return $"{_key} + {_modifiers.ItemsToString()}";

        return
            _key.ToString();
    }

    public enum ClickType
    {
        Click,
        DoubleClick,
        Hold
    }
}