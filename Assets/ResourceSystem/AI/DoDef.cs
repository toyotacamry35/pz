using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    public class DoDef : BehaviourNodeDef
    {
        //public ResourceRef<MetricDef> TimeLimit { get; set; }
        public ResourceRef<BehaviourNodeDef> Action { get; set; }
        public ResourceRef<MetricDef> ChanceToDo { get; set; }
        public ScriptResultType ResultOnNotDoing { get; set; } = ScriptResultType.Succeeded;
    }
}
