using Core.Environment.Logging.Extension;
using NLog;

namespace Assets.Src.RubiconAI.BehaviourTree
{
    public class StrategyStatus
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        public ScriptResultType Result;
        public StrategyDef Def;
        public bool TerminatedByEvent = false;
        BehaviourStatus _rootStatus;

        public BehaviourStatus RootNodeTick
        {
            get { return _rootStatus; }
            set
            {
                if (_rootStatus != null)
                    Logger.IfError()?.Message("Double assign root status").Write();
                _rootStatus = value;
                _rootStatus.ParentStrategy = this;
            }
        }
    }
}
