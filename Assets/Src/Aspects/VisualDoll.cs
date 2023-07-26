using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Src.Character;
using Assets.Src.Lib.Wrappers;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.Shared;
using JetBrains.Annotations;
using NLog;
using NLog.Fluent;
using UnityEngine;
using UnityEngine.Serialization;
using SharedCode.Aspects.Item.Templates;
using Assets.Src.ResourceSystem;
using Assets.Src.SpawnSystem;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using MongoDB.Driver;
using Object = System.Object;

namespace Assets.Src.Aspects
{
    [Serializable]
    public class BodyPartResourceRef : JdbRef<BodyPartResource>
    {
    }

    [Serializable]
    public struct BodyPartWithGo
    {
        public BodyPartResourceRef BodyPartResource;
        public GameObject[] BodyParts;
        [Obsolete] public GameObject BodyPartGo; // оставлено для совместимости со старыми префабами
    }

    public class VisualDoll : ColonyBehaviour
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger("Visual.Doll");

        public Dictionary<ResourceRef<BodyPartResource>, BodyPartHideState> BodyPartsWithHideState { get; private set; }

        public List<BodyPartWithGo> BodyParts = new List<BodyPartWithGo>();
        [SerializeField] private GameObject _allBonesOwner;
        
        private readonly Dictionary<string,Transform> _allModelBones = new Dictionary<string, Transform>();
        private Dictionary<SlotDef, VisualSlot> _allSlots;

        // --- Unity Methods: --------------------------------------------------

        private void Awake()
        {
            if (!_allBonesOwner)
                throw new NullReferenceException($"nameof(_allBonesOwner) is null in {transform.FullName()}");
            
            foreach (var child in _allBonesOwner.GetComponentsInChildren<Transform>())
            {
                var name = child.name;
                if (!_allModelBones.ContainsKey(name))
                    _allModelBones.Add(name, child);
                else
                    if (Logger.IsWarnEnabled) Logger.IfWarn()?.Message("Two bones with same name is detected | {@}", new {Object = transform.FullName(), FirstBone = _allModelBones[name].FullName(transform), SecondBone = child.FullName(transform)}).Write();
            }

            // Check source data for duplicates:
#if UNITY_EDITOR || DEVELOPMENT_BUILD            
            var duplicates = BodyParts.GroupBy(x => x.BodyPartResource.Target)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key).ToList();
            if (duplicates.Count > 0)
                Logger.IfError()?.Message($"Duplicates keys in {nameof(BodyParts)}: {duplicates}").Write();
#endif
            
            // Fill `BodyPartsWithHideState`:
            BodyPartsWithHideState = new Dictionary<ResourceRef<BodyPartResource>, BodyPartHideState>();
            foreach (var bp in BodyParts)
            {
                var resource = bp.BodyPartResource.Target;
                BodyPartsWithHideState[resource] = new BodyPartHideState(bp.BodyParts.Append(bp.BodyPartGo));
            }

            _allSlots = GetComponentsInChildren<VisualSlot>()
                .Where(x => x.SlotDefRef.Target != null)
                .GroupBy(x => x.SlotDefRef.Target)
                .Select(g =>
                {
                     if (g.Skip(1).Any()) Logger.IfWarn()?.Message($"Duplicate slot {g.Key} in {name}: {string.Join(", ", g.Select(x => x.transform.FullName(transform)))}").UnityObj(this).Entity(gameObject.GetEntityGameObject().EntityId).Write();
                     return g;
                })
                .ToDictionary(g => g.Key, g => g.First());
        }

        // --- API: -----------------------------------------------------------
        [Pure]
        public Transform GetBoneByName(string boneName)
        {
            return _allModelBones != null && _allModelBones.TryGetValue(boneName, out var rv) ? rv : null;
        }

        public bool TryGetVisualSlot(SlotDef slotDef, out VisualSlot visualSlot) => _allSlots.TryGetValue(slotDef, out visualSlot);

        // --- Internal Types: -------------------------------------------------

        public class BodyPartHideState
        {
            private readonly GameObject[] _gameObjects;
            private readonly HashSet<GameObject> _hiders = new HashSet<GameObject>();

            public BodyPartHideState(IEnumerable<GameObject> gameObjects)
            {
                _gameObjects = gameObjects.ToArray();
            }
            
            public void Add(GameObject go)
            {
                _hiders.Add(go);
                foreach (var o in _gameObjects)
                    if (o)
                        o.SetActive(false);
            }

            public void Remove(GameObject go)
            {
                _hiders.Remove(go);
                if (_hiders.Count == 0)
                {
                    foreach (var o in _gameObjects)
                        if (o)
                            o.SetActive(true);
                }
            }
        }

        public Transform GetNearestBone(Vector3 worldPoint)
        {
            Transform nearestBone = null;
            float nearestDistanceSq = float.MaxValue; 
            foreach (var bone in _allModelBones.Values)
            {
                var distanceSq = Vector3.Distance(bone.position, worldPoint);
                if (distanceSq < nearestDistanceSq)
                {
                    nearestBone = bone;
                    nearestDistanceSq = distanceSq;
                }
            }
            return nearestBone;
        }

        public void HideItemInSlot(SlotDef slot, object causer)
        {
            
        }
    }
}
