using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;
using ColonyShared.ManualDefsForSpells;
using ColonyShared.SharedCode.InputActions;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities.GameObjectEntities;
using System.Collections.Generic;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    public class MoveBotDef : BehaviourNodeDef
    {
        public float SmoothTime { get; set; } = 0f;
        public float TimeoutSeconds { get; set; } = 0f; //<=0 - infinity timeout
        public ResourceRef<TargetSelectorDef> Target { get; set; }
    }
}
