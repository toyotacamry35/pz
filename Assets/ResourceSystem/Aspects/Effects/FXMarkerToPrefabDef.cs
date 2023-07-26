using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using Newtonsoft.Json;
using NLog;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.ResourceSystem.Aspects.Effects
{
    public class FXMarkerToPrefabDef : BaseResource
    {
        [JsonIgnore]
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        public ResourceRef<FxMarkerPrefabPairDef>[] Pairs { get; set; }

        [JsonIgnore]
        private Dictionary<BaseResource, Object> _dictionary;

        public GameObject Get(BaseResource marker)
        {
            Object result = GetObject(marker);
            return result is GameObject ? result as GameObject : null;
        }

        public Object GetObject(BaseResource marker)
        {
            if (_dictionary == null)
                CreateDictionary();

            if (_dictionary != null && _dictionary.ContainsKey(marker))
                return _dictionary[marker];
            else
            {
                return null;
            }
        }

        public bool Contains(BaseResource marker)
        {
            if (marker == null)
                return false;
            if (_dictionary == null)
                CreateDictionary();

            return _dictionary != null && _dictionary.ContainsKey(marker);
        }

        private void CreateDictionary()
        {
            _dictionary = new Dictionary<BaseResource, Object>();
            for (int i = 0; i < Pairs.Length; i++)
            {
                if (_dictionary.ContainsKey(Pairs[i].Target.Marker.Target))
                {
                    //Logger.WarnZ("ERROR FXMarkerToPrefab {0}. {1} -> {2} or {3} ?", ____GetDebugShortName(),
                    //    Pairs[i].Target.Marker.Target.____GetDebugShortName(),
                    //    _dictionary[Pairs[i].Target.Marker.Target].name, Pairs[i].Target.Prefab.Target.name);
                }
                else
                {
                    _dictionary.Add(Pairs[i].Target.Marker.Target, Pairs[i].Target.Prefab.Target);
                }
            }
        }
        public class FxMarkerPrefabPairDef : BaseResource
        {
            public ResourceRef<BaseResource> Marker { get; set; }
            public UnityRef<Object> Prefab { get; set; }
        }
    }
}
