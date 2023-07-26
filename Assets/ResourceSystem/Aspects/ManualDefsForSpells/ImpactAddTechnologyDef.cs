using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.ResourceSystem.Aspects.Links;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Science;
using SharedCode.Utils;
using SharedCode.Wizardry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ResourceSystem.Aspects.ManualDefsForSpells
{
    public class ImpactAddTechnologyDef : SpellImpactDef
    {
        public ResourceRef<TechnologyDef> Technology;
    }
}
