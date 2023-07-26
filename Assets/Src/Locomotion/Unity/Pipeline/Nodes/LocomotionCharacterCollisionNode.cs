using System;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Locomotion.Debug;
using ColonyShared.SharedCode.Aspects.Locomotion;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using Plugins.DebugDrawingExtension;
using UnityEngine;
using static Src.Locomotion.DebugTag;
using static Src.Locomotion.LocomotionDebug;
using static Src.Locomotion.Unity.PhysicsUtils;

namespace Src.Locomotion.Unity
{
    public delegate bool LocomotionStickinessPredicate(CollisionInfo nfo, LocomotionVector selfPos, LocomotionVector selfVelocity);
    
    public class LocomotionCharacterCollisionNode : ILocomotionPipelinePassNode
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly DebugDraw Drawer = DebugDraw.Manager.GetDrawer(Logger.Name);
        
        private static readonly RaycastHit[] Buffer = new RaycastHit[256];

        private readonly ISettings _settings;
        private readonly Collider _collider;
        private readonly GameObjectMatcher _objectMatcher;
        private readonly ILocomotionCollisionsReceiver _collisionReceiver;
        private readonly LocomotionStickinessPredicate _stickinessPredicate;
        private readonly IDumpingLoggerProvider _loggerProvider;
        private readonly Type _thisType = typeof(LocomotionCharacterCollisionNode);

        public LocomotionCharacterCollisionNode(
            ISettings settings, 
            [NotNull] Collider collider, 
            GameObjectMatcher objectMatcher, 
            ILocomotionCollisionsReceiver collisionReceiver,
            LocomotionStickinessPredicate stickinessPredicate,
            IDumpingLoggerProvider loggerProvider)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _collider = collider ?? throw new ArgumentNullException(nameof(collider));
            _objectMatcher = objectMatcher;
            _collisionReceiver = collisionReceiver;
            _stickinessPredicate = stickinessPredicate;
            _loggerProvider = loggerProvider;
        }

        public bool IsReady => true;

        public LocomotionVariables Pass(LocomotionVariables vars, float dt)
        {
            //#Dbg:
            if (_loggerProvider?.LogBackCounter > 0)
                _loggerProvider?.DLogger?.IfActive?.Log(_thisType, _loggerProvider.LogBackCounter, Consts.DbgTagIn, vars);

            if (vars.Flags.Any(LocomotionFlags.Teleport))
            {
                //#Dbg:
                if (_loggerProvider?.LogBackCounter > 0)
                    _loggerProvider?.DLogger?.IfActive?.Log(_thisType, _loggerProvider.LogBackCounter, Consts.DbgTagOut1, vars);

                return vars;
            }

            var startPosition = _collider.transform.position;
            var (direction, distance) = (vars.Position.ToWorld().ToUnity() - startPosition).Normalized();
            if (distance > 0)
            {
                var (position, stricted) = Step(startPosition, direction, distance, vars.Velocity, null);
                if (stricted)
                {
                    var newPosition = position.ToLocomotion();
                    DebugAgent.Set(CollisionDetection, vars.Position - newPosition);
                    //DbgLog.Log($"@@@CollisionDetection: {(vars.Position - newPosition).Magnitude}");
                    Drawer.Debug?.Collider(_collider, position, _collider.transform.rotation, Color.black);
                    vars.Position = newPosition;
                    vars.Velocity = (newPosition - _collider.transform.position.ToLocomotion()) / dt;
                }
            }

            //#Dbg:
            if (_loggerProvider?.LogBackCounter > 0)
                _loggerProvider?.DLogger?.IfActive?.Log(_thisType, _loggerProvider.LogBackCounter, Consts.DbgTagOut2, vars);

            return vars;
        }

        private (Vector3, bool) Step(Vector3 startPosition, Vector3 direction, float distance, LocomotionVector velocity, Collider prevObstacle, int iteration = 0)
        {
            if (iteration >= _settings.MaxIterations)
                return (startPosition, true);
            
            var count = CastColliderNonAlloc(_collider, startPosition, _collider.transform.rotation, direction, distance, Buffer, _objectMatcher.Layers, QueryTriggerInteraction.Collide);
            bool hasHit = false;
            bool overlaps = false;
            RaycastHit closestHit = default(RaycastHit);
            for (int i = 0; i < count; ++i)
            {
                var hit = Buffer[i];
                if (hit.collider == prevObstacle)
                    continue;
                if (IsSameObject(hit.collider, _collider))
                    continue;
                if (!_objectMatcher.IsMatch(hit.collider))
                    continue;
                if (Physics.GetIgnoreCollision(hit.collider, _collider))
                    continue;
                if (hit.point == Vector3.zero) // игнорируем коллайдеры с которыми мы уже пересекаемся, так как в этом случае cast возвращает некорректные данные в hit
                {
                    if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{hit.collider.name} overlaped with {_collider.name}").Write();
                    overlaps = true;
                    continue;
                }

                if (!hasHit || hit.distance <= closestHit.distance)
                {
                    if (Vector3.Dot(hit.normal, direction) < 0)
                    {
                        closestHit = hit;
                        hasHit = true;
                    }
                }
            }

            Drawer.Trace?
                .Line(startPosition, startPosition + direction * distance, hasHit ? Color.green : overlaps ? Color.red : Color.cyan)
                .Collider(_collider, startPosition + direction * 0.01f, _collider.transform.rotation, hasHit ? Color.green : overlaps ? Color.red : Color.blue)
                .Collider(_collider, startPosition + direction * distance, _collider.transform.rotation, hasHit ? Color.green : overlaps ? Color.red : Color.white);
            
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (hasHit)
            {
                var tangent = Vector3.Cross(closestHit.normal, Vector3.Cross(closestHit.normal, -direction));
                var normTangent = tangent.Normalized();
                var collisionInfo = new CollisionInfo(
                    point: closestHit.point.ToLocomotion(),
                    normal: closestHit.normal.ToLocomotion(),
                    objectLayer: closestHit.collider.gameObject.layer,
                    objectPosition: closestHit.collider.transform.position.ToLocomotion());
                
                _collisionReceiver?.OnCollision(collisionInfo);

                var sticky = _stickinessPredicate?.Invoke(collisionInfo, startPosition.ToLocomotion(), velocity) ?? false;
                
                Drawer.Debug?
                    .Line(closestHit.point, closestHit.point + closestHit.normal * 0.5f, Color.magenta)
                    .Line(closestHit.point, closestHit.point + tangent * 0.5f, Color.blue)
                    .Collider(closestHit.collider, closestHit.collider.transform.position, closestHit.collider.transform.rotation, Color.white);

                var nextPosition = startPosition + direction * closestHit.distance + closestHit.normal * _settings.DepenetrationOffset;

                DebugAgent.Set(Sticking, sticky);
                
                return sticky ? 
                    (nextPosition, true) :
                    Step(nextPosition, normTangent.Item1, (distance - closestHit.distance) * normTangent.Item2, velocity, closestHit.collider, iteration + 1);
            }
            
            return (startPosition + direction * distance, iteration > 0);
        }

        public interface ISettings
        {
            int MaxIterations { get; }
            float DepenetrationOffset { get; }
        }
    }
}