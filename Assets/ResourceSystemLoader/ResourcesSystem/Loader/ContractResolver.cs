using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Src.ResourcesSystem.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ResourcesSystem.Loader
{
    internal class ContractResolver : DefaultContractResolver
    {
        // As of 7.0.1, Json.NET suggests using a static instance for "stateless" contract resolvers, for performance reasons.
        // http://www.newtonsoft.com/json/help/html/ContractResolver.htm
        // http://www.newtonsoft.com/json/help/html/M_Newtonsoft_Json_Serialization_DefaultContractResolver__ctor_1.htm
        // "Use the parameterless constructor and cache instances of the contract resolver within your application for optimal performance."

        public static ContractResolver Instance { get; } = new ContractResolver();

        private ContractResolver()
        { }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);
            return properties?.OrderBy(p => p.DeclaringType.BaseTypesAndSelf().Count()).ToList();
        }
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty prop = base.CreateProperty(member, memberSerialization);

            if (member.DeclaringType == typeof(IRefBase) && prop.PropertyName == "Id")
                prop.Writable = true;

            return prop;
        }
    }

    internal static class TypeExtensions
    {
        public static IEnumerable<Type> BaseTypesAndSelf(this Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }
        
        public static Type GetContainerType(this Type type)
        {
            return type != null && type.IsNested ? type.DeclaringType : null;
        }
    }

}
