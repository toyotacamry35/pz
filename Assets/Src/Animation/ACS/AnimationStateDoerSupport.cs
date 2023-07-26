using System;
using Assets.Src.Utils;
using ResourceSystem.Aspects.Misc;
using UnityEngine;

namespace Src.Animation.ACS
{
    [SharedBetweenAnimators]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AnimationStateDescriptor))]
    [RequireComponent(typeof(AnimationStateProfile))]
    public class AnimationStateDoerSupport : AnimationStateComponent
    {
        [Foldout("Internal", false)]
        [SerializeField, DisabledInInspector] private AnimationStateHeader _defaultState;
        [SerializeField, DisabledInInspector] private string _speedParameter;
        [SerializeField, DisabledInInspector] private string _runParameter;
        [SerializeField, DisabledInInspector] private string _stayParameter;

        private int _speedParameterHash;
        private int _runParameterHash;
        private int _stayParameterHash;
        
        public AnimationStateDef StateDef => GetComponent<AnimationStateDescriptor>().StateDef;
        
        public AnimationStateProfile DefaultState => _defaultState.GetComponent<AnimationStateProfile>();

        public string SpeedParameter => _speedParameter;

        public int SpeedParameterHash => _speedParameterHash == 0 ? (_speedParameterHash = Animator.StringToHash(_speedParameter)) : _speedParameterHash;
        
        public string RunParameter => _runParameter;

        public int RunParameterHash => _runParameterHash == 0 ? (_runParameterHash = Animator.StringToHash(_runParameter)) : _runParameterHash;
        
        public string StayParameter => _stayParameter;

        public int StayParameterHash => _stayParameterHash == 0 ? (_stayParameterHash = Animator.StringToHash(_stayParameter)) : _stayParameterHash;
        
        public AnimationStateParameters Parameters => new AnimationStateParameters(RunParameterHash, StayParameterHash, SpeedParameterHash);
        
        #region Internal

#if UNITY_EDITOR

        public bool SetDefaultState(AnimationStateHeader defaultState)
        {
            if (defaultState == null) throw new ArgumentNullException(nameof(defaultState));
            if (_defaultState != defaultState)
            {
                _defaultState = defaultState;
                return true;
            }

            return false;
        }

        public bool SetSpeedParameter(string speedParameter)
        {
            if (_speedParameter != speedParameter)
            {
                _speedParameter = speedParameter;
                return true;
            }
            return false;
        }

        public bool SetRunParameter(string runParameter)
        {
            if (_runParameter != runParameter)
            {
                _runParameter = runParameter;
                return true;
            }
            return false;
        }

        public bool SetStayParameter(string stayParameter)
        {
            if (_stayParameter != stayParameter)
            {
                _stayParameter = stayParameter;
                return true;
            }
            return false;
        }
        
#endif

        #endregion
    }
}