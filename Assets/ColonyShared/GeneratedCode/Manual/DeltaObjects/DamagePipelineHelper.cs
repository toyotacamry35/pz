using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.Tools;
using ColonyShared.GeneratedCode.Shared.Aspects;
using ColonyShared.SharedCode.Aspects.Combat;
using ColonyShared.SharedCode.Aspects.Misc;
using ColonyShared.SharedCode.Entities.Reactions;
using ResourceSystem.Utils;
using SharedCode.Aspects.Item.Templates;
using SharedCode.DeltaObjects.Building;
using SharedCode.Entities.Building;
using SharedVector2 = SharedCode.Utils.Vector2;
using SharedVector3 = SharedCode.Utils.Vector3;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Serializers;
using SharedCode.Utils.DebugCollector;
using SharedCode.Utils.Extensions;
using SharedCode.Aspects.Building;
using Assets.Src.Tools;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.Src.Arithmetic;
using ColonyShared.SharedCode;
using Assets.Src.Aspects.Impl.Factions.Template;
using ColonyShared.SharedCode.Modifiers;
using ColonyShared.GeneratedCode.Manual.DeltaObjects;
using ColonyShared.SharedCode.Reactions;
using ColonyShared.GeneratedCode.Manual.DeltaObjects;
using ColonyShared.SharedCode.Entities;
using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using ResourceSystem.Aspects;
using SharedCode.Repositories;
using SharedCode.Utils;
using SharedCode.Wizardry;

namespace Assets.Src.Aspects
{
    public static class DamagePipelineHelper
    {
        // ReSharper disable once UnusedMember.Local 
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger("Combat");

        public static DamageCalculationRoot DamageCalculationRoot => DamageCalculationRoot.Instance;

        public static DamageTypeDef GetCurrentWeaponDamageType(
            [CanBeNull] IItemsStatsAccumulatorClientFull itemsStatAccumulator,
            [CanBeNull] IHasSpecificStatsClientFull brute, 
            [CanBeNull] ItemSpecificStats overriddenWeaponStats = null)
        {
            if (overriddenWeaponStats != null)
                return overriddenWeaponStats.DamageType.Target;
            bool hasWeapon = itemsStatAccumulator?.HasActiveWeapon ?? false;
            return (hasWeapon ? itemsStatAccumulator.ActiveWeaponDamageType : (brute != null ? brute.SpecificStats : null)?.DamageType.Target) ?? GlobalConstsHolder.GlobalConstsDef.DefaultDamageType;
        }

        // ReSharper disable once FunctionComplexityOverflow
        public static async Task ExecuteStrike(
            OuterRef aggressorRef,
            AttackTargetInfo victimInfo,
            [NotNull] IEntitiesRepository repository,
            [NotNull] IAttackDescriptor attackDesc,
            [NotNull] IReadOnlyList<AttackModifierDef> attackModifiers,
            long attackTimestamp
            )
        {
            //DbgLog.Log(13334, "0. ExecuteStrike");

            SharedCode.Logging.Log.AttackStopwatch.Milestone("Damage");

            var victimRef = victimInfo.Target;
            var victimSubRef = victimInfo.SubTarget;

            
            Logger.IfDebug()?.Message($"ExecuteStrike | Aggressor:{aggressorRef} Victim:{victimRef} VictimSubRef:{victimSubRef} Timestamp:{attackTimestamp} AttackType:{attackDesc.AttackType}" +
                                             $" Modifiers:{attackModifiers.ToStringExt()}  " +
                                             (attackDesc.ActionsOnAttacker != null ? $"OnAggressor:[{string.Join(", ", attackDesc.ActionsOnAttacker)}]  " : String.Empty) +
                                             (attackDesc.ActionsOnVictim != null ? $"OnVictim:[{string.Join(", ", attackDesc.ActionsOnVictim)}]  " : String.Empty))
                .Write();

            Collect.IfActive?.Event("DamagePipelineHelper.ExecuteStrike", aggressorRef);

            if (!aggressorRef.IsValid || !victimRef.IsValid)
                return;

            using (var entitiesContainer = await repository.Get(EntityBatch.Create().Add(aggressorRef.TypeId, aggressorRef.Guid).Add(victimRef.TypeId, victimRef.Guid)))
            {
                //DbgLog.Log(13334, "1. ExecuteStrike");

                if (entitiesContainer == null)
                    return;

                //DbgLog.Log(13334, "2. ExecuteStrike");

                var aggressorStats = entitiesContainer.Get<IHasStatsEngineServer>(aggressorRef, ReplicationLevel.Server);
                var aggressorSpecificStats = entitiesContainer.Get<IHasSpecificStatsServer>(aggressorRef, ReplicationLevel.Server);
                var aggressorBrute = entitiesContainer.Get<IHasBruteServer>(aggressorRef, ReplicationLevel.Server)?.Brute;
                var aggressorWorldObject = PositionedObjectHelper.GetPositionedMaster(entitiesContainer, aggressorRef.TypeId, aggressorRef.Guid);

                var victimEntity = entitiesContainer.Get<IEntity>(victimRef, ReplicationLevel.Server);
                var victimStats = entitiesContainer.Get<IHasStatsEngineServer>(victimRef, ReplicationLevel.Server);
                var victimSpecificStats = entitiesContainer.Get<IHasSpecificStatsServer>(victimRef, ReplicationLevel.Server);
                var victimBrute = entitiesContainer.Get<IHasBruteServer>(victimRef, ReplicationLevel.Server)?.Brute;
                var victimDef = entitiesContainer.Get<IEntityObjectServer>(victimRef, ReplicationLevel.Server)?.Def;
                var victimWorldObject = PositionedObjectHelper.GetPositionedMaster(entitiesContainer, victimRef.TypeId, victimRef.Guid);
                var victimHealthOwner = entitiesContainer.Get<IHasHealthServer>(victimRef, ReplicationLevel.Server);
                var victimHitZonesOwner = entitiesContainer.Get<IHitZonesOwnerServer>(victimRef, ReplicationLevel.Server);
                entitiesContainer.TryGet<IHasIncomingDamageMultiplier>(victimRef.TypeId, victimRef.Guid, ReplicationLevel.Master, out var victimHasIncomingDamageMultiplier);

                //DbgLog.Log(13334, "3. ExecuteStrike");
#if false                
                {///#Dbg:
                    var msg = new StringBuilder("---------1-0---------- who is null:");
                  //  if (aggressorWithStatistics       == null) msg.Append($"\n{nameof(aggressorWithStatistics)} is NULL");
                    if (aggressorBrute                == null) msg.Append($"\n{nameof(aggressorBrute)} is NULL");
                    if (aggressorWorldObject          == null) msg.Append($"\n{nameof(aggressorWorldObject)} is NULL");
                  //  if (aggressorHasWizard            == null) msg.Append($"\n{nameof(aggressorHasWizard)} is NULL");
                    if (aggressorItemsStatAccumulator == null) msg.Append($"\n{nameof(aggressorItemsStatAccumulator)} is NULL");

                    if (victimBrute         == null) msg.Append($"\n{nameof(victimBrute)} is NULL");
                    if (victimHealthOwner   == null) msg.Append($"\n{nameof(victimHealthOwner)} is NULL");
                    if (victimHitZonesOwner == null) msg.Append($"\n{nameof(victimHitZonesOwner)} is NULL");
                    if (victimWorldObject   == null) msg.Append($"\n{nameof(victimWorldObject)} is NULL");
                    //if (victimHasWizard     == null) msg.Append($"\n{nameof(victimHasWizard)} is NULL");
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(msg.ToString()).Write();
                }
#endif

                if (aggressorBrute == null)
                {
                    Logger.IfDebug()
                        ?.Message(aggressorRef.Guid, $"Attacker has not {nameof(IHasBruteServer)}")
                        .Write();
                    return;
                }

                //DbgLog.Log(13334, "4. ExecuteStrike");

                float incomingDamageMultiplier = 1;
                
                if (victimHasIncomingDamageMultiplier != null)
                {
                    var calcer = await victimHasIncomingDamageMultiplier.GetIncomingDamageMultiplier();
                    if (calcer != null)
                    {
                        var relationshipContext = new Relationship.Context(thisEntity: victimInfo.Target, otherEntity: aggressorRef);
                        var argsBuffer = new List<ArgTuple>();
                        Relationship.MapRelationshipArgs(Constants.RelationshipConstants.ArgsMapping, relationshipContext, argsBuffer);
                        var calcerContext = new CalcerContext(entitiesContainer, relationshipContext.ThisEntity, repository, null, argsBuffer.ToCalcerArgs());
                        incomingDamageMultiplier = await calcer.CalcAsync(calcerContext);
                    }
                }
                
                if (victimStats != null) // имунность от стата. используется для отключения урона в хабе
                {
                    incomingDamageMultiplier *= 1f + (await victimStats.Stats.TryGetValue(DamageCalculationRoot.IncomingDamageMultiplierStat)).Item2;
                }

                if (incomingDamageMultiplier <= 0)
                {
                    Logger.IfDebug()
                        ?.Message(aggressorRef.Guid, $"Incoming Damage Multiplier is ZERO | Victim:{victimRef}")
                        .Write();
                    return;
                }

                var (hasWeapon, damageType, weaponSize) = GetWeaponStats(entitiesContainer, aggressorRef);
                float destructionPowerRequired = (victimDef as IBruteDef)?.DestructionPowerRequired ?? 0;
                var destructionPower = !hasWeapon ? Constants.AttackConstants.DefaultDestructionPower : (aggressorBrute == null ? Constants.AttackConstants.DefaultDestructionPowerWeapons : (aggressorSpecificStats.SpecificStats).DestructionPower == -1 ? Constants.AttackConstants.DefaultDestructionPowerWeapons : (aggressorSpecificStats.SpecificStats).DestructionPower);
                if (destructionPower < destructionPowerRequired)
                {
                    Logger.IfDebug()
                        ?.Message(aggressorRef.Guid, $"Insufficiently destruction power | Power:{destructionPower} Required:{destructionPowerRequired} Victim:{victimRef}")
                        .Write();
                    return;
                }

                var miningLootMultiplier = await GetStatVal(DamageCalculationRoot.MiningLootMultiplierStat, aggressorStats, aggressorSpecificStats, attackModifiers);
                var attackType = attackDesc.AttackType ?? GlobalConstsHolder.GlobalConstsDef.DefaultAttackType;
                var aggressorTransform = aggressorWorldObject.History.GetTransformAt(attackTimestamp);
                var victimTransform = victimWorldObject.History.GetTransformAt(attackTimestamp);
                var directionCoefficient = await CalcDirectionCoefficient(aggressorTransform, victimTransform, victimBrute);
                bool blocked = await CheckBlockActive(victimStats);
                // if (victimBrute != null && await CheckBlockActive(victimStats))
                // {
                //     ItemSpecificStats baseDefendStats = (victimWeapon?.ItemResource as IHasStatsResource)?.SpecificStats.Target ?? victimSpecificStats.SpecificStats;
                //     blocked = CheckBlockSector(baseDefendStats, aggressorTransform.Position, victimTransform.Position, victimTransform.Forward);
                //     if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("------------------- Block is Active. blocked = {0}", blocked).Write();
                // }
				var staggerUndRecoil = await CalcStaggerAndRecoil(aggressorStats, aggressorSpecificStats, victimStats, victimSpecificStats, attackModifiers, blocked);
				var battleDamage = await CalcDamage(
                    aggressorStats: aggressorStats,
                    aggressorSpecificStats: aggressorSpecificStats,
                    damageStat: DamageCalculationRoot.DamageStat,
                    damageMultiplierStat: DamageCalculationRoot.DamageMultiplierStat,
                    damageModifierStat: DamageCalculationRoot.OutgoingDamageMultiplierStat,
                    modifiers: attackModifiers,
                    directionCoefficient: directionCoefficient);
                battleDamage = await ApplyResistance(battleDamage, damageType, aggressorStats, aggressorSpecificStats, victimStats, attackModifiers);
                battleDamage *= staggerUndRecoil.DamageMultiplier;
                battleDamage *= incomingDamageMultiplier;
 
                Logger.IfDebug()
                    ?.Message(aggressorRef.Guid, $"CalcDamage | HasWeapon:{hasWeapon} DamageType:{damageType.DisplayName} BattleDamage:{battleDamage} IncomingDamageMultiplier:{incomingDamageMultiplier} StaggerOrRecoilDamageMultiplier:{staggerUndRecoil.DamageMultiplier}")
                    .Write();

                var hitDirection = (victimTransform.Position - aggressorTransform.Position).normalized;
                var reactionsContext = new ActionContext
                (
                    attacker: aggressorRef,
                    victimInfo: victimInfo,
                    staggerTime: staggerUndRecoil.StaggerTime,
                    recoilTime: staggerUndRecoil.RecoilTime,
                    damage: battleDamage,
                    hitDirection: new SharedVector2(SharedVector3.Dot(victimTransform.Forward, hitDirection), SharedVector3.Dot(victimTransform.Right, hitDirection)).normalized,
                    damageType: damageType,
                    weaponSize: weaponSize,
                    attackType: attackType,
                    hitMaterial: GetHitMaterial(victimEntity),
                    block: blocked
                );

                using (var reactions = attackModifiers.ApplyAttackReactionsModifier(attackDesc.ActionsOnVictim, AttackActionTarget.Victim))
                    InvokeActions(victimRef, reactions.Collection, reactionsContext, repository);
                
                using (var reactions = attackModifiers.ApplyAttackReactionsModifier(attackDesc.ActionsOnAttacker, AttackActionTarget.Attacker))
                    InvokeActions(aggressorRef, reactions.Collection, reactionsContext, repository);

                var damageInfo = new Damage(
                    aggressor: aggressorRef,
                    battleDamage: battleDamage,
                    damageType: damageType,
                    isMiningDamage: attackDesc.IsMiningDamage,
                    miningLootMultiplier: miningLootMultiplier,
                    aggressionPoint: await aggressorBrute.GetAggressionPoint()
                    );

                //DbgLog.Log(13334, "5. ExecuteStrike: isM:" + damageInfo.IsMiningDamage);

                Logger.IfDebug()
                    ?.Message(aggressorRef.Guid, "------------------- damage info: {0}", damageInfo)
                    .Write();

                if (victimHitZonesOwner != null)
                    await victimHitZonesOwner.InvokeHitZonesDamageReceivedEvent(damageInfo);

                var victimBuild = entitiesContainer.Get(victimRef.To<IHasBuildPlaceServer>(), ReplicationLevel.Server);
                if (victimBuild != null)
                { // костыль для нанесения урона по зданиям, которые не имеют HealthEngine
                    Logger.IfDebug()
                        ?.Message(aggressorRef.Guid, "------------------- victimBuild.Operate({0})", damageInfo.BattleDamage)
                        .Write();
                    var operationResultEx = await victimBuild.BuildPlace.Operate(BuildType.Any, aggressorRef.Guid, victimSubRef, new DamageData { Type = OperationType.Damage, Damage = damageInfo.BattleDamage, DestructionPower = destructionPower });
                    if (operationResultEx != null && operationResultEx.Result == ErrorCode.Success && operationResultEx.OperationData is OperationDataWithRecipe operationDataWithRecipe)
                    {
                        BuildRecipeDef buildRecipeDef = operationDataWithRecipe.Recipe;
                        AsyncUtils.RunAsyncTask(() => PostDamage(aggressorRef, buildRecipeDef.Stats.PassiveDamage, buildRecipeDef.Stats.PassiveDamageType, damageInfo.IsMiningDamage, victimRef, repository), repository);
                    }
                }
                else
                if (victimHealthOwner != null)
                {
                    Logger.IfDebug()
                        ?.Message(aggressorRef.Guid, "------------------- victimHealthOwner.ReceiveDamage({0})", damageInfo)
                        .Write();

                    //DbgLog.Log(13334, "6. ExecuteStrike: dmg:" + damageInfo.BattleDamage);
                    using (var wrapper = await repository.Get(victimRef.TypeId, victimRef.Guid))
                    {
                        var victimMortal = wrapper.Get<IHasMortalServer>(victimRef.TypeId, victimRef.Guid, ReplicationLevel.Server);
                        if (victimMortal != null)
                            await victimMortal.Mortal.AddStrike(aggressorRef.To<IEntity>());
                    }
                    var result = await victimHealthOwner.Health.ReceiveDamage(damageInfo, aggressorRef.To<IEntity>());

                    //DbgLog.Log(13334, $"8. ExecuteStrike: Res.IsAppl:{result.IsApplied},  .Dmg:{result.Damage}");

                    if (victimSpecificStats != null && result.IsApplied)
                        AsyncUtils.RunAsyncTask(() => PostDamage(result.Damage, damageInfo.DamageType, damageInfo.IsMiningDamage, aggressorRef, victimRef, repository), repository);
                }
            }
        }

        private static void InvokeActions(OuterRef entityRef, IReadOnlyList<AttackActionDef> actions, ActionContext actionContext, IEntitiesRepository repository)
        {
            if (actions == null)
                return;

            var actionsBuffer = PooledArray<AttackActionDef>.Create(actions.Count);
            int actionsCount = 0;
            for (int i = 0; i < actions.Count; ++i)
                if(CheckAction(actions[i], actionContext))
                    actionsBuffer.Array[actionsCount++] = actions[i];

            if (actionsCount > 0)
                AsyncUtils.RunAsyncTask(
                    async () =>
                    {
                        using (var cnt = await repository.Get(entityRef.TypeId, entityRef.Guid))
                            if (cnt.TryGet<IHasReactionsOwner>(entityRef.TypeId, entityRef.Guid, out var hasReactions))
                            {
                                var argsBuffer = new List<ArgTuple>();
                                for (int i = 0; i < actionsCount; ++i)
                                    if (actionsBuffer.Array[i] is AttackInvokeReactionDef reaction)
                                    {
                                        argsBuffer.Clear();
                                        MapReactionArgs(reaction.Args, actionContext, argsBuffer);
                                        await hasReactions.ReactionsOwner.InvokeReaction(reaction.Reaction, argsBuffer.ToArray());
                                    }
                            }

                        using (var cnt = await repository.Get(entityRef.TypeId, entityRef.Guid))
                            if (cnt.TryGet<IHasWizardEntity>(entityRef.TypeId, entityRef.Guid, out var hasWizard))
                                using (var cnt2 = await repository.Get(hasWizard.Wizard.TypeId, hasWizard.Wizard.Id))
                                    if (cnt2.TryGet<IWizardEntity>(hasWizard.Wizard.TypeId, hasWizard.Wizard.Id, out var wizard))
                                    {
                                        var argsBuffer = new List<SpellCastParameter>();
                                        for (int i = 0; i < actionsCount; ++i)
                                            if (actionsBuffer.Array[i] is AttackCastSpellDef spell)
                                            {
                                                argsBuffer.Clear();
                                                CreateSpellParams(spell.Params, actionContext, argsBuffer);
                                                await wizard.CastSpell(new SpellCastWithParameters {Def = spell.Spell, Parameters = argsBuffer.ToArray()});
                                            }
                                    }

                        actionsBuffer.Dispose();
                    });
        }

        private static (bool hasWeapon, DamageTypeDef damageType, WeaponSizeDef weaponSize) GetWeaponStats(IEntitiesContainer container, OuterRef entityRef)
        {
            var res = GetWeaponStatsImpl(container, entityRef);
            return (
                res.hasWeapon,
                res.damageType ?? GlobalConstsHolder.GlobalConstsDef.DefaultDamageType,
                res.weaponSize ?? GlobalConstsHolder.GlobalConstsDef.DefaultWeaponSize
                );
        }
        
        // TODO: В будущем разнести этоу логику по конкретным типам entity (персонаж мобы и т п).
        // На данный же момент не понятно как сделать отдельный тип мобов с оружием, чтобы только им добавить item stats accumulator,
        // так как добавлять его вообще всем мобам не очень хочется.
        private static (bool hasWeapon, DamageTypeDef damageType, WeaponSizeDef weaponSize) GetWeaponStatsImpl(IEntitiesContainer container, OuterRef entityRef)
        {
            // Для персонажа c оружием
            if (container.TryGet<IHasItemsStatsAccumulatorServer>(entityRef.TypeId, entityRef.Guid, ReplicationLevel.Server, out var hasItemsStatAccumulator))
            {
                var itemsStatAccumulator = hasItemsStatAccumulator.ItemsStatsAccumulator;
                if (itemsStatAccumulator.HasActiveWeapon)
                    return (true, itemsStatAccumulator.ActiveWeaponDamageType, itemsStatAccumulator.ActiveWeaponSize);
            }

            // Для мобов/NPC с оружием
            if (container.TryGet<IHasDollClientBroadcast>(entityRef.TypeId, entityRef.Guid, ReplicationLevel.ClientBroadcast, out var hasDoll))
            {
                var weaponResource = ClusterHelpers.GetActiveWeaponResource(hasDoll);
                if (weaponResource.Key is IHasStatsResource hasStatsResource)
                {
                    var specificStats = hasStatsResource.SpecificStats.Target;
                    if (specificStats != null)
                        return (true, specificStats.DamageType.Target, specificStats.WeaponSize.Target);
                }
            }
            
            // Для всех прочих или если нет оружия   
            if (container.TryGet<IHasSpecificStatsServer>(entityRef.TypeId, entityRef.Guid, ReplicationLevel.Server, out var hasSpecificStats))
            {
                var specificStats = hasSpecificStats.SpecificStats;
                if (specificStats != null)
                    return (false, specificStats?.DamageType.Target, specificStats?.WeaponSize.Target);
            }

            return default;
        }
        
        private static async ValueTask<float> CalcDirectionCoefficient(Transform aggressorTransform,
            Transform victimTransform, IBruteServer victimBrute)
        {
            if (victimBrute == null)
                return 1;
            var vectorToAggressor = (aggressorTransform.Position - victimTransform.Position).Normalized;
            var victimForward = victimTransform.Forward;
            var scalar = SharedVector3.Dot(vectorToAggressor, victimForward);
            var directionCoefficient = (scalar >= 0.71)
                ? await victimBrute.GetForwardDamageMultiplier()
                : ((scalar <= -0.71)
                    ? await victimBrute.GetBackwardDamageMultiplier()
                    : await victimBrute.GetSideDamageMultiplier());
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("------------------- directionCoefficient = {0}", directionCoefficient).Write();
            return directionCoefficient;
        }

        private static async Task PostDamage(float damage, DamageTypeDef damageType, bool isMiningDamage, OuterRef aggressor, OuterRef victim, IEntitiesRepository repository)
        {
            using (var entitiesContainer = await repository.Get(EntityBatch.Create().Add(aggressor.TypeId, aggressor.Guid).Add(victim.TypeId, victim.Guid)))
            {
                var aggressorWithStatistics = entitiesContainer.Get<IHasStatisticsServer>(aggressor.TypeId, aggressor.Guid, ReplicationLevel.Server);
                var aggressorDoll = entitiesContainer.Get<IHasDollServer>(aggressor.TypeId, aggressor.Guid, ReplicationLevel.Server);
                var victimStats = entitiesContainer.Get<IHasSpecificStatsServer>(victim.TypeId, victim.Guid, ReplicationLevel.Server);
                var victimDoll = entitiesContainer.Get<IHasDoll>(victim.TypeId, victim.Guid, ReplicationLevel.Master);
                var victimEntity = entitiesContainer.Get<IEntityObjectServer>(victim.TypeId, victim.Guid, ReplicationLevel.Server);
                await DealPassiveDamageToVictim(victimDoll, damage, damageType);
                var (feedbackDamageType, feedbackDamage) = CalcPassiveDamageToAttacker(victimStats, victimDoll);
                await DealPassiveDamageToAggressor(aggressorDoll, victim, feedbackDamage, feedbackDamageType, isMiningDamage);
                if (aggressorWithStatistics != null && victimEntity != null)
                        await aggressorWithStatistics.StatisticEngine.PostStatisticsEvent(new SharedCode.Quests.DealDamageEventArgs { TargetObjectDef = victimEntity.Def, ObjectType = (victimEntity.Def as IHasStatisticsTypeDef)?.ObjectType, Value = damage });
            }
        }

        private static async Task PostDamage(OuterRef aggressor, float passiveDamage, DamageTypeDef passiveDamageType, bool isMiningDamage, OuterRef victim, IEntitiesRepository repository)
        {
            using (var entitiesContainer = await repository.Get(aggressor.TypeId, aggressor.Guid))
            {
                //                var aggressorWithStatistics = entitiesContainer.Get<IHasStatisticsServer>(aggressor.TypeId, aggressor.Guid, ReplicationLevel.Server);
                var aggressorDoll = entitiesContainer.Get<IHasDollServer>(aggressor.TypeId, aggressor.Guid, ReplicationLevel.Server);
                await DealPassiveDamageToAggressor(aggressorDoll, victim, passiveDamage, passiveDamageType, isMiningDamage);
                //                if (aggressorWithStatistics != null)
                //                    await aggressorWithStatistics.StatisticEngine.PostStatisticsEvent(new SharedCode.Quests.DealDamageEventArgs { TargetObjectDef = victimDeltaObj.Def, ObjectType = victimBrute.ObjectType, Value = damage });
            }
        }

        /// <summary>
        /// Deal damage(износ) to agressors' weapon:
        /// </summary>
        private static async Task DealPassiveDamageToAggressor(IHasDollServer aggressorDoll, OuterRef victimBrute, float passiveDamage, DamageTypeDef passiveDamageType, bool isMiningDamage)
        {
            if (passiveDamage == 0)
                return;

            var usedSlots = aggressorDoll?.Doll?.UsedSlots;
            if (usedSlots == null || !usedSlots.Any())
                return;

            var usedIndex = ItemHelper.TmpPlug_GetIndexBySlotResourceIdFull(usedSlots.First());
            if (!aggressorDoll.Doll.Items.TryGetValue(usedIndex, out var activeSlot))
                return;

            if (passiveDamageType == null)
                passiveDamageType = GlobalConstsHolder.GlobalConstsDef.DefaultDamageType;

            using (var resistanceStats = DamageCalculationRoot.PassiveDamageChannels.Target.DamageChannels.Where(v => v.DamageType == passiveDamageType).GetEnumerator())
                if (resistanceStats.MoveNext())
                {
                    var resistanceStat = resistanceStats.Current.ResistanceStat.Target;
                    float resistance = (await activeSlot.Item.Stats.TryGetValue(resistanceStat)).Item2 +
                        (activeSlot.Item.ItemResource as IHasStatsResource)?.SpecificStats.Target?.Stats?.Where(v => v.Stat.Target == resistanceStat).FirstOrDefault().Value ?? 0;

                    if (resistance < 0 || resistance > 1)
                        Logger.IfError()?.Message("Resistance = {0} mast be in range of [0, 1] for item '{1}'", resistance, activeSlot.Item.ItemResource).Write();

                    passiveDamage *= (1 - resistance);
                }

            passiveDamage /= activeSlot.Stack;

            var damageInfo = new Damage(
                aggressor: victimBrute,
                battleDamage: passiveDamage,
                damageType: passiveDamageType,
                isMiningDamage: isMiningDamage,
                miningLootMultiplier: 0f
            );

            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("Passive damage to {0} of {2}.{3} is {1}", activeSlot.Item.ItemResource, damageInfo, ((IEntity)aggressorDoll).TypeName, ((IEntity)aggressorDoll).Id).Write();
            await activeSlot.Item.Health.ReceiveDamage(damageInfo, victimBrute.To<IEntity>());
        }

        /// <summary>
        /// Deal damage to victims' armor
        /// </summary>
        private static async Task DealPassiveDamageToVictim(IHasDoll victimDoll, float activeDamage, DamageTypeDef passiveDamageType)
        {
            if (victimDoll != null)
            {
                foreach (var slotCoefficient in DamageCalculationRoot.SlotPassiveDamage)
                {
                    var slotId = slotCoefficient.Slot.Target.SlotId;
                    if (victimDoll.Doll.Items.TryGetValue(slotId, out var slotItem))
                    {
                        var passiveDamage = activeDamage * slotCoefficient.Coefficient;
                        using (var resistanceStats = DamageCalculationRoot.PassiveDamageChannels.Target.DamageChannels.Where(v => v.DamageType == passiveDamageType).GetEnumerator())
                            if (resistanceStats.MoveNext())
                            {
                                var resistanceStat = resistanceStats.Current.ResistanceStat.Target;

                                float resistance = (await slotItem.Item.Stats.TryGetValue(resistanceStat)).Item2 +
                                    (slotItem.Item.ItemResource as IHasStatsResource)?.SpecificStats.Target?.Stats?.Where(v => v.Stat.Target == resistanceStat).FirstOrDefault().Value ?? 0;

                                passiveDamage *= (1 - resistance);
                            }
                        passiveDamage /= slotItem.Stack;

                        await slotItem.Item.Health.ChangeHealth(-passiveDamage);
                    }
                }
            }
        }

        private static (DamageTypeDef, float) CalcPassiveDamageToAttacker(IHasSpecificStatsServer victimStats, IHasDoll victimDoll)
        {
            float totalPassiveDamage = 0;
            DamageTypeDef damageType = default(DamageTypeDef);

            var defStats = victimStats?.SpecificStats;

            if (defStats != null)
            {
                totalPassiveDamage += defStats.Stats.FirstOrDefault(v => v.Stat == DamageCalculationRoot.PassiveDamageStat).Value;
                damageType = defStats.DamageType.Target;
            }

            if (victimDoll != null)
            {
                foreach (var slotCoefficient in DamageCalculationRoot.SlotPassiveDamage)
                {
                    var slodId = slotCoefficient.Slot.Target.SlotId;
                    if (victimDoll.Doll.Items.TryGetValue(slodId, out var slotItem))
                    {
                        var itemResource = slotItem.Item.ItemResource as IHasStatsResource;
                        if (itemResource?.SpecificStats.Target != null)
                            if (itemResource.SpecificStats.Target.TryGetStat(DamageCalculationRoot.PassiveDamageStat, out var passiveDamage))
                                totalPassiveDamage += passiveDamage;
                    }
                }
            }

            return (damageType, totalPassiveDamage);
        }

        public static async ValueTask<float> CalcDamage(
            IHasStatsEngineServer aggressorStats,
            IHasSpecificStatsServer aggressorSpecificStats,
            StatResource damageStat,
            StatResource damageModifierStat,
            StatResource damageMultiplierStat,
            IReadOnlyList<AttackModifierDef> modifiers,
            float directionCoefficient = 1)
        {
            var originalDamage = await GetStatVal(damageStat, aggressorStats, aggressorSpecificStats, modifiers);
            var damageModifier = await GetStatVal(damageModifierStat, aggressorStats, aggressorSpecificStats, modifiers);
            var damageMultiplier = modifiers.ApplyAttackStatModifiers(damageMultiplierStat, modifiers.ApplyAttackStatOverride(damageMultiplierStat, 1));
            var multipliedDamage = originalDamage * (1.0f + damageModifier) * damageMultiplier * directionCoefficient;
            if (Logger.IsDebugEnabled)
                Logger.IfDebug()?.Message($"{damageStat.____GetDebugRootName()}:{originalDamage} * (1 + {damageModifierStat.____GetDebugRootName()}:{damageModifier}) * {damageMultiplierStat.____GetDebugRootName()}:{damageMultiplier} * DirectionCoefficient:{directionCoefficient} = {multipliedDamage}").Write();
            return multipliedDamage;
        }

        private static async ValueTask<StaggerAndRecoil> CalcStaggerAndRecoil(
            IHasStatsEngineServer aggressorStats,
            IHasSpecificStatsServer aggressorSpecificStats,
            IHasStatsEngineServer victimStats,
            IHasSpecificStatsServer victimSpecificStats,
            IReadOnlyList<AttackModifierDef> modifiers,
            bool blocked)
        {
            var attackerPower = await GetStatVal(DamageCalculationRoot.PowerStat, aggressorStats, aggressorSpecificStats, modifiers);
            var victimStability = victimStats != null ? await victimStats.Stats.TryGetValue(DamageCalculationRoot.StabilityStat) : (false, 0);

            StaggerAndRecoil result;
            if (!blocked)
            {
                float balance = attackerPower - victimStability.Item2;
                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Balance:{balance} = AttackerPower:{attackerPower} - VictimStability:{victimStability}").Write();
                result = new StaggerAndRecoil(
                    staggerTime: DamageCalculationRoot.PowerVsStability.Target.StaggerTime.Target.Evaluate(balance),
                    recoilTime: DamageCalculationRoot.PowerVsStability.Target.RecoilTime.Target.Evaluate(balance),
                    damageMultiplier: DamageCalculationRoot.PowerVsStability.Target.DamageMultiplier.Target.Evaluate(balance)
                );
            }
            else
            {
                float victimPower = await GetStatVal(DamageCalculationRoot.PowerStat, victimStats, victimSpecificStats, null);
                float balance = attackerPower - (victimStability.Item2 + victimPower);
                if (Logger.IsTraceEnabled)
                    Logger.Trace(
                        $"Balance:{balance} = AttackerPower:{attackerPower} - ( VictimStability:{victimStability} + VictimPower:{victimPower} )");
                result = new StaggerAndRecoil(
                    staggerTime: DamageCalculationRoot.PowerVsStabilityWhenBlocked.Target.StaggerTime.Target.Evaluate(balance),
                    recoilTime: DamageCalculationRoot.PowerVsStabilityWhenBlocked.Target.RecoilTime.Target.Evaluate(balance),
                    damageMultiplier: DamageCalculationRoot.PowerVsStabilityWhenBlocked.Target.DamageMultiplier.Target.Evaluate(balance)
                );
            }

            Logger.IfDebug()
                ?.Message(aggressorStats.ParentEntityId, $"StaggerTime:{result.StaggerTime} RecoilTime:{result.RecoilTime} DamageMult:{result.DamageMultiplier}")
                .Write();
                return result;
        }

        private static async ValueTask<float> ApplyResistance(
            float damage,
            DamageTypeDef damageType,
            IHasStatsEngineServer aggressorStats,
            IHasSpecificStatsServer aggressorSpecificStats,
            IHasStatsEngineServer victimStats,
            IReadOnlyList<AttackModifierDef> modifiers
            )
        {
            if (victimStats == null)
                return damage;
//            float armorPen = await GetStatVal(DamageCalculationRoot.ArmorPenetrationStat, aggressorStats, aggressorBrute, overriddenWeaponStats);
            //            if (float.IsNaN(armorPen)) 
            //                armorPen = 0;
            var channel = DamageCalculationRoot.IncomingDamageChannels.Target.DamageChannels.FirstOrDefault(v => v.DamageType == damageType);
            var resistanceStat = channel.ResistanceStat.Target ?? DamageCalculationRoot.IncomingDamageChannels.Target.GenericResistance.Target;
            if (resistanceStat.AssertIfNull(nameof(resistanceStat)))
                return damage;
            (bool hasResistance, float resistance) = await victimStats.Stats.TryGetValue(resistanceStat);
            if (hasResistance)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Before resistance | Damage:{damage}").Write();
                //                float effectiveDamageResistance = Math.Max(0.0f, resistance - armorPen);
                float damageMult = (float)Math.Pow(DamageCalculationRoot.ArmorEfficiency, 0.1f * resistance);
                damage *= damageMult;
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"After resistance | Damage:{damage} DamageMult:{damageMult} Resistance:{resistance}").Write();
                //                if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"After resistance | Damage:{damage} EffectiveDamageResistance:{effectiveDamageResistance}").Write();
            }
            return damage;
        }

        private static async ValueTask<float> GetStatVal(StatResource statRes, IHasStatsEngineServer subjectStats, IHasSpecificStatsServer subjectSpecificStats, IReadOnlyList<AttackModifierDef> modifiers, float @default = 0)
        {
            if (statRes == null)
                return @default;
            float result;
            if (subjectStats == null || !((_, result) = await subjectStats.Stats.TryGetValue(statRes)).Item1) // у персонажа параметры оружия (урон и т п) записываются в статы
                if(subjectSpecificStats == null || !(subjectSpecificStats.SpecificStats).TryGetStat(statRes, out result)) // у мобов эти параметры берутся из DefaultStats
                    result = @default;
            return modifiers.ApplyAttackStatModifiers(statRes, modifiers.ApplyAttackStatOverride(statRes, result));
        }

        // ReSharper disable UnusedParameter.Local
        private static bool CheckBlockSector(ItemSpecificStats victimWeaponStats, SharedVector3 attackerPos, SharedVector3 victimPos, SharedVector3 victimForward)
        // ReSharper restore UnusedParameter.Local
        {
            //#TC-4350 jin fix. #TODO: return checking angles logic, when it'll be fixed
            return true;
#if false
            if (victimPos.IsDefault)
                return false;
            if (attackerPos.IsDefault)
                return true;

            if (victimWeaponStats != null)
            {
                var toAttacker = (new SharedVector2(attackerPos.x, attackerPos.z) - new SharedVector2(victimPos.x, victimPos.z)).Normalized;
                var cos = SharedVector2.Dot(toAttacker, new SharedVector2(victimForward.x, victimForward.z).Normalized);
                var angle = Math.Acos(cos) * SharedHelpers.Rad2Deg;
                return angle <= victimWeaponStats.BlockSector * 0.5f;
            }

            return false;
#endif
        }

        private static HitMaterialDef GetHitMaterial(this IEntity entity)
        {
            var entityServer = entity.GetReplicationLevel(ReplicationLevel.Server) ?? throw new Exception($"Can't get entity server replication level");
            
            if (entityServer is IHasDollServer hasDoll)
            {
                foreach (var slot in DamageCalculationRoot.HitMaterialSlots)
                {
                    foreach (var pair in hasDoll.Doll.Items)
                    {
                        if (pair.Key == slot.SlotId && pair.Value?.Item?.ItemResource is ItemResource itemRes)
                        {
                            var material = itemRes.SpecificStats.Target?.HitMaterial;
                            if (material != null)
                                return material;
                        }
                    }
                }
            }

            if (entityServer is IEntityObjectServer entityObject && entityObject.Def is IHasDefaultStatsDef def)
            {
                return def.DefaultStats.Target?.HitMaterial;
            }

            Logger.IfDebug()?.Message(entity.Id, $"No HitMaterial found for {entity.Id} {entity.TypeId}").Write();
            
            return GlobalConstsHolder.GlobalConstsDef.DefaultHitMaterial;
        }

        
        private static async ValueTask<bool> CheckBlockActive(IHasStatsEngineServer subjectStats)
        {
            if (subjectStats == null) return false;
            var (hasStat, statValue) = await subjectStats.Stats.TryGetValue(DamageCalculationRoot.BlockActive);
            return hasStat && statValue > 0;
        }

        private static bool CheckAction(AttackActionDef reaction, in ActionContext ctx)
        {
            if (reaction == null)
                return false;
            switch (reaction.When)
            {
                case AttackActionDef.ApplyWhen.Always:
                    return true;
                case AttackActionDef.ApplyWhen.Damage:
                    return ctx.Damage > 0;
                case AttackActionDef.ApplyWhen.Stagger:
                    return ctx.StaggerTime > 0;
                case AttackActionDef.ApplyWhen.Recoil:
                    return ctx.RecoilTime > 0;
                case AttackActionDef.ApplyWhen.Block:
                    return ctx.Block;
                case AttackActionDef.ApplyWhen.BlockAndStagger:
                    return ctx.Block && ctx.StaggerTime > 0;
                case AttackActionDef.ApplyWhen.BlockAndNoStagger:
                    return ctx.Block && ctx.StaggerTime <= 0;
                default:
                    throw new NotImplementedException($"{reaction.When}");
            }
        } 
        
        private static void MapReactionArgs(AttackArgsMappingDef args, in ActionContext ctx, List<ArgTuple> rv)
        {
            if (args == null)
                return;
            if (args.Attacker != null)
                rv.Add(ArgTuple.Create(args.Attacker, ArgValue.Create(ctx.Attacker)));
            if (args.Victim != null)
                rv.Add(ArgTuple.Create(args.Victim, ArgValue.Create(ctx.VictimInfo.Target)));
            if (args.Damage != null)
                rv.Add(ArgTuple.Create(args.Damage, ArgValue.Create(ctx.Damage)));
            if (args.DamageType != null)
                rv.Add(ArgTuple.Create(args.DamageType, ArgValue.Create((BaseResource)ctx.DamageType)));
            if (args.WeaponSize != null)
                rv.Add(ArgTuple.Create(args.WeaponSize, ArgValue.Create((BaseResource)ctx.WeaponSize)));
            if (args.AttackType != null)
                rv.Add(ArgTuple.Create(args.AttackType, ArgValue.Create((BaseResource)ctx.AttackType)));
            if (args.HitMaterial != null)
                rv.Add(ArgTuple.Create(args.HitMaterial, ArgValue.Create((BaseResource)ctx.HitMaterial)));
            if (args.RecoilTime != null)
                rv.Add(ArgTuple.Create(args.RecoilTime, ArgValue.Create(ctx.RecoilTime)));
            if (args.StaggerTime != null)
                rv.Add(ArgTuple.Create(args.StaggerTime, ArgValue.Create(ctx.StaggerTime)));
            if (args.HitDirection != null)
                rv.Add(ArgTuple.Create(args.HitDirection, ArgValue.Create(ctx.HitDirection)));
            if (args.HitPoint != null)
                rv.Add(ArgTuple.Create(args.HitPoint, ArgValue.Create(ctx.VictimInfo.HitPoint)));
            if (args.HitLocalPoint != null)
                rv.Add(ArgTuple.Create(args.HitLocalPoint, ArgValue.Create(ctx.VictimInfo.HitLocalPoint)));
            if (args.HitRotation != null)
                rv.Add(ArgTuple.Create(args.HitRotation, ArgValue.Create(ctx.VictimInfo.HitRotation)));
            if (args.HitLocalRotation != null)
                rv.Add(ArgTuple.Create(args.HitLocalRotation, ArgValue.Create(ctx.VictimInfo.HitLocalRotation)));
            if (args.HitObject != null)
                rv.Add(ArgTuple.Create(args.HitObject, ArgValue.Create(ctx.VictimInfo.HitObject)));
        }

        private static void CreateSpellParams(AttackCastSpellDef.SpellParams args, in ActionContext ctx, List<SpellCastParameter> rv)
        {
            if (args == null)
                return;
            if (args.Damage != null)
                rv.Add(SpellCastParameters.Create(args.Damage.Target, new Value(ctx.Damage)));
            if (args.DamageType != null)
                rv.Add(SpellCastParameters.Create(args.DamageType.Target, new Value((BaseResource)ctx.DamageType)));
            if (args.WeaponSize != null)
                rv.Add(SpellCastParameters.Create(args.WeaponSize.Target, new Value((BaseResource)ctx.WeaponSize)));
            if (args.AttackType != null)
                rv.Add(SpellCastParameters.Create(args.AttackType.Target, new Value((BaseResource)ctx.AttackType)));
            if (args.HitMaterial != null)
                rv.Add(SpellCastParameters.Create(args.HitMaterial.Target, new Value((BaseResource)ctx.HitMaterial)));
            if (args.RecoilTime != null)
                rv.Add(SpellCastParameters.Create(args.RecoilTime.Target, new Value(ctx.RecoilTime)));
            if (args.StaggerTime != null)
                rv.Add(SpellCastParameters.Create(args.StaggerTime.Target, new Value(ctx.StaggerTime)));
            if (args.HitDirection != null)
                rv.Add(SpellCastParameters.Create(args.HitDirection.Target, new Value(ctx.HitDirection)));
            if (args.Attacker != null)
                rv.Add(SpellCastParameters.Create(args.Attacker.Target, new Value(ctx.Attacker)));
            if (args.Victim != null)
                rv.Add(SpellCastParameters.Create(args.Victim.Target, new Value(ctx.VictimInfo.Target)));
            if (args.HitPoint != null)
                rv.Add(SpellCastParameters.Create(args.HitPoint.Target, new Value(ctx.VictimInfo.HitPoint)));
            if (args.HitLocalPoint != null)
                rv.Add(SpellCastParameters.Create(args.HitLocalPoint.Target, new Value(ctx.VictimInfo.HitLocalPoint)));
            if (args.HitRotation != null)
                rv.Add(SpellCastParameters.Create(args.HitRotation.Target, new Value(ctx.VictimInfo.HitRotation)));
            if (args.HitLocalRotation != null)
                rv.Add(SpellCastParameters.Create(args.HitLocalRotation.Target, new Value(ctx.VictimInfo.HitLocalRotation)));
            if (args.HitObject != null)
                rv.Add(SpellCastParameters.Create(args.HitObject.Target, new Value(ctx.VictimInfo.HitObject)));
        }
        
        // Is needed 'cos we can't pass "ref" param to async method
        private readonly struct StaggerAndRecoil
        {
            public readonly float StaggerTime;
            public readonly float RecoilTime;
            public readonly float DamageMultiplier;
            public StaggerAndRecoil(float staggerTime, float recoilTime, float damageMultiplier)
            {
                StaggerTime = staggerTime;
                RecoilTime = recoilTime;
                DamageMultiplier = damageMultiplier;
            }
        }

        private readonly struct ActionContext
        {
            public readonly OuterRef Attacker;
            public readonly AttackTargetInfo VictimInfo;
            public readonly float Damage;
            public readonly bool Block;
            public readonly DamageTypeDef DamageType;
            public readonly WeaponSizeDef WeaponSize;
            public readonly AttackTypeDef AttackType;
            public readonly HitMaterialDef HitMaterial;
            public readonly float StaggerTime;
            public readonly float RecoilTime;
            public readonly SharedVector2 HitDirection;

            public ActionContext(
                OuterRef attacker,
                AttackTargetInfo victimInfo,
                float staggerTime,
                float recoilTime,
                float damage,
                bool block,
                SharedVector2 hitDirection,
                DamageTypeDef damageType,
                WeaponSizeDef weaponSize,
                AttackTypeDef attackType,
                HitMaterialDef hitMaterial)
            {
                Attacker = attacker;
                VictimInfo = victimInfo;
                StaggerTime = staggerTime;
                RecoilTime = recoilTime;
                Damage = damage;
                Block = block;
                HitDirection = hitDirection;
                DamageType = damageType;
                WeaponSize = weaponSize;
                AttackType = attackType;
                HitMaterial = hitMaterial;
            }
        }
    }
}