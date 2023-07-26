using Assets.ResourceSystem.Aspects.FX.FullScreenFx;
using Assets.Src.ResourceSystem;
using NLog;
using System;
using Core.Environment.Logging.Extension;
using UnityEngine;
using UnityEngine.Animations;

namespace Assets.Src.Character.Effects
{
    public class DisplayTraumaFX : StateMachineBehaviour
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public JdbMetadata _effectDef;
        protected Material _animatedMaterial;
        public CurveAnimation[] ShaderAnimationList;
        protected FXMaterialController _materialController;

        void Awake()
        {
            if (ShaderAnimationList == null)
                ShaderAnimationList = new CurveAnimation[0];
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_effectDef != null)
            {
                var matDef = _effectDef.Get<MaterialDef>();
                var materialRef = matDef.Material;
                _animatedMaterial = materialRef.Target;
                if (_animatedMaterial)
                {
                    if (!_materialController)
                    {
                        _materialController = animator.gameObject.GetComponent<FXMaterialController>();
                        if (!_materialController)
                        {
                            Logger.IfWarn()?.Message("No {0} found in children of {1}", nameof(FXMaterialController), animator.gameObject.name).Write();
                            return;
                        }
                    }
                    _materialController.AddMaterial(_animatedMaterial, layerIndex);
                }
                else
                    Logger.IfWarn()?.Message("Value of field '{0}' is null in '{1}'", nameof(Material), matDef).Write();
            }
            else
                Logger.IfWarn()?.Message("You forgot to set animated effect in {0} animator for {1} state", animator.name, stateInfo.GetType().Name).Write();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            if (_materialController)
                foreach (var shaderProperty in ShaderAnimationList)
                {
                    var x = controller.GetFloat(shaderProperty.AnimatorParameter);
                    if (x == float.NaN)
                        x = 0;
                    x = Mathf.Clamp01(x);
                    var y = shaderProperty.curve.Evaluate(x);
                    _materialController.SetMaterialProperty(_animatedMaterial, shaderProperty.MaterialProperty, y);
                }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this stat
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _materialController.RemoveMaterial(_animatedMaterial, layerIndex);
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //}
    }

    [Serializable]
    public class CurveAnimation
    {
        public string AnimatorParameter;
        public string MaterialProperty;
        public AnimationCurve curve;
    }
}
