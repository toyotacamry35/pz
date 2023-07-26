using System;
using System.Collections.Generic;
using System.Text;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.ResourcesSystem.Base;
using Entities.GameMapData;
using SharedCode.Wizardry;

namespace Entities.GameMapData
{
    public class WorldObjectInformationClientSubSetDef : SaveableBaseResource
    {
        public ResourceRef<WorldObjectInformationSetDef> DataSet { get; set; }

        public ResourceRef<PredicateDef> PredicateFilter { get; set; }

        public ResourceRef<SpellDef> SpellFilter { get; set; }

        public ResourceRef<GameMapWorldObjectVisualizerDef> Visualizer { get; set; }
    }
}
