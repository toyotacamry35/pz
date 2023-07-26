using System;
using System.Collections.Generic;
using UnityEngine;

#if VEGETATION_STUDIO
namespace AwesomeTechnologies.Vegetation
{
    public class RegisterMegaSplatParamTextureMask : ITextureMask
    {
        public Type GetMaskType()
        {
            return typeof(MegaSplatParamTextureMask);
        }

        public string GetMaskTypeName()
        {
            return "Mega Splat Param(puddle) Mask";
        }

        public string GetTextureMaskTypeID()
        {
            return "VS_MegsSplatParam";
        }
    }

    [Serializable]
    public class MegaSplatParamTextureMask : TextureMaskBase
    {
        private Color32[] _textureColors;
        private int _textureWidth;
        private int _textureHeight;

        public MegaSplatParamTextureMask()
        {
            Init();

            MaskName = "Mega Splat Param Mask";
            UseTiling = false;
            UseTexture = true;
            TextureMaskTypeReadableName = "Mega Splat Param (puddle) Mask";
            TextureMaskTypeId = "VS_MegsSplatParam";           
        }

        protected override float SampleTextureMask(UnityEngine.Vector2 normalizedPosition, List<TextureMaskProperty> itemTextureMaskPropertiesList)
        {
            if (!MaskTexture) return 0;

            int xPos = Mathf.Clamp(Mathf.FloorToInt(normalizedPosition.x * _textureWidth), 0, _textureWidth - 1);
            int yPos = Mathf.Clamp(Mathf.FloorToInt(normalizedPosition.y * _textureHeight), 0, _textureHeight - 1);

            Color32 sampleColor = _textureColors[xPos + yPos * _textureWidth];

            return (float) sampleColor.a / 255;
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
