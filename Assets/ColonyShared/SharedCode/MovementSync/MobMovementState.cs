using System;
using System.Linq;
using ColonyShared.SharedCode.Aspects.Locomotion;
using ProtoBuf;
using SharedCode.Entities.GameObjectEntities;
using Quaternion = SharedCode.Utils.Quaternion;
using Vector3 = SharedCode.Utils.Vector3;
using static SharedCode.MovementSync.MovementStatePacker;

namespace SharedCode.MovementSync
{    
    public struct MobMovementState : IPointSpatialHashType
    {
        public readonly Vector3 Position;

        public readonly Vector3 Velocity;

        public readonly float Orientation;

        public readonly Quaternion Rotation;

        public readonly float AngularVelocity;
        
        public readonly LocomotionFlags Flags;

        public IEntityObjectDef Def { get; set; } ///Task #PZ-9627: #TODO: Разделить на 2 типа - отдельный для ~spatialHashGrid'а

        public MobMovementState(Vector3 position, Vector3 velocity, float orientation, float angularVelocity, LocomotionFlags flags)
        {
            Position = position;
            Velocity = velocity;
            Orientation = orientation;
            Rotation = Quaternion.EulerRad(0, orientation, 0);
            AngularVelocity = angularVelocity;
            Flags = flags;
            Def = null;
        }

        public MobMovementState(Vector3 position, Vector3 velocity, Quaternion rotation, float angularVelocity, LocomotionFlags flags)
        {
            Position = position;
            Velocity = velocity;
            Orientation = rotation.eulerAnglesRad.y;
            AngularVelocity = angularVelocity;
            Rotation = rotation;
            Flags = flags;
            Def = null;
        }

        public int StaticCellSize => 100;

        public int StaticHeightSize => 200;

        public int StaticReplicationRadius => 100;

        public bool ShouldCheckForRadius => true;
    }


    [ProtoContract]
    public struct MobMovementStatePacked
    {
        [ProtoMember(1)] public long _counter;
        [ProtoMember(2)] public Vector3 _position;
        [ProtoMember(3)] public PackedVector3 _velocity;
        [ProtoMember(4)] public byte _orientation;
        [ProtoMember(5)] public UInt16 _flags;
        [ProtoMember(6)] public UInt16 _angularVelocity;

        [ProtoIgnore] public Vector3 Position => _position;
        [ProtoIgnore] public long Counter => _counter;
        
        public static MobMovementStatePacked Pack(MobMovementState state, long counter)
        {
            return new MobMovementStatePacked
            {
                _counter = counter,
                _position = state.Position,
                _velocity = PackVector3(state.Velocity),
                _orientation = PackAngle(state.Orientation),
                _angularVelocity = PackAngularVelocity(state.AngularVelocity),
                _flags = (UInt16) state.Flags
            };
        }

        public static MobMovementState Unpack(MobMovementStatePacked data)
        {
            return new MobMovementState(
                position: data._position, 
                velocity: UnpackVector3(data._velocity), 
                orientation: UnpackAngle(data._orientation), 
                angularVelocity: UnpackAngularVelocity(data._angularVelocity), 
                flags: (LocomotionFlags)data._flags
                );
        }

        static MobMovementStatePacked()
        {
            if ((uint) Enum.GetValues(typeof(LocomotionFlags)).Cast<LocomotionFlags>().Max() > ushort.MaxValue)
                throw new ArgumentOutOfRangeException($"{nameof(LocomotionFlags)} has too big value for storing in UInt16", nameof(_flags));
        }
    }
}