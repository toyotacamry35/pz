using System;
using System.Linq;
using Src.Animation.ACS;
using UnityEditor.Animations;
using UnityEngine;

namespace Assets.Editor.Animation.ACS
{
    public abstract class AnimationStateComponentUpdater<T> : AnimationStateComponentUpdater where T : AnimationStateComponent
    {
        protected abstract void Update(AnimationStateComponentUpdaterContext ctx, T comp, AnimatorState state, AnimatorStateMachine machine, int layer);

        public override void Update(AnimationStateComponentUpdaterContext ctx, AnimationStateComponent comp, AnimatorState state, AnimatorStateMachine machine, int layer) 
            => Update(ctx, (T) comp, state, machine, layer);
    }
 
    public abstract class AnimationStateComponentBatchUpdater<T> : AnimationStateComponentBatchUpdater where T : AnimationStateComponent
    {
        protected abstract void Update(AnimationStateComponentUpdaterContext ctx, (T, AnimatorState, AnimatorStateMachine, int)[] comps);

        public override void Update(AnimationStateComponentUpdaterContext ctx, (AnimationStateComponent, AnimatorState, AnimatorStateMachine, int)[] comps) 
            => Update(ctx, comps.Select(x => ((T)x.Item1, x.Item2, x.Item3, x.Item4)).ToArray());
    }
    
    public abstract class AnimationStateComponentUpdater
    {
        public abstract void Update(AnimationStateComponentUpdaterContext ac, AnimationStateComponent comp, AnimatorState state, AnimatorStateMachine machine, int layer);
    }
    
    public abstract class AnimationStateComponentBatchUpdater
    {
        public abstract void Update(AnimationStateComponentUpdaterContext ctx, (AnimationStateComponent, AnimatorState, AnimatorStateMachine, int)[] comps);
    }
    
    public readonly struct AnimationStateComponentUpdaterContext
    {
        public readonly AnimatorController AnimationController;
        public readonly AnimatorOverrideController AnimatorOverrideController;
        public readonly string AssetPath;

        public AnimationStateComponentUpdaterContext(AnimatorController animationController, AnimatorOverrideController animatorOverrideController, string assetPath)
        {
            AnimationController = animationController;
            AnimatorOverrideController = animatorOverrideController;
            AssetPath = assetPath;
        }
    }
}