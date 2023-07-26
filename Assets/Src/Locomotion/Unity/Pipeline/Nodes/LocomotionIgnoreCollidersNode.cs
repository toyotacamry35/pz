using System;
using System.Collections.Generic;
using System.Linq;
using ColonyShared.SharedCode.Aspects.Locomotion;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using UnityEngine;
using static UnityEngine.Mathf;

namespace Src.Locomotion.Unity
{
    public class LocomotionIgnoreCollidersNode : ILocomotionPipelineCommitNode, IDisposable
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        private const float RadiusFactor = 1.1f;
        private const float SwipeDistanceLimitSqr = 50f * 50f;
        private static readonly Collider[] OverlappingColliders = new Collider[256];

        private readonly LocomotionFlags _flag;
        private readonly LayerMask _mask;
        private readonly float _radius;
        private readonly Transform _root;
        private readonly Collider[] _colliders;
        private HashSet<Collider> _ignoredColliders = new HashSet<Collider>();
        private HashSet<Collider> _newIgnoredColliders = new HashSet<Collider>();

        public LocomotionIgnoreCollidersNode(Transform root, Collider[] colliders, LocomotionFlags flag, LayerMask mask)
        {
            _root = root ?? throw new ArgumentNullException(nameof(root));
            _colliders = colliders ?? throw new ArgumentNullException(nameof(colliders));
            _flag = flag;
            _mask = mask;
            
            var bounds = new Bounds(root.position, Vector3.zero);
            foreach (var collider in _colliders)
                bounds.Encapsulate(collider.bounds);
            _radius = Max(bounds.extents.x, bounds.extents.y, bounds.extents.z, _radius) * RadiusFactor;
            
            if (_radius > 5)
                Logger.IfError()?.Message($"{nameof(LocomotionIgnoreCollidersNode)} radius is too big: {_radius}").Write();
        }

        public bool IsReady => true;
        
        public void Commit(ref LocomotionVariables inVars, float dt)
        {
            if (_colliders.Length == 0)
                return;
            
            bool ignore = inVars.Flags.Any(_flag);
            if (ignore)
                ProcessIgnoreCollision(_root.position, inVars.Position.ToWorld().ToUnity(), inVars.Flags.Any(LocomotionFlags.Teleport));
            else
                RestoreCollision();
        }

        void IDisposable.Dispose()
        {
            foreach (var collider in _colliders)
                foreach (var ignoredCollider in _ignoredColliders)
                    if (ignoredCollider)
                        Physics.IgnoreCollision(collider, ignoredCollider, false);
            _ignoredColliders.Clear();
        }
        
        private void ProcessIgnoreCollision (Vector3 prevPosition, Vector3 nextPosition, bool teleport)
        {
            var shiftSqr = (nextPosition - prevPosition).sqrMagnitude;
            int count = 0;
            if (!teleport && shiftSqr > 0.01f)
            {
                if (shiftSqr < SwipeDistanceLimitSqr)
                {
                    count = Physics.OverlapCapsuleNonAlloc(prevPosition, nextPosition, _radius,
                        layerMask: _mask, queryTriggerInteraction: QueryTriggerInteraction.Ignore, results: OverlappingColliders);
                }
                else
                    Logger.IfError()?.Message($"{nameof(LocomotionIgnoreCollidersNode)} swipe distance is too big: {(nextPosition - prevPosition).magnitude} PrevPt:{prevPosition} NextPt:{nextPosition}").Write();
            }
            else
                count = Physics.OverlapSphereNonAlloc(nextPosition, _radius, 
                    layerMask: _mask, queryTriggerInteraction: QueryTriggerInteraction.Ignore, results: OverlappingColliders);

            for (int i = 0; i < count; i++)
            {
                var overlappingCollider = OverlappingColliders[i];
                if (!_colliders.Contains(overlappingCollider))
                {
                    _newIgnoredColliders.Add(overlappingCollider);
                    if (!_ignoredColliders.Remove(overlappingCollider))
                    {
                        foreach (var collider in _colliders)
                        {
                            if (collider)
                            {
                                Physics.IgnoreCollision(collider, overlappingCollider, true);
                                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Ignore collision between {collider.name} and {overlappingCollider.name}").Write();
                            }
                        }
                    }
                }
            }
            
            foreach (var ignoredCollider in _ignoredColliders)
            {
                if (ignoredCollider)
                {
                    foreach (var collider in _colliders)
                    {
                        if (collider)
                        {
                            Physics.IgnoreCollision(collider, ignoredCollider, false);
                            if (Logger.IsDebugEnabled)
                                Logger.IfDebug()?.Message($"Restore collision between {collider.name} and {ignoredCollider.name}").Write();
                        }
                    }
                }
            }

            _ignoredColliders.Clear();
            (_ignoredColliders, _newIgnoredColliders) = (_newIgnoredColliders, _ignoredColliders);
        }

        private void RestoreCollision()
        {
            foreach (var collider in _colliders)
            {
                int count = -1;
                foreach (var ignoredCollider in _ignoredColliders)
                {
                    if (!ignoredCollider)
                        continue;

                    if (count == -1)
                        count = PhysicsUtils.OverlapColliderNonAlloc(collider, collider.transform.position, collider.transform.rotation,
                            layerMask: _mask, queryTriggerInteraction: QueryTriggerInteraction.Ignore, results: OverlappingColliders);

                    // если мы все еще пересекаемся с другим коллайдером, то не убираем этот коллайдер из игнорируемых, пока пересечение не исчезнет
                    bool skip = false;
                    for (int i = 0; i < count && !skip; ++i)
                        if (OverlappingColliders[i] == ignoredCollider)
                            skip = true;

                    if (!skip)
                    {
                        Physics.IgnoreCollision(collider, ignoredCollider, false);
                        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Restore collision between {collider.name} and {ignoredCollider.name}").Write();
                    }
                    else
                        _newIgnoredColliders.Add(ignoredCollider);
                }
            }
            _ignoredColliders.Clear();
            (_ignoredColliders, _newIgnoredColliders) = (_newIgnoredColliders, _ignoredColliders);
        }
    }
}