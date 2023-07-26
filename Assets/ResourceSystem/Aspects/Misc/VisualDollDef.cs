using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using UnityEngine;

namespace ResourceSystem.Aspects.Misc
{
    public class VisualDollDef : GameObjectMarkerDef
    {
        public UnityRef<GameObject> Prefab;
    }
}