using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;
using System.Collections.Generic;
using ColonyShared.SharedCode.Utils;
using System.Threading.Tasks;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    class DoForTime : BehaviourNode
    {
        public override IEnumerable<BehaviourNode> GetSubNodes()
        {
            return new BehaviourNode[1] { _action };
        }
        long _duration = 0;
        long _finishTime = 0;
        bool _doUntilEnd = false;
        DoForTimeDef _doForTimeDef;
        BehaviourNode _action;
        DoForTimeDef _selfDef;
        protected override async ValueTask OnCreate(BehaviourNodeDef def)
        {
            _selfDef = ((DoForTimeDef)def);
            _doForTimeDef = _selfDef;
            _action = await HostStrategy.GetNode(this, _selfDef.Action);
            _doUntilEnd = _selfDef.DoUntilEnd;
        }

        public override async ValueTask<ScriptResultType> OnStart()
        {
            _duration = SyncTime.FromSeconds(await _selfDef.Time.Get(HostStrategy));
            _finishTime = SyncTime.Now + _duration;
            HostStrategy.ShouldTickWithDelay(SyncTime.ToSeconds(_duration + 20));
            return ScriptResultType.Running;
        }
        

        protected override async ValueTask<ScriptResultType> OnTick()
        {
            if (SyncTime.InThePast(_finishTime))
            {
                await HostStrategy.Terminate(_action);
                if(_doForTimeDef.FailOnTimeout)
                    return ScriptResultType.Failed;
                else
                    return ScriptResultType.Succeeded;
            }
            await _action.Run();
            var actionStatus = _action.LastStatus;
            if (actionStatus == ScriptResultType.Failed)
            {
                if(_selfDef.TryAgain)
                    return ScriptResultType.Running;
                else
                    return ScriptResultType.Failed;
                
            }
                if (actionStatus == ScriptResultType.Succeeded)
            {
                if(!_doUntilEnd)
                    return ScriptResultType.Succeeded;
            }
            return ScriptResultType.Running;
        }
        public override async ValueTask OnTerminate()
        {
            await HostStrategy.Terminate(_action);
        }
        
    }
}
