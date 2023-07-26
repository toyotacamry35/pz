using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwesomeTechnologies.Common.Interfaces
{
    [System.Serializable]
    public enum VegetationItemShaderPropertyType
    {
        Int = 0,
        Float = 1
    }

    [System.Serializable]
    public class VegetationItemShaderProperty
    {
        public string PropertyName;
        public string DisplayName;
        public VegetationItemShaderPropertyType ShaderPropertyType;
        public int IntValue;
        public float FloatValue;
    }

    [System.Serializable]
    public class VegetationItemShaderInterface
    {
        public string ShaderName;
        public string DisplayName;
        public List<VegetationItemShaderProperty> ShaderProperties = new List<VegetationItemShaderProperty>();
    }

#if VEGETATION_STUDIO
    [System.Serializable]
    public class RockShaderInterface : VegetationItemShaderInterface
    {
        void Awake()
        {
            ShaderName = "RockCompany/DiffuseRockShader";
            DisplayName = "Custom Rocks";
            ShaderProperties.Add(new VegetationItemShaderProperty(){PropertyName = "_SnowAmount",DisplayName="Snow Amount", FloatValue = 0.5f, ShaderPropertyType = VegetationItemShaderPropertyType.Float});
        }
    }
#endif
}
