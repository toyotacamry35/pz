using System;
using Assets.Src.Aspects.RegionsScenicObjects;
using Assets.Src.RubiconAI;
using Assets.Src.Shared.Impl;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using UnityEngine;

namespace Assets.Src.Wizardry
{
    public static class CastDataExtension
    {
        public static GameObject GetCaster(this SpellPredCastData castData)
        {
            OuterRef<IEntity> ent = castData.Caster;
            var targetGo = GameObjectCreator.ClusterSpawnInstance.GetImmediateObjectFor(new OuterRef<IEntityObject>(ent.Guid, ent.TypeId));
            return targetGo;
        }

        public static GameObject GetGo(this SpellEntityDef targetDef, SpellPredCastData castData)
        {
            OuterRef<IEntity> ent = default(OuterRef<IEntity>);

            if (targetDef != null)
            {
                if (targetDef is SpellCasterDef)
                    ent = castData.Caster;
                else if (targetDef is SpellTargetDef)
                    ent = (castData.CastData as IWithTarget)?.Target ?? default(OuterRef<IEntity>);
                else
                    throw new NotSupportedException($"{targetDef.GetType()}");
            }
            else
                SpellDoerAsync.Logger.IfDebug()?.Message($"{nameof(targetDef)} == null").Write();

            if (!ent.IsValid)
            {
                SpellDoerAsync.Logger.IfDebug()?.Message($"{targetDef?.GetType().Name ?? "null"}: !ent.{nameof(ent.IsValid)}").Write();
                return null;
            }

            var targetGo = GameObjectCreator.ClusterSpawnInstance.GetImmediateObjectFor(new OuterRef<IEntityObject>(ent.Guid, ent.TypeId));
            if (targetGo == null)
                SpellDoerAsync.Logger.IfDebug()?.Message($"{nameof(GameObjectCreator.ClusterSpawnInstance.GetImmediateObjectFor)} returns null.").Write();

            return targetGo;
        }

        public static Vector3 GetVec3(this SpellVector3Def targetDef, SpellPredCastData castData, Vector3 defaultVec)
        {
            return targetDef?.GetVector(castData).ToXYZ() ?? default;
        }

        public static Legionary GetLegionary(this SpellEntityDef targetDef, SpellPredCastData castData)
        {
            if (targetDef == null) throw new ArgumentNullException(nameof(targetDef));
            var targetGo = targetDef.GetGo(castData);
            if (!targetGo) throw new Exception($"Can't get target legionary GO | {castData}");
            var targetLegionaryComponent = targetGo.GetComponent<ISpatialLegionary>();
            if (targetLegionaryComponent == null) throw new Exception($"Can't get target legionary component from {targetGo.transform.FullName()} | {castData}");
            return targetLegionaryComponent.Legionary;
        }

        public static Legionary TryGetLegionary(this SpellEntityDef targetDef, SpellPredCastData castData)
        {
            return targetDef?.GetGo(castData)?.GetComponent<ISpatialLegionary>()?.Legionary;
        }
    }
}