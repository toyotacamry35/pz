using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using GeneratedCode.DeltaObjects;
using ResourceSystem.Aspects.Misc;
using Src.Animation;
using Src.Locomotion.Unity;
using Src.Tools;
using UnityEngine;
using static Assets.Src.Shared.GameObjectTags;
using static Assets.Src.Shared.PhysicsLayers;

namespace Assets.Src.Character.Events
{
    public delegate void AttackHitDelegate(SpellPartCastId castId, in HitInfo hit);
    
    [RequireComponent(typeof(GameObjectMarker))]
    public class AttackEventSubscriptionHandler : MonoBehaviour
    {
        private static readonly Lazy<GameObjectMatcher> ObjectMatcher = new Lazy<GameObjectMatcher>(() => new GameObjectMatcher(
            layers: DefaultMask | DestructablesMask | ActiveMask,
            tagsNot: new[] {SoftColliderTag})); 
        
        [SerializeField] private List<HitColliderHandler> _defaultColliders;
        
        private readonly List<HitColliderHandler> _colliders = new List<HitColliderHandler>();

        private List<HitColliderHandler> CurrentColliders => _colliders.Count > 0 ? _colliders : _defaultColliders;

        
        public event AttackHitDelegate AttackHitDetected;

        private void Awake()
        {
            if (_defaultColliders != null)
                foreach (var c in _defaultColliders)
                    c.Setup(OnAttackColliderHit, transform, default(DamageTypeDef), ObjectMatcher.Value);
        }

        public void SetupColliders(IEnumerable<HitColliderHandler> colliders, DamageTypeDef damageType)
        {
            if (colliders != null)
                _colliders.AddRange(colliders
                    .Except(_colliders) // FIXME: или же вместо этого сначала делать RemoveColliders?
                    .Select(c => c.Setup(OnAttackColliderHit, transform, damageType, ObjectMatcher.Value)));
        }

        public void DisposeColliders(IEnumerable<HitColliderHandler> colliders)
        {
            if (colliders != null)
                foreach(var e in colliders.Select(c => c.Dispose()))
                    _colliders.Remove(e);
        }
 
        public void ActivateColliders(SpellPartCastId attackId, Transform body, AnimationTrajectory trajectory, GameObjectMarkerDef colliderMarker, float trajectoryStart, float trajectoryEnd, float speed)
        {
            var cldr = CurrentColliders.FirstOrDefault(x => x.Marker == colliderMarker) ?? CurrentColliders.FirstOrDefault(x => x.Marker == null) ?? CurrentColliders.First();
            cldr.ActivateCollider(attackId, body, trajectory, trajectoryStart, trajectoryEnd, speed);
        }

        public void DeactivateColliders(SpellPartCastId attackId, float trajectoryEnd)
        {
            foreach (var hitColliderHandler in CurrentColliders)
                hitColliderHandler.DeactivateCollider(attackId, trajectoryEnd);
        }

        private void OnAttackColliderHit(SpellPartCastId attackId, in HitInfo hitInfo)
        {
            AttackHitDetected?.Invoke(attackId, hitInfo);
        }
    }
}
