using System;
using System.Linq;
using UnityEngine;

namespace Assets.ColonyShared.SharedCode.Utils
{
#if UNITY_5_3_OR_NEWER
    [CreateAssetMenu(menuName = "Curves/Curve3 Set ")]
    public class Curve3Set : ScriptableObject
    {
        [SerializeField] private Entry[] _entries;

        public Curve3 Get(string id)
        {
            if (_entries != null)
                for (int i = 0; i < _entries.Length; i++)
                    if (_entries[i].Id == id)
                        return _entries[i].Curve;
            return null;
        }

#if UNITY_EDITOR        
        public void Set(string id, Curve3 curve)
        {
            if (_entries == null)
            {
                _entries = new[] {new Entry {Id = id, Curve = curve}};
                return;
            }
            for (int i = 0; i < _entries.Length; i++)
                if (_entries[i].Id == id)
                {
                    _entries[i].Curve = curve;
                    return;
                }
            _entries = _entries.Append(new Entry {Id = id, Curve = curve}).ToArray();
        }

        public void Remove(string id)
        {
            _entries = _entries.Where(x => x.Id != id).ToArray();
        }
#endif        
        
        [Serializable]
        struct Entry
        {
            [SerializeField] public string Id;
            [SerializeField] public Curve3 Curve;
        }
    }
#else
    public class Curves3Set : UnityEngine.Object
    {
    }
#endif
}