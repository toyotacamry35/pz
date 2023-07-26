using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;
using UnityEngine;

namespace Shared.ManualDefsForSpells
{
    public class ImpactLaunchDef : SpellImpactDef
    {
        public ResourceRef<SpellVector3Def> Direction { get; set; }
        public ResourceRef<SpellEntityDef> From { get; set; }
        public UnityRef<GameObject> Prefab { get; set; }
        public SharedCode.Utils.Vector3 Force { get; set; }
        public SharedCode.Utils.Vector3 Offset { get; set; }
    }
}
