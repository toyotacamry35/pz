using System;
using UnityEngine;

namespace Src.Animation
{
    [Serializable]
    public struct AnimationStateInfo
    {
        public string Name;
        public string FullName;
        public string LayerName;
        public float Length;

        public static bool operator !=(AnimationStateInfo lhv, AnimationStateInfo rhv) => !(lhv == rhv);

        public static bool operator==(AnimationStateInfo lhv, AnimationStateInfo rhv) 
            => lhv.Name == rhv.Name && 
               lhv.FullName == rhv.FullName && 
               lhv.LayerName == rhv.LayerName && 
               Mathf.Approximately(lhv.Length, rhv.Length);
    }


    public readonly struct AnimationStateRuntimeInfo
    {
        public readonly string Name;
        public readonly string FullName;
        public readonly string LayerName;
        public readonly float Length;
        public readonly int NameHash;
        public readonly int FullNameHash;
        public readonly int LayerIndex;

        public AnimationStateRuntimeInfo(AnimationStateInfo nfo, Animator animator)
        {
            Name = nfo.Name;
            FullName = nfo.FullName;
            LayerName = nfo.LayerName;
            Length = nfo.Length;
            NameHash = Animator.StringToHash(nfo.Name);
            FullNameHash = Animator.StringToHash(nfo.FullName);
            LayerIndex = animator.GetLayerIndex(nfo.LayerName);
        }

        public AnimationStateRuntimeInfo(AnimationStateRuntimeInfo nfo)
        {
            this = nfo;
        }
        
        public AnimationStateRuntimeInfo(AnimationStateRuntimeInfo nfo, float length)
        {
            this = nfo;
            Length = length;
        }
    }

    public readonly struct AnimationStateParameters
    {
        public readonly int RunParameterHash;
        public readonly int StayParameterHash;
        public readonly int SpeedParameterHash;

        public AnimationStateParameters(int runParameterHash, int stayParameterHash, int speedParameterHash)
        {
            RunParameterHash = runParameterHash;
            StayParameterHash = stayParameterHash;
            SpeedParameterHash = speedParameterHash;
        }
    }

}