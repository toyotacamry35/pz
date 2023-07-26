using ResourcesSystem.Loader;
using Newtonsoft.Json;
using NLog;
using SharedCode.Entities.GameObjectEntities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Environment.Logging.Extension;

namespace Assets.Src.SpawnSystem
{
    public static class SpawnDaemonsInfo
    {
        private static NLog.Logger Logger = LogManager.GetLogger("SpawnDaemonsInfo");

        public static bool AcceptLogs { get; set; } = false;

        public static readonly ConcurrentDictionary<Guid, SpawnDaemonInfo> _daemonsList = new ConcurrentDictionary<Guid, SpawnDaemonInfo>();

        public static void Add(Guid guid, string name, IEntityObjectDef resource)
        {
            if (AcceptLogs == false)
                return;
            if (!_daemonsList.ContainsKey(guid))
            {
                var dInfo = new SpawnDaemonInfo();
                dInfo.SpawnDaemonName = name;
                _daemonsList.TryAdd(guid, dInfo);
            }
            _daemonsList[guid].Add(resource);
        }

        public static void Remove(Guid guid, IEntityObjectDef resource)
        {
            if (AcceptLogs == false)
                return;
            if (_daemonsList.ContainsKey(guid))
                _daemonsList[guid].Remove(resource);
        }

        private static StringWriter GenerateJSON(IEnumerable<KeyValuePair<Guid,SpawnDaemonInfo>> daemons)
        {
            var str = new StringWriter();
            JsonSerializer serializer = new JsonSerializer();
            var resources = new GameResources(null);

            serializer.Converters.Add(new DefReferenceConverter(resources.Deserializer, false));
            serializer.Converters.Add(new DefConverter(resources.LoadedResources, resources.Deserializer));
            serializer.Formatting = Formatting.Indented;
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            serializer.Serialize(str, daemons);
            return str;
        }

        public static void OnUpdate()
        {
            var daemons = SpawnDaemonsInfo._daemonsList.Where(x => x.Value.SpawnedObjects.Any(y => y.Value.RemovedLastTime != 0 || y.Value.SpawnedLastTime != 0));
            if (daemons.Count() == 0)
                return;
            var jsonString = GenerateJSON(daemons);
            //var spw = SpawnDaemonsInfo._daemonsList.Values.SelectMany(v => v.SpawnedObjects.Values);
            //var jsonString = spw.Sum(v => v.SpawnedLastTime) + " | " + spw.Sum(v => v.SpawnedFromStart);
            foreach (var daemon in _daemonsList)
                daemon.Value.Reset();
            
            Logger.IfDebug()?.Message(jsonString.ToString()).Write();
        }
    }

    public class SpawnDaemonInfo
    {
        public string SpawnDaemonName { get; set; }
        [JsonIgnore]
        public ConcurrentDictionary<IEntityObjectDef, CreationInfo> SpawnedObjects { get; private set; } = new ConcurrentDictionary<IEntityObjectDef, CreationInfo>();
        [JsonProperty("SpawnedObjects")]
        public IEnumerable<CreationInfo> NonZeroInfos => SpawnedObjects.Where(x => x.Value.SpawnedFromStart != 0 || x.Value.RemovedLastTime != 0).Select(x => x.Value);
        public void Add(IEntityObjectDef resource)
        {

            CreationInfo info = SpawnedObjects.GetOrAdd(resource, x => new CreationInfo());
            if (info != null)
            {
                info.Object = resource;
                SpawnedObjects[resource].Increase();
            }
        }

        public void Reset()
        {
            foreach (var obj in SpawnedObjects)
                obj.Value.Reset();
        }

        public void Remove(IEntityObjectDef resource)
        {
            CreationInfo info;
            SpawnedObjects.TryGetValue(resource, out info);
            info.Decrease();
        }
    }

    // [JsonConverter(typeof(CustomCreationInfoJSONConverter))]
    public class CreationInfo
    {
        public IEntityObjectDef Object { get; set; }
        public int SpawnedFromStart { get; private set; } = 1;
        public int SpawnedLastTime { get; private set; } = 1;
        public int RemovedLastTime { get; private set; } = 0;
        public int Exists { get; private set; } = 1;
        public void Increase()
        {
            lock (this)
            {
                SpawnedFromStart++;
                SpawnedLastTime++;
                Exists++;
            }
        }
        public void Reset()
        {
            lock (this)
            {
                SpawnedLastTime = 0;
                RemovedLastTime = 0;
            }
        }
        public void Decrease()
        {
            lock (this)
            {
                RemovedLastTime++;
                Exists--;
            }
        }
    }

    class CustomCreationInfoJSONConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var currentObj = value as CreationInfo;
            if (currentObj.SpawnedLastTime != 0)
            {
                //writer.WriteStartObject();
                // var property = value.GetType().GetProperty("spawnedObjects");
                // writer.WritePropertyName(property.Name);
                //serializer.Serialize(writer, value);
                //writer.WriteEndObject();
                writer.WriteValue(value);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsClass;
        }
    }

}