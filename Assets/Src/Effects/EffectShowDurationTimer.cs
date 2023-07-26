using Assets.ColonyShared.SharedCode.GeneratedDefsForSpells;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;
using JetBrains.Annotations;
using L10n;
using Uins;
using SharedCode.Serializers;
using System.Collections.Concurrent;
using SharedCode.Entities.Engine;
using System.Threading;
using System;
using ColonyShared.SharedCode.Utils;
using UnityEngine;
using NLog;

namespace Assets.Src.Effects
{
    /// <summary>
    /// Фразы Гордона, сообщения о травмах
    /// </summary>
    [UsedImplicitly]
    class EffectShowDurationTimer : IClientOnlyEffectBinding<EffectShowDurationTimerDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        static ConcurrentDictionary<ModifierCauser, CancellationTokenSource> _timers = new ConcurrentDictionary<ModifierCauser, CancellationTokenSource>();
        public async ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectShowDurationTimerDef def)
        {
            await EffectShowText.ShowText(cast, repo, def, false);
            CancellationTokenSource ts = new CancellationTokenSource();
            var effectId = new ModifierCauser(def, cast.SpellId.Counter);
            _timers.TryAdd(effectId, ts);
            var range = cast.WordTimeRange;
            var start = SyncTime.Now;
            var entity = cast.Caster;
            using (var wrapper = await repo.Get(entity.TypeId, entity.Guid))
            {
                var rlevel = def.ShowForEveryone ? ReplicationLevel.ClientBroadcast : ReplicationLevel.ClientFull;
                var e = wrapper.Get<IEntity>(entity.TypeId, entity.Guid, rlevel);
                if (!cast.IsSlave || !cast.SlaveMark.OnClient || (e == null))
                    return;
            }
            AsyncUtils.RunAsyncTask(async () =>
            {

                while (!ts.IsCancellationRequested)
                {
                    await UnityQueueHelper.RunInUnityThread(() =>
                    {
                        var leftTime = range.Duration - (SyncTime.Now - start);
                        var leftSeconds = Mathf.Ceil(SyncTime.ToSeconds(leftTime));
                        if(leftTime > 0)
                        {
                            WarningMessager.Instance?.ShowWarningMessage($"{leftSeconds}s", def.Color, null, null, def.Color, false);
                        }

                    });
                    await Task.Delay(TimeSpan.FromSeconds(def.Period));
                }
            });
            return;
        }


        public async ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectShowDurationTimerDef def)
        {
            await EffectShowText.ShowText(cast, repo, def, true);
            var effectId = new ModifierCauser(def, cast.SpellId.Counter);
            if (_timers.TryRemove(effectId, out var ts))
            {
                ts.Cancel();
            }
        }
    }
}