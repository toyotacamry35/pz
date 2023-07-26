using UnityEngine;
using UnityEngine.UI;

public static partial class Extensions
{
    public static void SetAlpha(this Graphic graphic, float newAlpha)
    {
        if (graphic == null)
            return;

        var color = graphic.color;
        graphic.color = new Color(color.r, color.g, color.b, newAlpha);
    }


    /// <summary>
    /// Sets RGB-components, preserves original alpha
    /// </summary>
    public static void SetColoring(this Graphic graphic, Color coloring)
    {
        if (graphic == null)
            return;

        graphic.color = new Color(coloring.r, coloring.g, coloring.b, graphic.color.a);
    }
}
