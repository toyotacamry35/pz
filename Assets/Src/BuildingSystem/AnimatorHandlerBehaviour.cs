using System.Collections.Generic;
using UnityEngine;

namespace Assets.Src.BuildingSystem
{
    class AnimatorHandlerBehaviour : StatHandlerBehaviour
    {
        public Animator Animator = null;
        public string AnimatorKey = string.Empty;
        public bool AnimatorKeyIsBool = true;

        // ----------------------------------------------------------------------------------------
        private void ConnectToAnimator()
        {
            if (Animator == null)
            {
                Animator = gameObject.GetComponent<Animator>();
            }
            if (Animator == null)
            {
                Animator = gameObject.GetComponentInChildren<Animator>();
            }
        }

        private void SetAnimator(float value)
        {
            if ((Animator != null) && !string.IsNullOrEmpty(AnimatorKey))
            {
                if (AnimatorKeyIsBool)
                {
                    Animator.SetBool(AnimatorKey, (value > 0.5f));
                }
                else
                {
                    Animator.SetFloat(AnimatorKey, value);
                }
            }
        }

        // ----------------------------------------------------------------------------------------
        protected override void OnAwakeHandler()
        {
            ConnectToAnimator();
        }
        protected override void OnDestroyHandler() { }
        protected override void OnGotClient() { }
        protected override void OnLostClient() { }

        protected override void OnStatChanged(float newValue)
        {
            SetAnimator(newValue);
        }
        protected override void OnSetInitialValue(float initialValue)
        {
            SetAnimator(initialValue);
        }
        protected override void OnSetFinalValue(float finalValue)
        {
            SetAnimator(finalValue);
        }
    }
}
