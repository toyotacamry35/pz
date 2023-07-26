using UnityEditor;
using UnityEngine;

public static class EditorGuiAdds
{
    private static GUIStyle _horizontalLineStyle;

    private static GUIStyle HorizontalLineStyle
    {
        get
        {
            if (_horizontalLineStyle == null)
            {
                _horizontalLineStyle = new GUIStyle();
                _horizontalLineStyle.normal.background = EditorGUIUtility.whiteTexture;
                _horizontalLineStyle.margin = new RectOffset(0, 0, 4, 4);
                _horizontalLineStyle.fixedHeight = 1;
            }

            return _horizontalLineStyle;
        }
    }

    //=== Public. Custom controls  ============================================

    public static void HorizontalLine()
    {
        HorizontalLine(Color.gray);
    }

    public static void HorizontalLine(Color color)
    {
        var prevColor = GUI.color;
        GUI.color = color;
        GUILayout.Box(GUIContent.none, HorizontalLineStyle);
        GUI.color = prevColor;
    }

    public static void ColoredTextArea(Color bgColor, string text, GUIStyle style, params GUILayoutOption[] guiLayoutOptions)
    {
        var prevColor = GUI.color;
        GUI.color = bgColor;
        EditorGUILayout.TextArea(text, style, guiLayoutOptions);
        GUI.color = prevColor;
    }

    public static void ColoredTextArea(Color bgColor, string text, params GUILayoutOption[] guiLayoutOptions)
    {
        var prevColor = GUI.color;
        GUI.color = bgColor;
        EditorGUILayout.TextArea(text, guiLayoutOptions);
        GUI.color = prevColor;
    }

    public static bool ColoredButton(Color bgColor, string text, params GUILayoutOption[] guiLayoutOptions)
    {
        var prevColor = GUI.color;
        GUI.color = bgColor;
        var res = GUILayout.Button(text, guiLayoutOptions);
        GUI.color = prevColor;
        return res;
    }


    //=== Public. Custom styles ===============================================

    public static GUIStyle GetStyle(Color32 someColor, bool isBold = false, GUIStyle fromStyle = null)
    {
        GUIStyle newStyle = (fromStyle == null) ? new GUIStyle() : new GUIStyle(fromStyle);
        newStyle.fontStyle = isBold ? FontStyle.Bold : FontStyle.Normal;
        newStyle.normal.textColor = newStyle.onNormal.textColor =
            newStyle.hover.textColor = newStyle.onHover.textColor =
                newStyle.active.textColor = newStyle.onActive.textColor =
                    newStyle.focused.textColor = newStyle.onFocused.textColor = someColor;
        return newStyle;
    }

    public static GUIStyle GetStyle(Color32 someColor, Color32 backgroundColor, bool isBold = false, GUIStyle fromStyle = null)
    {
        var style = GetStyle(someColor, isBold, fromStyle);
        var bgTexture = MakeTex(100, 10, backgroundColor);
        style.normal.background = style.onNormal.background =
            style.hover.background = style.onHover.background =
                style.active.background = style.onActive.background =
                    style.focused.background = style.onFocused.background = bgTexture;
        return style;
    }

    private static Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
}