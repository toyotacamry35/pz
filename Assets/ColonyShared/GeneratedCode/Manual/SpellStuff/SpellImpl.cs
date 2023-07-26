using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ColonyShared.SharedCode.Utils;
using ResourceSystem.Aspects;
using SharedCode.Utils;
using SharedCode.Wizardry;

namespace GeneratedCode.DeltaObjects
{
    public partial class Spell
    {
        private bool _updating;
        private int _enterToUpdateAttempts;
        private SpellModifierDef[] _modifiers;
        
        bool ITimelineSpell.EnterToUpdate()
        {
            if (_updating)
            {
                _enterToUpdateAttempts++;
                if(_enterToUpdateAttempts % 20 == 0)
                    throw new Exception("Can't Enter To Update too many time");
                return false;
            }
            _enterToUpdateAttempts = 0;
            _updating = true;
            return true;
        }

        bool ITimelineSpell.ExitFromUpdate()
        {
            _updating = false;
            return _enterToUpdateAttempts > 0;
        }

        ISpellCast ITimelineSpell.CastData => CastData;

        IReadOnlyList<SpellModifierDef> ITimelineSpell.Modifiers => Modifiers;

        bool ITimelineSpell.IsFinished => this.IsFinished();
            
        long ITimelineSpell.StartAt => CastData.StartAt;

        SpellId ITimelineSpell.SpellId => Id;

        bool ITimelineSpell.IsAskedToFinish => this.IsAskedToFinish();

        SpellDef ITimelineSpell.SpellDef => CastData.Def;

        ITimelineSpellStatus ITimelineSpell.Status => Status;
        
        void ITimelineSpell.AskToFinish(long at, SpellFinishReason reason)
        {
            AskedToFinish = at;
            AskedToBeFinishedWithReason = reason;
        }

        void ITimelineSpell.Finish(long at, SpellFinishReason reason)
        {
            Finished = at;
            FinishReason = reason;
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool withSubSpells)
        {
            var sb = StringBuildersPool.Get;
                sb
                .Append("[")
                .Append("Id:").Append(Id)
                .Append(" ").Append(CastData.Def.____GetDebugAddress())
                .Append(" Status:").Append(Status.ToString(false, false));
                if(Finished != 0)
                    sb.Append(" FIN:").Append(FinishReason).Append(":").Append(Finished);
                else
                if(AskedToFinish != 0)
                    sb.Append(" ATF:").Append(AskedToBeFinishedWithReason).Append(":").Append(AskedToFinish);
                else
                if(StopCast != 0)
                    sb.Append(" STP:").Append(StopCastWithReason).Append(":").Append(StopCast);
                if (withSubSpells)
                {
                    sb.Append(" SubSpells:[");
                    if(Status.SubSpells != null)
                        foreach (var subSpell in Status.SubSpells)
                            sb.Append(subSpell.ToString(true, true));
                    sb.Append("] ");
                }
                sb.Append("]");
                return sb.ToStringAndReturn();
        }
    }
}