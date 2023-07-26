using Assets.ColonyShared.GeneratedCode.Regions;
using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using NLog;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;
using SharedCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ColonyShared.GeneratedCode
{
    public class BoxShape : IShapeBinding<BoxShapeDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<bool> CheckCollision(BoxShapeDef def, Transform shapeOwnerTransform, Vector3 point, IEntitiesRepository repo)
        {
            Quaternion shapeRotation;
            if (def.Rotation == Vector3.zero)
                shapeRotation = shapeOwnerTransform.Rotation;
            else
                shapeRotation = Quaternion.Euler(def.Rotation) * shapeOwnerTransform.Rotation;

#if UNITY_EDITOR
            {
                BoxShapeDef debugDef = new BoxShapeDef();
                debugDef.Extents = def.Extents;
                debugDef.Position = shapeOwnerTransform.TransformPoint(def.Position);
                debugDef.Rotation = shapeRotation.eulerAngles;
                EntitytObjectsUnitySpawnService.SpawnService.DrawShape(debugDef, Color.red, 4f);
                debugDef = null;
                SphereShapeDef debugShapeDef = new SphereShapeDef();
                debugShapeDef.Position = point;
                debugShapeDef.Radius = 0.5f;
                EntitytObjectsUnitySpawnService.SpawnService.DrawShape(debugShapeDef, Color.blue, 4f);
                debugShapeDef = null;
            }
#endif

            return GeometryHelpers.IsPointInsideBox(shapeOwnerTransform.InverseTransformPoint(point), def.Position, def.Extents, Quaternion.Inverse(Quaternion.Euler(def.Rotation)));
        }

        public async Task<List<VisibilityDataSample>> GetPosibleVisibilityData(BoxShapeDef def, SpellPredCastData castData, Guid worldspaceId, Transform shapeOwnerTransform, IEntitiesRepository repo)
        {
            var allAround = new Dictionary<OuterRef<IEntity>, VisibilityDataSample>();

            Vector3 shapeWorldPosition = shapeOwnerTransform.TransformPoint(def.Position);
            float radius = def.GetBoundingRadius();

            var grid = VisibilityGrid.Get(worldspaceId, repo);
            grid.SampleDataForAllAround(shapeWorldPosition, allAround, radius, true);

            return allAround.Values.ToList(); 
        }
    }
}
