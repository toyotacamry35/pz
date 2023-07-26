using System;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Locomotion.Debug;
using ColonyShared.SharedCode.Aspects.Locomotion;
using JetBrains.Annotations;
using UnityEngine;
using static Src.Locomotion.DebugTag;
using static Src.Locomotion.LocomotionDebug;

namespace Src.Locomotion.Unity
{
    public class LocomotionCharacterDepenetrationNode : ILocomotionPipelinePassNode
    {
        private static readonly Collider[] OverlappedColliders = new Collider[256];

        private readonly ISettings _setiings;
        private readonly Collider _collider;
        private readonly GameObjectMatcher _objectMatcher;
        private readonly string[] _tags;
        private readonly ILocomotionCollisionsReceiver _collisionReceiver;
        private readonly IDumpingLoggerProvider _loggerProvider;
        private readonly Type _thisType = typeof(LocomotionCharacterDepenetrationNode);

        public LocomotionCharacterDepenetrationNode(
            [NotNull] ISettings setiings, 
            [NotNull] Collider collider, 
            GameObjectMatcher objectMatcher, 
            ILocomotionCollisionsReceiver collisionReceiver,
            IDumpingLoggerProvider loggerProvider)
        {
            _setiings = setiings ?? throw new ArgumentNullException(nameof(setiings));
            _collider = collider ?? throw new ArgumentNullException(nameof(collider));
            _objectMatcher = objectMatcher;
            _collisionReceiver = collisionReceiver;
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

            var rotation = LocomotionHelpers.LocomotionToWorldOrientation(vars.Orientation).ToUnity();
            var position = vars.Position.ToWorld().ToUnity();
            var overlapCount = PhysicsUtils.OverlapColliderNonAlloc(_collider, position, rotation, OverlappedColliders, _objectMatcher.Layers, QueryTriggerInteraction.Collide);
            bool affected = false;
            for (int i = 0; i < overlapCount; i++)
            {
                var overlapCollider = OverlappedColliders[i];
                if (PhysicsUtils.IsSameObject(overlapCollider, _collider))
                    continue;
                if (!_objectMatcher.IsMatch(overlapCollider))
                    continue;
                if (Physics.GetIgnoreCollision(_collider, overlapCollider))
                    continue;
                if (Physics.ComputePenetration(_collider, position, rotation,
                    overlapCollider, overlapCollider.transform.position, overlapCollider.transform.rotation,
                    out var dir, out float dist))
                {
                    dir.y = 0; // Не двигаем вверх! Да, при этом мы можем не полностью вытянуть себя из другого объекта, но это лучше чем подскакивание.
                    position += dir * (dist + _setiings.AdditionalOffset);
                    affected = true;

                    if (_collisionReceiver != null && CreateCollisionInfo(position, dir, dist, overlapCollider, out var nfo))
                        _collisionReceiver.OnCollision(nfo);
                }
            }

            if (affected)
            {
                var originalPosition = vars.Position;
                vars.Position = position.ToLocomotion();
                DebugAgent?.Set(Depenetration, originalPosition - vars.Position);
                //DbgLog.Log($"@@@Depenetration: {(originalPosition - vars.Position).Magnitude}");
            }

            //#Dbg:
            if (_loggerProvider?.LogBackCounter > 0)
                _loggerProvider?.DLogger?.IfActive?.Log(_thisType, _loggerProvider.LogBackCounter, Consts.DbgTagOut2, vars);

            return vars;
        }

        private bool CreateCollisionInfo(Vector3 position, Vector3 direction, float distance, Collider obstacle, out CollisionInfo nfo)
        {
            // очень приблизительно вычисляем точку контакта, предполагая, что _collider сфера или капсула.
            var (directionNormalized, magn) = direction.Normalized();
            if (magn.ApproximatelyZero())
            {
                nfo = new CollisionInfo(
                    point: position.ToLocomotion(), 
                    normal: LocomotionVector.Zero, 
                    objectLayer: obstacle.gameObject.layer,
                    objectPosition: obstacle.transform.position.ToLocomotion());
                return false;
            }
            var ray = new Ray(position, -directionNormalized);
            if (obstacle.Raycast(ray, out var hit, distance * 2))
            {
                nfo = new CollisionInfo(
                    point: hit.point.ToLocomotion(), 
                    normal: hit.normal.ToLocomotion(), 
                    objectLayer: obstacle.gameObject.layer,
                    objectPosition: obstacle.transform.position.ToLocomotion());
                return true;
            }
            nfo = default(CollisionInfo);
            return false;
        }

        public interface ISettings
        {
            float AdditionalOffset { get; }
        }
    }
}