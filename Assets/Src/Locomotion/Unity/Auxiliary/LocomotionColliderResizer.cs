using System;
using System.Collections.Generic;
using System.Linq;
using ColonyShared.SharedCode.Aspects.Locomotion;
using UnityEngine;
using UnityEngine.Assertions;

namespace Src.Locomotion.Unity
{
    public class LocomotionColliderResizer : ILocomotionPipelineCommitNode
    {
        private readonly ISettings _settings;
        private readonly Collider _collider;
        private readonly (Condition Condition,Preset Preset)[] _presets;
        private readonly IColliderAdapter _colliderAdapter;
        private int _presetId;
        private Preset _prevPreset;
        private float _blendFactor;
        private readonly bool _keepLocalPosition;
        
        public LocomotionColliderResizer(ISettings settings, Collider collider, bool keepLocalPosition = false)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _collider = collider ?? throw new ArgumentNullException(nameof(collider));
            _keepLocalPosition = keepLocalPosition;
            _colliderAdapter = CreateColliderAddapter(_collider);
            Assert.AreEqual(_colliderAdapter.direction, 1);
            _presets = _settings.Presets.Where(x => x.Preset != null).Prepend((null, GetCurrent())).ToArray();
        }
        
        bool ILocomotionPipelineCommitNode.IsReady => true;

        //#Important!: @param `inVars`: 'ref' === 'in' here - i.e. "const-ref" (only to avoid struct copying)
        // So DO NOT change `inVars`!
        void ILocomotionPipelineCommitNode.Commit(ref LocomotionVariables inVars, float dt)
        {
            if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## Loco Commit: k)LocoColliderResizer");

            if (_colliderAdapter == null)
                return;
            
            var newPresetId = 0;
            for (int i = 1; i <_presets.Length && newPresetId == 0; ++i)
            {
                var condition = _presets[i].Condition;
                if ((inVars.Flags & condition.WithFlags) == condition.WithFlags && (inVars.Flags & condition.WithoutFlags) == 0)
                    newPresetId = i;
            }

            if (newPresetId != _presetId && newPresetId < _presets.Length && _presets[newPresetId].Preset.Enabled)
            {
                _prevPreset = GetCurrent();
                _presetId = newPresetId;
                _blendFactor = 0;
            }

            if (_prevPreset != null && _blendFactor >= 0 && _blendFactor < 1)
            {
                var preset = _presets[_presetId].Preset;
                var blendTime = preset.BlendTime > 0 ? preset.BlendTime : _settings.BlendTime;
                _blendFactor = blendTime > 0.001f ? Mathf.Clamp01(_blendFactor + Time.deltaTime / blendTime) : 1;
                ApplyPreset(_prevPreset, preset, _blendFactor);
            }

            LocomotionProfiler.EndSample();
        }

        private static IColliderAdapter CreateColliderAddapter(Collider collider)
        {
            switch (collider)
            {
                case CapsuleCollider c: return new CapsuleColliderAdapter(c);
                case CharacterController c: return new ControllerColliderAdapter(c);
                case null: throw new NullReferenceException("Collider");
                default: throw new NotSupportedException($"Collider is {collider?.GetType()}");
            }
        }

        private void ApplyPreset(Preset preset1, Preset preset2, float t)
        {
            if (preset1 == null) throw new ArgumentNullException(nameof(preset1));
            if (preset2 == null) throw new ArgumentNullException(nameof(preset2));
            
            var newCenter = Vector3.Lerp(preset1.Center, preset2.Center, t);
            var newHeight = Mathf.Lerp(preset1.Height, preset2.Height, t);

            if (!_keepLocalPosition)
            {
                var sweepBgn =  _collider.transform.TransformPoint(_colliderAdapter.center + Vector3.down * (_colliderAdapter.height * 0.5f - _colliderAdapter.radius));
                var sweepEnd = _collider.transform.TransformPoint(newCenter + Vector3.down * (newHeight * 0.5f - _colliderAdapter.radius));
                var sweepDir = sweepEnd - sweepBgn;
                var sweepDistance = sweepDir.magnitude;
                var ray = new Ray(sweepBgn, sweepDir);
                if (Physics.SphereCast(ray, _colliderAdapter.radius, out var hit, sweepDistance))
                {
                    var point = ray.GetPoint(hit.distance) + sweepDir * (_colliderAdapter.radius - 0.01f);
                    var shift = point - sweepEnd;
                    _collider.transform.position += shift;
                }
            }

            _colliderAdapter.center = newCenter;
            _colliderAdapter.height = newHeight;
        }

        private Preset GetCurrent()
        {
            return new Preset { Center = _colliderAdapter.center, Height = _colliderAdapter.height, Enabled = true};
        }


        private interface IColliderAdapter
        {
            Vector3 center { get; set; }
            float radius { get; set; } 
            float height { get; set; }
            int direction { get; }
        }
        
        private class CapsuleColliderAdapter : IColliderAdapter
        {
            private readonly CapsuleCollider _collider;
            public CapsuleColliderAdapter(CapsuleCollider collider)
            {
                _collider = collider;
            }
            public Vector3 center { get => _collider.center; set => _collider.center = value; }
            public float radius { get => _collider.radius; set => _collider.radius = value; }
            public float height { get => _collider.height; set => _collider.height = value; }
            public int direction => _collider.direction;
        }

        private class ControllerColliderAdapter : IColliderAdapter
        {
            private readonly CharacterController _collider;
            public ControllerColliderAdapter(CharacterController collider)
            {
                _collider = collider;
            }
            public Vector3 center { get => _collider.center; set => _collider.center = value; }
            public float radius { get => _collider.radius; set => _collider.radius = value; }
            public float height { get => _collider.height; set => _collider.height = value; }
            public int direction => 1;
        }

        public interface ISettings
        {
            IEnumerable<(Condition Condition, Preset Preset)> Presets { get; }
            float BlendTime { get; }
        }

        public class Condition
        {
            public LocomotionFlags WithFlags;
            public LocomotionFlags WithoutFlags;
        }
        
        public class Preset
        {
            public bool Enabled;
            public Vector3 Center;
            public float Height;
            public float BlendTime;
        }
    }
}