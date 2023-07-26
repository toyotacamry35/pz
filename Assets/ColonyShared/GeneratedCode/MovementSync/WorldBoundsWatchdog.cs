using System;
using System.Threading.Tasks;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.MapSystem;
using NLog;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.MapSystem;
using SharedCode.Utils;

namespace ColonyShared.GeneratedCode.MovementSync
{
    public class WorldBoundsWatchdog
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        private Bounds _worldBounds;
        private IEntitiesRepository _repository;
        private OuterRef<IHasMortalServer> _owner;

        public async Task<WorldBoundsWatchdog> Initialize(OuterRef<IHasMortalServer> owner, IEntitiesRepository repo, MapOwner mapOwner)
        {
            if (!owner.IsValid) throw new ArgumentException(nameof(owner));

            _owner = owner;
            _repository = repo ?? throw new ArgumentNullException(nameof(repo));
            _worldBounds = new Bounds(Vector3.zero, Vector3.one * (float.MaxValue / 2));

            using (var mapWrapper = await repo.Get(mapOwner.OwnerMap))
            {
                var map = mapWrapper.Get<IMapEntityAlways>(mapOwner.OwnerMap, ReplicationLevel.Always);
                if (map != null)
                {
                    var sceneCollection = map.Map?.SceneCollectionClient.Target;
                    if (sceneCollection != null)
                    {
                        var min = new Vector3(
                            sceneCollection.SceneStart.x * sceneCollection.SceneSize.x,
                            sceneCollection.SceneStart.y * sceneCollection.SceneSize.y,
                            sceneCollection.SceneStart.z * sceneCollection.SceneSize.z);
                        var max = new Vector3(
                            min.x + sceneCollection.SceneCount.x * sceneCollection.SceneSize.x,
                            min.y + sceneCollection.SceneCount.y * sceneCollection.SceneSize.y,
                            min.z + sceneCollection.SceneCount.z * sceneCollection.SceneSize.z);
                        _worldBounds = new Bounds((min + max) * 0.5f, max - min);
                    }
                    else
                    {
                        Logger.IfWarn()?.Message($"No SceneCollectionClient in {map.Map?.____GetDebugAddress()}").Write();
                    }
                }
                else
                {
                    Logger.IfWarn()?.Message($"Couldn't get {nameof(IMapEntity)}").Write();
                }
            }
            
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{nameof(WorldBoundsWatchdog)} initialized. Bounds:[{_worldBounds.Min} - {_worldBounds.Max}]").Write();
            return this;
        }

        public async Task Update(Vector3 position)
        {
            if (!_worldBounds.Contains(position))
            {
                using (var wrapper = await _repository.Get(_owner))
                {
                    if(Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"{nameof(WorldBoundsWatchdog)} position: {position}").Write();
                    var victim = wrapper?.Get(_owner, ReplicationLevel.Server);
                    if (victim != null)
                    {
                        Logger.IfInfo()?.Message($"Entity {_owner} was killed due out of bounds. Position:{position}. Bounds:[{_worldBounds.Min} - {_worldBounds.Max}]").Write();
                        await victim.Mortal.Die();
                    }
                }
            }
        }
    }
}