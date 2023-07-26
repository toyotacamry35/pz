using Assets.Src.Utils;
using SharedCode.Utils.DebugCollector;
using UnityEngine;

namespace Src.Animation.ACS
{
    [SharedBetweenAnimators]
    [DisallowMultipleComponent]
    public class AnimationStateProfile : AnimationStateComponentWithGuid
    {
        #region Internal

// #if UNITY_EDITOR
//         public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//         {
//             Collect.Instance?.EventBgn($"AnimationState.{_info.FullName}", this);
//         }
//
//         public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//         {
//             Collect.Instance?.EventEnd(this);
//         }
// #endif

        #endregion
    }
}