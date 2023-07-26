using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.ColonyShared.SharedCode.Utils;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.Entities.Engine;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Quests;
using SharedCode.Serializers;
using SharedCode.Wizardry;
using Src.Aspects.Impl.Stats;
using Assets.Src.Aspects.Impl.Stats;
using System.Linq;
using ColonyShared.SharedCode.Entities;
using Assets.Src.ResourcesSystem.Base;
using GeneratedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using Assets.ColonyShared.GeneratedCode.Shared.Utils;
using Assets.ResourceSystem.Account;
using Assets.Src.Arithmetic;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.DeltaObjects
{
    public partial class Mortal : IMortalImplementRemoteMethods, IHookOnInit, IHookOnDatabaseLoad, IHookOnDestroy
    {
        // ReSharper disable once UnusedMember.Local
        private static readonly NLog.Logger Logger = LogManager.GetLogger("MortalObject");

        // Game-designer mistake fuse. If it == false (by dflt), death 'll be triggered by timeout after `OnZeroHealthSpell` casted.
        // #NOTE: It's just fuse for safety - don't rely on it - call Die yourself from `OnZeroHealthSpell`
        private bool _disablePreDeathHandlerAutoDeathByTimeout;
        private PreDeathHandler _preDeathHandler;


        // --- Implements: ----------------------------------------------------------

        public Task AddStrikeImpl(OuterRef<IEntity> objectRef)
        {
            if (LastStrike.TryGetValue(objectRef, out _))
                LastStrike[objectRef] = SyncTime.Now;
            else
                LastStrike.Add(objectRef, SyncTime.Now);
            return Task.CompletedTask;
        }

        private async Task PostStatisticsEventKill()
        {
            if (LastStrike.Count == 0)
                return;

            var repo = EntitiesRepository;
            IEntityObjectDef def;
            using (var entitiesContainer = await repo.Get(parentEntity.TypeId, parentEntity.Id))
            {
                var hasBruteServer = entitiesContainer.Get<IHasBruteServer>(parentEntity.TypeId, parentEntity.Id, ReplicationLevel.Server);
                if (hasBruteServer == null || !(hasBruteServer.GetBaseDeltaObject() is IEntityObject victimEObj) || victimEObj == null)
                {
                    Logger.IfError()?.Message(ParentEntityId, "Victim {2} {0} is not a {1}", hasBruteServer?.TypeName ?? "<null>", nameof(IEntityObject), parentEntity?.ToString() ?? "<null>").Write();
                    LastStrike.Clear();
                    return;
                }
                def = victimEObj.Def;
            }

            foreach (var pair in LastStrike)
                if (SyncTime.Now - pair.Value < Constants.WorldConstants.StrikeTimeout)
                {
                    using (var agressorContainer = await repo.Get(pair.Key))
                    {
                        var aggressorWithStatistics = agressorContainer.Get<IHasStatisticsServer>(pair.Key, ReplicationLevel.Server);
                        if (aggressorWithStatistics != null)
                            await aggressorWithStatistics.StatisticEngine.PostStatisticsEvent(new KillMortalObjectEventArgs() { MortalObjectDef = def });
                    }
                    break;
                }

            LastStrike.Clear();
        }

        public Task<bool> GetIsAliveImpl() => Task.FromResult(IsAlive);

        public async Task<bool> ZeroHealthReachedImpl()
        {
             Logger.IfDebug()?.Message(" 1. ZeroHealthReached").Write();;
            try
            {
                if (_preDeathHandler != null && await _preDeathHandler.HandleZeroHealthReached())
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.IfError()?.Exception(e).Write();
            }

            return await Die();
        }

        public async Task<bool> KnockDownImpl()
        {
            if (IsKnockedDown)
                return false;
            var selfDef = ((IMortalObjectDef) ((IEntityObject) parentEntity).Def); 
            var spell = selfDef.KnockDownSpell;
            if (spell == null)
                throw new NullReferenceException($"No knockdown spell defined in {(selfDef as BaseResource)?.____GetDebugAddress()}");
             Logger.IfDebug()?.Message(ParentEntityId, "KnockDown").Write();;
            IsKnockedDown = true;
            await OnKnockedDown();
            using (var wizardW = await EntitiesRepository.Get<IWizardEntity>(parentEntity.Id))
            {
                var wizard = wizardW.Get<IWizardEntity>(parentEntity.Id);
                wizard.Spells.OnItemRemoved -= OnSpellFinished;
                var oldSpellId = KnockDownSpellId;
                KnockDownSpellId = SpellId.Invalid;
                if (oldSpellId.IsValid)
                    await wizard.StopCastSpell(oldSpellId, SpellFinishReason.FailOnDemand);
                wizard.Spells.OnItemRemoved += OnSpellFinished;
                KnockDownSpellId = await wizard.CastSpell(new SpellCast {Def = spell});
                if (!KnockDownSpellId.IsValid)
                    wizard.Spells.OnItemRemoved -= OnSpellFinished;
                return KnockDownSpellId.IsValid;
            }
        }

        private async Task OnSpellFinished(DeltaDictionaryChangedEventArgs<SpellId, ISpell> eventArgs)
        {
            var removedSpell = eventArgs.Value;
            using (await parentEntity.GetThis())
                await HandleKnockdownSpellFinished(removedSpell.Id, removedSpell.FinishReason);
        }


        public async Task HandleKnockdownSpellFinishedImpl(SpellId spellId, SpellFinishReason finishReason)
        {
            if (KnockDownSpellId == spellId)
            {
                KnockDownSpellId = SpellId.Invalid;
                using (var wizardW = await EntitiesRepository.Get<IWizardEntity>(parentEntity.Id))
                {
                    var wizard = wizardW.Get<IWizardEntity>(parentEntity.Id);
                    wizard.Spells.OnItemRemoved -= OnSpellFinished;
                }
                await ZeroHealthReached();
            }
        }

        public async Task<bool> ReviveImpl()
        {
            if (IsKnockedDown)
            {
                IsKnockedDown = false;
                await StopKnockDownSpell();
                await OnReviveFromKnockdown();
                //var pos = (await GetCorpsePlaceAndRepayDelta());
                //return await Resurrect(new PositionRotation(pos.Position, pos.Rotation));
                return true;
            }
            return false;
        }
        
        async Task StopKnockDownSpell()
        {
            using (var wizardW = await EntitiesRepository.Get<IWizardEntity>(parentEntity.Id))
            {
                var wizard = wizardW.Get<IWizardEntity>(parentEntity.Id);
                wizard.Spells.OnItemRemoved -= OnSpellFinished;
                var spellId = KnockDownSpellId;
                KnockDownSpellId = SpellId.Invalid;
                await wizard.StopCastSpell(spellId, SpellFinishReason.FailOnDemand);
            }

        }
        
        public async Task<bool> FinishOffImpl()
        {
            Logger.IfDebug()?.Message(ParentEntityId, $"FinishOff IsKnockedDown:{IsKnockedDown} IsDying:{_preDeathHandler?.IsDying}").Write();
            if (IsKnockedDown && !(_preDeathHandler?.IsDying ?? false))
                await ZeroHealthReached();
            return false;
        }
        
        public async Task<bool> DieImpl()
        {

            Logger.IfDebug()?.Message(ParentEntityId, $"Die(1). IsAlive == {IsAlive} (`True` is expected most likely)").Write();

            if (!IsAlive)
            {
                Logger.IfDebug()?.Message(ParentEntityId, $"Die. IsAlive == {IsAlive} (should be True)  [ ?But may be it's intended ]").Write();
                return false;
            }
            LastDeathTime = SyncTime.NowUnsynced;
            IsAlive = false;
            if (IsKnockedDown)
            {
                IsKnockedDown = false;
                await StopKnockDownSpell();
            }

            Transform corpsePlace = await GetCorpsePlaceAndRepayDelta();

            if (parentEntity is IHasFaction hf)
            {
                var permadeath = hf.Faction?.Permadeath.Target;
                if (permadeath != null)
                {
                    var shouldPermadie = await permadeath.CalcAsync(new OuterRef<IEntity>(parentEntity), EntitiesRepository);
                    if (shouldPermadie)
                        PermaDead = true;
                }
            }
            await InvokeDieEvent((PositionRotation)corpsePlace);
            await PostStatisticsEventKill();
                
            
            if (PermaDead)
                if (parentEntity is IWorldCharacter wc)
                {
                    var accId = wc.AccountId;
                    _ = AsyncUtils.RunAsyncTask(async () =>
                    {
                        using (var w = await EntitiesRepository.GetFirstService<ILoginServiceEntityServer>())
                        {
                            var loginS = w.GetFirstService<ILoginServiceEntityServer>();
                            await loginS.GiveUpRealmOnDeath(accId);
                        }
                    });

                }
            return true;
        }

        // (aK): I know, it's not good name, but I don't see better solution
        private async Task<Transform> GetCorpsePlaceAndRepayDelta()
        {
            var positioned = PositionedObjectHelper.GetPositioned(parentEntity);
            if (positioned == null)
            {
                Logger.IfWarn()?.Message(ParentEntityId, $"{nameof(GetCorpsePlaceAndRepayDelta)} was called, but entity isn't {nameof(IPositionedObject)}. ({parentEntity.Id}, {parentEntity.TypeId})").Write();
                return Transform.Identity;
            }

#if false // Бесполезно, пока нет замены GetDropCorpsePosition
            var currentPlace = positioned.Transform;
            Transform corpsePlace;
            var repo = EntitiesRepository;
            using (var wrapper = await repo.GetMasterService<IWorldSpaceServiceEntity>())
            {
                var worldSpaceServiceEntity = wrapper.GetMasterService<IWorldSpaceServiceEntity>();
                //#Note: When we'll be able again get access from worldSpace to physics (Unity or may be our custom already), we should place corpse correctly (not in air):
                // var pos = await worldSpaceServiceEntity.UnityWorldSpace.GetDropCorpsePosition(currentPlace.Position);
                var pos = currentPlace.Position;
                corpsePlace = new Transform(pos, currentPlace.Rotation);
            }

            if ((currentPlace.Position - corpsePlace.Position).sqrMagnitude > 0.01f)
            {
                var positionable = PositionedObjectHelper.GetPositionable(parentEntity);
                if (positionable != null)
                {
                    positionable.SetPosition = corpsePlace.Position;
                }
            }

            return corpsePlace;
#else
            return positioned.Transform;
#endif
        }

        public async Task<bool> ResurrectImpl(PositionRotation at)
        {
            if (PermaDead)
            {
                Logger.IfDebug()
                    ?.Message(ParentEntityId, "PermaDead")
                    .Write();
                return false;
            }

            if (IsKnockedDown)
            {
                await Die();
            }
            Logger.IfDebug()?.Message(ParentEntityId, $"Resurrect(1). IsAlive == {IsAlive} (`False` is most expected)").Write();

            if (IsAlive)
            {
                Logger.IfDebug()?.Message(ParentEntityId, $"Resurrect. IsAlive == {IsAlive} (should be False)  [ But most likely it's intended (Ok) ]").Write();
                return false;
            }

            if (_preDeathHandler != null)
                if (!await _preDeathHandler.DeactivatePreDeathState())
                {
                    Logger.IfError()?.Message(ParentEntityId, $"Error on `{nameof(_preDeathHandler)}.{nameof(_preDeathHandler.DeactivatePreDeathState)}`.").Write();
                    return false;
                }

            if (at.IsValid)
            {
                var positionable = PositionedObjectHelper.GetPositionable(parentEntity);
                if (positionable != null)
                {
                    positionable.SetTransform = new Transform(at.Position, at.Rotation);
                }
            }
            IsKnockedDown = false;
            IsAlive = true;
            LastResurrectTime = SyncTime.Now;
            await InvokeResurrectEvent(at);

            var enttyObj = parentEntity as IEntityObject;
            var mortalDef = enttyObj?.Def as IMortalObjectDef;

            await SpellCastHelpers.CastSpells(parentEntity, mortalDef?.SpellsOnResurrect);

            if (parentEntity is IHasAccountStats hasAccStats) // parentEntty is already locked here
            {
                var exp = hasAccStats.AccountStats.AccountExperience;
                int lvl = LevelUpDatasHelpers.CalcAccLevel(exp);
                var spellsToCast = GlobalConstsHolder.GlobalConstsDef.LevelUpDatas.Target.GetAllAchievementSpellsToCastByLvl(lvl, LevelUpDatasDef.SpellsGroup.OnResurrect);
                if (spellsToCast != null)
                    await SpellCastHelpers.CastSpells(parentEntity, spellsToCast.ToArray());
            }

            // Spells on Resurrect
            return true;
        }

        public async Task<bool> InvokeDieEventImpl(PositionRotation corpsePlace)
        {
            await OnDieEvent(parentEntity.Id, parentEntity.TypeId, corpsePlace);
            return true;
        }

        public async Task<bool> InvokeResurrectEventImpl(PositionRotation at)
        {
            await OnBeforeResurrectEvent(parentEntity.Id, parentEntity.TypeId);
            await OnResurrectEvent(parentEntity.Id, parentEntity.TypeId, at);
            return true;
        }


        public async Task<bool> DeactivatePreDeathStateImpl()
        {
            if (_preDeathHandler == null)
                return true;

            return await _preDeathHandler.DeactivatePreDeathState();
        }

        // --- IHookReplicationLevelChanged implementation: ----------------------------------------------------------

        public async Task OnInit()
        {
            Initialization();
            var posdObj = PositionedObjectHelper.GetPositioned(parentEntity);
            var at = PositionRotation.InvalidInstatnce;
            if (posdObj != null)
                at = new PositionRotation(posdObj.Position, posdObj.Rotation);
            var typeId = parentEntity.TypeId;
            var eid = parentEntity.Id;
            var repo = EntitiesRepository;
            //Борис хотел таск, но кто-то, по видимому, завязывается на порядок исполнения, а у меня сейчас нет времени и возможности разбираться
            //Поэтому пока тут будет так, как было раньше, но без вызова Resurrect в OnDatabaseLoad
            await Resurrect(at);
            /*AsyncUtils.RunAsyncTask(async () => {
                using (var self = await repo.Get(typeId, eid))
                {
                    await self.Get<IMortalObjectWithImplementationServer>(typeId, eid, ReplicationLevel.Server).Resurrect(at);
                }
            }, _entity.EntitiesRepository);
            return Task.CompletedTask;*/
        }

        public Task OnDatabaseLoad()
        {
            Initialization();
            return Task.CompletedTask;
        }
        
        public async Task OnDestroy()
        {
            if (KnockDownSpellId.IsValid)
                await StopKnockDownSpell();
        }

        private void Initialization()
        {
            // 1. Init from def:
            var enttyObj = parentEntity as IEntityObject;
            var mortalDef = enttyObj?.Def as IMortalObjectDef;
            if (mortalDef != null)
            {
                _disablePreDeathHandlerAutoDeathByTimeout = mortalDef.DisablePreDeathHandlerAutoDeathByTimeout;
                // 2. InitPreDeathHandler:
                _preDeathHandler = new PreDeathHandler(parentEntity as IHasMortal, mortalDef, ParentEntityId);
            }

        }

        // --- Privates: ----------------------------------------------------------

        public void StartAutodeathDelayed(float delay)
        {
            var repo = EntitiesRepository;
            AsyncUtils.RunAsyncTask(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(delay));
                // Handling case of entty was teleported while delay:
                if (!((IEntityExt)parentEntity).IsMaster())
                    return;
                try
                {
                    using (var wrapper = await ((IEntitiesRepositoryExtension)repo).GetExclusive(parentEntity.TypeId, parentEntity.Id, nameof(PreDeathHandler)))
                    {
                        await Die();
                        Logger.IfDebug()?.Message(ParentEntityId, $"Z. StartAutodeathDelayed started.").Write();
                    }
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                }

            }, repo);
        }

        public ValueTask<MortalState> GetStateImpl()
        {
            if (IsKnockedDown) return new ValueTask<MortalState>(MortalState.KnockedDown);
            if (IsAlive) return new ValueTask<MortalState>(MortalState.Alive);
            return new ValueTask<MortalState>(MortalState.Dead);
        }
    }

    // #TODO(aK): Consider move this logic to a separate interface (later, when we'll meet more usecases)
    internal class PreDeathHandler
    {
        // ReSharper disable once UnusedMember.Local
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly SpellDef _onZeroHealthSpell;
        private readonly IHasMortal _ownerMortalEntity;
        private readonly IEntity _ownerEntity;
        private readonly bool _isHpRegenAllowedInPreDeathState;
        //private bool _isEnabled;
        private SpellId _lastOnZeroHealthSpellId;
        private IMortalObjectDef _selfDef;
        private readonly Guid _entityId;
        private bool _isDying;

        internal bool IsDying { get => _isDying; private set { Logger.IfDebug()?.Message(_entityId, $"Set IsDying to {value}").Write(); _isDying = value; } }
        internal bool IsPreDeathStateActive { get; private set; }
        public float OnZeroHealthSpellDuration { get; private set; } = 1f;
        // --- Ctor: ------------------------------------------------------------

        internal PreDeathHandler([NotNull] IHasMortal mortalEntity, [NotNull] IMortalObjectDef mortalDef, Guid entityId)
        {
            Logger.IfDebug()?.Message(_entityId, $"0. PreDeathHandler ctor: //mortalDef.OnZeroHealthSpell.IsValid?:{mortalDef.OnZeroHealthSpell.IsValid}").Write();
            _ownerMortalEntity = mortalEntity;
            _ownerEntity = (IEntity)mortalEntity;

            _onZeroHealthSpell = mortalDef.OnZeroHealthSpell;
            OnZeroHealthSpellDuration = _onZeroHealthSpell?.Duration ?? 1f;
            _isHpRegenAllowedInPreDeathState = mortalDef.IsHpRegenAllowedInPreDeathState;
            _selfDef = mortalDef;
            _entityId = entityId;
        }

        // --- API: -------------------------------------------------------------

        internal async Task<bool> HandleZeroHealthReached()
        {
            if (_selfDef.KnockedDownOnZeroHealth && !_ownerMortalEntity.Mortal.IsKnockedDown)
            {
                if (await _ownerMortalEntity.Mortal.KnockDown())
                    return true;
            }
            IsDying = true;
            return await TrySetIsPreDeathStateActive(true);
        }

        internal async Task<bool> DeactivatePreDeathState()
        {
            IsDying = false;
            return await TrySetIsPreDeathStateActive(false);
        }

        // --- Privates: --------------------------------------------------------
        
        internal async Task<bool> TrySetIsPreDeathStateActive(bool newVal)
        {
            Logger.IfDebug()?.Message(_entityId, $"2. TrySetIsPreDeathStateActive({newVal}) //currVal:{IsPreDeathStateActive}").Write();
            // If needed state is already active, just continue staying in it.
            if (newVal == IsPreDeathStateActive)
                return true;
            return (newVal)
                ? await TryActivatePreDeathState()
                : await TryDeactivatePreDeathState();
        }

        private async Task<bool> TryActivatePreDeathState()
        {
            Logger.IfDebug()?.Message(_entityId, $"3. TryActivatePreDeathState").Write();
            if (!await TryCastOnZeroHealthSpell())
                return false;

            if (!_isHpRegenAllowedInPreDeathState)
                await SwitchHpRegen(false);

            IsPreDeathStateActive = true;
            if (!_selfDef.DisablePreDeathHandlerAutoDeathByTimeout)
                if (_ownerMortalEntity.Mortal is Mortal mortal)
                    mortal.StartAutodeathDelayed(OnZeroHealthSpellDuration);
                else
                    Logger.IfError()?.Message("Owner has not a Mortal | {@}", new { OwenrType = _ownerMortalEntity?.GetType(), MortalType = _ownerMortalEntity?.Mortal?.GetType() }).Write();
            return true;
        }

        private async Task<bool> TryDeactivatePreDeathState()
        {
            Logger.IfDebug()?.Message(_entityId, $"TryDEactivatePreDeathState").Write();
            if (!await TryFinishLastOnZeroHealthSpell())
                return false;

            if (!_isHpRegenAllowedInPreDeathState)
                await SwitchHpRegen(true);

            IsPreDeathStateActive = false;
            return true;
        }

        private async Task<bool> TryCastOnZeroHealthSpell()
        {
            _lastOnZeroHealthSpellId = await SpellCastHelpers.CastSpell(_ownerEntity, _onZeroHealthSpell);
            return _lastOnZeroHealthSpellId.IsValid;
        }

        private ValueTask<bool> TryFinishLastOnZeroHealthSpell() => SpellCastHelpers.StopSpell(_ownerEntity, _lastOnZeroHealthSpellId);

        // Adds/removes clampMax
        private async Task<bool> SwitchHpRegen(bool enable)
        {
            Logger.IfDebug()?.Message(_entityId, $"5. HpRegenOnOff({enable})").Write();

            var hasStats = _ownerEntity as IHasStatsEngine;
            if (hasStats == null)
                return false;

            ModifierCauser causer = new ModifierCauser() { Causer = GlobalConstsHolder.GlobalConstsDef.PreDeathHandlerKey, SpellId = 0 };

            return enable
                ? await hasStats.Stats.RemoveModifiers(new[]{ new StatModifierInfo(GlobalConstsHolder.StatResources.HealthCurrentRegenStat, StatModifierType.ClampMax) }, causer)
                : await hasStats.Stats.SetModifiers(new[]{ new StatModifierData(GlobalConstsHolder.StatResources.HealthCurrentRegenStat, StatModifierType.ClampMax, 0) }, causer);
        }
    }
}
