using Assets.Src.ResourcesSystem.Base;
using Newtonsoft.Json;
using NLog;
using System.Collections.Generic;
using Core.Environment.Logging.Extension;

namespace Assets.ResourceSystem.Aspects.Effects
{
    public class FXMarkerToDictionaryDef : BaseResource
    {
        [JsonIgnore]
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        public ResourceRef<FxMarkerDictionaryPairDef>[] Pairs { get; set; }

        [JsonIgnore]
        private Dictionary<BaseResource, BaseResource> _dictionary;

        public BaseResource GetObject(BaseResource marker)
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
            _dictionary = new Dictionary<BaseResource, BaseResource>();
            for (int i = 0; i < Pairs.Length; i++)
            {
                if (_dictionary.ContainsKey(Pairs[i].Target.Marker.Target))
                {
                    Logger.IfWarn()?.Message("ERROR FXMarkerToPrefab {0}. {1} -> {2} or {3} ?", ____GetDebugShortName(),
                            Pairs[i].Target.Marker.Target.____GetDebugShortName(),
                            _dictionary[Pairs[i].Target.Marker.Target].____GetDebugShortName(), Pairs[i].Target.Marker.Target.____GetDebugShortName())
                        .Write();
                }
                else
                {
                    _dictionary.Add(Pairs[i].Target.Marker.Target, Pairs[i].Target.Dictionary.Target);
                }
            }
        }
        public class FxMarkerDictionaryPairDef : BaseResource
        {
            public ResourceRef<BaseResource> Marker { get; set; }
            public ResourceRef<BaseResource> Dictionary { get; set; }
        }
    }
}
