using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.Tools;
using Core.Environment.Logging.Extension;
using UnityEngine;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    class DoInSequence : BehaviourNode
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private DoInSequenceDef _def;
        List<BehaviourNode> _nodes = new List<BehaviourNode>();
        private int _nodeIndex = -1;
        public override IEnumerable<BehaviourNode> GetSubNodes()
        {
            return _nodes;
        }

        protected override async ValueTask OnCreate(BehaviourNodeDef def)
        {
            _def = (DoInSequenceDef)def;
        }
        public override async ValueTask<ScriptResultType> OnStart()
        {
            _nodeIndex = -1;
            _nodes.Clear();
            foreach (var resourceRef in _def.Actions)
            {
                var node = await HostStrategy.GetNode(this, resourceRef);
                _nodes.Add(node);
            }
            return ScriptResultType.Running;
        }
        public override async ValueTask OnTerminate()
        {
            if (!_nodeIndex.InRange(0, _nodes.Count))
                return;
            var node = _nodes[_nodeIndex];
            if (node.LastStatus != ScriptResultType.Failed)
                await HostStrategy.Terminate(node);
        }
        protected override async ValueTask<ScriptResultType> OnTick()
        {
            if (_nodeIndex == -1)
                _nodeIndex = 0;
            if(_nodeIndex >= _nodes.Count)
                Logger.IfError()?.Message($"Node index is wrong in AI Sequence {_nodeIndex} {_nodes.Count}").Write();
            await _nodes[_nodeIndex].Run();
            var nodeStatus = _nodes[_nodeIndex].LastStatus;
            if (nodeStatus == ScriptResultType.Running)
            {
                return ScriptResultType.Running;
            }
            else if (nodeStatus == ScriptResultType.Succeeded)
            {
                _nodeIndex++;
                if (_nodes.Count == _nodeIndex)
                    return ScriptResultType.Succeeded;
                else
                {
                    return await OnTick();
                }
            }

            return ScriptResultType.Failed;
        }
        
    }

}
