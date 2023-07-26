using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.ColonyShared.SharedCode.Entities;
using Core.Environment.Logging.Extension;
using NLog;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.Utils;

namespace GeneratedCode.DeltaObjects
{
    public partial class LimitedLifetime : ILimitedLifetimeImplementRemoteMethods, IHookOnInit
    {
        public static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        //=== Private =========================================================

        private async Task OnFirstOpenedOrLastClosed(bool isOpened)
        {
            Logger.IfInfo()?.Message($"LimitedLifetime: OnFirstOpenedOrLastClosed(isOpened = {isOpened})").Write();
            if (isOpened)
            {
                //Кто-то первый открыл - останавливаем отсчет
                if (CountdownToken == null)
                {
                    Logger.IfError()?.Message($"LimitedLifetime: OnFirstOpenedOrLastClosed. {nameof(OnFirstOpenedOrLastClosed)}() Unexpected: _countdownToken is null").Write();
                    return;
                }

                CancelCountdown(CountdownToken);
                CountdownToken = null;
                return;
            }

            var def = await GetLimitedLifetimeDef();
            if (def.DestroyIfBecomeEmpty && parentEntity is IHasOpenMechanics openMechanics && await openMechanics.OpenMechanics.IsEmpty())
            {
                Logger.IfInfo()?.Message($"LimitedLifetime: OnFirstOpenedOrLastClosed. await ((IHasDestroyable) parentEntity).Destroyable.DestroyAfterDelay(.2f)").Write();
                await ((IHasDestroyable) parentEntity).Destroyable.DestroyAfterDelay(.2f);
                return;
            }

            //Кто-то последний закрыл, заново запускаем отсчет
            if (CountdownToken != null)
            {
                Logger.IfInfo()?.Message($"LimitedLifetime: {nameof(OnFirstOpenedOrLastClosed)}() Unexpected: _countdownToken isn't null").Write();
                return;
            }

            await StartEntityCountdown();
        }

        private async Task StartEntityCountdown()
        {
            LifetimeLimit = await GetLifetimeLimit();
            CountdownStartTimestamp = DateTime.UtcNow.ToUnix();
            CountdownToken = await StartCountdown();
        }

        private void CancelCountdown(ChainCancellationToken countdownToken)
        {
            if (!countdownToken.AssertIfNull(nameof(countdownToken)))
                countdownToken.Cancel(EntitiesRepository);

            CountdownStartTimestamp = Int64.MaxValue;
        }

        public async Task OnInit()
        {
            var canBeOpened = parentEntity as IHasOpenMechanics;
            if (canBeOpened != null)
            {
                canBeOpened.OpenMechanics.FirstOpenedOrLastClosed += OnFirstOpenedOrLastClosed;
            }

            await StartEntityCountdown();
        }

        public ValueTask<LimitedLifetimeDef> GetLimitedLifetimeDefImpl()
        {
            var limitedLifetimeDef = (parentEntity as IEntityObject)?.Def as ILimitedLifetimeDef;
            limitedLifetimeDef.AssertIfNull(nameof(limitedLifetimeDef));
            return new ValueTask<LimitedLifetimeDef>(limitedLifetimeDef?.LimitedLifetimeDef.Target);
        }

        public async Task<long> GetLifetimeLimitImpl()
        {
            var limitedLifetimeDef = await GetLimitedLifetimeDef();
            if (limitedLifetimeDef == null)
                return 0;
            return (long)limitedLifetimeDef.DefaultLifetime;
        }

        public async Task<ChainCancellationToken> StartCountdownImpl()
        {
            var lifetimeLimit = await GetLifetimeLimit();
            if (lifetimeLimit == 0)
            {
                Logger.IfError()?.Message($"{nameof(StartCountdownImpl)}(), lifetimeLimit is 0").Write();
                return null;
            }

            return await (parentEntity as IHasDestroyable).Destroyable.DestroyAfterDelay(lifetimeLimit);
        }
    }
}