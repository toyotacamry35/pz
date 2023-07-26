using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Src.Tools
{
    class RandomAnimationScriptBehaviour : StateMachineBehaviour
    {
        public string Name;
        public int MaxValue = 1;
        public int MinValue = 0;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetInteger(Name, UnityEngine.Random.Range(MinValue, MaxValue + 1));
        }
    }
}
