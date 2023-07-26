using System.Collections.Generic;
using System.Linq;
using ColonyShared.SharedCode;

namespace Src.Locomotion
{
    public class LocomotionCompositeDebugAgent : ILocomotionDebugAgent
    {
        private readonly List<ILocomotionDebugAgent> _agents;
        private bool _isActive;
        
        public LocomotionCompositeDebugAgent(params ILocomotionDebugAgent[] agents)
        {
            _agents = agents.Where(x => x != null).ToList();
        }

        public bool IsActive => _isActive;

        public void Add(DebugTag id, Value entry)
        {
            if(_isActive)
                foreach (var agent in _agents)
                    if (agent.IsActive)
                        agent.Add(id, entry);
        }

        public void BeginOfFrame()
        {
            _isActive = false;
            foreach (var agent in _agents)
            {
                if (agent.IsActive)
                {
                    agent.BeginOfFrame();
                    _isActive = true;
                }
            }
        }

        public void EndOfFrame()
        {
            if(_isActive)
                foreach (var agent in _agents)
                    agent.EndOfFrame();
        }
    }
}