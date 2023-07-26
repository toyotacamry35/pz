using Assets.Src.RubiconAI.BehaviourTree.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    class Do : BehaviourNode
    {
        public override IEnumerable<BehaviourNode> GetSubNodes()
        {
            return new[] { _node };
        }
        ScriptResultType _typeOnChanceNotDoing;
        Metric _chanceMetric;
        BehaviourNode _node;
        protected override async ValueTask OnCreate(BehaviourNodeDef def)
        {
            var action = ((DoDef)def).Action.Target;
            _node = await HostStrategy.GetNode(this, action);
            _chanceMetric = (Metric)await ((DoDef)def).ChanceToDo.ExprOptional(HostStrategy);
            _typeOnChanceNotDoing = ((DoDef)def).ResultOnNotDoing;
        }
        static System.Random _rand = new System.Random();
        public override async ValueTask<ScriptResultType> OnStart()
        {
            if(_chanceMetric != null)
            {
                if(_rand.NextDouble() > await _chanceMetric.Evaluate(HostStrategy.CurrentLegionary))
                    return _typeOnChanceNotDoing;
            }
            return ScriptResultType.Running;
        }

        protected override async ValueTask<ScriptResultType> OnTick()
        {
            await _node.Run();
            return _node.LastStatus;
        }

        public override async ValueTask OnTerminate()
        {
            await HostStrategy.Terminate(_node);
        }
    }

}
