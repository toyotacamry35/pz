using System.Collections.Generic;

namespace Assets.Src.RubiconAI.BehaviourTree
{
    public class TickStrategyEvent : Detective.Event
    {
        public StrategyStatus Strategy;
        public HashSet<object> TickedNodes = new HashSet<object>();
    }
}
