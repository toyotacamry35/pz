using System.Collections.Generic;
using ColonyShared.SharedCode.EntitySystem;

namespace SharedCode.Wizardry
{
    /// <summary>
    ///     состояние выполнения спелла или сабспелла c сылками на состояния выполнения вложенных сабспеллов
    /// </summary>
    [CodeGenIgnore]
    public interface ITimelineSpellStatus
    {
        SpellDef SpellDef { get; }
        
        int SuccesfullPredicatesCheckCount { get; set; }
        
        int FailedPredicatesCheckCount { get; set; }
        
        int ActivationsCount { get; set; }
        
        int DeactivationsCount { get; set; }
        
        int SuccesfullActivationsCount { get; set; }
        
        SubSpell SubSpell { get; }
        
        IEnumerable<ITimelineSpellStatus> SubSpells { get; }
        
        SpellTimeLineData TimeLineData { get; set; }
        
        IEnumerable<(SpellWordDef,int)> WordActivations { get; }
        
        void IncrementWordActivations(SpellWordDef word, int activationIdx);
        
        bool DecrementWordActivations(SpellWordDef word);

        string ToString(bool withDef, bool withSubSpells);
    }


    public static class SpellExecutableStatusExtensions
    {
        public static bool IsActive(this ITimelineSpellStatus status) => status.ActivationsCount > status.DeactivationsCount;

        public static int FailedActivationsCount(this ITimelineSpellStatus status) => status.ActivationsCount - status.SuccesfullActivationsCount;
    }
}