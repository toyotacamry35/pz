using System;
using System.Collections.Generic;
using System.Linq;
using Src.Tools;
using UnityEngine;

namespace Src.Animation
{
    public partial class AnimationStateInfoStorage : ScriptableObject
    {
        [SerializeField] private List<Tuple> _serializedInfos;

        private Dictionary<SerializableGuid, AnimationStateInfo> _infos;

        public AnimationStateInfo GetInfo(SerializableGuid guid)
        {
            if (_infos == null)
                _infos = _serializedInfos.ToDictionary(x => x.Guid, x => x.Info);
            return _infos[guid];
        }
        
        public bool TryGetInfo(SerializableGuid guid, out AnimationStateInfo info)
        {
            if (_infos == null)
                _infos = _serializedInfos.ToDictionary(x => x.Guid, x => x.Info);
            return _infos.TryGetValue(guid, out info);
        }
        
        [Serializable]
        public struct Tuple
        {
            public SerializableGuid Guid;
            public AnimationStateInfo Info;
        }
    }
}