using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.Tools;
using UnityEngine;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    class Choose : BehaviourNode
    {
        public override IEnumerable<BehaviourNode> GetSubNodes()
        {
            return _nodes;
        }
        private ChooseDef _def;
        List<BehaviourNode> _nodes = new List<BehaviourNode>();
        private int _lastNodeIndex = -1;

        protected override async ValueTask OnCreate(BehaviourNodeDef def)
        {
            _def = (ChooseDef)def;
        }

        public override async ValueTask<ScriptResultType> OnStart()
        {
            AIProfiler.BeginSample("Choosing");
            _lastNodeIndex = -1;
            _nodes.Clear();
            foreach (var resourceRef in _def.Actions)
            {
                var node = await HostStrategy.GetNode(this, resourceRef);
                _nodes.Add(node);
            }

            AIProfiler.EndSample();
            return ScriptResultType.Running;
        }
        protected override async ValueTask<ScriptResultType> OnTick()
        {
            var lastTimeIndex = _lastNodeIndex;
            for (int i = 0; i < _nodes.Count; i++)
            {
                var node = _nodes[i];
                AIProfiler.BeginSample("Trying to choose");
                if (AIProfiler.EnableProfile)
                    AIProfiler.BeginSample(node.GetType().Name);
                await node.Run();
                if (AIProfiler.EnableProfile)
                    AIProfiler.EndSample();
                AIProfiler.EndSample();
                var status = node.LastStatus;
                if ((status == ScriptResultType.Running || status == ScriptResultType.Succeeded) && lastTimeIndex != i && lastTimeIndex != -1)
                    await HostStrategy.Terminate(_nodes[lastTimeIndex]);
                _lastNodeIndex = i;
                if (status == ScriptResultType.Running)
                {
                    //if (lastTimeIndex != _lastNodeIndex)
                    //    Debug.Log($"Changed ValueTask to {_nodes[_lastNodeIndex]}");
                    return status;
                }
                else if (status == ScriptResultType.Succeeded)
                {
                    //if (lastTimeIndex != _lastNodeIndex)
                    //    Debug.Log($"Finished chosen ValueTask {_nodes[_lastNodeIndex]}");
                    return status;
                }
                else if (status == ScriptResultType.Failed && i == _nodes.Count - 1)
                {
                    //if (lastTimeIndex != _lastNodeIndex)
                    //    Debug.Log($"Failed all tasks");
                    return status;
                }

            }
            return ScriptResultType.Succeeded;
        }
        public override async ValueTask OnTerminate()
        {
            foreach (var behaviourNode in _nodes)
            {
                if (behaviourNode.LastStatus == ScriptResultType.Running)
                    await HostStrategy.Terminate(behaviourNode);
            }
        }
    }

}
