using System.Collections.Generic;
using Assets.Src.Aspects.Doings;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.AI;

namespace ResourceSystem.Aspects.Misc
{
    public class BotsIndexDef : BaseResource
    {
        public Dictionary<string,ResourceRef<LegionaryEntityDef>> Index;
    }
}
