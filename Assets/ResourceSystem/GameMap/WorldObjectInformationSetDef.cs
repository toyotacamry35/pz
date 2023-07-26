using System;
using System.Collections.Generic;
using System.Text;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Entities.GameMapData
{
    public class WorldObjectInformationSetDef : SaveableBaseResource
    {
        public string EntityTypeName { get; set; }

        public List<string> ObjectsTypeFilter { get; set; }

        public ResourceRef<PredicateDef> PredicateFilter { get; set; }

        public ResourceRef<SpellDef> SpellFilter { get; set; }
    }
}
