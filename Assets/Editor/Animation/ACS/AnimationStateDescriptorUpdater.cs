using System;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using ResourceSystem.Aspects.Misc;
using Src.Animation.ACS;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Assets.Editor.Animation.ACS
{
    [UsedImplicitly]
    public class AnimationStateDescriptorUpdater : AnimationStateComponentUpdater<AnimationStateDescriptor>
    {
        protected override void Update(AnimationStateComponentUpdaterContext ctx, AnimationStateDescriptor comp, AnimatorState state, AnimatorStateMachine machine, int layer)
        {
            if (comp.StateRef == null)
                throw new Exception($"Missing reference on {nameof(AnimationStateDef)} in animator:{AssetDatabase.GetAssetPath(ctx.AnimationController)} state:{ctx.AnimationController.layers[layer].name}.{state.name}");

            var animationStateDef = comp.StateRef.GetFullTreeCopy();
            if (!(animationStateDef is AnimationStateDef))
                throw new Exception($"Reference type {(animationStateDef?.GetType().ToString() ?? "<no type>")} is not {nameof(AnimationStateDef)} in animator:{AssetDatabase.GetAssetPath(ctx.AnimationController)} state:{ctx.AnimationController.layers[layer].name}.{state.name}");            
        }
    }
    
    [UsedImplicitly]
    public class AnimationStateDescriptorBatchUpdater : AnimationStateComponentBatchUpdater<AnimationStateDescriptor>
    {
        protected override void Update(AnimationStateComponentUpdaterContext ctx, (AnimationStateDescriptor, AnimatorState, AnimatorStateMachine, int)[] comps)
        {
            foreach(var dups in comps.Where(x => x.Item1.EDITOR_StateDef!=null).GroupBy(x => x.Item1.EDITOR_StateDef).Where(x => x.Count() > 1))
                Debug.LogError($"Animation state {dups.Key.____GetDebugAddress()} used several times. Animator:{AssetDatabase.GetAssetPath(ctx.AnimationController)} States:[{string.Join(", ", dups.Select(x => x.Item2.name))}]");
            
        }
    }
}
