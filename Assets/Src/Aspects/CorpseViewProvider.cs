using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Src.Character;
using Assets.Src.NetworkedMovement;
using Assets.Src.SpawnSystem;
using Assets.Src.Tools;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using Assets.Tools;
using Core.Environment.Logging.Extension;
using SharedCode.EntitySystem;
using UnityEngine;

using GoWithTime = System.Tuple<UnityEngine.GameObject, float>;
using SharedCode.Serializers;

namespace Assets.Src.Aspects
{
    // Retrieves view g.o. from its static dic. or spawns default (if absent in dic.)
    // Should be component of corpse g.o.
    public class CorpseViewProvider : EntityGameObjectComponent
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("CorpseViewProvider");
        
        private static readonly Dictionary<OuterRef<IEntity>, List<GoWithTime>> ViewsTemporaryStorage = new Dictionary<OuterRef<IEntity>, List<GoWithTime>>();
        private static Coroutine _cleaningCoroutine;
        private static float _cleaningTimeOut = 5*60f;

        // --- Privates: ----------------------------------------------------

        [SerializeField] private string _playAnimation;
        [SerializeField] private string _playAnimationParameter;
        [SerializeField] private int _playAnimationParameterValue;
        // Used if not found in `ViewsTemporaryStorage`:
        [SerializeField] private GameObject _defaultViewPrefab;
        [SerializeField] private SkeletonViewer[] _lootBoxPrefabs; // эти префабы выставляются в позицию данного объекта (лутбокс) 
        [SerializeField] private SkeletonViewer[] _skeletonPrefabs; // эти префабы выставляются в позицию тела персонажа

        private GameObject _viewGo; // объект тела персонажа оторванное от Pawn'а

        //=== Override incl. Unity =======================================================

        protected override void GotClient()
        {
            if (_cleaningCoroutine == null)
                _cleaningCoroutine = GameState.Instance.StartInstrumentedCoroutine(CleanStoredViewsOnTimeout());
            GotEntity();
        }

        private void OnApplicationQuit()
        {
            StopCoroutine(_cleaningCoroutine);
        }

        // --- API -----------------------------------------------------------

        // Cause creation entry in dictionary (with empty queueList yet) to avoid concurrency
        public static void RegisterMortal(OuterRef<IEntity> @ref)
        {
            ViewsTemporaryStorage.GetOrCreate(@ref);
        }

        public static void EnqueAbandonedViewGo(OuterRef<IEntity> outRef, GameObject viewGo)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($" -==VIEW  0 ==- CoViPro.EnqueAbandonedViewGo(1).").Write();

            var queueList = ViewsTemporaryStorage.GetOrCreate(outRef);
            float timeToCleanOnTimeout = Time.realtimeSinceStartup + _cleaningTimeOut;
            lock (queueList)
            {
                queueList.Add(new GoWithTime(viewGo, timeToCleanOnTimeout));
            }

            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"<Dead>  0. New viewGo stored to ViewsTemporaryStorage by {outRef}. (N=={ViewsTemporaryStorage.Count}, [0]:{ViewsTemporaryStorage.Keys.First()}).  viewGo.pos: {viewGo.transform.position} / {viewGo.transform.localPosition}").Write();
        }

        //=== Private =========================================================

        private async void GotEntity()
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"<Dead>  [CrpsViPrvdr].GotEntity. {EntityId}").Write();

            var repo = ClientRepo;
            (var owner, var myRef) = await AsyncUtils.RunAsyncTask(async () =>
            {
                var myRefInner = GetOuterRef<IEntity>();
                using (var wrapper = await repo.Get(myRefInner))
                {
                    var ent = wrapper?.Get<IHasOwnerClientBroadcast>(myRefInner, ReplicationLevel.ClientBroadcast);
                    var ownerInner = ent?.OwnerInformation?.Owner ?? OuterRef<IEntity>.Invalid;

                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"<Dead>  [CrpsViPrvdr] 1. owner: {ownerInner}. (myRef: {myRefInner}).").Write();
                    return (ownerInner, myRefInner);
                }
            });

            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"<Dead>  [CrpsViPrvdr] 2. owner: {owner}. (myRef: {myRef}.  ViewsTemporaryStorage: {ViewsTemporaryStorage.Count}, Keys: {ViewsTemporaryStorage.Keys} //pos: {transform.position}").Write();

            // 0. Find or create viewGo:
            bool usedDefaultPrefab = false;
            List<GoWithTime> queueList;
            GoWithTime queueEntry;
            if (!owner.IsValid || !ViewsTemporaryStorage.TryGetValue(owner, out queueList) || !queueList.TryDequeueInLock(out queueEntry))
            {
                //if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"<Dead>  [CrpsViPrvdr] Can't get {nameof(IHasOwnerClientBroadcast)} entty interface || View not found by OuterRef: {owner}. So dflt view prefab 'll be spawned. [may be it's ok - if player didn't know alive object]").Write();
                if (_defaultViewPrefab != null)
                    _viewGo = Instantiate(_defaultViewPrefab, transform.position, transform.rotation);
                usedDefaultPrefab = true;
            }
            else
                _viewGo = queueEntry?.Item1;

            //if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"<Dead>  [CrpsViPrvdr] 3. after TryDequeue N=={ViewsTemporaryStorage.Count}, Keys: {ViewsTemporaryStorage.Keys} //pos: {transform.position}.  viewGo.pos: {_viewGo?.transform.position} / {_viewGo?.transform.localPosition}").Write();

            if (_viewGo != null)
                _viewGo.transform.parent = transform;

            //if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"<Dead>  [CrpsViPrvdr] 3.1. after parenting.  _viewGo.pos: {_viewGo?.transform.position} / {_viewGo?.transform.localPosition}").Write();

            SkeletonViewsInstantiation();

            if (!usedDefaultPrefab)
                DoForNotDefaultViewPrefab();

            //if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"<Dead>  [CrpsViPrvdr] 4. usedDefaultPrefab: {usedDefaultPrefab} //pos: {transform.position}.  _viewGo.pos: {_viewGo.transform.position} / {_viewGo.transform.localPosition}").Write();
        }

        private static IEnumerator CleanStoredViewsOnTimeout()
        {
            while (true)
            {
                yield return new WaitForSeconds(_cleaningTimeOut);

                foreach (var kvp in ViewsTemporaryStorage)
                {
                    var queueList = kvp.Value;
                    bool somethingToClean = false;
                    // pre-check to avoid excess lock
                    foreach (var elem in queueList)
                        if (elem.Item2 < Time.realtimeSinceStartup)
                        {
                            somethingToClean = true;
                            break;
                        }

                    if (!somethingToClean)
                        continue;

                    // clean old entries:
                    lock (queueList)
                    {
                        for (int i = queueList.Count - 1;  i >= 0;  --i)
                            if (queueList[i].Item2 < Time.realtimeSinceStartup)
                                queueList.RemoveAt(i);
                    }
                }
            }//while true
            // ReSharper disable once IteratorNeverReturns
        }

        private void DoForNotDefaultViewPrefab()
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"DBDtchr::DoForNotDefaultViewPrefab.").Write();

            // a. Visual slots (if has):
            var visualSlots = transform.root.GetComponentsInChildren<VisualSlot>();
            if (visualSlots != null)
                foreach (var visualSlot in visualSlots)
                    visualSlot.Subscribe();

            // b. Play animation:

            Animator animator;
            // plan A. Try get for Mob: 
            var animPawnView = _viewGo.GetComponentInChildren<MobView>();
            if (animPawnView != null)
            {
                animator = animPawnView._animator;
            }
            else
            {
                // plan B. Try get for character:
                var thirdPersonCharacterView = _viewGo.GetComponentInChildren<ThirdPersonCharacterView>();
                if (thirdPersonCharacterView != null)
                {
                    animator = thirdPersonCharacterView.Animator;
                }
                else
                {
                    Logger.IfError()?.Message($"Can't find neither {nameof(MobView)}, nor {nameof(ThirdPersonCharacterView)} in children of root.").Write();
                    return;
                }
            }

            if (!_playAnimation.IsNullOrWhitespace())
                animator.SetTrigger(_playAnimation);
            if (!_playAnimationParameter.IsNullOrWhitespace())
                animator.SetInteger(_playAnimationParameter, _playAnimationParameterValue);
        }

        private void SkeletonViewsInstantiation()
        {
            if (_skeletonPrefabs == null || _skeletonPrefabs.Length == 0)
                return;
            InstantiatePrefabs(_skeletonPrefabs, _viewGo ? _viewGo.transform : transform.root, _viewGo);
            InstantiatePrefabs(_lootBoxPrefabs, transform, null);
        }

        private void InstantiatePrefabs(SkeletonViewer[] prefabs, Transform referenceTransform, GameObject bodyGo)
        {
            for (int i = 0; i < prefabs.Length; i++)
            {
                var skeletonPrefab = prefabs[i];
                if (skeletonPrefab == null)
                {
                    Logger.IfError()?.Message($"{transform.FullName()}: {nameof(skeletonPrefab)} [{i}] is null").Write();
                    continue;
                }
                this.StartInstrumentedCoroutine(SkeletonViewDelayedInstantiate(skeletonPrefab, referenceTransform, bodyGo));
            }
        }

        private IEnumerator SkeletonViewDelayedInstantiate(SkeletonViewer skeletonViewerPrefab, Transform referenceTransform, GameObject bodyGo)
        {
            if (skeletonViewerPrefab.StartInstantiate >= 0)
                yield return new WaitForSeconds(skeletonViewerPrefab.StartInstantiate);
            SkeletonViewer skeletonViewer = Instantiate(skeletonViewerPrefab, referenceTransform.position, referenceTransform.rotation, transform.root);
            skeletonViewer.Init(bodyGo);
        }

        private static string GetDbgLogViewsTemporaryStorage()
        {
            var res = $"VTStorage: N=={ViewsTemporaryStorage.Count}";
            int i = 0;
            foreach (var key in ViewsTemporaryStorage.Keys)
            {
                var list = ViewsTemporaryStorage[key];
                res += $"\n   [{i++}]: {key}:  ";
                if (list != null)
                {
                    res += $"list[N=={list.Count}]:";
                    for (int j = 0; j < list.Count; ++j)
                    {
                        var elem = list[j];
                        var viewGo = elem.Item1;
                        res += $"    -[{j}]:  {((viewGo != null) ? viewGo.name + ":" + viewGo.GetInstanceID() + $"AniPwnVi:{viewGo.GetComponent<MobView>() != null}" : "NULL")}:  {elem.Item2}";
                    }
                }
                else
                {
                    res += $"list == NULL.";
                }
            }

            return res;
        }
    }
}