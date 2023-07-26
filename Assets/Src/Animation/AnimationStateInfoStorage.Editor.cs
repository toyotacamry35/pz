#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using Src.Tools;

namespace Src.Animation
{
    public partial class AnimationStateInfoStorage
    {
        public bool UpdateInfo(IReadOnlyList<(SerializableGuid guid, AnimationStateInfo info)> list)
        {
            _serializedInfos = _serializedInfos ?? new List<Tuple>();
            
            bool rv = false;
            foreach (var tuple in list)
                rv |= UpdateInfo(tuple.guid, tuple.info);

            for (var index = 0; index < _serializedInfos.Count; index++)
            {
                var tuple = _serializedInfos[index];
                if (list.All(x => x.guid != tuple.Guid))
                {
                    _serializedInfos.RemoveAt(index);
                    rv = true;
                }
            }
            return rv;
        }

        public bool UpdateInfo(SerializableGuid guid, AnimationStateInfo info)
        {
            var idx = _serializedInfos.FindIndex(x => x.Guid == guid);
            if (idx == -1)
            {
                _serializedInfos.Add(new Tuple {Guid = guid, Info = info});
                return true;
            }
            if (_serializedInfos[idx].Info != info)
            {
                _serializedInfos[idx] = new Tuple {Guid = guid, Info = info};
                return true;
            }
            return false;
        }

        public AnimationStateInfo GetSerializedInfo(SerializableGuid guid)
        {
            return _serializedInfos != null ? _serializedInfos.FirstOrDefault(x => x.Guid == guid).Info : default;
        }
    }
}

#endif
