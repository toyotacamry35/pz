// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    [ProtoBuf.ProtoContract]
    [MongoDB.Bson.Serialization.Attributes.BsonDiscriminator("SlaveWizardHolder")]
    public partial class SlaveWizardHolder : SharedCode.EntitySystem.BaseDeltaObject, SharedCode.Wizardry.ISlaveWizardHolder
    {
        public SlaveWizardHolder()
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
            }

            constructor();
        }
    }
}