using Assets.ColonyShared.SharedCode.Aspects.Navigation;
using ProtoBuf;
using SharedCode.Utils;

namespace Assets.ColonyShared.SharedCode.Aspects.WorldObjects
{
    [ProtoContract]
    public class PointMarker
    {
        [ProtoMember(1)]
        public Vector3 Position;

        [ProtoMember(2)]
        public NavIndicatorDef NavIndicatorDef;
    }
}