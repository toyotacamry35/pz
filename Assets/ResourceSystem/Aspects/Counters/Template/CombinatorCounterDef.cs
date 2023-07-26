using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourcesSystem.Base;

namespace ResourceSystem.Aspects.Counters.Template
{
    public class CombinatorCounterDef : CounteredQuestCounterDef
    {
        public ResourceRef<QuestCounterDef>[] SubCounters { get; set; }
    }
}
