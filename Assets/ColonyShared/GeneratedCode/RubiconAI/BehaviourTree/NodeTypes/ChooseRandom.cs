using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;
using Assets.Src.Tools;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    class ChooseRandom : BehaviourNode
    {
        private ChooseRandomDef _def;
        public override IEnumerable<BehaviourNode> GetSubNodes()
        {
            return _nodes.Select(x => x.Node);
        }
        class WeightedActionRuntime
        {
            public BehaviourNode Node;
            public Metric WeightMetric;
        }
        List<WeightedActionRuntime> _nodes = new List<WeightedActionRuntime>();
        private BehaviourNode _selectedNode;
        protected override async ValueTask OnCreate(BehaviourNodeDef def)
        {
            _def = (ChooseRandomDef)def;
            _nodes.Clear();
            foreach (var resourceRef in _def.Actions)
            {
                var node = await HostStrategy.GetNode(this, resourceRef);
                _nodes.Add(new WeightedActionRuntime() { Node = node });
            }
            foreach (var resourceRef in _def.WeightedActions)
            {
                var node = await HostStrategy.GetNode(this, resourceRef.Action);
                var weightMetric = await resourceRef.Weight.Expr(HostStrategy);
                _nodes.Add(new WeightedActionRuntime() { Node = node, WeightMetric = (Metric)weightMetric });
            }

        }

        bool justStarted = false;
        List<WeightedActionRuntime> _cachedNodesList = new List<WeightedActionRuntime>();
        System.Random _rand = new System.Random();
        public override async ValueTask<ScriptResultType> OnStart()
        {
            justStarted = true;
            _selectedNode = null;
            _cachedNodesList.Clear();
            _cachedNodesList.AddRange(_nodes);
            bool done = false;
            while (!done)
            {
                
                var choice = await _cachedNodesList.WeightedChoice(async x => (int)(x.WeightMetric != null ? await x.WeightMetric.Evaluate(HostStrategy.CurrentLegionary) + 1 : 100));
                if (choice?.Node == null)
                    return ScriptResultType.Failed;
                await choice.Node.Run();
                var status = choice.Node.LastStatus;
                if (status == ScriptResultType.Running || status == ScriptResultType.Succeeded)
                {
                    if (status == ScriptResultType.Running)
                        _selectedNode = choice.Node;
                    return status;
                }
                else
                    _cachedNodesList.Remove(choice);
                if (_cachedNodesList.Count == 0)
                    done = true;
            }
            return ScriptResultType.Failed;
        }

        protected override async ValueTask<ScriptResultType> OnTick()
        {
            if (!justStarted)
                await _selectedNode.Run();
            justStarted = false;
            return _selectedNode.LastStatus;
        }

        public override async ValueTask OnTerminate()
        {
            if (_selectedNode != null)
                await HostStrategy.Terminate(_selectedNode);

        }

    }

}
