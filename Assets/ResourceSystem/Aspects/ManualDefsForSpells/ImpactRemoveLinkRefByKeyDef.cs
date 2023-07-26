using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ResourceSystem.Aspects.Links;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace ColonyShared.ManualDefsForSpells
{
    public class ImpactRemoveLinkRefByKeyDef : SpellImpactDef
    {
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public ResourceRef<SpellEntityDef> LinkedObject { get; set; }
        public ResourceRef<LinkTypeDef> LinkType { get; set; }
    }
}
