using System;
using System.Collections.Generic;
using UnityEngine;

#if VEGETATION_STUDIO
namespace AwesomeTechnologies.Vegetation
{
    public class RegisterColorTextureMask : ITextureMask
    {
        public Type GetMaskType()
        {
            return typeof(ColorTextureMask);
        }

        public string GetMaskTypeName()
        {
            return "Color Texture Mask";
        }

        public string GetTextureMaskTypeID()
        {
            return "VS_ColorTextureMask";
        }
    }

    [Serializable]
    public class ColorTextureMask : TextureMaskBase
    {
        private Color32[] _textureColors;
        private int _textureWidth;
        private int _textureHeight;

        public ColorTextureMask()
        {
            Init();

            MaskName = "Color Texture Mask";
            UseTiling = true;
            UseTexture = true;
            ShowInverseResult = true;
            ShowRotateTexture = true;
            TextureMaskTypeReadableName = "Color Texture Mask";
            TextureMaskTypeId = "VS_ColorTextureMask";

            TextureMaskProperty colorSelectorMaskProperty =
                new TextureMaskProperty
                {
                    TextureMaskPropertyType = TextureMaskPropertyType.ColorSelector,
                    ColorValue = Color.white,
                    PropertyName = "Select color",
                    PropertyDescription = "Select color"
                };
            AddTextureMaskProperty(colorSelectorMaskProperty);

            TextureMaskProperty toleranceTextureMaskProperty =
                new TextureMaskProperty
                {
                    TextureMaskPropertyType = TextureMaskPropertyType.Integer,
                    IntValue = 20,
                    IntMinValue = 1,
                    IntMaxValue = 255,
                    PropertyName = "Tolerance",
                    PropertyDescription = "Tolerance"
                };
            AddTextureMaskProperty(toleranceTextureMaskProperty);

            TextureMaskProperty multiplyAlphaProperty =
                new TextureMaskProperty
                {
                    TextureMaskPropertyType = TextureMaskPropertyType.Boolean,
                    BoolValue = false,
                    PropertyName = "Multiply with Alpha",
                    PropertyDescription = "Multiply with Alpha"
                };
            AddTextureMaskProperty(multiplyAlphaProperty);

            TextureMaskProperty returnColorToleranceProperty =
                new TextureMaskProperty
                {
                    TextureMaskPropertyType = TextureMaskPropertyType.Boolean,
                    BoolValue = true,
                    PropertyName = "Return color tolerance",
                    PropertyDescription = "Return color tolerance"
                };
            AddTextureMaskProperty(returnColorToleranceProperty);
        }

        protected override float SampleTextureMask(UnityEngine.Vector2 normalizedPosition, List<TextureMaskProperty> itemTextureMaskPropertiesList)
        {
            if (_textureWidth <= 0 || _textureHeight <= 0) return 0;

            int xPos = Mathf.FloorToInt((normalizedPosition.x * Scale) * _textureWidth);
            int yPos = Mathf.FloorToInt((normalizedPosition.y * Scale) * _textureHeight);

            xPos = xPos - Mathf.FloorToInt((float)xPos / _textureWidth) * _textureWidth;
            yPos = yPos - Mathf.FloorToInt((float)yPos / _textureHeight) * _textureHeight;

            Color selectedColor = GetColorPropertyValue("Select color", itemTextureMaskPropertiesList);
            int tollerance = GetIntPropertyValue("Tolerance", itemTextureMaskPropertiesList);
            bool multiplyAlpha = GetBooleanPropertyValue("Multiply with Alpha", itemTextureMaskPropertiesList);
            bool returnColortollerance = GetBooleanPropertyValue("Return color tolerance", itemTextureMaskPropertiesList);
            Color32 selectedColor32 = selectedColor;
            Color32 sampleColor = _textureColors[xPos + yPos * _textureWidth];

            float colorTolerance = GetColorTolerance(selectedColor32, sampleColor);           
            if (colorTolerance <= tollerance)
            {               
                float colorAccuracy = 1;
                if (returnColortollerance)
                {
                    colorAccuracy = 1 - colorTolerance / tollerance;
                }

                if (multiplyAlpha)
                {                    
                    return colorAccuracy * selectedColor32.a * 255f;
                }
                else
                {
                    return colorAccuracy;
                }               
            }

            return 0;         
        }

        float GetColorTolerance(Color32 selectedColor, Color32 sampledColor)
        {
            int tolerance = Math.Abs(selectedColor.r - sampledColor.r);
            tolerance = Mathf.Max(Math.Abs(selectedColor.g - sampledColor.g), tolerance);
            tolerance = Mathf.Max(Math.Abs(selectedColor.b - sampledColor.b), tolerance);
            return tolerance;
        }

        public override void RefreshTextureMask()
        {
            if (!MaskTexture) return;
            _textureColors = MaskTexture.GetPixels32();
            _textureWidth = MaskTexture.width;
            _textureHeight = MaskTexture.height;
        }
    }
}
#endif
