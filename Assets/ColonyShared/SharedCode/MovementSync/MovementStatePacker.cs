using System;
using Assets.ColonyShared.SharedCode.Utils;
using ProtoBuf;
using SharedCode.Utils;


namespace SharedCode.MovementSync
{
    public static class MovementStatePacker
    {
        public static byte PackAngle(float angle) =>  
            (byte)(SharedHelpers.Repeat(angle, SharedHelpers.DoublePi) * 255f / SharedHelpers.DoublePi);

        public static float UnpackAngle(byte data) => 
            data * (SharedHelpers.DoublePi / 255f);
        
        public static PackedVector3 PackVector3(Vector3 v) 
            => new PackedVector3 { x = HalfUtilities.Pack(v.x), y = HalfUtilities.Pack(v.y), z = HalfUtilities.Pack(v.z) };

        public static Vector3 UnpackVector3(PackedVector3 v) 
            => new Vector3(HalfUtilities.Unpack(v.x), HalfUtilities.Unpack(v.y), HalfUtilities.Unpack(v.z));

        public static UInt16 PackAngularVelocity(float value) =>  
            HalfUtilities.Pack(value);

        public static float UnpackAngularVelocity(UInt16 data) => 
            HalfUtilities.Unpack(data);

        [ProtoContract]
        public struct PackedVector3
        {
            [ProtoMember(1)] public ushort x;
            [ProtoMember(2)] public ushort y;
            [ProtoMember(3)] public ushort z;
        }
    }
}