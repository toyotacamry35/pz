using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Src.ResourceSystem;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using ResourceSystem.Aspects.Misc;
using Src.Tools;
using XftWeapon;

namespace Assets.Src.Effects.AnimatorEffects
{
    public class AnimationEventRelayTrail : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        [SerializeField] float _timeStopSmoothly;
        [SerializeField] private Animator _animator;

        private readonly Dictionary<GameObjectMarkerDef, TrailsBundle> _trails = new Dictionary<GameObjectMarkerDef, TrailsBundle>();
        private readonly Dictionary<VisualSlot, TrailsBundle> _slots = new Dictionary<VisualSlot, TrailsBundle>();
        private readonly HashSet<int> _activeAnimations = new HashSet<int>();

        private void Start()
        {
            _animator = _animator ? _animator : GetComponent<Animator>();
            if (!_animator)
                throw new NullReferenceException($"No animator in {transform.FullName()}");
            
            foreach (var slot in gameObject.GetComponentsInChildren<VisualSlot>())
            {
                var xform = slot.ActiveSlotInUseTransform; 
                if (!xform)
                    continue;
                var marker = xform.TryGetMarker();
                if (marker == null)
                    continue;
                if (!_trails.TryGetValue(marker, out var bundle))
                    _trails.Add(marker, bundle = new TrailsBundle());
                bundle.Slots.Add(slot);
                _slots.Add(slot, bundle); 
                OnAttachedObjChanged(slot, slot.AttachedObj);
                slot.AttachedObjChanged += OnAttachedObjChanged;
            }

            foreach (var trail in gameObject.GetComponentsInChildren<XWeaponTrail>(true))
            {
                var markerObj = trail.GetComponentInParent<GameObjectMarker>();
                if (!markerObj)
                    continue;
                var marker = markerObj.MarkerDef;
                if (marker == null)
                    continue;
                if (!_trails.TryGetValue(marker, out var bundle))
                    _trails.Add(marker, bundle = new TrailsBundle());
                bundle.DefaultTrails.Trails.Add(trail);
            }

            if (Logger.IsDebugEnabled)
            {
                foreach (var kv in _trails)
                    Logger.IfDebug()?.Message($"{kv.Key} -> Trails:[{string.Join(",", kv.Value.DefaultTrails.Trails.Concat(kv.Value.WeaponTrails.Trails))}] Slots:[{string.Join(", ", kv.Value.Slots)}]").Write();
            }
        }

        private void OnDestroy()
        {
            foreach (var bundle in _trails.Values)
            {
                foreach (var slot in bundle.Slots)
                    if (slot)
                        slot.AttachedObjChanged -= OnAttachedObjChanged;
                BreakTrails(bundle.DefaultTrails);
                BreakTrails(bundle.WeaponTrails);
            }
        }

        [UsedImplicitly]
        private void StartXTrail(AnimationEvent @event)
        {
            var bundle = FindTrailsBundle(@event);
            if (bundle != null)
                StartTrails(bundle.CurrentTrails, @event.animatorStateInfo.fullPathHash);
        }

        [UsedImplicitly]
        private void StopXTrail(AnimationEvent @event)
        {
            var bundle = FindTrailsBundle(@event);
            if (bundle != null)
                StopTrails(bundle.CurrentTrails, @event.animatorStateInfo.fullPathHash);
        }

        public void Update()
        {
            _activeAnimations.Clear();
            for (int layer = 0; layer < _animator.layerCount; ++layer)
                if (!_animator.IsInTransition(layer))
                    _activeAnimations.Add(_animator.GetCurrentAnimatorStateInfo(layer).fullPathHash);
            
            foreach (var bundle in _trails.Values)
            {
                CheckAnimations(bundle.WeaponTrails, _activeAnimations);
                CheckAnimations(bundle.DefaultTrails, _activeAnimations);
            }
        }
        
        private void OnAttachedObjChanged(VisualSlot slot, GameObject attachedObject)
        {
            if (_slots.TryGetValue(slot, out var bundle))
            {
                BreakTrails(bundle.WeaponTrails);
                BreakTrails(bundle.DefaultTrails);
                bundle.CurrentTrails = bundle.DefaultTrails;
                bundle.WeaponTrails.Trails.Clear();
                foreach (var s in bundle.Slots)
                    if (s.IsInUse && s.AttachedObj)
                    {
                        bundle.WeaponTrails.Trails.AddRange(s.AttachedObj.GetComponentsInChildren<XWeaponTrail>(true));
                        bundle.CurrentTrails = bundle.WeaponTrails;
                    }
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{slot} -> Trails:[{string.Join(",", bundle.DefaultTrails.Trails.Concat(bundle.WeaponTrails.Trails))}]").Write();
            }
        }

        private void CheckAnimations(TrailsSet trailsSet, HashSet<int> activeAnimations)
        {
            if (trailsSet.ActiveAnimations.Count != 0)
            {
                for (var i = trailsSet.ActiveAnimations.Count - 1; i >=0; --i)
                {
                    var stateHash = trailsSet.ActiveAnimations[i];
                    if (!activeAnimations.Contains(stateHash))
                        StopTrails(trailsSet, stateHash);
                }
            }
        }
        
        private TrailsBundle FindTrailsBundle(AnimationEvent @event)
        {
            var jdb = @event.objectReferenceParameter as JdbMetadata;
            if (jdb == null)
                throw new Exception($"Event {nameof(StartXTrail)} in {@event.animationState.clip.name} has no reference to GameObjectMarkerDef");
            var marker = jdb.Get<GameObjectMarkerDef>();
            if (marker == null)
                throw new Exception($"Event {nameof(StartXTrail)} in {@event.animationState.clip.name} has bad reference to GameObjectMarkerDef");
            _trails.TryGetValue(marker, out var set);
            return set;
        }

        private void StartTrails(TrailsSet trails, int animationStateHash)
        {
            if (trails != null)
            {
                if (!trails.ActiveAnimations.Contains(animationStateHash))
                {
                    trails.ActiveAnimations.Add(animationStateHash);
                    if (trails.ActiveAnimations.Count == 1)
                        foreach (var trail in trails.Trails)
                            if (trail)
                                trail.Activate();
                }
            }
        }
        
        private void StopTrails(TrailsSet trails, int animationStateHash)
        {
            if (trails != null)
            {
                if (trails.ActiveAnimations.Remove(animationStateHash) && trails.ActiveAnimations.Count == 0) 
                    foreach (var trail in trails.Trails)
                        if (trail)
                            trail.StopSmoothly(_timeStopSmoothly);
            }
        }
        
        private void BreakTrails(TrailsSet trails)
        {
            if (trails != null)
            {
                foreach (var trail in trails.Trails)
                    if (trail)
                        trail.Deactivate();
                trails.ActiveAnimations.Clear();
            }
        }

        private class TrailsSet
        {
            public readonly List<XWeaponTrail> Trails = new List<XWeaponTrail>();
            public readonly List<int> ActiveAnimations = new List<int>();
        }

        private class TrailsBundle
        {
            public readonly TrailsSet WeaponTrails = new TrailsSet();
            public readonly TrailsSet DefaultTrails = new TrailsSet();
            public readonly List<VisualSlot> Slots = new List<VisualSlot>();
            public TrailsSet CurrentTrails;

            public TrailsBundle()
            {
                CurrentTrails = DefaultTrails;
            }
        }
    }
}
