// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    [ProtoBuf.ProtoContract]
    [MongoDB.Bson.Serialization.Attributes.BsonDiscriminator("ChainCallServiceEntityExternal")]
    public partial class ChainCallServiceEntityExternal : SharedCode.EntitySystem.BaseEntity, SharedCode.Entities.Service.IChainCallServiceEntityExternal, IChainCallServiceEntityExternalImplementRemoteMethods
    {
        public override string CodeVersion => ThisAssembly.AssemblyInformationalVersion;
        public ChainCallServiceEntityExternal()
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
            }

            constructor();
        }

        public ChainCallServiceEntityExternal(System.Guid id): base(id)
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
            }

            constructor();
        }
    }
}