using Assets.ResourceSystem.Aspects.Links;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyShared.ManualDefsForSpells
{
    public class ImpactRemoveLinkRefDef : SpellImpactDef
    {
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public ResourceRef<SpellEntityDef> LinkedObject { get; set; }
    }
}
