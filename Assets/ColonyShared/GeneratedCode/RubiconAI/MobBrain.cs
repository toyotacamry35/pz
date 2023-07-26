using Assets.Src.RubiconAI.BehaviourTree;
using System.Threading.Tasks;

namespace Assets.Src.RubiconAI
{
    public class MobBrain : Brain
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly Strategy _strategyToRun;
        private bool _init = false;
        private MobLegionary _hostLegionary;

        public MobBrain(Strategy strategyToRun)
        {
            _strategyToRun = strategyToRun;
        }

        public override async ValueTask Init(MobLegionary hostLegionary)
        {
            _hostLegionary = hostLegionary;
            await _hostLegionary.Do(_strategyToRun);
        }

        public override async ValueTask Think()
        {
            if (_hostLegionary.ExecutingStrategy.LastState == ScriptResultType.Failed || _hostLegionary.ExecutingStrategy.LastState == ScriptResultType.Succeeded)
                await _hostLegionary.Do(_strategyToRun); // is called from Legionary & calls it back - It's over-engineered to (not used yet) case of replaceable brain.
        }
    }
}
