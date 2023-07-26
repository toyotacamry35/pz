using System;
using Boo.Lang;
using UnityEngine;

namespace Src.NetworkedMovement
{
    public class IsVisibleByCameraDetector
    {
        private readonly List<IsVisibleByCameraAgent> _agents = new List<IsVisibleByCameraAgent>();
        private int _visibleAgents;

        public bool IsVisible => _visibleAgents > 0;

        public void Attach(GameObject root)
        {
            Detach();
            foreach (var renderer in root.GetComponentsInChildren<Renderer>())
            {
                
                var agent = renderer.gameObject.AddComponent<IsVisibleByCameraAgent>();
                if (renderer.isVisible)
                {
                    agent.Visible = true;
                }
                
                if (agent.Visible)
                    _visibleAgents++;
                agent.VisibilityChanged += OnVisibilityChanged;
                _agents.Add(agent);
            }
        }

        public void Detach()
        {
            foreach (var agent in _agents)
            {
                agent.VisibilityChanged -= OnVisibilityChanged;
                GameObject.DestroyImmediate(agent);
            }
            _agents.Clear();
        }
        
        private void OnVisibilityChanged(IsVisibleByCameraAgent agent, bool visible)
        {
            if (visible)
            {
                if (_visibleAgents == _agents.Count) throw new Exception($"_visibleAgents > {_agents.Count} by agent {agent.transform.FullName()}");
                _visibleAgents++;
            }
            else
            {
                if (_visibleAgents == 0) throw new Exception($"_visibleAgents < 0 by agent {agent.transform.FullName()}");
                _visibleAgents--;
            }
        }
    }
}