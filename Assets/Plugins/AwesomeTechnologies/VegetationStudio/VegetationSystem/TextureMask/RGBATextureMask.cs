using System;
using System.Collections.Generic;
using UnityEngine;

#if VEGETATION_STUDIO
namespace AwesomeTechnologies.Vegetation
{
    public class RegisterRgbaTextureMask : ITextureMask
    {
        public Type GetMaskType()
        {
            return typeof(RgbaTextureMask);
        }

        public string GetMaskTypeName()
        {
            return "RGBA Texture Mask";
        }

        public string GetTextureMaskTypeID()
        {
            return "VS_RGBATextureMask";
        }
    }

    [Serializable]
    public class RgbaTextureMask : TextureMaskBase
    {
        private Color32[] _textureColors;
        private int _textureWidth;
        private int _textureHeight;

        public RgbaTextureMask()
        {
            Init();

            MaskName = "RGBA Texture Mask";
            UseTiling = true;
            UseTexture = true;
            ShowInverseResult = true;
            ShowRotateTexture = true;
            TextureMaskTypeReadableName = "RGBA Texture Mask";
            TextureMaskTypeId = "VS_RGBATextureMask";

            TextureMaskProperty rgbaSelectorMaskProperty =
                new TextureMaskProperty
                {
                    TextureMaskPropertyType = TextureMaskPropertyType.RgbaSelector,
                    IntMinValue = 0,
                    IntMaxValue = 3,
                    IntValue = 0,
                    PropertyName = "Select color channel",
                    PropertyDescription = "Select color channel"
                };
            AddTextureMaskProperty(rgbaSelectorMaskProperty);
        }

        protected override float SampleTextureMask(UnityEngine.Vector2 normalizedPosition, List<TextureMaskProperty> itemTextureMaskPropertiesList)
        {
            if (_textureWidth <= 0 || _textureHeight <= 0) return 0;

            int xPos = Mathf.FloorToInt((normalizedPosition.x * Scale) * _textureWidth);
            int yPos = Mathf.FloorToInt((normalizedPosition.y * Scale) * _textureHeight);

            xPos = xPos - Mathf.FloorToInt((float)xPos / _textureWidth) * _textureWidth;
            yPos = yPos - Mathf.FloorToInt((float)yPos / _textureHeight) * _textureHeight;

            int selectedChannel = GetIntPropertyValue("Select color channel", itemTextureMaskPropertiesList);
            Color32 sampleColor = _textureColors[xPos + yPos * _textureWidth];

            int channelValue = 0;
            switch (selectedChannel)
            {
                case 0:
                    channelValue = sampleColor.r;
                    break;
                case 1:
                    channelValue = sampleColor.g;
                    break;
                case 2:
                    channelValue = sampleColor.b;
                    break;
                case 3:
                    channelValue = sampleColor.a;
                    break;
            }

            float averageDensity = (float)channelValue / 255;
            return averageDensity;
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
