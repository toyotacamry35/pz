// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    [ProtoBuf.ProtoContract]
    [MongoDB.Bson.Serialization.Attributes.BsonDiscriminator("Destroyable")]
    public partial class Destroyable : SharedCode.EntitySystem.BaseDeltaObject, Assets.ColonyShared.SharedCode.Entities.IDestroyable, IDestroyableImplementRemoteMethods
    {
        public Destroyable()
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
            }

            constructor();
        }
    }
}