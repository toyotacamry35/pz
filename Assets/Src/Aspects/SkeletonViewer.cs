using Assets.Src.App.FXs;
using Assets.Src.Effects.FX;
using Assets.Src.SpawnSystem;
using Assets.Tools;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Src.Aspects
{
    public class SkeletonViewer : EntityGameObjectComponent
    {
        public float StartInstantiate;

        [UsedImplicitly]
        [SerializeField]
        AnimationCurve _curve;

        [UsedImplicitly]
        [SerializeField]
        string _propertyName = "_DeathCutoff";

        [UsedImplicitly]
        [SerializeField]
        GameObject _skeleton;

        [UsedImplicitly]
        [SerializeField]
        float _timeShowSkeleton;

        [UsedImplicitly]
        [SerializeField]
        float _timeDestroySkeleton;

        [UsedImplicitly]
        [SerializeField]
        FX[] _fx;

        bool _wasInit = false;
        float _maxTime = 0;
        float _currentTime = 0;
        Renderer[] _renderer;

        private GameObject _characterViewer; //полноценный труп персонажа, который после проигрывания эффекта нужно удалить
        private Dictionary<GameObject, FX> _fxObjects = new Dictionary<GameObject, FX>();

        public void Init(GameObject characterViewer = null)
        {
            _wasInit = true;

            if (_curve != null && !string.IsNullOrEmpty(_propertyName))
            {
                _characterViewer = characterViewer;
                _renderer = _characterViewer?.GetComponentsInChildren<Renderer>();

                foreach (var key in _curve.keys)
                {
                    if (key.time > _maxTime)
                    {
                        _maxTime = key.time;
                    }
                }
            }
        }

        private void Awake()
        {
            foreach (var fx in _fx.Where(v => !v.ShowOnlyForOwner))
                StartFX(fx);
        }

        protected override void GotClient()
        {
            base.GotClient();

            if (_fx.Where(v => v.ShowOnlyForOwner).Any())
            {
                var entityRef = GetOuterRef<IHasOwner>();
                if (entityRef.IsValid)
                {
                    var repository = ClusterCommands.ClientRepository;
                    AsyncUtils.RunAsyncTask(async () =>
                    {
                        using (var wrapper = await repository.Get(entityRef.TypeId, entityRef.Guid))
                        {
                            var entity = wrapper.Get<IHasOwnerClientBroadcast>(entityRef.TypeId, entityRef.Guid, ReplicationLevel.ClientBroadcast);
                            if (entity != null)
                            {
                                var owner = entity.OwnerInformation.Owner;
                                if (owner.IsValid && GameState.Instance.CharacterRuntimeData.CharacterId == owner.Guid)
                                {
                                    // TODOA: race condition c OnDestroy

                                    UnityQueueHelper.RunInUnityThread(() =>
                                        {
                                            //проигрываем fx
                                            foreach (var fx in _fx.Where(v => v.ShowOnlyForOwner))
                                                StartFX(fx);
                                        }
                                    );
                                }
                            }
                        }
                    });
                }
            }
        }

        private void StartFX(FX fx)
        {
            var fxInfo = new FXInfo(fx.parent, fx.parent != null ? fx.parent.transform.position : transform.position,
                    fx.parent != null ? fx.parent.transform.rotation : transform.rotation, null, fx.pair);
            var fxObject = FxPlayer.StartPlay(fx.prefab, fxInfo, true);

            if (fxObject != null)
                _fxObjects.Add(fxObject, fx);
        }

        public void StopAllFX(bool onlyForOnOpen = true)
        {
            var fxs = onlyForOnOpen ? _fxObjects.Where(v => v.Value.HideWhenOpened) : _fxObjects;
            foreach (var fxObject in fxs)
            {
                DestroyImmediate(fxObject.Key);
            }
            _fxObjects.Clear();
        }

        private void OnDestroy()
        {
            StopAllFX(false);
        }

        private void Update()
        {
            if (_wasInit)
            {
                if (_currentTime > _timeShowSkeleton && _timeShowSkeleton > 0)
                {
                    _skeleton?.SetActive(true);
                    _timeShowSkeleton = -1;
                }

                if (_currentTime > _timeDestroySkeleton && _timeDestroySkeleton > 0)
                {
                    _skeleton?.SetActive(false);
                    _timeDestroySkeleton = -1;
                }

                if (_curve != null && !string.IsNullOrEmpty(_propertyName))
                {
                    if (_currentTime < _maxTime)
                    {
                        if (_renderer != null)
                        {
                            foreach (var render in _renderer)
                            {
                                if (render != null && render.materials != null)
                                {
                                    foreach (var material in render.materials)
                                    {
                                        material.SetFloat(_propertyName, _curve.Evaluate(_currentTime));
                                    }
                                }
                            }
                        }

                        //Shader.SetGlobalFloat(_propertyName, _curve.Evaluate(_currentTime));
                    }
                    else if (_characterViewer != null)
                    {
                        Destroy(_characterViewer);
                    }
                }

                _currentTime += Time.deltaTime;
            }
        }

        [System.Serializable]
        public class FX
        {
            public GameObject prefab;
            public GameObject parent;
            public bool ShowOnlyForOwner = false;
            public bool HideWhenOpened = true;
            public FXParamsOnObj.FXParamsValue[] pair;
        }
    }
}