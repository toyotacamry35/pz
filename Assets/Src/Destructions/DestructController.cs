//#define aK_DbgDrawExplosionPoint

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.ColonyShared.SharedCode.Shared;
using Assets.Src.Shared;
using UnityEngine.Serialization;
using NLog.Fluent;
using Assets.Src.Aspects.Impl;
using Assets.Src.Lib.Extensions;
using Assets.Src.SpawnSystem;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Assets.Tools;
using NLog;
using SharedCode.Aspects.Item.Templates;
using SharedCode.EntitySystem;
using Assets.Src.Lib.ProfileTools;
using Core.Environment.Logging.Extension;
using Random = UnityEngine.Random;
using SharedPosRot = SharedCode.Entities.GameObjectEntities.PositionRotation;
using UnityUpdate;
using SharedCode.Serializers;

namespace Assets.Src.Destructions
{
    /// <summary>
    /// Summary description of this logic see here: https://centre.atlassian.net/wiki/pages/viewpage.action?pageId=37787477
    /// </summary>
    // Most of 3-slash comments are about decals (which are tmp-rary absent)
    public class DestructController : EntityGameObjectComponent//ColonyBehaviour
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("DestructController");

        //[NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        // -- Public --------------------------------------------------------------------------------------------------

        // "part" - means "доля" [0 .. 1]
        //[Range(0f, 1f)]
        private float _beginDestructWhenHpLowerThanPart = 0.99f;

        // Threshold mass of chunk - we'll drop all with less val. on 1st hit to prevent cactus needles dangling in the air
        public float LightMassThreshold = 0.1f;

        public event Action<GameObject> InstantiateFracturedEvent;
        public event Action<FracturedChunk> DetachFracturedChunkEvent;

        // -- Private --------------------------------------------------------------------------------------------------

        [SerializeField] private EntityGameObject _ego;
        [FormerlySerializedAs("_mesh")]
        [SerializeField] private Collider _collider;

        // Consisting of chunks model
        [FormerlySerializedAs("FracturedPrefab")]
        [SerializeField]
        private GameObject _fracturedPrefab;

        [SerializeField]
        private GameObject _scratchedObject;

        [FormerlySerializedAs("HitFx")]
        [SerializeField]
        private GameObject _hitFx;
        [SerializeField]
        private GameObject _zeroDamageHitFx;

        [SerializeField] float _soundDelay;
        [SerializeField] AkEvent _soundPrefab;
        AkEvent _sound;
        [SerializeField] AkEvent _soundFinalPrefab;

        /// <summary>
        /// по мере добычи ресурсов способ их добычи может меняться (из средней атаки в более низкую)
        /// </summary>
        [SerializeField] ChangeInteractionType[] _changeInteractionType;

        // Are loaded from jdb:
        private float _explosionForceFactor;// = 4000f; //2000f;
        private float _explosionR;// = 0.1f; //0.2f;
        private float _torqueMagnitude;// = 5000f;
        private /*const*/ float HitSphereCastR;// = 0.4f; // 0.5f; //1f;    //TODO(later): calc. R from target bounds
        private /*const*/ float HitSphereCastDist;// = 3f; //2f;   //TODO(later): calc. dist. from to-target dist. & target bounds
        private /*const*/ float SmallGap;// = 0.01f;

        [FormerlySerializedAs("CurrState")]
        private DestructionState _сurrState = DestructionState.N1_FullHealth;

        // Cached center of mass point
        // Note: scale should be const
        private Vector3 _centerPoint;

        // Chunks of fractured prefab. Are sorted by Y-coordinate from min (in [0] position to max in [chunks.Count-1] position:
        private List<FracturedChunk> _chunks = new List<FracturedChunk>();

        // ReSharper disable once RedundantDefaultMemberInitializer
        private bool _hasRootSupportChunk = false;
        private GameObject _fracturedObject;

        // Add every instantiated hitFx here. Used to clear them when needed:
        private readonly List<GameObject> _fxList = new List<GameObject>();

        //TODO(later): рассмотреть вар.: изнач-ное кол-во жизней объекта не полное
        // Here an assumption is made: that Hps are divided evenly(равномерно) between bars
        private float _hpLeftPart = 1f;

        private Vector3 _modelGeometricCenterPoint = Vector3.zero;
        private Vector3 ModelGeometricCenterPoint => /*_modelGeometricCenterPoint*/transform.position - new Vector3(0, 0.1f, 0);

        private Interactive _interactive;
        private bool _isInvalid;


    // --- Overrides: ---------------------------------------------------------
    #region Overrides


        //protected override void AnyStart()
        private void Start()
        {
            if (!_collider)
                _collider = GetComponent<Collider>();

            //_isInvalid |= _fracturedPrefab.AssertIfNull(nameof(_fracturedPrefab), gameObject);
            _isInvalid |= _collider.AssertIfNull(nameof(_collider), gameObject);
            if (_isInvalid)
                return;

            if (!_zeroDamageHitFx)
                _zeroDamageHitFx = _hitFx;

            InitConstsFromJdb();

            Debug.Assert(_explosionForceFactor > 0, gameObject);
            Debug.Assert(_explosionR > 0, gameObject);
            if (_isInvalid)
                return;

            if (_scratchedObject)
                _scratchedObject.SetActive(false);
            // else (f.e. for fungi) obj. 've not scratched 3d-model

            _centerPoint = _collider.bounds.center;
        }

        protected override void GotClient()
        {
            if (_isInvalid)
                return;

            if (_ego == null)
                _ego = GetComponent<EntityGameObject>();
            if (_ego == null)
            {
                Logger.IfWarn()?.Message($"{gameObject.name} does not has EntityGameObject").Write();
                return;
            }

            var gameObjName = gameObject.name;
            var repo = NodeAccessor.Repository;
            AsyncUtils.RunAsyncTask(async () =>
            {
                var typeId = _ego.TypeId;
                var entId = _ego.EntityId;
                using (var wrapper = await repo.Get(typeId, entId))
                {
                    var entity = wrapper?.Get<IHasHealthClientBroadcast>(typeId, entId, ReplicationLevel.ClientBroadcast);
                    if (entity != null)
                    {
                        var entityMortal = wrapper?.Get<IHasMortalClientBroadcast>(typeId, entId, ReplicationLevel.ClientBroadcast);
                        if (entityMortal != null)
                            entityMortal.Mortal.DieEvent += OnDie;
                        entity.Health.DamageReceivedEvent += OnDamageReceivedClient;
                    }
                    else
                    {
                        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{gameObjName}. entity = wrapper?.Get<IHealthOwnerClientBroadcast>(_ego.TypeId, _ego.EntityId, ReplicationLevel.ClientBroadcast) == null ({typeId}, {entId}).").Write();
                    }
                }
            });

            #region Dbg
            _dbgSphereSignPrefab = Profile.Load<GameObject>("Debug/DebugSphere");
            #endregion
        }

        private void OnDestroy()
        {
            if (_isInvalid)
                return;

            // Unsubscribe:
            var repo = NodeAccessor.Repository;
            if (repo == null)
                return;

            AsyncUtils.RunAsyncTask(async () =>
            {
                if (_ego != null)
                {
                    var cachedTypeId = _ego.TypeId;
                    var cachedEntityId = _ego.EntityId;
                    using (var wrapper = await repo.Get(cachedTypeId, cachedEntityId))
                    {
                        if (wrapper != null && wrapper.TryGet<IHasHealthClientBroadcast>(cachedTypeId, cachedEntityId, out var entity))
                        {
                            var entityMortal = wrapper?.Get<IHasMortalClientBroadcast>(cachedTypeId, cachedEntityId, ReplicationLevel.ClientBroadcast);
                            if (entityMortal != null)
                                entityMortal.Mortal.DieEvent -= OnDie;
                            entity.Health.DamageReceivedEvent -= OnDamageReceivedClient;
                        }
                    }
                }
            });
        }

    #endregion Overrides

    // --- API: ---------------------------------------------------------
    #region API

        public FracturedChunk GetHigherChunk()
        {
            if (_isInvalid)
                return null;

            for (int i = _chunks.Count - 1; i >= 0; --i)
            {
                var c = _chunks[i];
                if (c && c.IsSupportChunk)
                    return c;
            }

            return null;
        }


        public static void OnDetachFromObjectDelegate(FracturedChunk fc, GameObject contextGo)
        {
            var dc = contextGo.GetComponent<DestructController>();
            if (dc.AssertIfNull(nameof(dc), contextGo) || dc._isInvalid)
                return;

            if (fc.gameObject)
            {
                fc.gameObject.layer = PhysicsLayers.DetachedDestructChunks;

                //#Impulse bug fixing attempt
                // impact on callback
                ////var p = GetImpactPoint(fc);
                //var dY = new Vector3(0f, 0.01f, 0f);
                //var p = fc.transform.position - dY;
                //fc.Impact(p, /*_explosionForce*/200f, 0.1f, false/*true*/);
                dc.TryAddExplosionForce(fc, dc._explosionForceFactor, 0f);
                dc.DetachFracturedChunkEvent?.Invoke(fc);
            }

            else
                if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("!fc.gameObject").Write();;
        }

    #endregion API

    // -- Privates: ---------------------------------------------------------
    #region Privates

        private void InitConstsFromJdb()
        {
            var staticConstsDef = GameState.Instance?.StaticDefsHolder?.DestrucControllersStaticConstsJdb?.Get<DestructControllerStaticConstsDef>();
            if (staticConstsDef != null)
            {
                _explosionForceFactor = staticConstsDef.ExplosionForceFactor;
                _explosionR = staticConstsDef.ExplosionR;
                _torqueMagnitude = staticConstsDef.TorqueMagnitude;
                HitSphereCastR = staticConstsDef.HitSphereCastR;
                HitSphereCastDist = staticConstsDef.HitSphereCastDist;
                SmallGap = staticConstsDef.SmallGap;
            }
        }

        private Task OnDie(Guid entityId, int entityTypeId, SharedPosRot corpsePlace)
        {
            //TODO: throw all left chunks
            foreach (var ch in _chunks)
            {
                if (ch && ch.IsSupportChunk)
                    MakeAllAttachedFree(ch);
            }
            return Task.CompletedTask;
        }

        private Task OnDamageReceivedClient(float prevHealthVal, float newHealthVal, float maxHealth, Damage damage)
        {
            UnityQueueHelper.RunInUnityThread(() =>
            {
                OnDamageReceivedClientUnity(prevHealthVal, newHealthVal, maxHealth, damage);
            });
            return Task.CompletedTask;
        }

        private void OnDamageReceivedClientUnity(float prevHealthVal, float newHealthVal, float maxHealth, Damage damage)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"TC-3466 ##_5B-Z `{nameof(DestructController)}.{nameof(OnDamageReceivedClient)}`({nameof(prevHealthVal)}: {prevHealthVal};  {nameof(newHealthVal)}: {newHealthVal}; maxHealth: {maxHealth}; {nameof(damage.HasAggressionPoint)}: {damage.HasAggressionPoint}; {nameof(damage.AggressionPoint)}: {damage.AggressionPoint}).  {nameof(_сurrState)}=={_сurrState}").Write();

            RaycastHit hitInfo;
            if (!GetCastHit(damage.HasAggressionPoint, damage.AggressionPoint.ToUnityVector3(), out hitInfo))
            {
                //#Dbg:
                #region Dbg
                Vector3 fromPoint = damage.HasAggressionPoint
                    ? damage.AggressionPoint.ToUnityVector3() //#dont_del: aggressionPoint + (aggressionPoint - _centerPoint).normalized * (HitSphereCastR + SmallGap)
                    : new Vector3(
                        _centerPoint.x,
                        _collider.bounds.max.y + HitSphereCastR + SmallGap,
                        _centerPoint.z);
                #endregion //Dbg

                Logger./*Warn*/IfDebug()?.Message($"Can't find hit point(`{nameof(OnDamageReceivedClient)}`). aggressionPoint: {damage.AggressionPoint}, direction-to: {_centerPoint}, pivot: {transform.position} l: {HitSphereCastDist}. From point: {fromPoint} (isAgrPointValid {damage.HasAggressionPoint})").Write();
                return;
            }

            if (Mathf.Approximately(newHealthVal, prevHealthVal) || (newHealthVal > prevHealthVal)) //for case prev
            {
                if (_zeroDamageHitFx != null)
                    PlayHitFx(hitInfo.point, hitInfo.normal, _zeroDamageHitFx, false);

                return;
            }

            Logger.IfDebug()?.Message/*Warn*/($"#Dbg: -+-+  -+- + -+-  +-+- Next Bar Done: prevVal: {prevHealthVal}, newVal: {newHealthVal}. State=={_сurrState}").Write();

            bool b1stHit = false;
            bool b1stHitAferDestruct2 = false;
            DestructionState prevState = _сurrState;

            if (_сurrState == DestructionState.N1_FullHealth)
                b1stHit = true;

            ManageDestructionModelStep1(newHealthVal, maxHealth);

            if (prevState == DestructionState.N2_Scratched && _сurrState == DestructionState.N32_ChunkedDone) // == DestructionState.N3_Chunked) 
                b1stHitAferDestruct2 = true;

            if (_сurrState == DestructionState.N2_Scratched)
            {
                float playHitDelay = 0f; // play hit delay is not used now

                if (b1stHit)
                    // start logic after 1 frame to be sure, all transition processes are done:
                    this.StartInstrumentedCoroutine(PlayHitDecalOn1stHit(hitInfo.point, hitInfo.normal, playHitDelay));
                else
                    this.StartInstrumentedCoroutine(PlayHitDecal(hitInfo.point, hitInfo.normal, playHitDelay));
            }
            else if (_сurrState == DestructionState.N32_ChunkedDone)// == DestructionState.N3_Chunked)
            {
                if (b1stHitAferDestruct2)
                    // start logic after 1 frame to let the fractured model be constructed:
                    this.StartInstrumentedCoroutine(PlayHitImpactOn1stChunkedHit(hitInfo, _explosionForceFactor, _explosionR));
                else
                    PlayHitImpact(hitInfo, _explosionForceFactor, _explosionR);
            }
            else if (_сurrState == DestructionState.N31_ChunkedStarted)
            {
                // Do nothing
            }
            else
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Unexpected curState == {_сurrState}").Write();
            }
        }

        //@param `castToCenter` - if `false`, 'll be casted to pivot
        private bool GetCastHit(bool isAggressionPointValid, Vector3 aggressionPoint, out RaycastHit hitInfo, int attemptNumber = 0, bool castToCenter = true)
        {
            Vector3 fromPoint = isAggressionPointValid
                ? aggressionPoint //#dont_del: aggressionPoint + (aggressionPoint - _centerPoint).normalized * (HitSphereCastR + SmallGap)
                : new Vector3(
                    _centerPoint.x,
                    _collider.bounds.max.y + HitSphereCastR + SmallGap,
                    _centerPoint.z);

            //#Fast attempt to plug "Can't find hit point .."
            if (!isAggressionPointValid)
            {
                switch (attemptNumber)
                {
                    case 1:
                        fromPoint.y += 1; // +1m
                        break;
                    case 2:
                        fromPoint.y += 2; // +1m
                        break;
                }
            }

            Vector3 fromToVector = _centerPoint - fromPoint;

            int layer = _сurrState < DestructionState.N32_ChunkedDone// .N3_Chunked
                ? PhysicsLayers.DestructablesMask
                : PhysicsLayers.DestructChunksMask;

            bool result = FindHitPoint(fromPoint, HitSphereCastR, fromToVector, out hitInfo, HitSphereCastDist, layer);

            /// if (!result && castToCenter)
            ///     //return GetCastHit(isAggressionPointValid, aggressionPoint, out hitInfo, false);
            ///     return GetCastHit(false, aggressionPoint, out hitInfo, false);
            /// else
            ///     return result;


            if (!result)
            {
                switch (attemptNumber)
                {
                    case 0:
                        if (castToCenter)
                            return GetCastHit(isAggressionPointValid, aggressionPoint, out hitInfo, ++attemptNumber, false);
                        break;

                    case 1:
                        var modedAggressionPoint = aggressionPoint;
                        modedAggressionPoint.y += 1; //+1m up
                        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("#Dbg: modedAggressionPoint (1): " + modedAggressionPoint).Write();
                        return GetCastHit(false, modedAggressionPoint, out hitInfo, ++attemptNumber, true);

                    case 2:
                        var modedAggressionPoint2 = aggressionPoint;
                        modedAggressionPoint2.y += 1; //+1m up
                        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("#Dbg: modedAggressionPoint2 (2): " + modedAggressionPoint2).Write();
                        return GetCastHit(false, modedAggressionPoint2, out hitInfo, ++attemptNumber, true);

                    default:
                        //Nothing helped:
                        return result;
                }
            }

            return result;
        }

        private const int MaxNumOfRecasts = 10;

        // If hitResult is not our GO, recasts farther (num of recasts is limited)
        private readonly RaycastHit[] _hits = new RaycastHit[1];
        private bool FindHitPoint(Vector3 from, float r, Vector3 fromToVector, out RaycastHit hitInfo, float distance, int layers)
        {
            var direction = fromToVector.normalized;
            hitInfo = new RaycastHit();
            bool isLastCastSuccessful = false;
            int attemptNum = 0;
            while (attemptNum < MaxNumOfRecasts)
            {
                ++attemptNum;
                //isLastCastSuccessful = Physics.SphereCast(from, r, direction, out hitInfo, distance, layers);
                isLastCastSuccessful = 0 < Physics.SphereCastNonAlloc(from, r, direction, _hits, distance, layers, QueryTriggerInteraction.Ignore);
                
                if (!isLastCastSuccessful)
                    return false;

                hitInfo = _hits[0];

                var parent = GetParentGo(GetParentGo(hitInfo.collider));

                // if (gameObject == hitInfo.collider.gameObject
                //     || gameObject == parent)
                // {
                //     break;
                // }
                // else
                // {
                //     from += direction.normalized * hitInfo.distance;
                //     distance -= hitInfo.distance;
                // }
                if (gameObject == hitInfo.collider.gameObject)
                    break;
                else if (parent && gameObject == parent)
                    break;
                else
                {
                    if (!parent)
                        Logger.IfWarn()?.Message("Can't find parent").Write();

                    from += direction * hitInfo.distance;
                    distance -= hitInfo.distance;
                }

            }

            return isLastCastSuccessful;
        }

        private GameObject GetParentGo(GameObject go)
        {
            try
            {
                return go?.transform.parent?.gameObject;
            }
            catch (Exception e)
            {
                Logger.IfWarn()?.Message("got exception: " + e).Write();
                return null;
            }
        }

        private GameObject GetParentGo(Component comp)
        {
            return GetParentGo(comp.gameObject);
        }

        // (Only if has "scratched" object) Replaces Lods by "scratched" model (destroys lods, & enables scratched)
        // Also changes `_сurrState` val. to "N2_Scratched"
        private void DestructStep1()
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"DestructStep1(). _сurrState== {_сurrState} -->N2_Scratched").Write();

            // 0. Checks:

            if (_fracturedObject != null)
                if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("fracturedObject != null").Write();;

            if (_сurrState != DestructionState.N1_FullHealth)
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"currState != DestructionState.N1_FullHealth ({_сurrState})").Write();

            // 1. Do work:

            _сurrState = DestructionState.N2_Scratched;
            ChangeType();

            if (_scratchedObject)
            {
                OffLods();
                _scratchedObject.SetActive(true);
            }
            // else (f.e. for fungi) obj. in N2_Scratched state 've same 3d-model, as in N1_FullHealth, so it's no need
        }

        // (Only if has "fractured" object) Replaces Lods by "scratched" model (destroys lods, & enables scratched)
        // Replaces "scratched" object (or lods, if don't 've scratched) by "fractured" object. And init `_chunks`
        // Also changes `_сurrState` val. to "N3_Chunked"
        private void DestructStep2()
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"DestructStep2(). _сurrState== {_сurrState} -->N3_Chunked").Write();

            if (_сurrState != DestructionState.N2_Scratched)
                if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("currState != DestructionState.N2_Scratched").Write();;

            if (_fracturedPrefab.AssertIfNull(nameof(_fracturedPrefab)))
                return;

            _сurrState = DestructionState.N31_ChunkedStarted; //DestructionState.N3_Chunked;
            ChangeType();

            OffLods();
            _fracturedObject = Instantiate(_fracturedPrefab, transform.position, transform.rotation, transform);
            InstantiateFracturedEvent?.Invoke(_fracturedObject);

            // Destroy prev. body(ies):
            if (_scratchedObject)
                Destroy(_scratchedObject);

            InitAndOrganizeChunks();

            // Off all decals:

            //#Вопрос (Почему не работало ничего из этого?):
            // EasyDecal[] decals = GetComponentsInChildren<EasyDecal>();
            // DbgLog("\t " + decals.Length + " decals found.");
            // foreach (var d in decals)
            // {
            //     //d.enabled = false;
            //     //Destroy(/*d*/d.gameObject);
            //     //d.gameObject.SetActive(false);
            //     //d.FadeoutTime = 0f;
            //     //d.StartFade();
            // }

            ///int dbgCount = 0;
            foreach (var fx in _fxList)
            {
                if (!fx)
                    continue;
                // TODO: use new decal-tool
                /// EasyDecal[] decals = fx.GetComponentsInChildren<EasyDecal>();
                /// dbgCount += decals.Length;
                /// foreach (var decal in decals)
                /// {
                ///     decal.enabled = false;
                ///     decal.gameObject.SetActive(false);
                ///     Destroy(decal.gameObject);
                /// }
            }
        }

        private void OffLods()
        {
            var lodGroup = GetComponent<LODGroup>();
            if (!lodGroup)
            {
                Logger.IfWarn()?.Message($"Can't find {nameof(LODGroup)}").Write();
                return;
            }

            var lods = lodGroup.GetLODs();
            foreach (var l in lods)
                foreach (var r in l.renderers)
                    //r.enabled = false;
                    r.gameObject.SetActive(false);

            lodGroup.enabled = false;
        }

        private void InitAndOrganizeChunks()
        {
            // Take account of chunks:
            _chunks = new List<FracturedChunk>(_fracturedObject.GetComponentsInChildren<FracturedChunk>());
            // sort:
            _chunks.Sort((a, b) => { return a.transform.position.y.CompareTo(b.transform.position.y); });

            var rootSupportChunks = new List<FracturedChunk>();

            // switch `IsSupport` On:
            foreach (var chunk in _chunks)
            {
                chunk.IsSupportChunk = true;
                // Subscribe to change layer from "DestructChanks" to "DetachedDestructChanks" OnDetachFromObject ..
                //.. to prevent receiving hits by detached chunks ('cos it can't pass logic to parent any more)
                chunk.AddDelegate(OnDetachFromObjectDelegate, gameObject);

                // fulfill `roots`:
                if (chunk.IsROOTSupportChunk)
                    rootSupportChunks.Add(chunk);

                var rb = chunk.GetComponent<Rigidbody>();
                if (rb)
                    rb.interpolation = RigidbodyInterpolation.Interpolate;
            }

            // Leave only 1 chunk with `IsRootSupportChunk` flag On - multiple chunks with this flag are used only to 
            //  randomize the very last chunk among instances.
            if (rootSupportChunks.Count > 1)
            {
                int theOnlyRootInd =
                    UnityEngine.Random.Range(0,
                        rootSupportChunks.Count); // Here `rootSupportChunks.Count` (not "-1") is Ok, 'cos upper is exclusive.
                for (int i = 0; i < rootSupportChunks.Count; ++i)
                {
                    if (i != theOnlyRootInd)
                        rootSupportChunks[i].IsROOTSupportChunk = false;
                }

                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"\t theOnlyRootInd == {theOnlyRootInd}").Write();
            }

            if (rootSupportChunks.Count > 0)
                _hasRootSupportChunk = true;

            // Drop tiny pieces on 1st hit:
            foreach (var c in _chunks)
            {
                var rb = c.GetComponent<Rigidbody>();
                if (rb != null && rb.mass < LightMassThreshold)
                    c.OffSupportFlagIfCan();
            }

            // Del all support planes (they are not used in curr.logic):
            var fo = _fracturedObject.GetComponent<FracturedObject>();
            if (fo)
                fo.ListSupportPlanes.Clear();

            if (_chunks.Count > 1)
                _modelGeometricCenterPoint = new Vector3(0,
                    (_chunks[0].transform.position.y + _chunks[_chunks.Count - 1].transform.position.y) / 2f, 0);

            #region Dbg

            if (_chunks.Count > 1)
            {
                for (int i = 0; i < _chunks.Count - 1; ++i)
                    Debug.Assert(_chunks[i + 1].transform.position.y >= _chunks[i].transform.position.y);
            }

            #endregion
        }

        // Skip 1 frame, then do logic:
        private IEnumerator PlayHitDecalOn1stHit(Vector3 hitPoint, Vector3 hitNormal, float delaySec)
        {
            // just skip 1st frame:
            yield return null;

            yield return this.StartInstrumentedCoroutine(PlayHitDecal(hitPoint, hitNormal, delaySec));
        }


        // Skip 1 frame, then do logic:
        //private IEnumerator PlayHitImpactOn1stChunkedHit(Vector3 hitPoint, Vector3 hitNormal, float explF, float explR)
        private IEnumerator PlayHitImpactOn1stChunkedHit(RaycastHit hitInfo, float explF, float explR)
        {
            // just skip 1st frame:
            yield return null;

            PlayHitImpact(hitInfo, explF, explR);
        }


        private IEnumerator PlayHitDecal(Vector3 hitPoint, Vector3 hitNormal, float delaySec)
        {
            // A. Wait:

            float elapsedSec = 0f;
            // wait delay:
            while (elapsedSec < delaySec)
            {
                elapsedSec += Time.deltaTime;
                yield return null;
            }

            // B. Do work:

            if (_hitFx != null)
            {
                PlayHitFx(hitPoint, hitNormal, _hitFx);
            }
        }

        private GameObject _dbgSphereSign;
        private GameObject _dbgSphereSignPrefab;

        //private void PlayHitImpact(Vector3 hitPoint, Vector3 hitNormal, float explF, float explR)
        private void PlayHitImpact(RaycastHit hitInfo, float explF, float explR)
        {
            Vector3 point = hitInfo.point;
            Vector3 n = hitInfo.normal;

            // 1. Impact chunks:

            FracturedChunk chunk = hitInfo.collider.GetComponent<FracturedChunk>();

            if (chunk == null)
            {
                Logger.IfWarn()?.Message($"Raycast for DestructChunksMask failed. hitPoint: {point}, hitNormal: {n}, _chunks.N: {_chunks.Count}").Write();
                return;
            }

            //Log("Hit");

            ///var p = GetImpactPoint(chunk);
            ///chunk.Impact(p, explF, explR, /*false*/true);
            //#Impulse bug fixing attempt 
            TryAddExplosionForce(chunk, explF, /*GetImpactPoint(chunk),*/ 0f);

#if aK_DbgDrawExplosionPoint
            if (IsClient && _dbgSphereSignPrefab)
            {
                if (_dbgSphereSign)
                    Destroy(_dbgSphereSign);

                _dbgSphereSign = Instantiate(_dbgSphereSignPrefab, p, Quaternion.identity);
            }
#endif //aK_DbgDrawExplosionPoint

            // An controversial solution - hit every !IsSupport chunk also:
            /**/
            foreach (var ch in _chunks)
            /**/
            {
                /**/
                if (ch && !ch.IsSupportChunk)
                /**/
                {
                    /**/        //ch.Impact(GetImpactPoint(ch), explF, explR, false);
                                /**/        //#Impulse bug fixing attempt 
                                            /**/
                    TryAddExplosionForce(ch, explF, /*GetImpactPoint(ch),*/ 0f);
                    /**/
                }
                /**/
            }

            // 2. Play fx:

            if (_hitFx != null)
            {
                PlayHitFx(hitInfo.point, hitInfo.normal, _hitFx, false);
            }
        }

        private void TryAddExplosionForce(FracturedChunk chunk, float forceFactor, /*Vector3 point,*/ float radius = 0f)
        {
            //TODO: tmp plug - do it on detach callback instead of having `AlreadyGotImpulse` field
            if (!chunk.AlreadyGotImpulse)
            {
                var rigid = chunk.GetComponent<Rigidbody>();
                Vector3 randomImpactDirUnitVector = ColonyHelpers.SharedHelpers.RndVec3().ToUnityVector3();
                float randomImpactDirFactor = 0.7f; //Random.Range(0f, 0.3f);
                //old: rigid.AddExplosionForce(rigid.mass * forceFactor, GetImpactPoint(chunk, randomImpactDirUnitVector, randomImpactDirFactor), radius);
                rigid.AddForce(GetImpactDir(chunk, randomImpactDirUnitVector, randomImpactDirFactor) * forceFactor, ForceMode.VelocityChange);

                Vector3 randomTorqueUnitVector = ColonyHelpers.SharedHelpers.RndVec3().ToUnityVector3();
                float rndTorqueFactor = Random.Range(0.7f, 1f);
                var torqueVec = randomTorqueUnitVector * _torqueMagnitude * rndTorqueFactor;
                rigid.AddTorque(torqueVec, ForceMode.VelocityChange);

                chunk.AlreadyGotImpulse = true;
            }
        }

        // @param `randomFactor` - recommended range [0 .. 0.2]. F.e. value "0.1" means 10% deviation in direction by `randomDir` param
        // @param `randomDir` - should be unit vector
        private Vector3 GetImpactPoint(FracturedChunk chunk, Vector3 randomDir /*= new Vector3()*/, float randomFactor = 0f)
        {
            const float small = /*0.1f;*/ 0.05f; //0.01f;
            var toCenter = ModelGeometricCenterPoint - chunk.transform.position;
            var toCenterWithRandom = toCenter + toCenter.magnitude * randomFactor * randomDir;
            return chunk.transform.position + toCenterWithRandom.normalized * small;
            //return transform.position;
            //return _modelGeometricCenterPoint;
        }
        private Vector3 GetImpactDir(FracturedChunk chunk, Vector3 randomDir /*= new Vector3()*/, float randomFactor = 0f)
        {
            const float thresholdProportion = 0.2f; // Range: (0, 1)
            var toCenter = ModelGeometricCenterPoint - chunk.transform.position;
            var toCenterWithRandom = toCenter + toCenter.magnitude * randomFactor * randomDir;
            var result = -toCenterWithRandom;

            // Ensure result 've at least `thresholdProportion` horizontal part
            var hor = new Vector3(result.x, 0, result.z);
            var ver = new Vector3(0, result.y, 0);
            var proportion = hor.magnitude / ver.magnitude;
            if (proportion < thresholdProportion)
                result = ver + hor + hor.normalized * thresholdProportion * ver.magnitude;

            //tmp-plug: ensure y is "+"
            if (result.y < 0)
                //result.y = -result.y;
                result = new Vector3(result.x, -result.y, result.z);

            return result.normalized;
        }

        private void PlayHitFx(Vector3 hitPoint, Vector3 hitNormal, GameObject hitFx, bool bCreateDecal = true)
        {
            if (hitFx == null)
            {
                Logger.Error("Passed `hitFx` prefab == null");
                return;
            }
            GameObject fx = Instantiate(hitFx, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitNormal), transform);
            if (fx == null)
            {
                Logger.Error("Instantiated `hitFx` == null");
                return;
            }
            _fxList.Add(fx);
            var fxController = fx.GetComponent<FxController>();
            fxController?.Init(bCreateDecal);
        }

        // Manage destruction model after receiving damage
        private void ManageDestructionModelStep1(float newHealthVal, float maxHealth)
        {
            // 1. Actualize cached & init locals:
            //#order! 1
            float prevHpLeftPart = _hpLeftPart;

            _hpLeftPart = newHealthVal / maxHealth;

            // if 1st hit:
            //#order! 2
            if (Mathf.Approximately(prevHpLeftPart, 1f))
                DestructStep1();

            float hpLostPart = 1f - _hpLeftPart;

            //sound
            if (Mathf.Approximately(hpLostPart, 1f))
            {
                if (_soundFinalPrefab != null)
                {
                    AkEvent soundFinal = Instantiate<AkEvent>(_soundFinalPrefab, gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
                    this.StartInstrumentedCoroutine(SoundCoroutine(soundFinal));
                }
            }
            else if (_soundPrefab != null)
            {
                if (_sound == null)
                    _sound = Instantiate<AkEvent>(_soundPrefab, gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);

                this.StartInstrumentedCoroutine(SoundCoroutine(_sound));
            }

            // If no `_fracturedPrefab`, `N2_Scratched` is our last state ('cos no destruction logic 'll be applied).
            if (_fracturedPrefab == null)
                return;

            // if thresholdHp is crossed now:
            if (prevHpLeftPart >= _beginDestructWhenHpLowerThanPart && _hpLeftPart < _beginDestructWhenHpLowerThanPart)
            {
                if (_сurrState == DestructionState.N31_ChunkedStarted)
                    // We managed to fall here 2nd time in 1 tick (after chunked is started, but before it is done)
                    return;

                DestructStep2();
                this.StartInstrumentedCoroutine(StartManageDistructionStep2(prevHpLeftPart, hpLostPart));
                return;
            }

            ManageDistructionModelAferReceivingDamageStep2(prevHpLeftPart, hpLostPart);
        }

        IEnumerator SoundCoroutine(AkEvent sound)
        {
            yield return new WaitForSeconds(_soundDelay);
            sound.HandleEvent(sound.gameObject);
            yield return null;
        }

        private IEnumerator StartManageDistructionStep2(float prevHpLeftPart, float hpLostPart)
        {
            yield return new WaitForFixedUpdate();
            ManageDistructionModelAferReceivingDamageStep2(prevHpLeftPart, hpLostPart);
        }

        // Cutting single method to 2 steps is tmp plug to avoid problem:(wrong work on 1st destruction right after fractured prefab is instantiated);
        //   is cutted to be able to defer destruction for 1 tick.
        private void ManageDistructionModelAferReceivingDamageStep2(float prevHpLeftPart, float hpLostPart)
        {
            if (_сurrState == DestructionState.N31_ChunkedStarted)
            {
                _сurrState = DestructionState.N32_ChunkedDone;
                ChangeType();
            }

            if (_chunks.Count <= 0)
                return;

            // Free some chunks:
            int numOfFreeChunks = Mathf.Max(Convert.ToInt32(_chunks.Count * hpLostPart) - 1, 0); // "-1" here means - one last chunk lives till the end (until object death).

            int iterateBackTillIncl = _chunks.Count - numOfFreeChunks;

            // Find the lowest chunk with `IsSupportChunk` flag [this step is needed, 'cos the lowest chunk could be already dropped (e.g. by LIGHT_MASS filter]
            //  Described case is possible only "if (!hasRootSupportChunk)":
            if (!_hasRootSupportChunk)
            {
                int lastRootChunkInd = 0;
                if (!_chunks[lastRootChunkInd] || !_chunks[lastRootChunkInd].IsSupportChunk)
                {
                    lastRootChunkInd = ColonyShared.SharedCode.Utils.SharedHelpers.InvalidIndex;
                    for (int i = lastRootChunkInd + 1; i < _chunks.Count; ++i)
                    {
                        if (_chunks[i] && _chunks[i].IsSupportChunk)
                        {
                            lastRootChunkInd = i;
                            break;
                        }
                    }

                    if (lastRootChunkInd == ColonyShared.SharedCode.Utils.SharedHelpers.InvalidIndex)
                        return;
                }

                iterateBackTillIncl = Mathf.Max(iterateBackTillIncl, lastRootChunkInd + 1);
            }

            bool bAtLeast1ChunkDetachedThisFrame = false;

            Action<FracturedChunk> freeChunk = delegate (FracturedChunk fc)
            {
                fc.OffSupportFlagIfCan();
                MakeAllAttachedFree(fc);
                bAtLeast1ChunkDetachedThisFrame = true;
            };

            for (int i = _chunks.Count - 1; i >= iterateBackTillIncl; --i)
            {
                //#Tmp Plug:
                FracturedChunk chunk;
                try
                {
                    chunk = _chunks[i];
                }
                catch (Exception e)
                {
                    Logger.IfWarn()?.Message(e, $"Got exception. _chunks.Count=={_chunks.Count}, i=={i}, iterateBackTillIncl=={iterateBackTillIncl}. \nException: {e}.").Write();
                    break;
                }


                //#Note: It's not the rule any more, that 
                //  ["if any == null, then all further == null, 'cos sorted by "y" coord"]
                //  'cos, f.e. little pieces are made free at 1st shot
                //  so we can't just `break` if (!c)
                if (chunk)
                {
                    if (!chunk.IsROOTSupportChunk)
                    {
                        if (chunk.IsSupportChunk)
                            freeChunk(chunk);
                    }
                    else
                    {
                        --iterateBackTillIncl;
                    }
                }
            } //for

            // Enhance player feedback - try to drop at least 1 chunk per hit:
            if (!bAtLeast1ChunkDetachedThisFrame)
            {
                // Count remained support chunks:
                var remainedChunks = new List<FracturedChunk>();
                foreach (var c in _chunks)
                {
                    if (c && c.IsSupportChunk)
                        remainedChunks.Add(c);
                }

                // Is there enough chunks to drop at least 1 per hit:
                float lastDamage = prevHpLeftPart - _hpLeftPart;
                if (remainedChunks.Count - 1 > _hpLeftPart / lastDamage)
                {
                    for (int i = remainedChunks.Count - 1; i >= 0; --i)
                    {
                        if (remainedChunks[i].IsROOTSupportChunk != true)
                        {
                            freeChunk(remainedChunks[i]);
                            //Log("\t Drop at least 1 chunk for enhance player feedback. + - + - + - + - + - + - + - +");

                            break;
                        }
                    }
                }
            } //if
        }


        private void MakeAllAttachedFree(FracturedChunk chunk)
        {
            ///foreach (var c in _chunks)
            ///{
            ///    if (chunk.IsConnectedTo(c))
            ///    {
            ///        chunk.DisconnectFrom(c);
            ///        //c.OffSupportFlag();
            ///    }
            ///}
            //#Impulse bug fixing attempt 
            /**/
            chunk.DetachFromObject(true);
        }

        private void ChangeType()
        {
            if (_interactive == null)
            {
                _interactive = gameObject.GetComponent<Interactive>();
            }
            if (_interactive != null)
            {
                foreach (var change in _changeInteractionType)
                {
                    if (change.State == _сurrState)
                    {
                        _interactive.InteractionType = change.Type;
                        break;
                    }
                }
            }
        }

    #endregion Privates

    // === Inner types: ===============================================================================================
    #region Inner types

        public enum DestructionState
        {
            None = 0, // means not initialized invalid val.
            N1_FullHealth = 1, // was not hit 
            N2_Scratched = 2, // was hit at least once
            //N3_Chunked = 3, // began to break up into pieces
            N31_ChunkedStarted = 3, // began to break up into pieces
            N32_ChunkedDone = 4, // process started in 31 is done
        }

        [Serializable]
        public class ChangeInteractionType
        {
            public InteractionType Type;
            public DestructionState State;
        }

    #endregion Inner types

    }
}