// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    [ProtoBuf.ProtoContract]
    [MongoDB.Bson.Serialization.Attributes.BsonDiscriminator("ContainerServiceEntity")]
    public partial class ContainerServiceEntity : SharedCode.EntitySystem.BaseEntity, SharedCode.Entities.Service.IContainerServiceEntity, IContainerServiceEntityImplementRemoteMethods
    {
        public override string CodeVersion => ThisAssembly.AssemblyInformationalVersion;
        public ContainerServiceEntity()
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
            }

            constructor();
        }

        public ContainerServiceEntity(System.Guid id): base(id)
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
            }

            constructor();
        }
    }
}