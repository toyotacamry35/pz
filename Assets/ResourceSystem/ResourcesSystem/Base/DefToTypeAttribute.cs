using System;
using SharedCode.Wizardry;

namespace Assets.Src.ResourcesSystem.Base
{
    public class DefToTypeAttribute : Attribute
    {
        /// <summary>
        /// FullName типа
        /// </summary>
        public readonly string TypeName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typename">FullName типа</param>
        public DefToTypeAttribute(string typename)
        {
            TypeName = typename;
        }
    }
    
    public class TypeToDefAttribute : Attribute
    {
        public readonly Type DefType;

        public TypeToDefAttribute(Type type)
        {
            if (!typeof(ISpellParameterDef).IsAssignableFrom(type)) throw new ArgumentException($"Wrong spell parameter type {type}");
            DefType = type;
        }
    }
}