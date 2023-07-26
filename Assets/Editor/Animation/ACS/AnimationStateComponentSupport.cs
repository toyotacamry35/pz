using System;
using System.Linq;
using Src.Animation.ACS;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Assets.Editor.Animation.ACS
{
    public static class AnimationStateComponentSupport
    {
        public static bool DebugEnabled = false;
    
        public static AnimationStateHeader GetHeader(this AnimatorState state)
        {
            var header = state.behaviours.OfType<AnimationStateHeader>().SingleOrDefault();
            return header;
        }

        public static AnimationStateHeader GetOrCreateHeader(this AnimatorState state)
        {
            var header = state.behaviours.OfType<AnimationStateHeader>().SingleOrDefault();
            if(!header)
            {
                header = state.AddStateMachineBehaviour<AnimationStateHeader>();
                state.Dirty();
            }   
            return header;
        }

        public static T CreateComponent<T>(this AnimatorState state) where T : AnimationStateComponent
        {
            return (T) CreateComponent(state, typeof(T));
        }
        
        public static AnimationStateComponent CreateComponent(AnimatorState state, Type type)
        {
            if(!typeof(AnimationStateComponent).IsAssignableFrom(type)) throw new ArgumentException($"Type:{type} is not {nameof(AnimationStateComponent)}"); 
           
            var header = GetOrCreateHeader(state); 
            var comp = (AnimationStateComponent)state.AddStateMachineBehaviour(type);
            header.AddComponent(comp);
            header.Dirty();
            comp.SetHeader(header);
            comp.Dirty();
            return comp;
        }

        public static T GetOrCreateComponent<T>(this AnimatorState state) where T : AnimationStateComponent
        {
            return (T) GetOrCreateComponent(state, typeof(T));
        } 
        
        public static AnimationStateComponent GetOrCreateComponent(this AnimatorState state, Type type, bool debugEnabled = false)
        {
            if(!typeof(AnimationStateComponent).IsAssignableFrom(type)) throw new ArgumentException($"Type:{type} is not {nameof(AnimationStateComponent)}"); 

            var header = GetOrCreateHeader(state); 
            var comp = (AnimationStateComponent)state.behaviours.FirstOrDefault(type.IsInstanceOfType);
            if (!comp)
            {
                comp = (AnimationStateComponent)state.AddStateMachineBehaviour(type);
                state.Dirty();
            }

            var compInHeader = header.GetComponent(type);
            if (ReferenceEquals(compInHeader, null))
            {
                if(debugEnabled) Debug.Log($"Create required Component:{type.Name} on State:{state.name}");
                header.AddComponent(comp);
                header.Dirty();
                comp.SetHeader(header);
                comp.Dirty();
            }            
            else
            if (comp != compInHeader)
            {
                if(debugEnabled) Debug.Log($"Replace required Component:{type.Name} on State:{state.name}");
                header.ReplaceComponent(compInHeader, comp);
                header.Dirty();
                comp.SetHeader(header);
                comp.Dirty();
                UnityEngine.Object.DestroyImmediate(compInHeader, true);
            }                
            return comp;
        }
        
        public static void AddComponent(this AnimatorState state, AnimationStateComponent comp)
        {
            var header = GetOrCreateHeader(state); 
            header.AddComponent(comp);
            header.Dirty();
            comp.SetHeader(header);
            comp.Dirty();
        } 
        
        public static void AddComponentIfNeed(this AnimationStateHeader header, AnimationStateComponent comp)
        {            
            if (!header.IsComponentExists(comp))
            {
                header.AddComponent(comp);
                header.Dirty();
                comp.SetHeader(header);
                comp.Dirty();
            }            
        }

        public static void Dirty(this UnityEngine.Object comp, string name = null)
        {
            if (!comp)
                return;
            if (DebugEnabled)
                Debug.Log($"Set dirty for {name ?? comp.name}");
            EditorUtility.SetDirty(comp);
        }
    }
}
