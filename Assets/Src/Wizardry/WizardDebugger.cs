using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.SpawnSystem;
using Assets.Src.Tools;
using Assets.Tools;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GameplayDebugger;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Serializers;
using SharedCode.Wizardry;
using UnityEngine;

namespace Assets.Src.Wizardry
{
    public class WizardDebugger : EntityGameObjectComponent
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        private static readonly WaitForSeconds UpdateWait = new WaitForSeconds(0.1f);
        
        [SerializeField] private bool _shouldUpdateDebugging;

        private readonly ConcurrentDictionary<SpellId, SpellCastStatus> _currentSpells = new ConcurrentDictionary<SpellId, SpellCastStatus>();
        private Coroutine _coroutine;
        private bool _subscribed;

#if UNITY_EDITOR        
        private bool ShouldUpdateDebugging => _shouldUpdateDebugging;
#else
        private bool ShouldUpdateDebugging => false;
#endif

        protected override void GotClient()
        {
            if (!ShouldUpdateDebugging || !GlobalConstsDef.DebugFlagsGetter.IsDebugSpells(GlobalConstsHolder.GlobalConstsDef))
                return;
            
            var ownerRef = OuterRef;
            var repo = ClientRepo;
            AsyncUtils.RunAsyncTask(async () =>
            {
                try
                {
                    for (;; await Task.Delay(TimeSpan.FromMilliseconds(100), repo.StopToken)) // TODO: переделать с подпиской на появление энтити в репе?
                    {
                        repo.StopToken.ThrowIfCancellationRequested();
                        using (var hasWizard = await repo.Get(ownerRef.TypeId, ownerRef.Guid))
                        {
                            var has = hasWizard.Get<IHasWizardEntityClientBroadcast>(ownerRef.TypeId, ownerRef.Guid, ReplicationLevel.ClientBroadcast);
                            if (has != null)
                            {
                                using (var wizCnt = await repo.Get(has.Wizard.TypeId, has.Wizard.Id))
                                {
                                    var wizard = wizCnt.Get<IWizardEntityClientBroadcast>(has.Wizard.TypeId, has.Wizard.Id, ReplicationLevel.ClientBroadcast);
                                    foreach (var spell in wizard.Spells)
                                        _currentSpells.TryAdd(spell.Key, new SpellCastStatus() { StartTimeStamp = spell.Value.Started, SpellDesc = spell.Value.CastData });
                                    wizard.Spells.OnItemAddedOrUpdated += OnSpellStarted;
                                    wizard.Spells.OnItemRemoved += OnSpellFinished;
                                    _subscribed = true;
                                    UnityQueueHelper.RunInUnityThreadNoWait(() => { _coroutine = this.StartInstrumentedCoroutine(DebuggingUpdateRoutine(repo)); });
                                }
                                break;
                            }
                        }
                    }
                } catch (OperationCanceledException) {}
            });
        }

        protected override void LostClient()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            if (_subscribed)
            {
                var ownerRef = OuterRef;
                var repo = ClientRepo;
                AsyncUtils.RunAsyncTask(async () =>
                {
                    try
                    {
                        repo.StopToken.ThrowIfCancellationRequested();
                        using (var hasWizard = await repo.Get(ownerRef.TypeId, ownerRef.Guid))
                        {
                        	hasWizard.TryGet<IHasWizardEntityClientBroadcast>(ownerRef.TypeId, ownerRef.Guid, ReplicationLevel.ClientBroadcast, out var has);
                            if (has != null)
                            {
                                using (var wizCnt = await repo.Get(has.Wizard.TypeId, has.Wizard.Id))
                                {
                                    var wizard = wizCnt.Get<IWizardEntityClientBroadcast>(has.Wizard.TypeId, has.Wizard.Id, ReplicationLevel.ClientBroadcast);
                                    wizard.Spells.OnItemAddedOrUpdated -= OnSpellStarted;
                                    wizard.Spells.OnItemRemoved -= OnSpellFinished;
                                }
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                    }
                });
            }
        }
        
        private Task OnSpellStarted(DeltaDictionaryChangedEventArgs<SpellId, ISpellClientBroadcast> args)
        {
            try
            {
                var newSpell = args.Value;
                var id = newSpell.Id;
                var start = newSpell.Started;
                var cast = newSpell.CastData;
                _currentSpells.TryAdd(args.Key, new SpellCastStatus() { StartTimeStamp = start, SpellDesc = cast });
            } catch (Exception e) { Logger.IfError()?.Exception(e).Write(); }
            return Task.CompletedTask;
        }

        private Task OnSpellFinished(DeltaDictionaryChangedEventArgs<SpellId, ISpellClientBroadcast> args)
        {
            try
            {
                if (_currentSpells.TryGetValue(args.Key, out var spell))
                {
                    spell.FinishReason = args.Value.FinishReason;
                    spell.RemoveAt = SyncTime.NowUnsynced + 2000;
                    spell.EndTimeStamp = args.Value.Finished;
                }
            } catch (Exception e) { Logger.IfError()?.Exception(e).Write(); }
            return Task.CompletedTask;
        }

        private IEnumerator DebuggingUpdateRoutine(IEntitiesRepository repo)
        {
            if (!GlobalConstsDef.DebugFlagsGetter.IsDebugSpells(GlobalConstsHolder.GlobalConstsDef))
                yield break;

            while (true)
            {
                foreach(var kv in _currentSpells)
                    if (kv.Value.RemoveAt < SyncTime.NowUnsynced && kv.Value.FinishReason != SpellFinishReason.None)
                        _currentSpells.TryRemove(kv.Key, out _);
                while (!ShouldUpdateDebugging)
                    yield return CoroutineAwaiters.GetTick(1);
                CollectSnapshot();
                yield return UpdateWait;
            }
        }
        
        private void CollectSnapshot()
        {
            if (!ShouldUpdateDebugging || !GlobalConstsDef.DebugFlagsGetter.IsDebugSpells(GlobalConstsHolder.GlobalConstsDef))
                return;

            if (!DebuggedObjectsScope.Instance.Contains(this.gameObject))
                return;

            DebuggedObjectsScope.Instance.IfContains(this.gameObject, recorder =>
            {
                var now = SyncTime.Now;
                recorder.StoreSnapshot(new ObjectSnapshot()
                {
                    PosterType = typeof(SpellDoerAsync),
                    DrawSelf = () =>
                    {
                        int index = 0;
                        var pos = (transform.position + new Vector3(0, 2.5f, 0)).WorldToGuiPoint();
                        if (pos.z < 0)
                            return;
                        var texRect = new Rect(pos, new Vector2(150, 25));
                        var statuses = _currentSpells.ToDictionary(x => x.Key, x => new SpellCastStatus(x.Value));
                        foreach (var status in statuses)
                        {
                            var spell = status.Value;
                            var spellId = status.Key;
                            var rect = texRect;
                            rect.position += index * new Vector2(0, rect.size.y);
                            float fill = 1;
                            Color fillColor = Color.blue;
                            if (spell.SpellDesc.Def.IsInfinite)
                            {
                                fillColor = Color.yellow;
                            }
                            else
                            {
                                fill = SyncTime.ToSeconds((Math.Min(spell.EndTimeStamp, now) - spell.StartTimeStamp)) / spell.SpellDesc.Def.Duration;
                            }

                            switch (spell.FinishReason)
                            {
                                case SpellFinishReason.SucessOnTime:
                                case SpellFinishReason.SucessOnDemand:
                                    fillColor = Color.green;
                                    break;
                                case SpellFinishReason.FailOnStart:
                                case SpellFinishReason.FailOnDemand:
                                case SpellFinishReason.FailOnEnd:
                                    fillColor = Color.red;
                                    break;
                            }

                            GUIExtensions.DrawRectangle(new Rect(rect.position, new Vector2(rect.width * fill, rect.height)), fillColor);
                            var textArea = new Rect(new Vector2(rect.xMax, rect.y), new Vector2(150, 25));
                            GUI.Box(textArea, $"{spellId} {spell.SpellDesc.Def.____GetDebugShortName()}");
                            index++;
                        }
                    },
                    ObjectId = new GameObjectId(this.gameObject) {Name = this.gameObject.name}
                });
            });
        }
    }
}