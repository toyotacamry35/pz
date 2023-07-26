using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.ResourceSystem.AI;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    class HighFrequentlyUpdated : BehaviourNode
    {
        private new HighFrequentlyUpdatedDef _def;
        private BehaviourNode _subnode;

        public override IEnumerable<BehaviourNode> GetSubNodes()
        {
            return new[] { _subnode };
        }

        protected override async ValueTask OnCreate(BehaviourNodeDef def)
        {
            _def = (HighFrequentlyUpdatedDef)def;
            _subnode = await HostStrategy.GetNode(this, _def.SubNode);
        }

        public override async ValueTask<ScriptResultType> OnStart()
        {
            ++HostStrategy.CurrentLegionary.ExecutingHighFreqUpdatedBehNodes;
            return ScriptResultType.Running;
        }

        protected override async ValueTask<ScriptResultType> OnTick()
        {
            await _subnode.Run();
            return _subnode.LastStatus;
        }

        public override ValueTask OnFinish()
        {
            --HostStrategy.CurrentLegionary.ExecutingHighFreqUpdatedBehNodes;
            return default;
        }

        public override async ValueTask OnTerminate()
        {
            await HostStrategy.Terminate(_subnode);
        }
    }

}
