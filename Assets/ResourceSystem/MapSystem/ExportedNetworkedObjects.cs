using Assets.Src.ResourcesSystem.Base;
using Newtonsoft.Json;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace ResourceSystem.MapSystem
{
    public class ExportedNetworkedObjectsDef : BaseResource
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public List<ResourceRef<ExportedNetworkedObjectDef>> Objects { get; set; } = new List<ResourceRef<ExportedNetworkedObjectDef>>();
        [JsonIgnore]
        Dictionary<Guid, ResourceRef<ExportedNetworkedObjectDef>> _objects;
        [JsonIgnore]
        public Dictionary<Guid, ResourceRef<ExportedNetworkedObjectDef>> ObjectsMap
        {
            get
            {
                lock (this)
                {
                    if (_objects == null)
                    {
                        _objects = new Dictionary<Guid, ResourceRef<ExportedNetworkedObjectDef>>();
                        foreach (var obj in Objects)
                            if (!_objects.ContainsKey(obj.Target.StaticId))
                                _objects.Add(obj.Target.StaticId, obj);
                            else
                                Logger.IfError()?.Message($"Duplicate guid {((BaseResource)obj.Target.Object.Target).____GetDebugShortName()} {obj.Target.StaticId}").Write();
                    }
                    return _objects;
                }
            }
        }
    }

    public class ExportedNetworkedObjectDef : BaseResource
    {
        public Guid StaticId { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public ResourceRef<IEntityObjectDef> Object { get; set; }
        public float TimeToRespawn { get; set; }
    }

}
