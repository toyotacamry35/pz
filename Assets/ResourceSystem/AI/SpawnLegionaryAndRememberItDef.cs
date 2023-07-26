using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;
using Assets.Src.RubiconAI.KnowledgeSystem.Memory;
using UnityEngine;
using Vector3 = SharedCode.Utils.Vector3;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    public class SpawnLegionaryAndRememberItDef : BehaviourNodeDef
    {
        public ResourceRef<MemorizedStatDef> AsStat { get; set; }
        public UnityRef<GameObject> Prefab { get; set; }
        public float ForTime { get; set; }
        public Vector3 Offset { get; set; }
        public ResourceRef<TargetSelectorDef> Target { get; set; }
        public ResourceRef<TargetSelectorDef> Legion { get; set; }
        public ResourceRef<TargetSelectorDef> MemoryOf { get; set; }
    }
}
