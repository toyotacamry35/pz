using System.Collections.Generic;
using UnityEngine;

namespace Src.Animation
{
    public class AnimationTrajectoriesSettings : ScriptableObject
    {
        [SerializeField] private float _timeStep = 1 / 60f; 
        [SerializeField] private float _equalsTolerance = 0.005f; 
        [SerializeField] private float _optimizationTolerance = 0.01f; 
        [SerializeField] private GameObject _body;
        [SerializeField] private bool _forceUpdate;

        public GameObject Body => _body;

        public float TimeStep => _timeStep;
        
        public float EqualsTolerance => _equalsTolerance;

        public float OptimizationTolerance => _optimizationTolerance;

        public bool ForceUpdate => _forceUpdate;
    }
}