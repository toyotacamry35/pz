using GeneratedCode.DeltaObjects;
using GeneratedCode.Repositories;
using GeneratedDefsForSpells;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Repositories;
using SharedCode.Utils;
using SharedCode.Wizardry;
using System;
using System.Threading.Tasks;

namespace Assets.Src.Impacts
{
    class ImpactSpawnObject : IImpactBinding<ImpactSpawnObjectDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactSpawnObjectDef def)
        {
            if (cast.IsSlave)
                return;
            var selfDef = (ImpactSpawnObjectDef)def;
            var target = await selfDef.At.Target.GetOuterRef(cast, repo);
            var caster = cast.Caster;
            using (var casterAndTarget = await repo.Get(EntityBatch.Create()
                .Add(target, ReplicationLevel.Server)
                .Add(caster, ReplicationLevel.Server)))
            {
                var targetPos = PositionedObjectHelper.GetPositioned(casterAndTarget, target.TypeId, target.Guid).Transform.Position;
                var casterPos = PositionedObjectHelper.GetPositioned(casterAndTarget, caster.TypeId, caster.Guid)?.Transform.Position;// FIXME: target? или всё таки caster?
                Vector3 resultPos;
                if (casterPos.HasValue)
                {
                    var normalVector = Vector3.Cross((targetPos - casterPos.Value), Vector3.up).Normalized;
                    var projTowards = Vector3.Cross(Vector3.up, normalVector);
                    var up = Vector3.up;
                    Vector3.Scale(selfDef.Offset, ref normalVector, ref projTowards, ref up);
                    resultPos = targetPos + normalVector + projTowards + up;
                }
                else
                {
                    resultPos = targetPos;
                }
                var type = DefToType.GetEntityType(selfDef.EntityObject.Target.GetType());
                var typeId = ReplicaTypeRegistry.GetIdByType(type);
                await repo.Create(typeId, Guid.NewGuid(), async e => { ((IEntityObject)e).Def = selfDef.EntityObject.Target;
                {
                    var positionable = PositionedObjectHelper.GetPositionable(e);
                    if (positionable != null)
                    {
                        positionable.SetPosition = resultPos;
                    }
                } });

            }
            }
    }
}
