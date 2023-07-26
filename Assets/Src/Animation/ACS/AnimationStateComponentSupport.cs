#if false
using System;
using System.Collections.Generic;
using System.Linq;
using Src.Animation;
using Src.Animation.ACS;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using AnimationStateInfo = Src.Animation.AnimationStateInfo;

namespace Assets.Editor.Animation.ACS
{
    public static class AnimationStateComponentSupport
    {
        public static AnimationStateHeader GetHeader(AnimatorState state)
        {
            var header = state.behaviours.OfType<AnimationStateHeader>().FirstOrDefault();
            return header;
        }

        public static AnimationStateHeader GetOrCreateHeader(AnimatorState state)
        {
            var header = state.behaviours.OfType<AnimationStateHeader>().FirstOrDefault();
            if(!header)
            {
                header = state.AddStateMachineBehaviour<AnimationStateHeader>();
                EditorUtility.SetDirty(state);
            }   
            return header;
        }
        
        public static T CreateComponent<T>(AnimatorState state) where T : AnimationStateComponent
        {
            var header = GetOrCreateHeader(state); 
            var comp = state.AddStateMachineBehaviour<T>();
            header.AddComponent(comp);
            EditorUtility.SetDirty(header);
            comp.SetHeader(header);
            EditorUtility.SetDirty(comp);
            return comp;
        }

        public static T GetOrCreateComponent<T>(AnimatorState state) where T : AnimationStateComponent
        {
            var header = GetOrCreateHeader(state); 
            var comp = state.behaviours.OfType<T>().FirstOrDefault();
            if (!comp)
            {
                comp = state.AddStateMachineBehaviour<T>();
                EditorUtility.SetDirty(state);
            }

            var compInHeader = header.GetComponent<T>();
            if (ReferenceEquals(compInHeader, null))
            {
                header.AddComponent(comp);
                EditorUtility.SetDirty(header);
                comp.SetHeader(header);
                EditorUtility.SetDirty(comp);
            }            
            else
            if (comp != compInHeader)
            {
                header.ReplaceComponent(compInHeader, comp);
                EditorUtility.SetDirty(header);
                comp.SetHeader(header);
                EditorUtility.SetDirty(comp);
                UnityEngine.Object.DestroyImmediate(compInHeader, true);
            }                
            return comp;
        }
        
        public static void AddComponent(AnimatorState state, AnimationStateComponent comp)
        {
            var header = GetOrCreateHeader(state); 
            header.AddComponent(comp);
            EditorUtility.SetDirty(header);
            comp.SetHeader(header);
            EditorUtility.SetDirty(comp);
        }
        
        public static void GetStateInfo(AnimatorState state, int layer, AnimatorController ac, ref AnimationStateInfo info)
        {
            var layerName = ac.layers[layer].name;
            var stateName = state.name;
            info.Name = stateName;
            info.FullName = layerName + "." + stateName;
            info.Length = GetMotionDuration(state.motion);
            info.LayerName = layerName;
        }
        
        public static float GetMotionDuration(Motion motion)
        {
            var clip = motion as AnimationClip;
            if (clip)
                return clip.length;
            var blend = motion as BlendTree;
            if (blend)
                return blend.children.Select(x => GetMotionDuration(x.motion)).Max();
            return 0;
        }

        public static Tuple<AnimatorController, AnimatorState, AnimatorStateMachine, int> GetStateMachineBehaviourLocation(StateMachineBehaviour smb)
        {
            Debug.Log($"SMB:{smb} ({smb.GetInstanceID()})");
            var acs = Profile.FindObjectsOfTypeAll<AnimatorController>();
            foreach (var ac in acs)
            {
                Debug.Log($"AC:{ac.name}");
                var tuples = FindBehaviours<StateMachineBehaviour>(ac);
                foreach (var tuple in tuples)
                {
                    Debug.Log($"AC:{ac.name} State:{tuple.Item2.name} Behaviour:{tuple.Item1} ({tuple.Item1.GetInstanceID()})");
                    if (tuple.Item1 == smb)
                        return Tuple.Create(ac, tuple.Item2, tuple.Item3, tuple.Item4);
                }
            }
            return null;
        }
        
        public static ValueTuple<T, AnimatorState, AnimatorStateMachine, int>[] FindBehaviours<T>(AnimatorController ac) where T : StateMachineBehaviour
        {
            return ac.layers.SelectMany((x, i) => FindBehaviours<T>(i, x.stateMachine)).ToArray();
        }
        
        private static IEnumerable<ValueTuple<T, AnimatorState, AnimatorStateMachine, int>> FindBehaviours<T>(int layer, AnimatorStateMachine asm) where T : StateMachineBehaviour
        {
            return asm.states
                .Select(x => x.state).Select(x => ValueTuple.Create(x.behaviours.OfType<T>().FirstOrDefault(), x, asm, layer)).Where(x => x.Item1)
                .Concat(asm.stateMachines.Select(x => x.stateMachine).SelectMany(x => FindBehaviours<T>(layer, x)));
        }
    }
}
#endif
