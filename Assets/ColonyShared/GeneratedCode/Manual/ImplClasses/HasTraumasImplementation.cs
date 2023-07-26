using Assets.ColonyShared.SharedCode.Entities;
using Assets.Src.Arithmetic;
using Assets.Src.Aspects.Impl.Traumas.Template;
using NLog;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using Src.Aspects.Impl.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Entities.Service;
using SharedCode.Serializers;
using Assets.ColonyShared.SharedCode.Player;
using GeneratedCode.EntitySystem;
using ResourceSystem.Entities;
using ResourceSystem.Utils;
using SharedCode.Entities.GameObjectEntities;
using System.Threading;
using Assets.ColonyShared.SharedCode.Wizardry;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.Repositories;
using SharedCode.EntitySystem.Delta;
using SharedCode.Repositories;

namespace GeneratedCode.DeltaObjects
{
    public partial class Traumas : ITraumasImplementRemoteMethods, IHookOnInit, IHookOnDatabaseLoad, IHookOnStart, IHookOnDestroy, IHookOnUnload
    {
        // ReSharper disable once UnusedMember.Local
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly HashSet<string> _currentlyStartingTraumas = new HashSet<string>();

        private CancellationTokenSource _cts;

        private TraumasDef AllTraumas => ((parentEntity as IEntityObject).Def as IHasTraumasDef)?.AllTraumas;

        // --- IHookReplicationLevelChanged implementation: ---------------------------------------
        #region IHookReplicationLevelChanged implementation

        public Task OnInit() => Initialization();
        public async Task OnDatabaseLoad()
        {
            await Initialization();
            List<string> toRemove = null;
            Logger.IfDebug()?.Message(ParentEntityId, $"Saved traumas:\n{string.Join("\n", SaveableActiveTraumas.Values)}").Write();
            foreach (var tuple in SaveableActiveTraumas)
            {
                var (id, trauma) = (tuple.Key, tuple.Value);
                if (!ActiveTraumas.ContainsKey(id) &&
                    parentEntity is IHasBuffs hasBuffs &&
                    trauma.Def is SaveableTraumaDef &&
                    trauma.HasActiveSpell() &&
                    hasBuffs.Buffs.All.ContainsKey(new SpellId(trauma.SpellId)))
                {
                    ActiveTraumas.Add(id, trauma); // не нужно стартовать
                }
                else
                {
                    (toRemove ?? (toRemove = new List<string>())).Add(id);
                    Logger.IfDebug()?.Message(ParentEntityId, $"Saved trauma has bo buff {trauma}").Write();
                }
            }
            if (toRemove != null)
                foreach (var id in toRemove)
                    SaveableActiveTraumas.Remove(id);
        }

        private Task Initialization()
        {
            if(ActiveTraumas == null)
                ActiveTraumas = new DeltaDictionary<string, ITraumaGiver>();
            if(SaveableActiveTraumas == null)
                SaveableActiveTraumas = new DeltaDictionary<string, ITraumaGiver>();

            if (parentEntity is IWorldCharacter wc)
            {
                wc.OnIdleModeStarted += SuspendTraumas;
                wc.OnIdleModeStopped += StartTraumas;
            }
            if (parentEntity is IHasBuffs hasBuffs)
            {
                hasBuffs.Buffs.All.OnItemRemoved += Buffs_OnItemRemoved;
            }
            return Task.CompletedTask;

        }

        public async Task OnStart()
        {
            if (AllTraumas.DoNotWork)
                return;

            await SubscribeToMortal();

            // Если спелл соответствующий травме закончился или отменился
            if (parentEntity is IHasWizardEntity wizardEntityContainer)
            {
                using (var wrapper = await EntitiesRepository.Get(wizardEntityContainer.Wizard.TypeId, wizardEntityContainer.Wizard.Id))
                {
                    IWizardEntity wizardEntity = wrapper.Get<IWizardEntity>(wizardEntityContainer.Wizard.Id);
                    wizardEntity.Spells.OnItemRemoved += Spells_OnItemRemoved;
                }
            }
        }

        public Task OnUnload() => Deinitialization();
        public Task OnDestroy() => Deinitialization();

        private async Task Deinitialization()
        {
            if (parentEntity is IWorldCharacter wc)
            {
                wc.OnIdleModeStarted -= SuspendTraumas;
                wc.OnIdleModeStopped -= StartTraumas;
            }

            if (parentEntity is IHasBuffs hasBuffs)
                hasBuffs.Buffs.All.OnItemRemoved -= Buffs_OnItemRemoved;

            if (parentEntity is IHasWizardEntity wizardEntityContainer)
                using (var wrapper = await EntitiesRepository.Get(wizardEntityContainer.Wizard.TypeId, wizardEntityContainer.Wizard.Id))
                    if(wrapper.TryGet<IWizardEntity>(wizardEntityContainer.Wizard.Id, out var wizardEntity))
                        wizardEntity.Spells.OnItemRemoved -= Spells_OnItemRemoved;
        }

        #endregion IHookReplicationLevelChanged implementation


        // --- API: -------------------------------------------------------------
        #region API

        public async Task<bool> RecalculateTraumasImpl()
        {
            if (((IWorldCharacter)parentEntity).IsIdle)
                return true;
            if (AllTraumas.DoNotWork)
                return true;

            using (var entityCnt = await parentEntity.GetThis())
            {
                var ctx = new CalcerContext(entityCnt, new OuterRef(parentEntity.Id, parentEntity.TypeId), EntitiesRepository);
                foreach (var trauma in AllTraumas.TraumaGivers)
                {
                    TraumaDef def = trauma.Value.Target;
                    bool isActive = ActiveTraumas.TryGetValue(trauma.Key, out var activeTraumaGiver);

                    switch (def)
                    {
                        case TraumaGiverDef giverDef:
                            if (isActive)
                            {
                                if (giverDef.TraumaPoints == 0)
                                {
                                    if (giverDef.EndPredicate.Target != null && await giverDef.EndPredicate.Target.CalcAsync(ctx))
                                        await StopTrauma(trauma.Key, activeTraumaGiver);
                                    else 
                                    if (giverDef.Predicate.Target != null && !await giverDef.Predicate.Target.CalcAsync(ctx))
                                        await StopTrauma(trauma.Key, activeTraumaGiver);
                                }
                                else if (giverDef.TraumaPoints < 0)
                                {
                                    await StopTrauma(trauma.Key, activeTraumaGiver);
                                }
                            }
                            else
                            {
                                if (giverDef.Predicate.Target != null && /*!def.TriggerOnly && */ await giverDef.Predicate.Target.CalcAsync(ctx))
                                    await StartTrauma(trauma.Key, new TraumaGiver() {Def = def});
                            }
                            break;

                        case SaveableTraumaDef traumaDef:
                            if (isActive)
                            {
                                if (traumaDef.TraumaPoints == 0)
                                {
                                    if (traumaDef.EndPredicate.Target != null && await traumaDef.EndPredicate.Target.CalcAsync(ctx))
                                        await StopTrauma(trauma.Key, activeTraumaGiver);
                                }
                                else if (traumaDef.TraumaPoints < 0)
                                {
                                    await StopTrauma(trauma.Key, activeTraumaGiver);
                                }
                            }
                            break;
                        
                        default: throw new NotImplementedException($"{def}");
                    }
                }
            }
            return true;
        }

        public async Task StartTraumaImpl(string traumaKey)
        {
            if (AllTraumas.DoNotWork)
                return;
            TraumaDef traumaDef = AllTraumas.TraumaGivers.FirstOrDefault(v => v.Key == traumaKey).Value;
            if (!(traumaDef is SaveableTraumaDef)) // травма сработает но не сохранится в базе
                Logger.IfError()?.Message(ParentEntityId, $"Implicit start of predicate based (not saveable) trauma {traumaDef}").Write();
            if (traumaDef != null)
                await StartTrauma(traumaKey, new TraumaGiver() { Def = traumaDef });
            else
                Logger.IfError()?.Message(ParentEntityId, $"Unknown trauma {traumaKey}").Write();
        }

        public async Task StopTraumaImpl(string traumaKey)
        {
            if (AllTraumas.DoNotWork)
                return;
            var traumaGiver = ActiveTraumas.FirstOrDefault(v => v.Key == traumaKey).Value;
            if (traumaGiver != null)
                await StopTrauma(traumaKey, traumaGiver);
            else
                Logger.IfWarn()?.Message(ParentEntityId, $"Trauma {traumaKey} is not active").Write();
        }

        public Task ChangeTraumaPointsImpl(string traumaKey, int delta)
        {
            if (!AllTraumas.DoNotWork)
            {
                if (ActiveTraumas.TryGetValue(traumaKey, out var trauma))
                    trauma.CurrentTraumaPoints += delta;
            }
            return Task.CompletedTask;
        }

        public async Task StartTraumaImpl(string traumaKey, ITraumaGiver traumaGiver)
        {
            if (AllTraumas.DoNotWork)
                return;
            Logger.IfDebug()?.Message(ParentEntityId, $"StartTrauma({traumaKey})").Write();
            if (!ActiveTraumas.ContainsKey(traumaKey))
            {
                if (_currentlyStartingTraumas.Add(traumaKey))
                    if (await traumaGiver.StartTrauma(parentEntity))
                    {
                        ActiveTraumas.Add(traumaKey, traumaGiver);
                        if (traumaGiver.Def is SaveableTraumaDef)
                            SaveableActiveTraumas.Add(traumaKey, traumaGiver);
                    }
                _currentlyStartingTraumas.Remove(traumaKey);
            }
        }

        public async Task StopTraumaImpl(string traumaKey, ITraumaGiver activeTraumaGiver)
        {
            if (AllTraumas.DoNotWork)
                return;
            Logger.IfDebug()?.Message(ParentEntityId, $"StopTrauma({traumaKey})").Write();
            await activeTraumaGiver.StopTrauma(parentEntity);
            await RemoveTraumaImpl(traumaKey, activeTraumaGiver);
        }

        public async Task SuspendTraumaImpl(string traumaKey, ITraumaGiver activeTraumaGiver)
        {
            if (AllTraumas.DoNotWork)
                return;
            switch(activeTraumaGiver.Def)
            {
                case TraumaGiverDef _:
                    Logger.IfDebug()?.Message(ParentEntityId, $"SuspendTrauma {activeTraumaGiver}").Write();
                    await activeTraumaGiver.StopTrauma(parentEntity);
                    await RemoveTraumaImpl(traumaKey, activeTraumaGiver);
                break;
                case SaveableTraumaDef _: // FIXME: баффы не приостанавливаются 
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public ValueTask<bool> HasActiveTraumasImpl(string[] traumas)
        {
            foreach (var trauma in traumas)
                if (ActiveTraumas.ContainsKey(trauma))
                    return new ValueTask<bool>(true);
            return new ValueTask<bool>(false);
        }

        #endregion API


        // --- Privates: ------------------------------------------------------
        #region Privates

        private Task Spells_OnItemRemoved(SharedCode.EntitySystem.Delta.DeltaDictionaryChangedEventArgs<SpellId, ISpell> args)
        {
            return RemoveTraumaBySpellId<TraumaGiverDef>(args.Value.Id);
        }

        private Task Buffs_OnItemRemoved(SharedCode.EntitySystem.Delta.DeltaDictionaryChangedEventArgs<SpellId, IBuff> args)
        {
            return RemoveTraumaBySpellId<SaveableTraumaDef>(args.Value.Id);
        }

        private async Task RemoveTraumaBySpellId<T>(SpellId spellId)
        {
            using (var entContainer = await parentEntity.GetThis())
            {
                var hasTraumas = entContainer?.Get<IHasTraumas>(parentEntity.TypeId, parentEntity.Id);
                if (hasTraumas == null)
                {
                    Logger.IfError()?.Message("Spells_OnItemRemoved hasTraumas == null typeId {0} id {1}", parentEntity.TypeId, parentEntity.Id).Write();
                    return;
                }
                foreach (var trauma in ActiveTraumas)
                    if (trauma.Value.Def is T && trauma.Value.SpellId == spellId.Counter)
                    {
                        await RemoveTrauma(trauma.Key, trauma.Value);
                        break;
                    }
            }
        }

        public ValueTask<bool> RemoveTraumaImpl(string traumaKey, ITraumaGiver traumaGiver)
        {
            Logger.IfDebug()?.Message(ParentEntityId, $"RemoveTrauma({traumaKey})").Write();
            bool rv = ActiveTraumas.Remove(traumaKey);
            if (traumaGiver.Def is SaveableTraumaDef)
                SaveableActiveTraumas.Remove(traumaKey);
            return new ValueTask<bool>(rv);
        }

        private async Task SubscribeToMortal()
        {
            var mortal = parentEntity as IHasMortal;
            if (mortal == null)
                return;

            mortal.Mortal.DieEvent += OnDie;
            mortal.Mortal.ResurrectEvent += OnResurrect;
            // manually call for case, if event is already fired:
            if (await mortal.Mortal.GetIsAlive())
            {
                var noMetterPos = PositionRotation.InvalidInstatnce;
                await OnResurrect(parentEntity.Id, parentEntity.TypeId, noMetterPos);
            }
        }

        private Task OnDie(Guid guid, int typeId, PositionRotation corpsePlace)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"HasTraumasImplementation::OnDie {typeId} / {guid}.").Write();
            return StopTraumas(true);
        }

        private Task OnResurrect(Guid guid, int typeId, PositionRotation posRot)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"HasTraumasImplementation::OnResurrect {typeId} / {guid}.").Write();
            return StartTraumas();
        }

        private Task StartTraumas()
        {
            if (_cts != null)
            {
                Logger.IfError()?.Message("Trauma recalculation cancellation token for obj {0} with id {1} is not null", ReplicaTypeRegistry.GetTypeById(parentEntity.TypeId), parentEntity.Id).Write();
                return Task.CompletedTask;
            }

            _cts = new CancellationTokenSource();
            StatsEngine.RunChainLight(parentEntity.TypeId, parentEntity.Id, RecalculateTraumas, TimeSpan.FromSeconds(AllTraumas.MeanTimeForTraumasToHappen), EntitiesRepository, _cts.Token);
            return Task.CompletedTask;
        }

        private async Task StopTraumas(bool onDeath)
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts = null;
            }
            else
                Logger.IfError()?.Message("Trauma recalculation cancellation token for obj {0} with id {1} is already null", ReplicaTypeRegistry.GetTypeById(parentEntity.TypeId), parentEntity.Id).Write();

            var activeTraumas = ActiveTraumas.ToArray();
            if (activeTraumas.Length != 0)
                foreach (var trauma in activeTraumas)
                    if (!onDeath || !trauma.Value.Def.IgnoresDeath)
                        await StopTrauma(trauma.Key, trauma.Value);
        }
        
        private async Task SuspendTraumas()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts = null;
            }
            else
                Logger.IfError()?.Message("Trauma recalculation cancellation token for obj {0} with id {1} is already null", ReplicaTypeRegistry.GetTypeById(parentEntity.TypeId), parentEntity.Id).Write();

            var activeTraumas = ActiveTraumas.ToArray();
            if (activeTraumas.Length != 0)
                foreach (var trauma in activeTraumas)
                    await SuspendTrauma(trauma.Key, trauma.Value);
        }
        
        #endregion Privates

    }
}
