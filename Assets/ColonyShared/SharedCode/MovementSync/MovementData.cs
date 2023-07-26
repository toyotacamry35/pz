using System;
using GeneratedCode.DeltaObjects;
using GeneratedDefsForSpells;
using SharedCode.Wizardry;
using ProtoBuf;

namespace SharedCode.MovementSync
{
    [ProtoContract]
    public struct MovementData : IEquatable<MovementData>
    {
        [ProtoMember(1)] public SpellId SpellId { get; set; }
        [ProtoMember(2)] public MoveEffectDef MoveEffectDef { get; set; }
        [ProtoMember(3)] public SpellCast CastData { get; set; }
        [ProtoMember(4)] public bool Start { get; set; } // true => start, false => finish

        [ProtoIgnore]
        public bool IsValid => SpellId.IsValid; // проверка только по SpelId по причине того, что у protobuff проблема с сериализацией null'ей 

        public static MovementData StartData(SpellId spellId, MoveEffectDef moveEffectDef, SpellCast castData) => 
            new MovementData(true, spellId, moveEffectDef, castData);

        public static MovementData FinishData(SpellId spellId, MoveEffectDef moveEffectDef, SpellCast castData) => 
            new MovementData(false, spellId, moveEffectDef, castData);

        private MovementData(bool start, SpellId spellId, MoveEffectDef moveEffectDef, SpellCast castData)
        {
            Start = start;
            SpellId = spellId.IsValid ? spellId : throw new ArgumentException(nameof(spellId));
            MoveEffectDef = moveEffectDef ?? throw new ArgumentNullException(nameof(moveEffectDef));
            CastData = castData ?? throw new ArgumentNullException(nameof(castData));
        }
        
        public bool Equals(MovementData other)
        {
            return SpellId.Equals(other.SpellId) && MoveEffectDef == other.MoveEffectDef && Start == other.Start;
        }

        public override bool Equals(object obj)
        {
            return obj is MovementData other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = SpellId.GetHashCode();
                hashCode = (hashCode * 397) ^ (MoveEffectDef != null ? MoveEffectDef.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Start.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"({(Start ? "Start" : "Finish")} SpellId:{SpellId} Spell:{CastData?.Def?.____GetDebugAddress()} Effect:{MoveEffectDef?.____GetDebugAddress()} CastData:{CastData})";
        }
    }
}
