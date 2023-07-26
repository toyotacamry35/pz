using ColonyShared.SharedCode.Utils;
using System.Threading.Tasks;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    class DummyNode : BehaviourNode
    {
        string _randomId;
        float _duration;
        protected override async ValueTask OnCreate(BehaviourNodeDef def)
        {
            _duration = ((DummyNodeDef)def).Time;
            _randomId = GetHashCode().ToString();
        }

        public override async ValueTask<ScriptResultType> OnStart()
        {
            HostStrategy.CurrentLegionary.TemporaryBlackboard[_randomId] = SyncTime.NowUnsynced + SyncTime.FromSeconds(_duration);
            return ScriptResultType.Running;
        }

        protected override async ValueTask<ScriptResultType> OnTick()
        {
            if (HostStrategy.CurrentLegionary.TemporaryBlackboard[_randomId] < SyncTime.NowUnsynced)
                return ScriptResultType.Succeeded;
            else
                return ScriptResultType.Running;
        }
    }

}
