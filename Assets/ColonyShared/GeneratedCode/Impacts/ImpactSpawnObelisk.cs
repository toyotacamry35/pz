using GeneratedCode.DeltaObjects;
using SharedCode.EntitySystem;
using SharedCode.Utils;
using SharedCode.Wizardry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneratedDefsForSpells;
using GeneratedCode.Repositories;
using SharedCode.Entities.GameObjectEntities;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Serializers;
using SharedCode.MapSystem;
using SharedCode.Entities.Service;
using SharedCode.Entities;
using SharedCode.Repositories;

namespace GeneratedCode.Impacts
{
    class ImpactSpawnObelisk : IImpactBinding<ImpactSpawnObeliskDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactSpawnObeliskDef def)
        {

            var selfDef = def;
            using (var self = await repo.Get(cast.Caster.TypeId, cast.Caster.Guid))
            {
                var spawnedType = DefToType.GetEntityType(selfDef.Obelisk.Target.GetType());
                var outerRef = new OuterRef<IEntityObject>(Guid.NewGuid(), ReplicaTypeRegistry.GetIdByType(spawnedType));
                var mapOwner = self.Get<IHasMappedServer>(cast.Caster.TypeId, cast.Caster.Guid, ReplicationLevel.Server).MapOwner;
                var wsGuid = self.Get<IHasWorldSpacedServer>(cast.Caster.TypeId, cast.Caster.Guid, ReplicationLevel.Server).WorldSpaced.OwnWorldSpace.Guid;

                var targetORef = await selfDef.On.Target.GetOuterRef(cast, repo);
                using (var targetPosition = await repo.Get(targetORef))
                {
                    var transform = PositionedObjectHelper.GetPositioned(targetPosition, targetORef.TypeId, targetORef.Guid).Transform;
                    AsyncUtils.RunAsyncTask(async () =>
                    {
                        var newEnt = await repo.Create(outerRef.TypeId, outerRef.Guid, x =>
                        {
                            if (x is IHasMapped mapped)
                            {
                                mapped.MapOwner = mapOwner;
                            }
                            if (x is IHasWorldSpaced worldSpaced)
                                worldSpaced.WorldSpaced.OwnWorldSpace = new OuterRef<IWorldSpaceServiceEntity>(wsGuid, ReplicaTypeRegistry.GetIdByType(typeof(IWorldSpaceServiceEntity)));

                            var eObj = (IEntityObject)x;
                            eObj.Def = selfDef.Obelisk.Target;
                            var positionable = PositionedObjectHelper.GetPositionable(x);
                            if (positionable != null)
                            {
                                positionable.SetTransform = transform;
                            }
                            return Task.CompletedTask;
                        });
                    }, repo);
                }
            }
            }
    }
}
