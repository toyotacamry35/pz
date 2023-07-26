using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Visual logging
/// </summary>
public static class VisualLogs
{
    public static bool DoLogging
    {
        get { return Debug.unityLogger.logEnabled && Application.isEditor; }
        set { Debug.unityLogger.logEnabled = value; }
    }

    public static HashSet<string> whiteList = new HashSet<string>()
    {
        VLogsSubjects.Destructing.ToString(),
        //...
    };

    // @param `color` is of nullable type as a surrogate of default param.value
    // @param `key` - subject for logging filtering
    public static void SubjDrawLine(string key, Vector3 start, Vector3 end, Color? color = null, float duration = 0.0f, bool depthTest = true)
    {
        if (!DoLogging)
            return;

        if (key != null && !whiteList.Contains(key))
            return;

        if (!color.HasValue)
            color = Color.white;

        Debug.DrawLine(start, end, color.GetValueOrDefault(), duration, depthTest);
    }

    // Draws multicolored line with red tip(наконечник).
    // @param `key` - subject for logging filtering
    public static void SubjDrawRainbowLine(string key, Vector3 start, Vector3 normal, float lengthFactor = 1f)
    {
        SubjDrawLine(key, start, start + lengthFactor * normal * 2, Color.cyan, 100f, false);
        SubjDrawLine(key, start, start + lengthFactor * normal, Color.green, 100f, false);
        SubjDrawLine(key, start, start + lengthFactor * normal / 2, Color.yellow, 100f, false);
        SubjDrawLine(key, start, start + lengthFactor * normal / 4, Color.red, 100f, false);
    }

}

public enum VLogsSubjects
{
    None,
    Destructing,
    //...

    All
}