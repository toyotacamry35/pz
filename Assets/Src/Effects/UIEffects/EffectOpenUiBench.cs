using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Tools;
using Core.Cheats;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedDefsForSpells;
using JetBrains.Annotations;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using SharedCode.Wizardry;
using Uins;

namespace Assets.Src.Effects.UIEffects
{
    [UsedImplicitly]
    public class EffectOpenUiBench : BaseEffectOpenUi, IEffectBinding<EffectOpenUiBenchDef>
    {
        const bool Dbg = true;

        [NotNull]
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(EffectOpenUiBench));

        private static readonly ConcurrentDictionary<SpellId, ICancelable> RequestedWaiters = new ConcurrentDictionary<SpellId, ICancelable>();
        private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(3f);

        private static void TryCleanWaiter(SpellId spellId, bool callCancel, bool silentMode = false)
        {
            if (RequestedWaiters.TryRemove(spellId, out var waiter))
            {
                if (callCancel)
                {
                    var cancelSucceed = waiter.Cancel();
                    if (Dbg)
                        if (DbgLog.Enabled)
                            DbgLog.Log(15200, $"15200::: {DateTime.UtcNow} cancelSucceed: " + cancelSucceed);
                }
            }
            else if (!silentMode)
                Logger.IfError()?.Message($"Unexpected: _requestedWaiters.TryRemove({spellId}) returned false.").Write();
        }

        protected async override Task OnOpenSuccess(EffectData effectData)
        {
            if (Dbg)
                if (DbgLog.Enabled)
                    DbgLog.Log("::: OnOpenSuccess.0");

            if (!effectData.FinalTargetOuterRef.IsValid)
            {
                LogError(effectData, $"OnOpenSuccess() {effectData.FinalTargetOuterRef} isn't valid");
                await StopSpellAsync(effectData.Cast, effectData.Repo);
                if (Dbg)
                    if (DbgLog.Enabled)
                        DbgLog.Log("::: OnOpenSuccess. Closed because has invalid FinalTargetOuterRef");
                return;
            }

            var waiter = EntityWaiter<IEntity>.NewOnEntityReceivedRequest(
                effectData.Repo,
                effectData.FinalTargetOuterRef.TypeId,
                effectData.FinalTargetOuterRef.Guid,
                ReplicationLevel.Always,
                RequestTimeout,
                doWhenGotEntity: () =>
                {
                    if (Dbg)
                        if (DbgLog.Enabled)
                            DbgLog.Log($"15200::: {DateTime.UtcNow} ##DBG:  OnOpenSuccess");

                    UnityQueueHelper.RunInUnityThreadNoWait(() => { InventoryUiOpener.Instance.OnOpenInventoryMachineTabFromSpell(effectData); });
                    TryCleanWaiter(effectData.Cast.SpellId, false);
                },
                onTimeout: () =>
                {
                    if (Dbg)
                        if (DbgLog.Enabled)
                            DbgLog.Log($"15200::: {DateTime.UtcNow} ##DBG:  onTimeout");
                    TryCleanWaiter(effectData.Cast.SpellId, false);
                });

            if (!RequestedWaiters.TryAdd(effectData.Cast.SpellId, waiter))
                Logger.IfError()?.Message($"Unexpected: _requestedWaiters.TryAdd({effectData.Cast.SpellId}) returned false.").Write();

            if (Dbg)
                if (DbgLog.Enabled)
                    DbgLog.Log("::: OnOpenSuccess.1");
        }

        protected override Task OnCloseAnyway(EffectData effectData)
        {
            TryCleanWaiter(effectData.Cast.SpellId, true, true);

            UnityQueueHelper.RunInUnityThreadNoWait(() => { InventoryUiOpener.Instance?.OnCloseInventoryMachineTabFromSpell(effectData); });
            return Task.CompletedTask;
        }

        public ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectOpenUiBenchDef def)
        {
            return base.Attach(cast, repo, def);
        }

        public ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectOpenUiBenchDef def)
        {
            return base.Detach(cast, repo, def);
        }

        // --- Dbg ------------------------------------------
        // internal static void DbgLog(string s)
        // {
        //     if (Dbg) if (ColonyShared.SharedCode.Utils.DbgLog.Enabled) ColonyShared.SharedCode.Utils.DbgLog.Log(s);
        // }
        // internal static void DbgLog(int i, string s)
        // {
        //     if (Dbg) if (ColonyShared.SharedCode.Utils.DbgLog.Enabled) ColonyShared.SharedCode.Utils.DbgLog.Log(i, s);
        // }


        // === TEST ==================================================================

        #region TEST

        /// <summary>
        /// </summary>
        /// <param name="mode">
        /// 1 - EnttyAlreadyReplicated
        /// 2 - wait2sec for entitty
        /// 3 - cancel manually (as by Effect detach) 
        /// 4 - cancel by timeout
        /// </param>
        [Cheat, UsedImplicitly]
        public static void TestCheckPZ15200Done(int mode)
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            if (mode < 1 || mode > 4)
            {
                Logger.IfError()?.Message("mode < 1 || mode > 4 : " + mode).Write();
                return;
            }

            AsyncUtils.RunAsyncTask(async () =>
            {
                var sceneId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
                using (var cheatServiceWrapper = await repo.Get<ICheatServiceEntityClientBroadcast>(sceneId))
                {
                    var serviceEntity = cheatServiceWrapper.Get<ICheatServiceEntityClientBroadcast>(sceneId);
                    switch (mode)
                    {
                        case 1:
                        {
                            _createdEnttyOutRef = await serviceEntity.TestCheckPZ15200Done(0);
                            await Task.Delay(1000);
                            MakeRequest(_createdEnttyOutRef.Guid, _createdEnttyOutRef.TypeId);
                            await Task.Delay(1000);
                            ManuallyCancel();
                        }
                            break;
                        case 2:
                        {
                            _createdEnttyOutRef = await serviceEntity.TestCheckPZ15200Done(waitBeforeReplicate: 1f);
                            //await Task.Delay(1000);
                            MakeRequest(_createdEnttyOutRef.Guid, _createdEnttyOutRef.TypeId);
                            await Task.Delay(2000);
                            ManuallyCancel();
                        }
                            break;
                        case 3:
                        {
                            _createdEnttyOutRef = await serviceEntity.TestCheckPZ15200Done(waitBeforeReplicate: 2f);
                            //await Task.Delay(1000);
                            MakeRequest(_createdEnttyOutRef.Guid, _createdEnttyOutRef.TypeId);
                            await Task.Delay(1000);
                            ManuallyCancel();
                        }
                            break;
                        case 4:
                        {
                            _createdEnttyOutRef = await serviceEntity.TestCheckPZ15200Done(waitBeforeReplicate: 4f);
                            //await Task.Delay(1000);
                            MakeRequest(_createdEnttyOutRef.Guid, _createdEnttyOutRef.TypeId);
                            await Task.Delay(5000);
                            ManuallyCancel();
                        }
                            break;
                        default: throw new ArgumentOutOfRangeException(mode.ToString());
                    }
                }
            });
        }

        private static OuterRef<IEntity> _createdEnttyOutRef;
        private static ulong _lastTestSpellId = 899;
        private static SpellId _lastTestSpellIdStruct;

        protected static void MakeRequest(Guid entId, int typeId)
        {
            Logger.IfError()?.Message($"#Dbg: 15200::: {DateTime.UtcNow} ##DBG:  MakeRequest: {typeId} {entId}.").Write();

            IEntitiesRepository repository = GameState.Instance.ClientClusterNode;
            _lastTestSpellId++;
            _lastTestSpellIdStruct = new SpellId(_lastTestSpellId);
            var waiter = EntityWaiter<IEntity>.NewOnEntityReceivedRequest(repository, typeId, entId, ReplicationLevel.Always, RequestTimeout,
                doWhenGotEntity: () =>
                {
                    UnityQueueHelper.RunInUnityThreadNoWait(() =>
                    {
                        Logger.IfError()?.Message($"#Dbg: 15200::: {DateTime.UtcNow} ##DBG:  MakeRequest.doWhenGotEntity: {typeId} {entId}.").Write();
                    });
                },
                onTimeout: () =>
                {
                    TryCleanWaiter(_lastTestSpellIdStruct, false);
                    Logger.IfError()?.Message($"#Dbg: 15200::: {DateTime.UtcNow} ##DBG:  MakeRequest.onTimeout: {typeId} {entId}.").Write();
                });

            if (!RequestedWaiters.TryAdd(_lastTestSpellIdStruct, waiter))
                Logger.IfError()?.Message($"#Dbg: 15200::: {DateTime.UtcNow} Unexpected: _requestedWaiters.TryAdd({_lastTestSpellIdStruct}) returned false.").Write();
        }


        protected static void ManuallyCancel()
        {
            TryCleanWaiter(_lastTestSpellIdStruct, true /*, true*/);

            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                Logger.IfError()?.Message($"#Dbg: 15200::: {DateTime.UtcNow} ##DBG:  ManuallyCancel: {_createdEnttyOutRef.TypeId} {_createdEnttyOutRef.Guid}.").Write();
            });
        }

        #endregion TEST
    }
}