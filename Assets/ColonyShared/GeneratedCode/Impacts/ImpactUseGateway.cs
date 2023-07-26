using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using NLog;
using Shared.ManualDefsForSpells;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using SharedCode.Aspects.Sessions;

namespace Assets.Src.Impacts
{
    public class ImpactUseGateway : IImpactBinding<ImpactUseGatewayDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("ImpactActivateBaken");

        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repository, ImpactUseGatewayDef def)
        {
            var selfDef = (ImpactUseGatewayDef)def;

            if (selfDef.Target.Target == null)
                return;

            var target = await selfDef.Target.Target.GetOuterRef(cast, repository);
            if (!target.IsValid)
                return;

            var source = cast.Caster;
            if (selfDef.Source.Target != null)
                source = await selfDef.Source.Target.GetOuterRef(cast, repository);

            if(!source.IsValid)
                source = cast.Caster;
            Guid userId;
            using (var wrapper = await repository.GetFirstService<ILoginInternalServiceEntityServer>())
            {
                var li = wrapper.GetFirstService<ILoginInternalServiceEntityServer>();
                userId = await li.GetUserIdByCharacterId(cast.Caster.Guid);
                //.RequestLoginToMap(new SharedCode.Entities.Service.MapLoginMeta() { UserId })
            }

            RealmRulesDef realmRules = null;
            using (var wrapper = await repository.Get(source.Type, source.Guid))
            {
                var scenicEntity = wrapper.Get<IScenicEntityServer>(source, ReplicationLevel.Server);
                if (scenicEntity != null)
                {
                    var mapEntityRef = scenicEntity.MapOwner.OwnerMap;
                    using (var cnt = await repository.Get(mapEntityRef.Type, mapEntityRef.Guid))
                    {
                        var mapEntity = cnt.Get<IMapEntityServer>(mapEntityRef, ReplicationLevel.Server);
                        if (mapEntity != null)
                        {
                            realmRules = mapEntity.RealmRules;
                            if (mapEntity.Map == def.Map)
                                return;
                        }
                        else
                            Logger.IfError()?.Message($"Can't get IMapEntityServer for {mapEntityRef}").Write();
                    }
                }
                else
                    Logger.IfError()?.Message($"Can't get IScenicEntityServer for {source}").Write();
            }
            
            using (var wrapper = await repository.GetFirstService<IWorldCoordinatorNodeServiceEntityServer>())
            {
                var wc = wrapper.GetFirstService<IWorldCoordinatorNodeServiceEntityServer>();
                var meta = new SharedCode.Entities.Service.MapLoginMeta() {UserId = userId, TargetMap = def.Map, RealmRules = realmRules};
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"MapLoginMeta:{meta}").Write();
                await wc.RequestLoginToMap(meta);
            }
            return;

            /*if (selfDef.CommonBaken)
            {
                using (var wrapper = await repository.Get<IWorldCharacter>(source.Guid))
                {
                    var worldCharacter = wrapper.Get<IWorldCharacterServer>(source.Guid);
                    var targetE = repository.TryGetLockfree<IScenicEntityServer>(target, ReplicationLevel.Server);
                    if (targetE != null)
                        return await worldCharacter.ActivateCommonBaken(targetE.StaticIdFromExport);
                    else
                        return false;
                }
                
            }
            else
            {
                using (var wrapper = await repository.GetFirstService<IBakenCoordinatorServiceEntityServer>())
                {
                    var bakenCoordinator = wrapper.GetFirstService<IBakenCoordinatorServiceEntityServer>();
                    if (await bakenCoordinator.ActivateBaken(source.Guid, target))
                        using (var wc = await repository.Get<IWorldCharacter>(source.Guid))
                        {
                            var worldCharacter = wc.Get<IWorldCharacterServer>(source.Guid);
                            return await worldCharacter.ActivateCommonBaken(Guid.Empty);
                        }
                    else
                        return false;
                }
            }*/
        }
    }
    public class PredicateIsMap : IPredicateBinding<PredicateIsMapDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("ImpactActivateBaken");

        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateIsMapDef def)
        {
            var source = cast.Caster;
            using (var wrapper = await repo.Get(source.Type, source.Guid))
            {
                var scenicEntity = wrapper.Get<IScenicEntityServer>(source, ReplicationLevel.Server);
                if (scenicEntity != null)
                {
                    var mapEntityRef = scenicEntity.MapOwner.OwnerMap;
                    using (var cnt = await repo.Get(mapEntityRef.Type, mapEntityRef.Guid))
                    {
                        var mapEntity = cnt.Get<IMapEntityServer>(mapEntityRef, ReplicationLevel.Server);
                        if (mapEntity != null)
                        {
                            if (mapEntity.Map == def.Map)
                                return true;
                        }
                        else
                            Logger.Error($"Can't get IMapEntityServer for {mapEntityRef}");
                    }
                }
                else
                    Logger.Error($"Can't get IScenicEntityServer for {source}");
            }
            return false;
        }
    }
}
