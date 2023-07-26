using System;
using Newtonsoft.Json.Serialization;

namespace ResourcesSystem.Loader
{
    public class KnownTypesBinder : ISerializationBinder
    {
        public static KnownTypesBinder Instance { get; } = new KnownTypesBinder();

        private KnownTypesBinder() { }

        public Type BindToType(string assemblyName, string typeName)
        {
            if (!KnownTypesCollector.KnownTypes.TryGetValue(typeName, out var type))
                GameResources.ThrowError($"Type {typeName} is unknown to resource system");
            return type;
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }
    }

}
