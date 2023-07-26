using System;
using System.Collections.Generic;
using UnityEngine;

#if VEGETATION_STUDIO
namespace AwesomeTechnologies.Vegetation
{
    public class RegisterMicroSplatStreamControlMask : ITextureMask
    {
        public Type GetMaskType()
        {
            return typeof(MicroSplatStreamControlMask);
        }

        public string GetMaskTypeName()
        {
            return "Micro Splat Stream Control Mask";
        }

        public string GetTextureMaskTypeID()
        {
            return "VS_MicroSplatStreamControlMask";
        }
    }

    [Serializable]
    public class MicroSplatStreamControlMask : TextureMaskBase
    {
        private Color32[] _textureColors;
        private int _textureWidth;
        private int _textureHeight;

        public MicroSplatStreamControlMask()
        {
            Init();

            MaskName = "Micro Splat Stream Control Mask";
            UseTiling = false;
            UseTexture = true;
            TextureMaskTypeReadableName = "Micro Splat Stream Control Mask";
            TextureMaskTypeId = "VS_MicroSplatStreamControlMask";

            TextureMaskProperty streamTypeSelectorMaskProperty =
                new TextureMaskProperty
                {
                    TextureMaskPropertyType = TextureMaskPropertyType.DropDownStringList,
                    IntMinValue = 0,
                    IntMaxValue = 3,
                    IntValue = 0,
                    PropertyName = "SelectStreamType",
                    PropertyDescription = "Category"
                };
            streamTypeSelectorMaskProperty.StringList.Add("Wetness");
            streamTypeSelectorMaskProperty.StringList.Add("Puddles");
            streamTypeSelectorMaskProperty.StringList.Add("Streames");
            streamTypeSelectorMaskProperty.StringList.Add("Lava");
            streamTypeSelectorMaskProperty.StringList.Add("Puddles and Streams");
            AddTextureMaskProperty(streamTypeSelectorMaskProperty);
        }

        protected override float SampleTextureMask(UnityEngine.Vector2 normalizedPosition, List<TextureMaskProperty> itemTextureMaskPropertiesList)
        {
            if (_textureWidth <= 0 || _textureHeight <= 0) return 0;

            int xPos = Mathf.FloorToInt((normalizedPosition.x * Scale) * _textureWidth);
            int yPos = Mathf.FloorToInt((normalizedPosition.y * Scale) * _textureHeight);

            xPos = xPos - Mathf.FloorToInt((float)xPos / _textureWidth) * _textureWidth;
            yPos = yPos - Mathf.FloorToInt((float)yPos / _textureHeight) * _textureHeight;

            int selectedChannel = GetIntPropertyValue("SelectStreamType", itemTextureMaskPropertiesList);
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
                case 4:
                    channelValue = Mathf.Max(sampleColor.g,sampleColor.b);
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