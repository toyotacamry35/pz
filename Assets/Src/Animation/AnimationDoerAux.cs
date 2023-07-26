using System;
using Assets.ColonyShared.SharedCode.Entities;
using JetBrains.Annotations;
using ResourceSystem.Aspects.Misc;
using Src.Animation.ACS;
using Src.Locomotion;
using UnityEngine;

namespace Src.Animation
{
    internal static class AnimationDoerAux
    {
        internal interface IModifierBuilder : IAnimationModifier
        {
            bool InsertDefaultOnSet { get; }
            object GetStackId(IAnimationDoerInternal doer);
            IModifierInstance Create(IAnimationDoerInternal doer);
            IModifierInstance CreateDefault(IAnimationDoerInternal doer);
        }

        internal interface IModifierInstance
        {
            bool Execute(IAnimationDoerInternal doer, bool hasOwner);
            void OnPop(IAnimationDoerInternal doer);
            void OnPull(IModifierInstance prev);
        }     

        internal interface IAnimationDoerInternal
        {
            Animator Animator { get; }
            [NotNull] AnimatorControllerParameter GetParameter(AnimationParameterDef def, AnimatorControllerParameterType type);
            [NotNull] AnimatorControllerParameter GetParameter(string def, AnimatorControllerParameterType type);
            (AnimationStateInfo Info, AnimationStateDoerSupport Support) GetState(AnimationStateDef def);
            AnimatorLayerDescriptor GetLayer(string name);
            void FireAnimationPlayStarted(in AnimationPlayInfo info);
        }
        
        internal class AnimatorLayerDescriptor : IEquatable<AnimatorLayerDescriptor>
        {
            private readonly string _layerName;

            public AnimatorLayerDescriptor(string layerName)
            {
                _layerName = layerName;
            }

            public bool Equals(AnimatorLayerDescriptor other) => !ReferenceEquals(null, other) && _layerName == other._layerName;
            public override bool Equals(object obj) => Equals(obj as AnimatorLayerDescriptor);
            public override int GetHashCode() => _layerName.GetHashCode();
        }
        
        internal static int GetLayerIndex(Animator animator, AnimationLayerDef layer)
        {
            var layerIndex = animator.GetLayerIndex(layer.Name);
            if(layerIndex == -1)
                throw new Exception($"Layer {layer.Name} ({layer.____GetDebugAddress()}) not exist in animator {animator.transform.FullName()}");
            return layerIndex;
        }

        internal static int GetLayerIndex(Animator animator, string layerName)
        {
            var layerIndex = animator.GetLayerIndex(layerName);
            if(layerIndex == -1)
                throw new Exception($"Layer {layerName} not exist in animator {animator.transform.FullName()}");
            return layerIndex;
        }
        
        internal static bool Evaluate(ref float currentValue, float startValue, float endValue, float time, float deltaTime)
        {
            if (!time.ApproximatelyZero())
            {
                var t = Mathf.InverseLerp(startValue, endValue, currentValue) + deltaTime / time;
                currentValue = Mathf.Lerp(startValue, endValue, t);
                return time > 0 ? t < 1 : t > 0;
            }
            currentValue = endValue;
            return false;
        }
    }
}
