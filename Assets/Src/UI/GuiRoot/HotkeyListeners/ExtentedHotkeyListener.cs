
using System;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class ExtentedHotkeyListener
{
    public HotkeyListener Hotkey;
    public float HoldTime;

    private State _state;
    private float _pressedAt;
        
    public ExtentedHotkeyListener()
    {}

    public ExtentedHotkeyListener([NotNull] HotkeyListener hotkey, float holdtime)
    {
        if (hotkey == null) throw new ArgumentNullException(nameof(hotkey));
        Hotkey = hotkey;
        HoldTime = holdtime;
    }
    
    enum State { Released, Pressed, Holded } 

    /// <summary>
    /// Кнопка нажата 
    /// </summary>
    public bool Pressed => Hotkey.IsPressed();

    /// <summary>
    /// Кнопка была нажата
    /// </summary>
    public bool Press { get; private set; }
    
    /// <summary>
    /// Кнопка была отпущена 
    /// </summary>
    public bool Release { get; private set; }

    /// <summary>
    /// Кнопка отпущена после короткого нажатия (меньше LongClickTime)
    /// </summary>
    public bool Click { get; private set; }

    /// <summary>
    /// Кнопка была нажата в течении LongClickTime
    /// </summary>
    public bool Hold { get; private set; }

    /// <summary>
    /// Кнопка была нажата LongClickTime или дольше
    /// </summary>
    public bool Holded => _state == State.Holded;
    
    public void Update()
    {
        Press = false;
        Release = false;
        Click = false;
        Hold = false;
        
        if (Hotkey == null)
            return;
            
        var state = _state;
        if (Hotkey.IsPressed())
        {
            if (state == State.Released)
            {
                _state = State.Pressed;
                _pressedAt = Time.unscaledTime;
                Press = true;
            }
            if (state == State.Pressed && Time.unscaledTime > _pressedAt + HoldTime)
            {
                _state = State.Holded;
                Hold = true;
            }
        }
        else
        {
            _state = State.Released;
            if (state != State.Released)
                Release = true;
            if (state == State.Pressed)
                Click = true;
        }
    }
}
