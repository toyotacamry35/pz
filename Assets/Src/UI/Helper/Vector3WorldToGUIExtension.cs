using UnityEngine;

public static class Vector3WorldToGUIExtension
{
    public static Vector3 WorldToGuiPoint(this Vector3 position)
    {
        if (UnityEngine.Camera.main == null)
            return position;
        var guiPosition = UnityEngine.Camera.main.WorldToScreenPoint(position);
        guiPosition.y = Screen.height - guiPosition.y;

        return guiPosition;
    }
}