using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.ColonyShared.SharedCode.Utils;
using SharedCode.Entities;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.Utils;
using SharedCode.Wizardry;
using Src.Aspects.Impl.Stats;
using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage;
using Assets.Src.Aspects;
using GeneratedCode.Repositories;
using SharedCode.Serializers.Protobuf;
using Assets.ColonyShared.GeneratedCode.Shared;
using SharedCode.Entities.GameObjectEntities;
using System.Threading;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using SharedCode.Repositories;

namespace GeneratedCode.DeltaObjects
{
    public partial class HealthEngine : IHookOnInit, IHookOnDatabaseLoad
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private CancellationTokenSource _cts;

        public Task OnInit()
        {
            return Initialization();
        }

        public Task OnDatabaseLoad()
        {
            return Initialization();
        }

        private Task Initialization()
        {
            this.AreadyInvokeZerohealth = false;
             Logger.IfDebug()?.Message(ParentEntityId, "Initialization").Write();
            if (parentEntity is IHasMortal mortal)
            {
                mortal.Mortal.ResurrectEvent += OnResurrect;
                mortal.Mortal.KnockedDown += OnKnockedDown;
                mortal.Mortal.ReviveFromKnockdown += OnReviveFromKnockdown;
            }
            // Logger.IfError()?.Message($"OnInit(). {EntitiesRepositoryBase.GetTypeById(_entity.TypeId).Name}").Write();
            if (parentEntity is IHasStatsEngine sasStats)
            {
                sasStats.Stats.StatsReparsedEvent += ParseHealth;
                //  Logger.IfError()?.Message($"sasStats.StatsReparsedEvent += _healthOwnerEntity.Health.ParseHealth. {EntitiesRepositoryBase.GetTypeById(_entity.TypeId).Name}").Write();
            }
            return Task.CompletedTask;
        }

        private Task OnKnockedDown()
        {
             Logger.IfDebug()?.Message(ParentEntityId, "OnKnockedDown").Write();
            AreadyInvokeZerohealth = false;
            return Task.CompletedTask;
        }

        private Task OnReviveFromKnockdown()
        {
             Logger.IfDebug()?.Message(ParentEntityId, "OnReviveFromKnockdown").Write();;
            AreadyInvokeZerohealth = false;
            return Task.CompletedTask;
        }

        public Task OnResurrectImpl(Guid arg1, int arg2, PositionRotation dummy)
        {
             Logger.IfDebug()?.Message(ParentEntityId, "OnResurrect").Write();;
            AreadyInvokeZerohealth = false;
            return Task.CompletedTask;
        }

        public async Task ChangeHealthImpl(float deltaValue)
        {
            if (deltaValue == 0)
                return;

            var statOwnerEntity = GetEngine<IHasStatsEngine>();
            if (statOwnerEntity != null)
            {
                var prevVal = await GetHealthCurrent();

                if (!(statOwnerEntity is IHasHealthWithCustomMechanics customMechanics) || !await customMechanics.ChangeHealthInternal(deltaValue))
                {
                    //Logger.IfError()?.Message($"#DBG: ChangeHealthImpl ({deltaValue}), {statOwnerEntity.ToString()}").Write();
                    var stat = GlobalConstsHolder.StatResources.HealthCurrentStat.Target;
                    await statOwnerEntity.Stats.ChangeValue(stat, deltaValue);
                }

                var newVal = await GetHealthCurrent();
                    //Logger.IfDebug()?.Message($"#Dbg: HealthCurrentSet: deltaValue:{deltaValue}, prevVal: {prevVal}, newVal:{newVal}").Write();

                if (prevVal > 0 && newVal <= 0f)
                {
                    if (_cts != null)
                        await StopDeathTimer();
                    if (!AreadyInvokeZerohealth)
                    {
                        AreadyInvokeZerohealth = true;
                        await InvokeZeroHealthEvent();
                        //#TODO: inverse dependence: mortal should be subscribed on `IHealthOwner.ZeroHealthEvent` & do `Die()` as dflt behaviour:
                        var mortal = statOwnerEntity as IHasMortal;
                        if (mortal != null)
                            await mortal.Mortal.ZeroHealthReached();
                    }
                    else
                         Logger.IfDebug()?.Message(ParentEntityId, "AreadyInvokeZerohealth").Write();;
                }
            }
        }

        private T GetEngine<T>() where T : class
        {
            var ownerEntity = parentDeltaObject as T;
            if (ownerEntity == null)
            {
                ownerEntity = parentEntity as T;
            }

            return ownerEntity;
        }

        public async Task<bool> InvokeZeroHealthEventImpl()
        {
            if (parentEntity != null && parentEntity.TypeId == ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter)))
            {
                GlobalLoggers.SubscribeLogger.IfInfo()?.Message(ParentEntityId, $"Invoke: [{((WorldCharacter)parentEntity).DebugObjectWatchID}; {ReplicaTypeRegistry.GetTypeById(parentEntity.TypeId)}; {parentEntity.Id} -> {ReplicaTypeRegistry.GetTypeById(TypeId).Name}] -> [{nameof(InvokeZeroHealthEventImpl)}]").Write();
            }

            var healthOwner = GetEngine<IHasHealth>();
            var entity = healthOwner as IEntity;
            if (entity != null)
                await OnZeroHealthEvent(entity.Id, entity.TypeId);
            else
                await OnZeroHealthEvent(Guid.Empty, 0);

            return true;
        }

        public async ValueTask<float> GetHealthCurrentImpl()
        {
            var statOwnerEntity = GetEngine<IHasStatsEngine>();
            if (statOwnerEntity != null)
            {
                var stat = GlobalConstsHolder.StatResources.HealthCurrentStat.Target;
                var health = await statOwnerEntity.Stats.GetStat(stat);
                if (health != null)
                    return await health.GetValue();
                else
                {
                    // Logger.IfError()?.Message($"Can't get stat `{stat.StatName}` for {statOwnerEntity.ToString()}").Write();
                }
            }

            return 0f;
        }

        public async ValueTask<float> GetMaxHealthImpl()
        {
            var statOwnerEntity = GetEngine<IHasStatsEngine>();
            if (statOwnerEntity != null)
            {
                var stat = GlobalConstsHolder.StatResources.HealthCurrentStat.Target;
                var healthCurr = await statOwnerEntity.Stats.GetStat(stat);
                if (healthCurr != null)
                    return healthCurr.LimitMaxCache;
                else
                {
                    //   Logger.IfError()?.Message($"Can't find stat {stat.StatName}.").Write();
                }
            }

            return 0f;
        }

        public async ValueTask<float> GetMinHealthImpl()
        {
            var statOwnerEntity = GetEngine<IHasStatsEngine>();
            if (statOwnerEntity != null)
            {
                var stat = GlobalConstsHolder.StatResources.HealthCurrentStat.Target;
                var healthCurr = await statOwnerEntity.Stats.GetStat(stat);
                if (healthCurr != null)
                    return healthCurr.LimitMinCache;
                else
                {
                    //   Logger.IfError()?.Message($"Can't find stat {stat.StatName}.").Write();
                }
            }

            return 0f;
        }

        public async ValueTask<float> GetMaxHealthAbsoluteImpl()
        {
            var statOwnerEntity = GetEngine<IHasStatsEngine>();
            if (statOwnerEntity != null)
            {
                var stat = GlobalConstsHolder.StatResources.HealthMaxAbsoluteStat.Target;
                var healthMaxAbsolute = await statOwnerEntity.Stats.GetStat(stat);
                if (healthMaxAbsolute != null)
                    return await healthMaxAbsolute.GetValue();
                else
                    return await GetMaxHealthImpl();
            }

            return 0f;
        }

        public async Task ParseHealthImpl()
        {
            var statOwnerEntity = GetEngine<IHasStatsEngine>();
            if (statOwnerEntity != null)
            {
                var health = await statOwnerEntity.Stats.GetStat(GlobalConstsHolder.StatResources.HealthCurrentStat.Target);
                TimeStat timeHealthStat = health as TimeStat;
                if (timeHealthStat != null)
                {
                    timeHealthStat.UnsubscribePropertyChanged(nameof(TimeStat.State), UpdateDeathTime);
                    timeHealthStat.UnsubscribePropertyChanged(nameof(TimeStat.LimitMinCache), UpdateDeathTime);
                    timeHealthStat.SubscribePropertyChanged(nameof(TimeStat.State), UpdateDeathTime);
                    timeHealthStat.SubscribePropertyChanged(nameof(TimeStat.LimitMinCache), UpdateDeathTime);
                }
            }
        }
        private async Task UpdateDeathTime(EntityEventArgs args)
        {
            var healthOwnerEntity = GetEngine<IHasHealth>();
            var statOwnerEntity = GetEngine<IHasStatsEngine>();
            if (healthOwnerEntity != null && statOwnerEntity != null)
            {
                if (_cts != null)
                    await StopDeathTimer();

                var newChangeRate = ((ITimeStat)(await statOwnerEntity.Stats.GetStat(GlobalConstsHolder.StatResources.HealthCurrentStat)))?.State.ChangeRateCache ?? 0;
                if (newChangeRate < 0 && await GetMinHealth() <= 0)
                {
                    var currentHealth = await GetHealthCurrent();
                    Logger.IfDebug()?.Message(ParentEntityId, $"currentHealth = {currentHealth}; newChangeRate = {newChangeRate}").Write();
                    if (currentHealth > 0)
                    {
                        var timeToDeath = currentHealth / -newChangeRate;
                        Logger.IfDebug()?.Message(ParentEntityId, $"timeToDeath = {timeToDeath}").Write();
                        await StartDeathTimer(timeToDeath);
                    }
                }
            }
        }

        public Task StartDeathTimerImpl(float timeToDie)
        {
            if (_cts != null)
            {
                Logger.IfError()?.Message(ParentEntityId, "Death Timer is already started, restarting").Write();
                _cts.Cancel();
            }
            _cts = new CancellationTokenSource();
            StatsEngine.RunChainLight(parentEntity.TypeId, parentEntity.Id, DeathTimerElapsed, TimeSpan.FromSeconds(timeToDie), EntitiesRepository, _cts.Token);
            return Task.CompletedTask;
        }

        public Task StopDeathTimerImpl()
        {
            if (_cts == null)
            {
                Logger.IfError()?.Message(ParentEntityId, "Death Timer is already started, restarting").Write();
                return Task.CompletedTask;
            }
            _cts.Cancel();
            _cts = null;
            return Task.CompletedTask;
        }

        public Task DeathTimerElapsedImpl()
        {
            if (parentEntity is IHasMortal mortal)
                return mortal.Mortal.ZeroHealthReached();

            return Task.CompletedTask;
        }

        public async Task<DamageResult> ReceiveDamageImpl(Damage damage, OuterRef<IEntity> aggressor)
        {
            var hasMortal = parentEntity as IHasMortal;
            float prevHealthValue = await GetHealthCurrent();
            var prevState = hasMortal != null ? await hasMortal.Mortal.GetState() : MortalState.Alive;
            var healthEntity = parentEntity as IHasHealthWithCustomMechanics;
            DamageResult result;
            if (healthEntity == null || !(result = await healthEntity.ReceiveDamageInternal(damage, aggressor.Guid, aggressor.TypeId)).IsApplied)
            {
                Logger.IfDebug()?.Message(ParentEntityId, "ReceiveDamageInternal Dmg:{0} Typ:{1} Victim:{2}.{3} Aggressor:{4}.{5}", damage.BattleDamage, damage.DamageType, parentEntity.TypeName, parentEntity.Id, ReplicaTypeRegistry.GetTypeById(aggressor.TypeId)?.GetFriendlyName() ?? aggressor.TypeId.ToString(), aggressor.Guid).Write();
                await ChangeHealth(-damage.BattleDamage);
                result = new DamageResult(damage.BattleDamage);
            }
            float currenthealthValue = await GetHealthCurrent();
            if (currenthealthValue < prevHealthValue)
            {
                var maxHealth = await GetMaxHealth();
                await OnDamageReceivedEvent(prevHealthValue, currenthealthValue, maxHealth, damage);
                if (hasMortal != null)
                    await OnDamageReceivedExtEvent(prevHealthValue, currenthealthValue, maxHealth, prevState, await hasMortal.Mortal.GetState(), damage);
            }
            return result;
        }
    }
}
