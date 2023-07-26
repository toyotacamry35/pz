using GeneratorAnnotations;
using Scripting;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using System.Collections.Generic;
using GeneratedCode.DeltaObjects;
using ResourceSystem.Aspects;

namespace SharedCode.Wizardry
{
    [GenerateDeltaObjectCode]
    public interface ISpell : IDeltaObject, ITimelineSpell
    {
        [ReplicationLevel(ReplicationLevel.Master)]
        long AskedToFinish { get; set; }
        [ReplicationLevel(ReplicationLevel.Master)]
        new SpellFinishReason AskedToBeFinishedWithReason { get; set; }
        [ReplicationLevel(ReplicationLevel.Master)]
        new long StopCast { get; set; }
        [ReplicationLevel(ReplicationLevel.Master)]
        new SpellFinishReason StopCastWithReason { get; set; }
        [ReplicationLevel(ReplicationLevel.Master)]
        new ISpellStatus Status { get; set; } //used to recreate spell state after teleportation

        [OverrideSerializeSettings(DynamicType = true, AsReference = false)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [LockFreeReadonlyProperty]
        new SpellCast CastData { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [LockFreeReadonlyProperty]
        new IDeltaList<SpellModifierDef> Modifiers { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        SpellFinishReason FinishReason { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        SpellId Id { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        long Finished { get; set; } //means that all impacts on end has been called, as well as effects
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        long Started { get; set; } // means that predicates were checked
        [ReplicationLevel(ReplicationLevel.Master)] 
        SpellPartCastId Causer { get; set; } //TODO: сделать более универсальным
        
        [RuntimeData] SpellStateValidator Validator { get; set; }
        //[RuntimeData] SpellTimeLineData TimeLineData { get; set; }
        //[RuntimeData] ISpellExecutable Executable { get; set; }
    }
    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface ISpellSlot : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Master)]
        ISpell CurrentSpell { get; set; }
        [ReplicationLevel(ReplicationLevel.Master)]
        ISpell NextChainedSpell { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface ISpellStatus : IDeltaObject, ITimelineSpellStatus
    {
        new SubSpell SubSpell { get; set; }
        SpellDef Spell { get; set; }
        IDeltaList<(SpellWordDef, int)> ActivationsPerWord { get; set; } //so I won't finish effects that were not started in the first place
        new int SuccesfullPredicatesCheckCount { get; set; }
        new int FailedPredicatesCheckCount { get; set; }
        new int ActivationsCount { get; set; }
        new int DeactivationsCount { get; set; }
        new int SuccesfullActivationsCount { get; set; }
        new IDeltaList<ISpellStatus> SubSpells { get; set; }
        int Activations { get; set; }
        long AccumulatedDelta { get; set; }
        long LastTimeUpdated { get; set; }
    }
    
    public static class SpellExtensions
    {
        public static bool IsFinished(this ISpell spell)
        {
            return spell.Finished != 0;
        }
        public static bool IsStarted(this ISpell spell)
        {
            return spell.Started != 0;
        }
        public static bool IsAskedToFinish(this ISpell spell)
        {
            return spell.AskedToFinish != 0;
        }
    }
}
