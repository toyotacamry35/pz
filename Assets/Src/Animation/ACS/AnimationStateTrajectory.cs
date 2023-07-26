using ResourceSystem.Aspects.Misc;
using Src.Tools;
using UnityEngine;

namespace Src.Animation.ACS
{
    [SharedBetweenAnimators]
    [RequireComponent(typeof(AnimationStateDescriptor))]
    public class AnimationStateTrajectory : AnimationStateComponentWithGuid
    {
        [SerializeField] public GameObjectMarkerRef BodyPart; 
        
        public AnimationStateDef StateDef => GetComponent<AnimationStateDescriptor>().StateDef;
    }
}