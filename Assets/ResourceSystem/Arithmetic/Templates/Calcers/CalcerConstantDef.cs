using System;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers
{
    public class CalcerConstantDef<ReturnType> : CalcerDef<ReturnType>
    {
        public CalcerConstantDef() 
        {}

        public CalcerConstantDef(ReturnType value)
        {
            Value = value;
        }
    
        public ReturnType Value { get; set; }
    }

    // В JDB, вместо CalcerConstant<Resource> нужно писать CalcerResource 
    public class CalcerResourceDef : CalcerDef<BaseResource>
    {
        public CalcerResourceDef() 
        {}

        public CalcerResourceDef(BaseResource value)
        {
            Value = value;
        }
    
        public ResourceRef<BaseResource> Value { get; set; }
    }

    
    [Obsolete("For backward compatibility. Use CalcerConstantDef<float> instead")]
    public class CalcerConstantDef : CalcerConstantDef<float>
    {}
}
