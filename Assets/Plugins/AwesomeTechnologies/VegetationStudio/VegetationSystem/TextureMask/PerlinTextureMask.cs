using System.Collections.Generic;
using UnityEngine;
using System;

#if VEGETATION_STUDIO
namespace AwesomeTechnologies.Vegetation
{
    public class RegisterPerlinTextureMask : ITextureMask
    {
        public Type GetMaskType()
        {
            return typeof(PerlinTextureMask);
        }

        public string GetMaskTypeName()
        {
            return "Perlin Noise Texture Mask";
        }

        public string GetTextureMaskTypeID()
        {
            return "VS_PerlinNoise";
        }
    }

    [Serializable]
    public class PerlinTextureMask : TextureMaskBase
    {
        public PerlinTextureMask()
        {
            Init();

            MaskName = "Perlin Noise Texture Mask";
            UseTiling = true;
            UseTexture = false;
            ShowInverseResult = true;
            TextureMaskTypeReadableName = "Perlin Noise Texture Mask";
            TextureMaskTypeId = "VS_PerlinNoise";
        }

        protected override float SampleTextureMask(UnityEngine.Vector2 normalizedPosition, List<TextureMaskProperty> itemTextureMaskPropertiesList)
        {
            return Mathf.PerlinNoise(normalizedPosition.x * Scale, normalizedPosition.y * Scale);
        }
    }
}
#endif
