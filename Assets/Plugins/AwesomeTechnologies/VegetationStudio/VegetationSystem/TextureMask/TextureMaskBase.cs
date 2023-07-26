using System;
using System.Collections.Generic;
using UnityEngine;


namespace AwesomeTechnologies.Vegetation
{

    [Serializable]
    public enum TextureMaskRotation
    {
        None = 0,
        Rotate90Degrees = 1,
        Rotate180Degrees = 2,
        Rotate270Degrees = 3
    }

    [Serializable]
    public enum TextureMaskTextureStorage
    {
        VegetationPackage = 0,
        VegetationSystem = 1
    }

    [Serializable]
    public class LocalTextureMaskTextureInfo
    {
        public string Id;
        public Texture2D MaskTexture;
    }

    [Serializable]
    public enum TextureMaskPropertyType
    {
        Integer,
        Float,
        RgbaSelector,
        ColorSelector,
        Boolean,
        DropDownStringList
    }

    [Serializable]
    public class TextureMaskProperty
    {
        public TextureMaskPropertyType TextureMaskPropertyType;
        public string PropertyName;
        public string PropertyDescription;
        public int IntValue;
        public int IntMinValue;
        public int IntMaxValue;
        public float FloatValue;
        public float FloatMinValue;
        public float FloatMaxValue;
        public Color ColorValue;
        public bool BoolValue;
        public List<string> StringList = new List<string>();

        public TextureMaskProperty(TextureMaskProperty original)
        {
            TextureMaskPropertyType = original.TextureMaskPropertyType;
            PropertyName = original.PropertyName;
            PropertyDescription = original.PropertyDescription;
            IntValue = original.IntValue;
            IntMinValue = original.IntMinValue;
            IntMaxValue = original.IntMaxValue;
            FloatValue = original.FloatValue;
            FloatMinValue = original.FloatMinValue;
            FloatMaxValue = original.FloatMaxValue;
            ColorValue = original.ColorValue;
            BoolValue = original.BoolValue;
            StringList.AddRange(original.StringList);            
        }

        public TextureMaskProperty()
        {

        }
    }

    [Serializable]
    public class TextureMaskBase
    {
        public string MaskID = "";
        public bool UseTiling = true;
        public bool UseTexture = true;
        public bool ShowInverseResult;
        public bool InverseResult;
        public bool ShowRotateTexture;
        public TextureMaskRotation TextureMaskRotation = TextureMaskRotation.None;
        public string MaskName = "Base Mask";
        public string TextureMaskTypeReadableName = "";
        public string TextureMaskTypeId = "";
        public float Scale = 1;
        public Texture2D MaskTexture;
        public TextureMaskTextureStorage TextureMaskTextureStorage;
        public List<TextureFormat> RequiredTextureFormatList = new List<TextureFormat>();

        public List<TextureMaskProperty> TextureMaskPropertiesList = new List<TextureMaskProperty>();

        public void CopySettingsFrom(TextureMaskBase sourceTextureMask)
        {
            MaskID = sourceTextureMask.MaskID;
            UseTiling = sourceTextureMask.UseTiling;
            UseTexture = sourceTextureMask.UseTexture;
            MaskName = sourceTextureMask.MaskName;
            TextureMaskTypeReadableName = sourceTextureMask.TextureMaskTypeReadableName;
            TextureMaskTypeId = sourceTextureMask.TextureMaskTypeId;
            Scale = sourceTextureMask.Scale;
            MaskTexture = sourceTextureMask.MaskTexture;
            RequiredTextureFormatList.AddRange(sourceTextureMask.RequiredTextureFormatList);
            TextureMaskTextureStorage = sourceTextureMask.TextureMaskTextureStorage;
            InverseResult = sourceTextureMask.InverseResult;
            ShowInverseResult = sourceTextureMask.ShowInverseResult;
            ShowRotateTexture = sourceTextureMask.ShowRotateTexture;
            TextureMaskRotation = sourceTextureMask.TextureMaskRotation;
        }

        public void Init()
        {
            MaskID = Guid.NewGuid().ToString();
        }

        public void AddTextureMaskProperty(TextureMaskProperty textureMaskProperty)
        {
            TextureMaskPropertiesList.Add(textureMaskProperty);
        }

        public virtual float SampleTextureMaskScaled(UnityEngine.Vector2 normalizedPosition, List<TextureMaskProperty> itemTextureMaskPropertiesList)
        {
            UnityEngine.Vector2 rotatedPosition = UnityEngine.Vector2.zero;

            switch (TextureMaskRotation)
            {
                case TextureMaskRotation.None:
                    rotatedPosition = new UnityEngine.Vector2(normalizedPosition.x, normalizedPosition.y);
                    break;
                case TextureMaskRotation.Rotate90Degrees:
                    rotatedPosition = new UnityEngine.Vector2(1 - normalizedPosition.y,normalizedPosition.x);
                    break;
                case TextureMaskRotation.Rotate180Degrees:
                    rotatedPosition = new UnityEngine.Vector2(1- normalizedPosition.x, 1 - normalizedPosition.y);
                    break;
                case TextureMaskRotation.Rotate270Degrees:
                    rotatedPosition = new UnityEngine.Vector2(normalizedPosition.y, 1 - normalizedPosition.x);
                    break;
            }

            if (InverseResult)
            {
                return 1 - SampleTextureMask(rotatedPosition, itemTextureMaskPropertiesList);
            }
            else
            {
                return SampleTextureMask(rotatedPosition, itemTextureMaskPropertiesList);
            }
            

            //TUDO finished localized masks

            //float maskWidth = 0.3f;
            //Vector2 maskCorner = new Vector2(0.6f, 0.2f);

            //if (normalizedPosition.x < maskCorner.x || normalizedPosition.x > maskCorner.x + maskWidth) return 0;
            //if (normalizedPosition.y < maskCorner.y || normalizedPosition.y > maskCorner.y + maskWidth) return 0;

            //normalizedPosition -= maskCorner;
            //normalizedPosition /= maskWidth;
                      
          
        }

        protected virtual float SampleTextureMask(UnityEngine.Vector2 normalizedPosition, List<TextureMaskProperty> itemTextureMaskPropertiesList)
        {
            return 0f;
        }

        public virtual void RefreshTextureMask()
        {

        }

        public int GetIntPropertyValue(string propertyName, List<TextureMaskProperty> itemTextureMaskPropertiesList)
        {
            for (int i = 0;
                i <= itemTextureMaskPropertiesList.Count - 1; i++)
            {
                if (itemTextureMaskPropertiesList[i].PropertyName == propertyName)
                {
                    return itemTextureMaskPropertiesList[i].IntValue;
                }
            }

            return 0;
        }

        public Color GetColorPropertyValue(string propertyName, List<TextureMaskProperty> itemTextureMaskPropertiesList)
        {
            for (int i = 0;
                i <= itemTextureMaskPropertiesList.Count - 1; i++)
            {
                if (itemTextureMaskPropertiesList[i].PropertyName == propertyName)
                {
                    return itemTextureMaskPropertiesList[i].ColorValue;
                }
            }

            return Color.white;
        }

        public bool GetBooleanPropertyValue(string propertyName, List<TextureMaskProperty> itemTextureMaskPropertiesList)
        {
            for (int i = 0;
                i <= itemTextureMaskPropertiesList.Count - 1; i++)
            {
                if (itemTextureMaskPropertiesList[i].PropertyName == propertyName)
                {
                    return itemTextureMaskPropertiesList[i].BoolValue;
                }
            }

            return false;
        }
    }
}

