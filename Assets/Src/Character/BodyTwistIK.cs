using System;
using SharedCode.Entities.Engine;
using Src.Locomotion.Unity;
using UnityEngine;

namespace Assets.Src.Character
{
    [RequireComponent(typeof(Animator))]
    public class BodyTwistIK : MonoBehaviour
    {
        [SerializeField] private float _weight = 1;
        [SerializeField] private float _bodyWeight = 0.3f;
        [SerializeField] private float _headWeight = 1;
        [SerializeField] private float _eyesWeight = 0;
        [SerializeField] private float _clampWeight = 0.5f;
        [SerializeField] private float _yFactor = 0; // Позволяет делать наклоны туловища вместо наклонов с помощью анимации в слое Pitch аниматора
        [SerializeField] private float _fadeInTime = 0.1f;
        [SerializeField] private float _fadeOutTime = 0.1f;
        [SerializeField] private string _animatorParameter = "BodyTwistFactor";

        private IGuideProvider _guideProvider;
        private Animator _animator;
        private bool _enabled;
        private float _fadeFactor;
        private float _animationFactor = 1;
        private int _animatorParameterHash;
        private bool _animatorParameterExists;
        
        public void SetGuideProvider(IGuideProvider guideProvider)
        {
            _guideProvider = guideProvider;
            if (Enabled && !this.enabled)
                this.enabled = true;
        }

        public void Enable()
        {
            _enabled = true;
            if (Enabled && !this.enabled)
                this.enabled = true;
        }

        public void Disable()
        {
            _enabled = false;
        }

        private bool Enabled => _enabled && _guideProvider != null;
        
        private void Awake()
        {
            this.enabled = false;
            _animator = (_animator ? _animator : GetComponent<Animator>()) ?? throw new NullReferenceException($"No animator on {transform.FullName()}");;
            _animatorParameterHash = Animator.StringToHash(_animatorParameter);
            _animatorParameterExists = _animator.IsParameterExists(_animatorParameterHash);
        }

        private void Update()
        {
            if (Enabled && _fadeFactor < 1)
            {
                _fadeFactor = _fadeInTime > 0 ? Mathf.Min(_fadeFactor + Time.deltaTime / _fadeInTime, 1) : 1;
            }
            else
            if (!Enabled && _fadeFactor > 0)
            {
                _fadeFactor = _fadeOutTime > 0 ? Mathf.Max(_fadeFactor - Time.deltaTime / _fadeOutTime, 0) : 0;
            }

            _animationFactor = _animatorParameterExists ? _animator.GetFloat(_animatorParameterHash) : 1;
        }
        
        private void OnAnimatorIK(int layerIndex)
        {
            if (_guideProvider == null)
                return;
            
            Transform head = _animator.GetBoneTransform(HumanBodyBones.Head);
            Vector3 guide = _guideProvider.Guide.ToUnity();
            guide.y *= _yFactor;
            Vector3 target = head.position + guide * 100;
            _animator.SetLookAtPosition(target);
            _animator.SetLookAtWeight(_weight * _fadeFactor * _animationFactor, _bodyWeight, _headWeight, _eyesWeight, _clampWeight);

            if (!Enabled && _fadeFactor <= 0 && this.enabled)
                this.enabled = false;
        }
    }
}