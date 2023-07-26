using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Aspects.FX.Decals;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Src.FX.Decals
{
    public class FxResourceSetDef : BaseResource
    {
        public List<HitDecalInfo> _fxResources;
    }

    public class HitDecalInfo : BaseResource
    {
        public ResourceRef<IHitDecalPlacerDef> _hitDecalPlacer { get; set; }
        public ResourceRef<DamageTypeDef> _damageType;
        public UnityRef<GameObject> _decal;
    }
}
