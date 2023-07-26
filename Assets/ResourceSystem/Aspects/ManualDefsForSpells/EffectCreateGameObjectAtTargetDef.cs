using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;
using UnityEngine;
using Vector3 = SharedCode.Utils.Vector3;

namespace Shared.ManualDefsForSpells
{
    public class EffectCreateGameObjectAtTargetDef : SpellEffectDef
    {
        public ResourceRef<SpellEntityDef> TargetToPlaceFxAtItsPosition { get; set; }
        public ResourceRef<SpellEntityDef> OptionalParentObj { get; set; }
        public Vector3 LocalPosition { get; set; }
        public UnityRef<GameObject> Prefab { get; set; }
    }
}