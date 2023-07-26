using System;
using System.Collections.Generic;
using System.Linq;
using Src.Animation;
using UnityEditor.Animations;
using UnityEngine;

namespace Assets.Editor.Tools
{
    public static class AnimatorControllerUtils
    {
        public static float GetMotionDuration(Motion motion)
        {
            var clip = motion as AnimationClip;
            if (clip)
                return clip.length;
            var blend = motion as BlendTree;
            if (blend && blend.children.Length > 0)
                return blend.children.Select(x => GetMotionDuration(x.motion)).Max();
            return 0;
        }       
                
        public static AnimationStateInfo GetStateInfo(AnimatorState state, int layer, AnimatorController ac, AnimatorOverrideController aoc, AnimationStateInfo info = default(AnimationStateInfo))
        {
            var clip = state.motion != null ? (aoc ? aoc[state.motion.name] ?? state.motion : state.motion) : null;
            var layerName = ac.layers[layer].name;
            var stateName = state.name;
            info.Name = stateName;
            info.FullName = layerName + "." + stateName;
            info.Length = GetMotionDuration(clip);
            info.LayerName = layerName;
            return info;
        }

        public static (T behaviour, AnimatorState state, AnimatorStateMachine stateMachine, int layer)[] FindBehaviours<T>(AnimatorController ac) where T : StateMachineBehaviour
        {
            return ac.layers.SelectMany((x, i) => FindBehaviours<T>(i, x.stateMachine)).ToArray();
        }
        
        private static IEnumerable<(T, AnimatorState, AnimatorStateMachine, int)> FindBehaviours<T>(int layer, AnimatorStateMachine asm) where T : StateMachineBehaviour
        {
            return asm.states
                .Select(x => x.state).SelectMany(x => x.behaviours.OfType<T>().Select(y => (y, x, asm, layer)))
                .Concat(asm.stateMachines.Select(x => x.stateMachine).SelectMany(x => FindBehaviours<T>(layer, x)));
        }
    }
}