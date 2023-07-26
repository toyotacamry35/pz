using UnityEngine;
using UnityEditor;

public class DualColor
{
    private Color _lightSkinColor;
    private Color _darkSkinColor;

    //--- Ctros ---------------------------------------------------------------

    /// <summary>
    /// Light RGB, Dark RGB
    /// </summary>
    public DualColor(Color forLightSkin, Color forDarkSkin)
    {
        _lightSkinColor = forLightSkin;
        _darkSkinColor = forDarkSkin;
    }

    /// <summary>
    /// Light RGB, Dark RGB
    /// </summary>
    public DualColor(byte lightSkinR, byte lightSkinG, byte lightSkinB, byte darkSkinR, byte darkSkinG, byte darkSkinB)
    {
        _lightSkinColor = new Color32(lightSkinR, lightSkinG, lightSkinB, 255);
        _darkSkinColor = new Color32(darkSkinR, darkSkinG, darkSkinB, 255);
    }

    //--- Props ---------------------------------------------------------------

    public Color Color => EditorGUIUtility.isProSkin ? _darkSkinColor : _lightSkinColor;
}