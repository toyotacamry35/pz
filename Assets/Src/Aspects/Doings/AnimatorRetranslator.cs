using System;
using JetBrains.Annotations;
using NLog;
using UnityEngine;


namespace Assets.Src.Character.Animations
{
    public class AnimatorRetranslator : MonoBehaviour
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        [SerializeField] private Animator _animator;
        public Animator Animator => _animator;


        private void Awake()
        {
            _animator.AssertIfNull(nameof(_animator), gameObject);
        }

        public void OnJumpImpulseKeyFrame()
        {}

        public void TrailOn()
        {
            //toggle weapon trail anim event            
        }

        public void TrailOff()
        {
            //toggle weapon trail anim event
        }

        // Called by animation event (from Attack)
        public event Action OnSmiteKeyFrameEvent;
        public void OnSmiteKeyFrame()
        {
            OnSmiteKeyFrameEvent?.Invoke();
        }

        //#not_used:
        // public event Action TestMidEvent;
        // public void TestMidFunc()
        // {
        //      Logger.IfDebug()?.Message("TestMidFunc").Write();;
        // }
        
        public event Action EnableApplyRootMotionEvent;
        public void EnableApplyRootMotion()
        {
            EnableApplyRootMotionEvent?.Invoke();
        }
        
        public event Action DisableApplyRootMotionEvent;
        public void DisableApplyRootMotion()
        {
            DisableApplyRootMotionEvent?.Invoke();
        }

    }

}