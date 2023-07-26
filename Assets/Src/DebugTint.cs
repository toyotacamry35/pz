using Assets.Src.ResourcesSystem.Base;
using SharedCode.Entities.Engine;
using SharedCode.Wizardry;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTint : MonoBehaviour
{
    ModifierCauser _currentCauser;
    public void StartTint(ModifierCauser causer, Color color)
    {
        _currentCauser = causer;
        foreach (var renderer in GetComponentsInChildren<Renderer>())
            renderer.material.color = color;
    }

    public void StopTint(ModifierCauser causer)
    {
        if (_currentCauser != causer)
            return;
        foreach (var renderer in GetComponentsInChildren<Renderer>())
            renderer.material.color = Color.white;
    }

}
