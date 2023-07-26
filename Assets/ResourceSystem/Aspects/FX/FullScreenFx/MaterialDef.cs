using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using UnityEngine;

namespace Assets.ResourceSystem.Aspects.FX.FullScreenFx
{
    public class MaterialDef : BaseResource
    {
        public UnityRef<Material> Material { get; set; }
    }
}