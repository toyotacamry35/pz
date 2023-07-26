using Assets.ColonyShared.SharedCode.Aspects;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.ResourceSystem.Arithmetic.Templates.Predicates;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Wizardry;
using UnityEngine;

namespace Assets.ResourceSystem.Aspects.Misc
{
    public class RiverExportSettingsDef : BaseResource
    {
        public ResourceRef<ToxicRiverSettingsDef> ToxicRiverSettings { get; set; }
        public ResourceRef<ClearRiverSettingsDef> ClearRiverSettings { get; set; }
        public UnityRef<GameObject> GuiBadgePrefab { get; set; }
    }

    public class RiverSettingsDef : BaseResource
    {
        public ResourceRef<NonEntityObjectDef> InteractionObject { get; set; }
        public ResourceRef<SpellDef> SpatialSpellForFlask { get; set; }
        public ResourceRef<PredicateIgnoreGroupDef> PredicateIgnoreGroup { get; set; }
    }

    public class ToxicRiverSettingsDef : RiverSettingsDef { }

    public class ClearRiverSettingsDef : RiverSettingsDef { }
}
