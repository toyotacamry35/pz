using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using NLog;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyShared.GeneratedCode;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.MovementSync;
using SharedCode.Utils;

namespace Assets.ColonyShared.GeneratedCode.Impacts.ShapeUtils
{
    public static class ShapeQuerySystem
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        [CollectTypes, UsedImplicitly] private static ShapeBindingsCollector _shapeImpls;

        public static async Task<List<OuterRef<IEntity>>> FindAffectedTargets(OuterRef<IEntity> shapeOwner, Guid worldspaceId, Transform shapeOwnerTransform, SpellWordCastData cast, ShapeDef shape, IEntitiesRepository repo, VisibilityDataSample target = default)
        {
            List<OuterRef<IEntity>> result = new List<OuterRef<IEntity>>(8);
            ShapeBinding shapeBinding;
            try
            {
                if (_shapeImpls.Collection.TryGetValue(shape.GetType(), out shapeBinding))
                {
                    List<VisibilityDataSample> allAround = new List<VisibilityDataSample>();
                    
                    if (shape.CheckTargetOnly && !target.Equals(default))
                    {
                        allAround.Add(target);
#if UNITY_EDITOR
                        {
                            SphereShapeDef debugDef = new SphereShapeDef();
                            debugDef.Position = target.Pos;
                            debugDef.Radius = 0.5f;
                            EntitytObjectsUnitySpawnService.SpawnService.DrawShape(debugDef, Color.red, 2f);
                            debugDef = null;
                        }
#endif
                    }
                    else
                    {
                        allAround = await shapeBinding.GetPosibleVisibilityData(shape, cast, worldspaceId, shapeOwnerTransform, repo);
#if UNITY_EDITOR
                        {
                            SphereShapeDef debugDef = new SphereShapeDef();
                            debugDef.Position = shapeOwnerTransform.TransformPoint(shape.Position);
                            debugDef.Radius = shape.GetBoundingRadius();
                            EntitytObjectsUnitySpawnService.SpawnService.DrawShape(debugDef, Color.green, 4);
                            debugDef = null;
                        }
#endif
                    }
                    foreach (var entPos in allAround)
                    {
                        if (!shapeOwner.Equals(entPos.Ref) && await shapeBinding.CheckCollision(shape, shapeOwnerTransform, entPos.Pos, repo))
                            result.Add(entPos.Ref);
                    }
                }
                else
                    Logger.IfError()?.Message($" {nameof(FindAffectedTargets)} is not implemented for {shape.GetType()}").Write();
            }
            catch (Exception e)
            {
                Logger.IfError()?.Exception(e).Write();
                throw;
            }

            return result;
        }
        public static Dictionary<OuterRef<IEntity>, VisibilityDataSample> GetEntitiesInRadiusVisibilityData(Guid worldSpaceId, OuterRef<IEntity> owner, float radius, IEntitiesRepository repo)
        {
            var allAround = new Dictionary<OuterRef<IEntity>, VisibilityDataSample>();
            try
            {
                var grid = VisibilityGrid.Get(worldSpaceId, repo);
                grid.SampleDataForAllAround(owner, allAround, radius, true);
            }
            catch (Exception e)
            {
                Logger.IfError()?.Exception(e).Write();
                throw;
            }

            return allAround;
        }
		
        // ReSharper disable once ClassNeverInstantiated.Local
        private class ShapeBindingsCollector : BindingCollector<IShapeBinding, ShapeBinding, ShapeDef>
        {
            public ShapeBindingsCollector() : base(typeof(IShapeBinding<>), typeof(ShapeBinding<>)) {}
        }
    }
}
