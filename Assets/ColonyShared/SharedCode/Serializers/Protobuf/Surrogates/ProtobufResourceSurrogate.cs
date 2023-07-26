using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using ProtoBuf;

namespace SharedCode.Serializers.Protobuf.Surrogates
{
    [ProtoContract]
    public class ProtobufResourceSurrogate<T> where T: IResource
    {
        private static ulong DefaultResourceIDFullCrc = default(ResourceIDFull).RootID();

        [ProtoMember(1)]
        public ulong ResourceID { get; set; }
        [ProtoMember(2)]
        public int Line { get; set; }
        [ProtoMember(3)]
        public int Col { get; set; }
        [ProtoMember(4)]
        public ushort ProtoIndex { get; set; }

        public static implicit operator ProtobufResourceSurrogate<T>(T resource)
        {
            var result = new ProtobufResourceSurrogate<T>();
            if (resource != null)
            {
                var id = GameResourcesHolder.Instance.GetIDWithCrc(resource);
                result.ResourceID = id.NetCrc64Id;
                result.Col = id.ResId.Col;
                result.Line = id.ResId.Line;
                result.ProtoIndex = (ushort)id.ResId.ProtoIndex;
            }

            return result;
        }

        public static implicit operator T(ProtobufResourceSurrogate<T> surrogate)
        {
            if (surrogate.ResourceID == DefaultResourceIDFullCrc)
                return default(T);
            var id = GameResourcesHolder.Instance.NetIDs.GetID(surrogate.ResourceID, (int)surrogate.Line, (int)surrogate.Col, (int)surrogate.ProtoIndex);
            return GameResourcesHolder.Instance.LoadResource<T>(id);
        }
    }
}
