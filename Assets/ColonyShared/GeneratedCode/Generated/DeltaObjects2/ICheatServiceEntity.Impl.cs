// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    [ProtoBuf.ProtoContract]
    [MongoDB.Bson.Serialization.Attributes.BsonDiscriminator("CheatServiceAgentEntity")]
    public partial class CheatServiceAgentEntity : SharedCode.EntitySystem.BaseEntity, SharedCode.Entities.Service.ICheatServiceAgentEntity, ICheatServiceAgentEntityImplementRemoteMethods
    {
        public override string CodeVersion => ThisAssembly.AssemblyInformationalVersion;
        public CheatServiceAgentEntity()
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
            }

            constructor();
        }

        public CheatServiceAgentEntity(System.Guid id): base(id)
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
            }

            constructor();
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    [ProtoBuf.ProtoContract]
    [MongoDB.Bson.Serialization.Attributes.BsonDiscriminator("CheatServiceEntity")]
    public partial class CheatServiceEntity : SharedCode.EntitySystem.BaseEntity, SharedCode.Entities.Service.ICheatServiceEntity, ICheatServiceEntityImplementRemoteMethods
    {
        public override string CodeVersion => ThisAssembly.AssemblyInformationalVersion;
        public CheatServiceEntity()
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
            }

            constructor();
        }

        public CheatServiceEntity(System.Guid id): base(id)
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
            }

            constructor();
        }
    }
}