using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;
using ColonyShared.SharedCode.Utils;
using UnityEngine;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    class DoWithCooldown : BehaviourNode
    {
        public override IEnumerable<BehaviourNode> GetSubNodes()
        {
            return new BehaviourNode[1] { _nodeToRun };
        }
        private BehaviourNode _nodeToRun;
        private float _failCooldown;
        private float _successCooldown;
        private DoWithCooldownDef _def;

        protected override async ValueTask OnCreate(BehaviourNodeDef def)
        {
            _def = (DoWithCooldownDef)def;
            _nodeToRun = await HostStrategy.GetNode(this, _def.Action);
        }
        public override async ValueTask<ScriptResultType> OnStart()
        {
            _ranAtLeastOneTick = false;
            _failCooldown = await _def.CooldownOnFail.Get(HostStrategy);
            _successCooldown = await _def.CooldownOnSuccess.Get(HostStrategy);
            return ScriptResultType.Running;
        }
        bool _ranAtLeastOneTick = false;
        protected override async ValueTask<ScriptResultType> OnTick()
        {
            if (HostStrategy.CurrentLegionary.TemporaryBlackboard.ContainsKey(_def.CooldownName) && (!_ranAtLeastOneTick || !_def.FromStart))
            {
                if (HostStrategy.CurrentLegionary.TemporaryBlackboard[_def.CooldownName] >= SyncTime.NowUnsynced)
                {
                    //Debug.LogWarning($"{_def.CooldownName} {HostStrategy.CurrentLegionary.TemporaryBlackboard[_def.CooldownName]} {HostStrategy.CurrentLegionary.TemporaryBlackboard[_def.CooldownName] - Time.time}");

                    return ScriptResultType.Failed;
                }
            }
            await _nodeToRun.Run();
            var status = _nodeToRun.LastStatus;
            if (!_ranAtLeastOneTick)
            {
                //if (HostStrategy.CurrentLegionary.TemporaryBlackboard.ContainsKey(_def.CooldownName))
                    //Debug.LogWarning($"GO {_def.CooldownName} {HostStrategy.CurrentLegionary.TemporaryBlackboard[_def.CooldownName]} {HostStrategy.CurrentLegionary.TemporaryBlackboard[_def.CooldownName] - Time.time}");

                if (status == ScriptResultType.Running)
                    _ranAtLeastOneTick = true;

            }
            if(_def.FromStart)
            {
                HostStrategy.CurrentLegionary.TemporaryBlackboard[_def.CooldownName] = SyncTime.NowUnsynced + SyncTime.FromSeconds(_successCooldown);

            }
            else
            {
                if (_ranAtLeastOneTick && status == ScriptResultType.Failed)
                {
                    HostStrategy.CurrentLegionary.TemporaryBlackboard[_def.CooldownName] = SyncTime.NowUnsynced + SyncTime.FromSeconds(_failCooldown);
                    //Debug.LogWarning($"SET {_def.CooldownName} {HostStrategy.CurrentLegionary.TemporaryBlackboard[_def.CooldownName]} {HostStrategy.CurrentLegionary.TemporaryBlackboard[_def.CooldownName] - Time.time}");
                }
                if (status == ScriptResultType.Succeeded)
                {
                    HostStrategy.CurrentLegionary.TemporaryBlackboard[_def.CooldownName] = SyncTime.NowUnsynced + SyncTime.FromSeconds(_successCooldown);
                    //if (HostStrategy.CurrentLegionary.TemporaryBlackboard.ContainsKey(_def.CooldownName))
                        //Debug.LogWarning($"SET {_def.CooldownName} {HostStrategy.CurrentLegionary.TemporaryBlackboard[_def.CooldownName]} {HostStrategy.CurrentLegionary.TemporaryBlackboard[_def.CooldownName] - Time.time}");
                }
            }

            return status;
        }

        public override async ValueTask OnTerminate()
        { 
            if (_ranAtLeastOneTick)
            {
                HostStrategy.CurrentLegionary.TemporaryBlackboard[_def.CooldownName] = SyncTime.NowUnsynced + SyncTime.FromSeconds(_failCooldown);
                //Debug.LogWarning($"SET {_def.CooldownName} {HostStrategy.CurrentLegionary.TemporaryBlackboard[_def.CooldownName]} {HostStrategy.CurrentLegionary.TemporaryBlackboard[_def.CooldownName] - Time.time}");
            }
            await HostStrategy.Terminate(_nodeToRun);
        }

    }
}
