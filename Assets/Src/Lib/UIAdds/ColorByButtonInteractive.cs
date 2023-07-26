using System;
using UnityEngine;
using UnityEngine.UI;

[Obsolete("Deprecated: Update() usage Bind Interactable Source To ColorAndSpriteSetter ")]
public class ColorByButtonInteractive : MonoBehaviour
{
    private const float MaxUpdateInterval = 0.3f;

    public Button Button;

    public Graphic[] Targets;

    public Color InteractableColor;
    public Color LockedColor;

    public bool ChangeColoring = true;
    public bool ChangeAlpha = true;

    private bool _lastIsInteractable;
    private float _lastUpdateTime = -0.1f;


    //=== Unity ===============================================================

    void Awake()
    {
        if (Button.AssertIfNull(nameof(Button)) ||
            Targets.IsNullOrEmptyOrHasNullElements(nameof(Targets)) ||
            (!ChangeAlpha && !ChangeColoring))
        {
            enabled = false;
            return;
        }

        foreach (var graphic in Targets)
            (graphic as Image)?.DisableSpriteOptimizations();
    }

    void Update()
    {
        if (_lastIsInteractable == Button.interactable &&
            _lastUpdateTime > 0 &&
            Time.time - _lastUpdateTime < MaxUpdateInterval)
            return;

        _lastUpdateTime = Time.time;
        _lastIsInteractable = Button.interactable;
        for (int i = 0, len = Targets.Length; i < len; i++)
        {
            if (ChangeAlpha && ChangeColoring)
            {
                Targets[i].color = _lastIsInteractable ? InteractableColor : LockedColor;
            }
            else
            {
                if (ChangeAlpha)
                    Targets[i].SetAlpha(_lastIsInteractable ? InteractableColor.a : LockedColor.a);
                else
                    Targets[i].SetColoring(_lastIsInteractable ? InteractableColor : LockedColor);
            }
        }
    }
}