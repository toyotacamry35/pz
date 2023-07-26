using System.Collections.Generic;
using ColonyShared.SharedCode.EntitySystem;
using ColonyShared.SharedCode.Utils;
using ResourceSystem.Aspects;
using Scripting;

namespace SharedCode.Wizardry
{
    [CodeGenIgnore]
    public interface ITimelineSpell
    {
        /// <summary>
        /// Проверяет. что спелл не обновляется в текущий момент и помечает его обновляющимся. Возвращает false, если спелл уже обновляется/  
        /// </summary>
        bool EnterToUpdate();

        /// <summary>
        /// Снимает пометку обновления со спелла. Возвращает true, если кто-то попытался начать обновление во время текущего 
        /// </summary>
        bool ExitFromUpdate();

        bool IsFinished { get; }

        long StartAt { get; }

        SpellId SpellId { get; }

        SpellDef SpellDef { get; }

        ISpellCast CastData { get; }

        IReadOnlyList<SpellModifierDef> Modifiers { get; }

        ITimelineSpellStatus Status { get; }

        long StopCast { get; }

        SpellFinishReason StopCastWithReason { get; }

        bool IsAskedToFinish { get; }

        SpellFinishReason AskedToBeFinishedWithReason { get; }

        void AskToFinish(long at, SpellFinishReason reason);

        void Finish(long at, SpellFinishReason reason);
        
        string ToString(bool withSubSpells);
    }
    
    public static class TimelineSpellExtensions
    {
        public static bool ItBlocksCastOf(this ITimelineSpell spell, SpellDef newSpell, SpellId ignoreSpell)
        {
            if (ignoreSpell.IsValid && spell.SpellId == ignoreSpell)
                return false;
            
            return !newSpell.AllowMultiple &&
                   spell.StopCast == 0 && 
                   !spell.IsAskedToFinish && 
                   !newSpell.ClearsSlot && (spell.CastData.Def == newSpell || spell.CastData.Def.Slot == newSpell.Slot && newSpell.Slot.Target != null);

//            return spell.StopCast == 0 && !spell.IsAskedToFinish &&
//                   (spell.SpellDef == newSpell && !newSpell.AllowMultiple || spell.SpellDef != newSpell && spell.SpellDef.Slot == newSpell.Slot && newSpell.Slot.Target != null && !newSpell.ClearsSlot);            
        }
    }
}