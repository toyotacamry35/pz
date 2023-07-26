using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using UnityEngine;

namespace SharedCode.Aspects.Building
{
    public class FenceLinkDef : BaseResource
    {                                     
        public float SelfDestructTime { get; set; } // self destruct time for destruction animation
        public Utils.Vector3 Center { get; set; } // box collider center
        public Utils.Vector3 Extents { get; set; } // box collider extents + distance from center to link edges (Z)
        public UnityRef<GameObject> Prefab { get; set; } // visual prefab
        public ResourceRef<UnityGameObjectDef> PrefabDef { get; set; } // additional visual prefab def
        public Utils.Vector3 LinkPoint { get; set; }
        public int MinLinks { get; set; }
    }
}
