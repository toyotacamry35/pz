using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;
using UnityEngine;

namespace Shared.ManualDefsForSpells
{
    public class EffectCreateGameObjectAtPointDef : SpellEffectDef
    {
        public ResourceRef<SpellEntityDef> OptionalParentObj { get; set; }
        public ResourceRef<SpellVector3Def> AtPoint { get; set; }
        public UnityRef<GameObject> Prefab { get; set; }
    }
}