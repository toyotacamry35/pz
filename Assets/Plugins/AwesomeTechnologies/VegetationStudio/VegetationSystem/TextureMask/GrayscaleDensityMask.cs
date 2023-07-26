using System;
using System.Collections.Generic;
using UnityEngine;

#if VEGETATION_STUDIO
namespace AwesomeTechnologies.Vegetation
{
    public class RegisterGrayscaleDensityMask: ITextureMask
    {
        public Type GetMaskType()
        {
            return typeof(GrayscaleDensityMask);
        }

        public string GetMaskTypeName()
        {
            return "Grayscale Density Texture Mask";
        }

        public string GetTextureMaskTypeID()
        {
            return "VS_GrayScaleDensity";
        }
    }
    
    [Serializable]
    public class GrayscaleDensityMask : TextureMaskBase
    {
        private Color32[] _textureColors;
        private int _textureWidth;
        private int _textureHeight;

        public GrayscaleDensityMask()
        {
            Init();

            MaskName = "Grayscale Density Texture Mask";
            UseTiling = true;
            UseTexture = true;
            ShowInverseResult = true;
            ShowRotateTexture = true;
            TextureMaskTypeReadableName = "Grayscale Density Texture Mask";
            TextureMaskTypeId = "VS_GrayScaleDensity";
        }

        protected override float SampleTextureMask(UnityEngine.Vector2 normalizedPosition, List<TextureMaskProperty> itemTextureMaskPropertiesList)
        {
            if (_textureWidth <= 0 || _textureHeight <= 0) return 0;

            int xPos = Mathf.FloorToInt((normalizedPosition.x * Scale) * _textureWidth);
            int yPos = Mathf.FloorToInt((normalizedPosition.y * Scale) * _textureHeight);
        
            xPos = xPos - Mathf.FloorToInt((float) xPos / _textureWidth) * _textureWidth;
            yPos = yPos - Mathf.FloorToInt((float) yPos / _textureHeight) * _textureHeight;

            Color32 sampleColor = _textureColors[xPos + yPos * _textureWidth];

            int totalColor = sampleColor.r + sampleColor.g + sampleColor.g;
            float averageDensity = (totalColor /3f) / 255;
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
