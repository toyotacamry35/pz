using Src.Tools;
using UnityEngine;

namespace Src.Animation.ACS
{
    public abstract class AnimationStateComponentWithGuid : AnimationStateComponent
    {
        //[HideInInspector]
        [SerializeField] private SerializableGuid _guid = SerializableGuid.NewGuid();

        public SerializableGuid Guid => _guid;

#if UNITY_EDITOR
        public void GenerateGuid(bool force)
        {
            if (force || !_guid.IsValid)
                _guid = SerializableGuid.NewGuid();
        }
#endif
    }
}