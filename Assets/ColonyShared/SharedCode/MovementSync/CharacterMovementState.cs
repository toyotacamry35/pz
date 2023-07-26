using Assets.ColonyShared.SharedCode.Utils;
using ColonyShared.SharedCode.Aspects.Locomotion;
using ColonyShared.SharedCode.Utils;
using ProtoBuf;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Utils;
using Quaternion = SharedCode.Utils.Quaternion;
using Vector3 = SharedCode.Utils.Vector3;
using static SharedCode.MovementSync.MovementStatePacker;

namespace SharedCode.MovementSync
{
    [ProtoContract]
    public struct CharacterMovementState : IPointSpatialHashType
    {
        [ProtoMember(1)]
        public Vector3 _position;
        [ProtoMember(2)]
        public PackedVector3 _velocity;
        [ProtoMember(3)]
        public byte _orientation;
        [ProtoMember(4)]
        public ushort _flags;

        public CharacterMovementState(Vector3 position, Vector3 velocity, float orientation, LocomotionFlags flags, IEntityObjectDef def = null)
        {
            _position = position;
            _velocity = PackVector3(velocity);
            _orientation = PackAngle(orientation);
            _flags = (ushort)flags;
            Def = def;
        }

        [ProtoIgnore]
        public Vector3 Position { get { return _position; } set { _position = value; } }
        
        [ProtoIgnore]
        public Vector3 Velocity { get { return UnpackVector3(_velocity); } set { _velocity = PackVector3(value); } }

        [ProtoIgnore]
        public float Orientation { get { return UnpackAngle(_orientation); } set { _orientation = PackAngle(value); } }

        [ProtoIgnore]
        public Quaternion Rotation { get { return Quaternion.EulerRad(0, Orientation, 0); } set { Orientation = value.eulerAnglesRad.y; } }
            
        [ProtoIgnore]
        public LocomotionFlags Flags { get { return (LocomotionFlags) _flags; } set { _flags = (ushort) value; } }

        public int StaticCellSize => 100;

        public int StaticHeightSize => 200;

        public int StaticReplicationRadius => 100;

        public bool ShouldCheckForRadius => true;

        public IEntityObjectDef Def { get; set; }

        public override string ToString()
        {
            return StringBuildersPool.Get
                .Append(Flags).Append(" | ")
                .Append(Position).Append(" | ")
                .Append(Velocity).Append(" | ")
                .Append(Orientation * SharedHelpers.Rad2Deg)
                .ToStringAndReturn();
        }
    }

    [ProtoContract]
    public struct CharacterMovementStateFrame
    {
        // It's timestamp actually
        [ProtoMember(1)]  public long Counter;
        [ProtoMember(2)]  public CharacterMovementState State;

        public CharacterMovementStateFrame(long counter, CharacterMovementState state)
        {
            Counter = counter;
            State = state;
        }
    }
}
