using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.ColonyShared.SharedCode.Utils;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.Chain;
using NLog;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;

using TimeUnits = System.Int64;

namespace GeneratedCode.DeltaObjects
{
    public partial class Lifespan : ILifespanImplementRemoteMethods, IHookOnInit, IHookOnDatabaseLoad
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private IEntity _entity => parentEntity;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private IHasLifespan _hasLifespan => (IHasLifespan)parentEntity;

        // --- `IHookOnInit` implementation: -----------------------------------------------

        public Task OnInit()
        {
            return Initialization();
        }

        public Task OnDatabaseLoad()
        {
            return Initialization();
        }

        private async Task Initialization()
        {
            // Fill IHasLifespan props from def:
            var entObj = _entity as IEntityObject;
            if (entObj != null)
            {
                var hasLifespanDef = (IHasLifespanDef)entObj.Def;
                DoOnExpired = hasLifespanDef.DoOnExpired;
                LifespanSec = hasLifespanDef.LifeSpanSec;
            }
            else
                return;

            if (DoOnExpired == OnLifespanExpired.Destroy)
            {
                var destroyable = _entity as IDestroyable;
                if (destroyable == null)
                {
                    var mortal = _entity as IHasMortal;
                    if (mortal == null)
                        Logger.IfError()?.Message($"Unexpected: {nameof(DoOnExpired)} == {DoOnExpired}. But entity is neither {nameof(IHasMortal)}, nor {nameof(IDestroyable)} (1).").Write();
                }
            }

            // Start lifespan countdown:
            await StartLifespanCountdown();
        }

        // --- Main implementation: -----------------------------------------------

        public Task StartLifespanCountdownImpl()
        {
            if (LifespanSec <= 0f)
                return Task.CompletedTask;

            switch (_hasLifespan)
            {
                case IInteractiveEntity interactive:
                    CountdownCancellationToken = interactive.Chain().Delay(LifespanSec).LifespanExpired().Run();
                    break;
                case ICorpseInteractiveEntity interactive:
                    CountdownCancellationToken = interactive.Chain().Delay(LifespanSec).LifespanExpired().Run();
                    break;
                case SharedCode.Entities.Mineable.IMineableEntity mineable:
                    CountdownCancellationToken = mineable.Chain().Delay(LifespanSec).LifespanExpired().Run();
                    break;
                default:
                    Logger.IfError()?.Message("Cant start lifespan: entity type {0} is unknown", parentEntity.TypeName).Write();
                    break;
            }

            BirthTime = SyncTime.Now;
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("StartLifespanCountdown " + BirthTime).Write();

            return Task.CompletedTask;
        }

        public Task<bool> CancelLifespanCountdownImpl()
        {
            if (CountdownCancellationToken == null)
            {
                Logger.IfWarn()?.Message($"Can't cancel. {nameof(CountdownCancellationToken)} == null.").Write();
                return Task.FromResult(false);
            }

            CountdownCancellationToken.Cancel(_entity.EntitiesRepository);
            return Task.FromResult(true);
        }

        public async Task LifespanExpiredImpl()
        {
            await InvokeLifespanExpiredEvent(DoOnExpired);
            switch (DoOnExpired)
            {
                case OnLifespanExpired.None:
                    break;
                case OnLifespanExpired.Destroy:
                    IsLifespanExpired = true;
                    // Die or destroy:
                    var mortal = _entity as IHasMortal;
                    if (mortal != null)
                        //#TC-4602: is it ok to trigger IMortal.Die from here? (what if vice-versa?)
                        await mortal.Mortal.Die();
                    else
                    {
                        var destroyable = _entity as IDestroyable;
                        if (destroyable != null)
                            await destroyable.Destroy();
                        else
                            Logger.IfError()?.Message($"Unexpected: {nameof(DoOnExpired)} == {DoOnExpired}. But entity is neither {nameof(IHasMortal)}, nor {nameof(IDestroyable)} (2).").Write();
                    }
                    break;
                case OnLifespanExpired.Reset:
                    ++LifespanCycleNumber;
                    await StartLifespanCountdown();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Task<float> GetExpiredLifespanPercentImpl()
        {
            var result = (SyncTime.Now - BirthTime) / (float)SyncTime.FromSeconds(LifespanSec);
            //if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"GetExpiredLifespanPercent: now({SyncTime.Now}) - BirthTime({_hasLifespan.BirthTime}) / SyncTime.FromSeconds(_hasLifespan.LifespanSec({_hasLifespan.LifespanSec}))({SyncTime.FromSeconds(_hasLifespan.LifespanSec)}) == {(float)(SyncTime.Now - _hasLifespan.BirthTime) / (float)SyncTime.FromSeconds(_hasLifespan.LifespanSec)}.").Write();
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("GetExpiredLifespanPercent. " + result).Write();
            return Task.FromResult(result);
        }

        public async Task<bool> IsExpiredLifespanPercentInRangeImpl(float fromIncluding, float tillExcluding)
        {
            var currPercent = await GetExpiredLifespanPercent();
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("IsExpiredLifespanPercentInRange: currPercent == " + currPercent).Write();
            return currPercent >= fromIncluding && currPercent < tillExcluding;
        }

        public async Task<bool> InvokeLifespanExpiredEventImpl(OnLifespanExpired whatToDo)
        {
            await OnLifespanExpiredEvent(parentEntity.Id, TypeId, whatToDo);
            return true;
        }

    }
}
