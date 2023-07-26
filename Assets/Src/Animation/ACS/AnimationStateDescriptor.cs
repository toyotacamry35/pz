using Assets.Src.ResourcesSystem.Base;
using Assets.Src.ResourceSystem;
using ResourceSystem.Aspects.Misc;
using UnityEngine;

namespace Src.Animation.ACS
{
    [SharedBetweenAnimators]
    [DisallowMultipleComponent]
    public class AnimationStateDescriptor : AnimationStateComponent
    {
        [SerializeField] public JdbMetadata StateRef;
        
        public AnimationStateDef StateDef => StateRef ? StateRef.Get<AnimationStateDef>() : null;
        
#if UNITY_EDITOR
        public EditorBaseResourceWrapper<AnimationStateDef> EDITOR_StateDef => 
            StateRef ? _EDITOR_StateDef ?? (_EDITOR_StateDef = new EditorBaseResourceWrapper<AnimationStateDef>(StateRef.GetFullTreeCopy<AnimationStateDef>())) : _EDITOR_StateDef = null;

        private EditorBaseResourceWrapper<AnimationStateDef> _EDITOR_StateDef;
#endif
    }
}
