using System;
using System.Collections.Generic;
using UnityEngine;

#if VEGETATION_STUDIO
namespace AwesomeTechnologies.Vegetation
{
    public class RegisterMegaSplatTextureMask : ITextureMask
    {
        public Type GetMaskType()
        {
            return typeof(MegaSplatTextureMask);
        }

        public string GetMaskTypeName()
        {
            return "Mega Splat Control Mask";
        }

        public string GetTextureMaskTypeID()
        {
            return "VS_MegsSplatControl";
        }
    }

    [Serializable]
    public class MegaSplatTextureMask : TextureMaskBase
    {
        private Color32[] _textureColors;
        private int _textureWidth;
        private int _textureHeight;

        public MegaSplatTextureMask()
        {
            Init();

            MaskName = "Mega Splat Texture Mask";
            UseTiling = false;
            UseTexture = true;
            TextureMaskTypeReadableName = "Mega Splat Control Mask";
            TextureMaskTypeId = "VS_MegsSplatControl";

            TextureMaskProperty indexProperty =
                new TextureMaskProperty
                {
                    TextureMaskPropertyType = TextureMaskPropertyType.Integer,
                    IntMinValue = 0,
                    IntMaxValue = 255,
                    IntValue = 0,
                    PropertyName = "MegaSplatTextureIndex",
                    PropertyDescription = "Mega Splat Texture Index"
                };
            AddTextureMaskProperty(indexProperty);
        }

        protected override float SampleTextureMask(UnityEngine.Vector2 normalizedPosition, List<TextureMaskProperty> itemTextureMaskPropertiesList)
        {
            if (!MaskTexture) return 1;

            int xPos = Mathf.Clamp(Mathf.FloorToInt(normalizedPosition.x * _textureWidth), 0, _textureWidth - 1);
            int yPos = Mathf.Clamp(Mathf.FloorToInt(normalizedPosition.y * _textureHeight), 0, _textureHeight - 1);

            Color32 sampleColor = _textureColors[xPos + yPos * _textureWidth];

            int selectedIndex = GetIntPropertyValue("MegaSplatTextureIndex", itemTextureMaskPropertiesList);

            int index1 = sampleColor.r;
            int index2 = sampleColor.g;

            if (selectedIndex == index1 )
            {
                return 1 - (float) sampleColor.b / 255;
            }
            else if(selectedIndex == index2)
            {
                return (float) sampleColor.b /255;
            }
            else
            {
                return 0;
            }
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
