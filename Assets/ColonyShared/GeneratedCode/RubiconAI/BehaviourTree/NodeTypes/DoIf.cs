using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    class DoIf : BehaviourNode
    {
        public override IEnumerable<BehaviourNode> GetSubNodes()
        {
            return new BehaviourNode[1] { _action };
        }
        private DoIfDef _def;
        private BehaviourNode _action;
        private Condition _condition;

        protected override async ValueTask OnCreate(BehaviourNodeDef def)
        {
            _def = (DoIfDef)def;
            _action = await HostStrategy.GetNode(this, _def.Action.Target);
            _condition = (Condition)await HostStrategy.MetaExpression.Get(HostStrategy, _def.Condition.Target);
            _started = false;
        }
        public override async ValueTask<ScriptResultType> OnStart()
        {
            AIProfiler.BeginSample("CheckConditionStart");
            _started = false;
            _action = await HostStrategy.GetNode(this, _def.Action.Target);
            _condition = (Condition)await HostStrategy.MetaExpression.Get(HostStrategy, _def.Condition.Target);
            AIProfiler.EndSample();
            return ScriptResultType.Running;
        }

        private bool _started = false;
        protected override async ValueTask<ScriptResultType> OnTick()
        {
            if (!_started)
            {
                AIProfiler.BeginSample("CheckCondition");
                var c = await _condition.Evaluate(HostStrategy.CurrentLegionary);
                AIProfiler.EndSample();
                if (c)
                {
                    AIProfiler.BeginSample("DoIfRun");
                    await _action.Run();
                    AIProfiler.EndSample();
                    var status = _action.LastStatus;
                    if (status != ScriptResultType.Failed)
                    {
                        _started = true;
                    }
                    return status;
                }
                else
                    return ScriptResultType.Failed;
            }
            else
            {

                AIProfiler.BeginSample("DoIfRun");
                await _action.Run();
                AIProfiler.EndSample();
            }
            return _action.LastStatus;
        }
        public override async ValueTask OnTerminate()
        {
            await HostStrategy.Terminate(_action);
        }
    }

}
