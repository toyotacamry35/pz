using System.Collections.Generic;
using Assets.Src.Tools;
using ColonyShared.SharedCode.Utils;
using SharedCode.EntitySystem.Delta;
using SharedCode.Utils;
using SharedCode.Wizardry;

namespace GeneratedCode.DeltaObjects
{
    public partial class SpellStatus
    {
        SpellDef ITimelineSpellStatus.SpellDef => Spell;

        SpellTimeLineData ITimelineSpellStatus.TimeLineData { get; set; }

        IEnumerable<ITimelineSpellStatus> ITimelineSpellStatus.SubSpells => SubSpells;

        public IEnumerable<(SpellWordDef, int)> WordActivations => ActivationsPerWord;

        void ITimelineSpellStatus.IncrementWordActivations(SpellWordDef word, int activationIdx)
        {
            (ActivationsPerWord ?? (ActivationsPerWord = new DeltaList<(SpellWordDef, int)>())).Add((word, activationIdx));
        }

        bool ITimelineSpellStatus.DecrementWordActivations(SpellWordDef word)
        {
            if (ActivationsPerWord != null)
                for(int idx = ActivationsPerWord.Count - 1; idx >= 0; --idx)
                    if (ActivationsPerWord[idx].Item1 == word)
                    {
                        ActivationsPerWord.RemoveAt(idx);
                        return true;
                    }
            return false;
        }

        public override string ToString()
        {
            return ToString(true, false);
        }

        public string ToString(bool withDef, bool withSubSpells)
        {
            var sb = StringBuildersPool.Get.Append("[");
            if (withDef)
                sb.Append(SubSpell != null ? SubSpell.____GetDebugAddress() : Spell?.____GetDebugAddress()).Append(" ");
            sb.Append("A:").Append(ActivationsCount).Append("/").Append(this.FailedActivationsCount());
            sb.Append(" D:").Append(DeactivationsCount);
            if (withSubSpells)
            {
                sb.Append(" SubSpells:[");
                if(SubSpells != null)
                    foreach (var subSpell in SubSpells)
                        sb.Append(subSpell.ToString(true, true));
                sb.Append("] ");
            }
            return sb.Append("]").ToStringAndReturn();
        }
    }
}