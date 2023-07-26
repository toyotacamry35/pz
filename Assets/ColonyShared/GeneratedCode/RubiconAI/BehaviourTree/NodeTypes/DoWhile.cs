using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;
using UnityEngine;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    class DoWhile : BehaviourNode
    {
        public override IEnumerable<BehaviourNode> GetSubNodes()
        {
            return new BehaviourNode[1] { _action };
        }
        private DoWhileDef _def;
        private BehaviourNode _action;
        private Condition _condition;
        bool _started = false;
        protected override async ValueTask OnCreate(BehaviourNodeDef def)
        {
            _def = (DoWhileDef)def;
            _action = await HostStrategy.GetNode(this, _def.Action);
            _condition = (Condition)await HostStrategy.MetaExpression.Get(HostStrategy, _def.Condition.Target);
        }

        public override async ValueTask<ScriptResultType> OnStart()
        {
            _started = false;
            return ScriptResultType.Running;
        }

        protected override async ValueTask<ScriptResultType> OnTick()
        {
            if (await _condition.Evaluate(HostStrategy.CurrentLegionary))
            {
                _started = true;
                await _action.Run();
                return _action.LastStatus;
            }
            else if(_started)
                await HostStrategy.Terminate(_action);
            return ScriptResultType.Failed;
        }

        public override async ValueTask OnTerminate()
        {
            await HostStrategy.Terminate(_action);
        }
    }
}
