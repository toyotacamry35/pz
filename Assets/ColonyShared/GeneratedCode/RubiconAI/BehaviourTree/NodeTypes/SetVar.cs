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
    class SetVar : BehaviourNode
    {

        private SetVarDef _def;
        protected override async ValueTask OnCreate(BehaviourNodeDef def)
        {
            _def = (SetVarDef)def;
        }

        public override async ValueTask<ScriptResultType> OnStart()
        {
            HostStrategy.CurrentLegionary.TemporaryBlackboard[_def.VarName] = _def.Value ? 1 : 0;
            return ScriptResultType.Succeeded;
        }
    }
}
