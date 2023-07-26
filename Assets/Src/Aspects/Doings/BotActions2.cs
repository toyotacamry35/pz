using Assets.ColonyShared.SharedCode.Shared;
using Assets.Src.Aspects.Impl;
using Assets.Src.Cluster.Cheats;
using Assets.Src.InteractionSystem;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using Assets.Src.Tools;
using Assets.Tools;
using ColonyShared.ManualDefsForSpells;
using ColonyShared.SharedCode.InputActions;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using Core.Reflection;
using SharedCode.Wizardry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using Assets.Src.SpawnSystem;
using ColonyShared.SharedCode.Wizardry;
using Core.Environment.Logging.Extension;
using SharedCode.Utils.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;
using Src.Aspects.Impl.Stats;
using SharedCode.Repositories;
using SharedCode.Serializers;

namespace Assets.Src.Aspects.Doings
{
    public static class BotActions2
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Bot");

        private static readonly ResourceRef<InputActionValueDef> MoveForward = new ResourceRef<InputActionValueDef>(@"/UtilPrefabs/Input/Actions/MoveForward");
        private static readonly ResourceRef<InputActionValueDef> MoveBackward = new ResourceRef<InputActionValueDef>(@"/UtilPrefabs/Input/Actions/MoveBackward");
        private static readonly ResourceRef<InputActionValueDef> MoveLeft = new ResourceRef<InputActionValueDef>(@"/UtilPrefabs/Input/Actions/MoveLeft");
        private static readonly ResourceRef<InputActionValueDef> MoveRight = new ResourceRef<InputActionValueDef>(@"/UtilPrefabs/Input/Actions/MoveRight");

        private delegate Task<bool> ActionDelegate(BotActionsStatesMachine2 m,BotActionDef d, IDictionary<string,object> k, CancellationToken t);
        
        private static readonly Dictionary<Type,ActionDelegate> Methods =
            typeof(BotActions2).GetMethodsSafe(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(v => v.GetParameters().Length == 4)
                .ToDictionary(x => x.GetParameters()[1].ParameterType, x => (ActionDelegate)((m,d,k,t) => (Task<bool>)x.Invoke(null, new object[] { m, d, k, t })));

        
        //public delegate Task<bool> BotAction<In>(BotActionsStateMachine2 sm, In def, IDictionary<string, object> localKnowledge) where In : BotActionDef;

        public static async Task<bool> InvokeAction(BotActionsStatesMachine2 sm, BotActionDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation, float timeout = -1)
        {
             Logger.IfInfo()?.Message("{0}: Starting {1}",  sm, def).Write();
            ActionDelegate method;
            try
            {
                method = Methods[def.GetType()];
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "{0}: Failed to find function for {1}", sm, def).Write();
                return false;
            }

            try
            {
                sm.PushCurrentAction(def);
                var retTask = method.Invoke(sm, def, localKnowledge, cancellation);
                if (timeout <= 0)
                    await retTask;
                else
                    await Task.WhenAny(retTask, Task.Delay(TimeSpan.FromSeconds(timeout), cancellation));
                if (!retTask.IsCompleted)
                {
                     Logger.IfError()?.Message("{0}: Action {1} timeout",  sm, def).Write();
                    return false;
                }

                 Logger.IfInfo()?.Message("{0}: {1} {2}", sm, retTask.Result ? "Completed" : "Failed",  def).Write();
                return retTask.Result;
            }
            catch (TaskCanceledException)
            {
                 Logger.IfInfo()?.Message("{0}: Cancelled {1}",  sm, def).Write();
                throw;
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "{0}: Failed {1} with exception: {2}", sm, def, e).Write();
                return false;
            }
            finally
            {
                sm.PopCurrentAction(def);
            }
        }

        public static async Task<bool> BotWait(BotActionsStatesMachine2 sm, BotWaitDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            await Task.Delay(def.DurationSeconds >= 0.0f ? TimeSpan.FromSeconds(def.DurationSeconds) : TimeSpan.FromMilliseconds(-1), cancellation);
            return true;
        }

        public static async Task<bool> BotWaitRandom(BotActionsStatesMachine2 sm, BotWaitRandomDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var delay = Random.Range(def.DurationSecondsMin, def.DurationSecondsMax);
            await Task.Delay(delay >= 0.0f ? TimeSpan.FromSeconds(delay) : TimeSpan.FromMilliseconds(-1), cancellation);
            return true;
        }

        public static async Task<bool> BotSelectTarget(BotActionsStatesMachine2 sm, BotSelectTargetDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            if (def.ObjectName == null)
                return false;

            var _currentPosition = sm.Transform.position;
            float lastDistance = def.BotSelectTargetType == BotSelectTargetType.Near ? float.MaxValue : 0;
            GameObject result = null;
            var collidersInSphere = Physics.OverlapSphere(_currentPosition, PhysicsChecker.CheckRadius(def.SearchRadius, "BotSelectTarget"));
            var regex = new Regex(def.ObjectName);
            var allowedObjects = new List<GameObject>();
            foreach (var collider in collidersInSphere)
            {
                var gameObj = collider.transform.root.gameObject;
                if (regex.IsMatch(gameObj.name))
                {
                    if (def.SkipSelf && gameObj == sm.Transform.root.gameObject)
                        continue;

                    if (def?.Conditional.Target != null)
                    {
                        localKnowledge["Target"] = gameObj;
                        var conditionalResult = await InvokeAction(sm, def.Conditional, localKnowledge, cancellation);
                        localKnowledge.Remove("Target");

                        if (!conditionalResult)
                            continue;
                    }

                    allowedObjects.Add(gameObj);

                }
            }

            if (allowedObjects.Count > 0)
                switch (def.BotSelectTargetType)
                {
                    case BotSelectTargetType.Near:
                        foreach (var gameObj in allowedObjects)
                        {
                            float distance = Vector3.Distance(_currentPosition, gameObj.transform.position);
                            if (distance < lastDistance)
                            {
                                lastDistance = distance;
                                result = gameObj;
                            }
                        }

                        break;
                    case BotSelectTargetType.Far:
                        foreach (var gameObj in allowedObjects)
                        {
                            float distance = Vector3.Distance(_currentPosition, gameObj.transform.position);
                            if (distance > lastDistance)
                            {
                                lastDistance = distance;
                                result = gameObj;
                            }
                        }
                        break;
                    case BotSelectTargetType.Random:
                        result = allowedObjects[(new System.Random((int)DateTime.UtcNow.Ticks)).Next(allowedObjects.Count)];
                        break;
                }

            if (result == null)
                return false;

            Vector3 cameraDirection = result.transform.position - sm.Transform.position;
            localKnowledge["Target"] = result;

            return true;
        }

        public static Task<bool> BotSelectRandomPoint(BotActionsStatesMachine2 sm, BotSelectRandomPointDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var point = UnityEngine.Random.insideUnitSphere * def.Radius + sm.Transform.position;
            localKnowledge["Point"] = point;
            return Task.FromResult(true);
        }

        public static async Task<bool> BotLookToTarget(BotActionsStatesMachine2 sm, BotLookToTargetDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var lastTime = Time.realtimeSinceStartup;
            do
            {
                if (!localKnowledge.TryGetValue("Target", out var target))
                    return false;

                Vector3 cameraDirection = ((GameObject)target).transform.position - sm.Transform.position;
                sm.CameraDirection = cameraDirection.normalized;
                sm.SmoothTime = def.SmoothTime;

                await Task.Delay(TimeSpan.FromSeconds(def.UpdatePeriod), cancellation);

            } while (def.TimeoutSeconds > 0 && Time.realtimeSinceStartup - lastTime <= def.TimeoutSeconds);

            return true;
        }

        public static async Task<bool> BotLookToPoint(BotActionsStatesMachine2 sm, BotLookToPointDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            if (!localKnowledge.TryGetValue("Point", out object target))
                return false;

            Vector3 cameraDirection = ((Vector3)target) - sm.Transform.position;
            sm.CameraDirection = cameraDirection.normalized;

            await Task.Delay(TimeSpan.FromSeconds(0.1));

            return true;
        }

        public static async Task<bool> BotAddUsedSlot(BotActionsStatesMachine2 sm, BotAddUsedSlotDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var repo = sm.Repository;
            var entityId = sm.EntityId;
            var slotResourceIDFull = GameResourcesHolder.Instance.GetID(def.SlotDef.Target);

            var result = await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get<IWorldCharacterClientFull>(entityId))
                {
                    if (wrapper.AssertIfNull(nameof(wrapper)))
                        return false;

                    await wrapper.Get<IWorldCharacterClientFull>(entityId).AddUsedSlot(slotResourceIDFull);
                    return true;
                }
            }, repo);
            return result;
        }

        public static async Task<bool> BotRemoveUsedSlot(BotActionsStatesMachine2 sm, BotRemoveUsedSlotDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var repo = sm.Repository;
            var entityId = sm.EntityId;
            var slotResourceIDFull = GameResourcesHolder.Instance.GetID(def.SlotDef.Target);

            var result = await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get<IWorldCharacterClientFull>(entityId))
                {
                    if (wrapper.AssertIfNull(nameof(wrapper)))
                        return false;

                    await wrapper.Get<IWorldCharacterClientFull>(entityId).RemoveUsedSlot(slotResourceIDFull);
                    return true;
                }
            }, repo);
            return result;
        }

        private static bool TryGetTargetEntityRef(IDictionary<string, object> localKnowledge, out OuterRef<IEntity> targetRef)
        {
            targetRef = default;
            if (localKnowledge.TryGetValue("Target", out var target))
            {
                if (((GameObject)target) == null)
                    localKnowledge.Remove("Target");
                else
                {
                    var ego = ((GameObject)target).GetComponent<EntityGameObjectComponent>();
                    if (ego != null)
                    {
                        targetRef = ego.GetOuterRef<IEntity>();
                        return targetRef != default;
                    }
                }
            }

            return false;
        }

        public static async Task<bool> BotCastSpell(BotActionsStatesMachine2 sm, BotCastSpellDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            if (def.Spells == null)
                return true;

            var repo = sm.Repository;
            OuterRef<IEntity> targetRef = default;
            if (localKnowledge.TryGetValue("Target", out var target))
            {
                if (((GameObject)target) == null)
                    localKnowledge.Remove("Target");
                else
                {
                    var ego = ((GameObject)target).GetComponent<EntityGameObjectComponent>();
                    if (ego != null)
                        targetRef = ego.GetOuterRef<IEntity>();
                }
            }

            OuterRef<IEntity> casterRef = sm.EntityRef.To<IEntity>();

            SharedCode.Utils.Vector3 targetPoint = default;
            if (localKnowledge.TryGetValue("Point", out var point))
                targetPoint = (SharedCode.Utils.Vector3)point;
            

            var result = await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get(sm.EntityRef.TypeId, sm.EntityRef.Guid))
                {
                    if (wrapper.AssertIfNull(nameof(wrapper)))
                        return false;

                    foreach (var defSpell in def.Spells)
                    {
                        OuterRef<IEntity> spellTargetRef;
                        if (def.Target.Target is SpellCasterDef)
                            spellTargetRef = casterRef;
                        else
                            spellTargetRef = targetRef;
                        var castBuilder = new SpellCastBuilder().SetTargetIfValid(spellTargetRef);

                        if (targetPoint != default)
                            castBuilder.SetDirection3(targetPoint);;

                        sm.SpellDoer.DoCast(castBuilder.SetSpell(defSpell).Build());
                    }
                    return true;
                }
            }, repo);
            return result;
        }

        public static Task<bool> BotCheckTargetDistance(BotActionsStatesMachine2 sm, BotCheckTargetDistanceDef def,
            IDictionary<string, object> localKnowledge, CancellationToken _)
        {
            if (!localKnowledge.TryGetValue("Target", out var target))
                return Task.FromResult(false);

            if (((GameObject) target) == null)
            {
                localKnowledge.Remove("Target");
                return Task.FromResult(false);
            }

            return Task.FromResult(Vector3.Distance(sm.Transform.position, ((GameObject)target).transform.position) <= def.DistanceMeters);
        }

        public static Task<bool> BotDropTarget(BotActionsStatesMachine2 sm, BotDropTargetDef def,
            IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            return Task.FromResult(localKnowledge.Remove("Target"));
        }

        public static async Task<bool> BotCheckStat(BotActionsStatesMachine2 sm, BotCheckStatDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var repo = sm.Repository;
            OuterRef<IEntity> targetRef = default;
            if (def.OnTarget)
            {
                if (localKnowledge.TryGetValue("Target", out var target))
                {
                    var ego = ((GameObject) target).GetComponent<EntityGameObjectComponent>();
                    if (ego != null)
                        targetRef = ego.GetOuterRef<IEntity>();
                }
                else
                    return false;
            }
            else
            {
                targetRef = sm.EntityRef.To<IEntity>();
            }


            var result = await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get(targetRef.TypeId, targetRef.Guid))
                {
                    if (wrapper.AssertIfNull(nameof(wrapper)))
                        return false;

                    var entity = wrapper.Get<IEntity>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.ClientBroadcast);
                    if (entity is IHasStatsEngineClientBroadcast)
                    {
                        var stat = await ((IHasStatsEngineClientBroadcast) entity).Stats.GetBroadcastStat(def.StatResource);
                        return compare(def.Operation, await stat.GetValue(), def.Value);
                    }

                    return false;
                }
            }, repo);
            return result;
        }

        public static async Task<bool> BotCheckStatPercent(BotActionsStatesMachine2 sm, BotCheckStatPercentDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var repo = sm.Repository;
            OuterRef<IEntity> targetRef = default;
            if (def.OnTarget)
            {
                if (localKnowledge.TryGetValue("Target", out var target))
                {
                    var ego = ((GameObject)target).GetComponent<EntityGameObjectComponent>();
                    if (ego != null)
                        targetRef = ego.GetOuterRef<IEntity>();
                }
                else
                    return false;
            }
            else
            {
                targetRef = sm.EntityRef.To<IEntity>();
            }


            var result = await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get(targetRef.TypeId, targetRef.Guid))
                {
                    if (wrapper.AssertIfNull(nameof(wrapper)))
                        return false;

                    var entity = wrapper.Get<IEntity>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.ClientBroadcast);
                    if (entity is IHasStatsEngineClientBroadcast)
                    {
                        var stat = await ((IHasStatsEngineClientBroadcast)entity).Stats.GetBroadcastStat(def.StatResource);
                        var statMax = await ((IHasStatsEngineClientBroadcast)entity).Stats.GetBroadcastStat(def.MaxStatResource);
                        if (await statMax.GetValue() <= 0)
                            return false;

                        return compare(def.Operation, (await stat.GetValue()) / (await statMax.GetValue()), def.Percent);
                    }

                    return false;
                }
            }, repo);
            return result;
        }


        private static bool compare(BotCompareOperationType compareType, float value, float comparand)
        {
            switch (compareType)
            {
                case BotCompareOperationType.Equal:
                    return Math.Abs(value - comparand) < 0.01;
                case BotCompareOperationType.Greater:
                    return value > comparand;
                case BotCompareOperationType.Lesser:
                    return value < comparand;
                default:
                    return false;
            }
        }

        public static async Task<bool> BotCompareStatWithTargetStat(BotActionsStatesMachine2 sm, BotCompareStatWithTargetStatDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var repo = sm.Repository;
            OuterRef<IEntity> targetRef = default;
            if (!TryGetTargetEntityRef(localKnowledge, out targetRef))
                return false;

            OuterRef<IEntity> casterRef = sm.EntityRef.To<IEntity>();

            var result = await AsyncUtils.RunAsyncTask(async () =>
            {
                var batch = EntityBatch.Create().Add(targetRef.TypeId, targetRef.Guid).Add(casterRef.TypeId, casterRef.Guid);
                using (var wrapper = await repo.Get(batch))
                {
                    if (wrapper.AssertIfNull(nameof(wrapper)))
                        return false;

                    var targetEntity = wrapper.Get<IEntity>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.ClientBroadcast);
                    var casterEntity = wrapper.Get<IEntity>(casterRef.TypeId, casterRef.Guid, ReplicationLevel.ClientBroadcast);
                    IStat targetStat = default;
                    if (targetEntity is IHasStatsEngineClientBroadcast)
                        targetStat = await ((IHasStatsEngineClientBroadcast)targetEntity).Stats.GetBroadcastStat(def.StatResource);
                    else
                        return false;
                    IStat casterStat = default;
                    if (casterEntity is IHasStatsEngineClientBroadcast)
                        casterStat = await ((IHasStatsEngineClientBroadcast)casterEntity).Stats.GetBroadcastStat(def.StatResource);
                    else
                        return false;

                    return compare(def.Operation, await casterStat.GetValue() + def.DiffValue, await targetStat.GetValue());
                }
            }, repo);
            return result;
        }
        public static async Task<bool> BotTargetIsAlive(BotActionsStatesMachine2 sm, BotTargetIsAliveDef def,
            IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            OuterRef<IEntity> targetRef = default;
            if (localKnowledge.TryGetValue("Target", out var target))
            {
                if (((GameObject) target) == null)
                {
                    localKnowledge.Remove("Target");
                    return false;
                }

                var ego = ((GameObject)target).GetComponent<EntityGameObjectComponent>();
                if (ego != null)
                    targetRef = ego.GetOuterRef<IEntity>();
            }
            else
                return false;

            var repo = sm.Repository;
            var result = await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get(targetRef.TypeId, targetRef.Guid))
                {
                    if (wrapper.AssertIfNull(nameof(wrapper)))
                        return false;

                    var entity = wrapper.Get<IEntity>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.ClientBroadcast);
                    if (entity is IHasMortalClientBroadcast)
                        return ((IHasMortalClientBroadcast) entity).Mortal.IsAlive;
                    return false;
                }
            }, repo);
            return result;
        }

        public static async Task<bool> BotConsumeItem(BotActionsStatesMachine2 sm, BotConsumeItemDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var repo = sm.Repository;
            var entityId = sm.EntityId;
            var typeId = ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter));

            var result = await AsyncUtils.RunAsyncTask<bool>(async () =>
            {
                bool fromInventory;
                int spellGroupIndex = -1;
                using (var wrapper = await repo.Get<IWorldCharacterClientFull>(entityId))
                {
                    if (wrapper.AssertIfNull(nameof(wrapper)))
                        return false;

                    var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(entityId);
                    if (worldCharacter.AssertIfNull(nameof(worldCharacter)))
                        return false;

                    IItemsContainerClientFull items;
                    switch (def.Container)
                    {
                        case ContainerType.Doll:
                            items = worldCharacter.Doll;
                            fromInventory = false;
                            break;
                        case ContainerType.Inventory:
                            items = worldCharacter.Inventory;
                            fromInventory = true;
                            break;
                        default:
                            return false;
                    }

                    var itemResource = items.Items[def.Slot]?.Item?.ItemResource as ItemResource;
                    if (itemResource.AssertIfNull(nameof(itemResource)))
                        return false;

                    if (itemResource.ConsumableData.Target.AssertIfNull(nameof(itemResource.ConsumableData))
                        || !itemResource.ConsumableData.Target.HasSpells)
                    {
                         Logger.IfError()?.Message("ConsumableData is null or empty").Write();;
                        return false;
                    }

                    var spellGroups = itemResource.ConsumableData.Target.SpellsGroups;
                    for (int i = 0; i < spellGroups.Length; i++)
                    {
                        var spellGroup = spellGroups[i];
                        var spellRefs = spellGroup.Spells;
                        if (spellRefs.AssertIfNull(nameof(spellRefs)))
                            return false;
                        if (spellRefs.Any(spellRef => spellRef.Target == def.Spell.Target))
                        {
                            spellGroupIndex = i;
                            break;
                        }
                    }

                    if (spellGroupIndex == -1)
                        return false;

                    var worldCharacterConsumer = wrapper.Get<IHasConsumerClientFull>(typeId, entityId, ReplicationLevel.ClientFull)?.Consumer;
                    return await worldCharacterConsumer.ConsumeItemInSlot(def.Slot, spellGroupIndex, fromInventory);
                }
            }, repo);
            return result;
        }

        public static async Task<bool> BotAddOneOfItems(BotActionsStatesMachine2 sm, BotAddOneOfItemsDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            ClusterCheats.GetItemResourcePackFromString(def.ItemNames, def.Count, out List<ItemResourcePack> resources);
            var itemPack = resources[Mathf.RoundToInt(Random.value * (resources.Count - 1))];

            var repo = sm.Repository;
            var containerRef = new OuterRef<IEntity>(sm.EntityRef.Guid, sm.EntityRef.TypeId);
            if (!containerRef.IsValid)
                return false;

            var result = await AsyncUtils.RunAsyncTask(async () =>
            {
                var containerAddr = await BotActions2Support.GetPropertyAddress(containerRef, def.Container, repo);

                using (var cheatServiceWrapper = await GameState.Instance.ClientClusterNode.Get<ICheatServiceEntityServer>(GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId))
                {
                    if (cheatServiceWrapper.AssertIfNull(nameof(cheatServiceWrapper)))
                        return false;

                    var cheatServiceEntity = cheatServiceWrapper
                        .Get<ICheatServiceEntityServer>(GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId);

                    if (cheatServiceEntity.AssertIfNull(nameof(cheatServiceEntity)))
                        return false;

                    await cheatServiceEntity.AddItemsInSlot(itemPack, containerAddr, def.Slot);
                    return true;
                }
            }, repo);
            return result;
        }

        public static async Task<bool> BotCheckItemsInContainer(BotActionsStatesMachine2 sm, BotCheckItemsInContainerDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            ClusterCheats.GetItemResourcePackFromString(def.ItemNames, def.Count, out List<ItemResourcePack> itemsToFind);

            var repo = sm.Repository;
            var result = await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get(sm.EntityRef.TypeId, sm.EntityRef.Guid))
                {
                    if (wrapper.AssertIfNull(nameof(wrapper)))
                        return false;

                    IItemsContainerClientFull container = null;
                    switch (def.Container)
                    {
                        case ContainerType.Inventory:
                            container = wrapper.Get<IHasInventoryClientFull>(sm.EntityRef.TypeId, sm.EntityRef.Guid, ReplicationLevel.ClientFull).Inventory;
                            break;
                        case ContainerType.Doll:
                            container = wrapper.Get<IHasDollClientFull>(sm.EntityRef.TypeId, sm.EntityRef.Guid, ReplicationLevel.ClientFull).Doll;
                            break;
                    }

                    if (container == null)
                        return false;

                    foreach (var itemPack in itemsToFind)
                    {
                        var count = 0;
                        foreach (var slotItemClientFull in container.Items)
                        {
                            if (slotItemClientFull.Value.Item.ItemResource == itemPack.ItemResource)
                                count += slotItemClientFull.Value.Stack;
                        }

                        if (count < itemPack.Count)
                            return false;
                    }
                    return true;
                }
            }, repo);
            return result;
        }

        public static async Task<bool> BotCheckEmptyInventorySlots(BotActionsStatesMachine2 sm, BotCheckEmptyInventorySlotsDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var repo = sm.Repository;
            var result = await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get(sm.EntityRef.TypeId, sm.EntityRef.Guid))
                {
                    if (wrapper.AssertIfNull(nameof(wrapper)))
                        return false;

                    IItemsContainerClientFull container = wrapper.Get<IHasInventoryClientFull>(sm.EntityRef.TypeId, sm.EntityRef.Guid, ReplicationLevel.ClientFull).Inventory;
                    return container.Size - container.Items.Count >= def.Count;
                }
            }, repo);
            return result;
        }

        public static async Task<bool> BotCheckEmptyDollSlot(BotActionsStatesMachine2 sm, BotCheckEmptyDollSlotDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var repo = sm.Repository;
            var result = await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get(sm.EntityRef.TypeId, sm.EntityRef.Guid))
                {
                    if (wrapper.AssertIfNull(nameof(wrapper)))
                        return false;

                    IItemsContainerClientFull container = wrapper.Get<IHasDollClientFull>(sm.EntityRef.TypeId, sm.EntityRef.Guid, ReplicationLevel.ClientFull).Doll;
                    return !container.Items.ContainsKey(def.SlotDef.Target.SlotId);
                }
            }, repo);
            return result;
        }
        public static Task<bool> BotAddFlag(BotActionsStatesMachine2 sm, BotAddFlagDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            if (localKnowledge.ContainsKey(def.FlagName))
                return Task.FromResult(false);
            localKnowledge[def.FlagName] = true;
            return Task.FromResult(true);
        }
        public static Task<bool> BotCheckFlag(BotActionsStatesMachine2 sm, BotCheckFlagDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            return Task.FromResult(localKnowledge.ContainsKey(def.FlagName));
        }
        public static Task<bool> BotRemoveFlag(BotActionsStatesMachine2 sm, BotRemoveFlagDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var result = localKnowledge.Remove(def.FlagName);
            return Task.FromResult(result);
        }
        public static Task<bool> BotSetTimestamp(BotActionsStatesMachine2 sm, BotSetTimestampDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            localKnowledge[def.Name] = DateTime.UtcNow;
            return Task.FromResult(true);
        }
        public static Task<bool> BotRemoveTimestamp(BotActionsStatesMachine2 sm, BotRemoveTimestampDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var result = localKnowledge.Remove(def.Name);
            return Task.FromResult(result);
        }
        public static Task<bool> BotIsElapsedTimestamp(BotActionsStatesMachine2 sm, BotIsElapsedTimestampDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            object value;
            if (!localKnowledge.TryGetValue(def.Name, out value))
                return Task.FromResult(false);

            return Task.FromResult((DateTime.UtcNow - (DateTime)value).TotalSeconds >= def.Seconds);
        }
        public static async Task<bool> BotCheckMutation(BotActionsStatesMachine2 sm, BotCheckMutationDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var repo = sm.Repository;
            OuterRef<IEntity> targetRef = default;
            if (def.OnTarget)
            {
                if (!TryGetTargetEntityRef(localKnowledge, out targetRef))
                    return false;
            }
            else
            {
                targetRef = new OuterRef<IEntity>(sm.EntityRef.Guid, sm.EntityRef.TypeId);
            }

            var result = await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get(targetRef.TypeId, targetRef.Guid))
                {
                    if (wrapper.AssertIfNull(nameof(wrapper)))
                        return false;

                    var hasFaction = wrapper.Get<IHasMutationMechanicsClientBroadcast>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.ClientBroadcast);
                    if (hasFaction == null)
                        return false;

                    return hasFaction.MutationMechanics.Stage == def.Mutation;
                }
            }, repo);
            return result;
        }

        public static async Task<bool> BotTargetIsSameMutation(BotActionsStatesMachine2 sm, BotTargetIsSameMutationDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var repo = sm.Repository;
            OuterRef<IEntity> targetRef = default;
            OuterRef<IEntity> casterRef = default;
            if (!TryGetTargetEntityRef(localKnowledge, out targetRef))
                return false;
            casterRef = sm.EntityRef.To<IEntity>();

            var result = await AsyncUtils.RunAsyncTask(async () =>
            {
                var batch = EntityBatch.Create().Add(targetRef.TypeId, targetRef.Guid).Add(casterRef.TypeId, casterRef.Guid);
                using (var wrapper = await repo.Get(batch))
                {
                    if (wrapper.AssertIfNull(nameof(wrapper)))
                        return false;

                    var hasFactionTarget = wrapper.Get<IHasMutationMechanicsClientBroadcast>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.ClientBroadcast);
                    if (hasFactionTarget == null)
                        return false;

                    var hasFactionCaster = wrapper.Get<IHasMutationMechanicsClientBroadcast>(casterRef.TypeId, casterRef.Guid, ReplicationLevel.ClientBroadcast);
                    if (hasFactionCaster == null)
                        return false;

                    return hasFactionTarget.MutationMechanics.Stage == hasFactionCaster.MutationMechanics.Stage;
                }
            }, repo);
            return result;
        }

        public static async Task<bool> BotNot(BotActionsStatesMachine2 sm, BotNotDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var result = await InvokeAction(sm, def.Action, localKnowledge, cancellation);
            return !result;
        }

        public static async Task<bool> BotMoveItem(BotActionsStatesMachine2 sm, BotMoveItemDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var repo = sm.Repository;
            var fromToRef = new OuterRef<IEntity>(sm.EntityRef.Guid, sm.EntityRef.TypeId);
            if (!fromToRef.IsValid)
                return false;

            bool result = false;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var fromAddress = await BotActions2Support.GetPropertyAddress(fromToRef, def.FromContainer, repo);
                PropertyAddress toAddress;
                if (def.FromContainer == def.ToContainer)
                    toAddress = fromAddress;
                else
                    toAddress = await BotActions2Support.GetPropertyAddress(fromToRef, def.ToContainer, repo);

                if (fromAddress != null && toAddress != null)
                {
                    var batch = EntityBatch.Create().Add<IWorldCharacterClientFull>(sm.EntityId);
                    if (!batch.HasItem(fromAddress.EntityTypeId, fromAddress.EntityId))
                        batch.Add(fromAddress.EntityTypeId, fromAddress.EntityId);
                    if (!batch.HasItem(toAddress.EntityTypeId, toAddress.EntityId))
                        batch.Add(toAddress.EntityTypeId, toAddress.EntityId);
                    using (var wrapper = await NodeAccessor.Repository.Get(batch))
                    {
                        var characterEntity = wrapper.Get<IWorldCharacterClientFull>(sm.EntityId);
                        if (characterEntity == null)
                            return;

                        var fromEntityClientFullTypeId =
                            EntitiesRepository.GetReplicationTypeId(fromAddress.EntityTypeId, ReplicationLevel.ClientFull);
                        var fromEntity = wrapper.Get<IEntity>(fromEntityClientFullTypeId, fromAddress.EntityId);
                        if (fromEntity == null)
                            return;

                        var toEntityClientFullTypeId =
                            EntitiesRepository.GetReplicationTypeId(toAddress.EntityTypeId, ReplicationLevel.ClientFull);
                        var toEntity = wrapper.Get<IEntity>(toEntityClientFullTypeId, toAddress.EntityId);
                        if (toEntity == null)
                            return;

                        var fromContainer = EntityPropertyResolver.Resolve<IItemsContainerClientFull>(fromEntity, fromAddress);
                        var items = fromContainer.Items;
                        if (!fromContainer.Items.ContainsKey(def.FromSlot))
                            return;

                        Guid fromItemGuid = items[def.FromSlot].Item.Id;
                        var moveItemResult = await characterEntity.MoveItem(fromAddress, def.FromSlot, toAddress, def.ToSlot, def.Count, fromItemGuid);

                        result = moveItemResult.IsSuccess;
                    }
                }
            }, repo);
            return result;
        }


        public static async Task<bool> BotTryEquipItems(BotActionsStatesMachine2 sm, BotTryEquipItemsDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var names = def.ItemNames.Split('+');
            var itemResources = names.Select(x => GameResourcesHolder.Instance.LoadResource<BaseItemResource>(x)).ToList();

            var repo = sm.Repository;
            var fromToRef = new OuterRef<IEntity>(sm.EntityRef.Guid, sm.EntityRef.TypeId);
            if (!fromToRef.IsValid)
                return false;

            var result = await AsyncUtils.RunAsyncTask<bool>(async () =>
            {
                var fromAddress = await BotActions2Support.GetPropertyAddress(fromToRef, ContainerType.Inventory, repo);
                var toAddress = await BotActions2Support.GetPropertyAddress(fromToRef, ContainerType.Doll, repo);

                if (fromAddress != null && toAddress != null)
                {
                    var equipped = false;
                    using (var wrapper = await repo.Get<IWorldCharacterClientFull>(sm.EntityId))
                    {
                        var characterEntity = wrapper.Get<IWorldCharacterClientFull>(sm.EntityId);
                        if (characterEntity == null)
                            return false;

                        var dollSlotsList = GameResourcesHolder.Instance.LoadResource<SlotsListDef>("/UtilPrefabs/Slots/CharacterDollSlots");

                        foreach (var slotItemClientFull in characterEntity.Inventory.Items.ToList())
                        {
                            if (itemResources.Contains(slotItemClientFull.Value.Item.ItemResource))
                            {
                                Guid fromItemGuid = slotItemClientFull.Value.Item.Id;
                                foreach (var slotId in getItemDollSlots(dollSlotsList,
                                    slotItemClientFull.Value.Item.ItemResource.ItemType.Target))
                                {
                                    if (characterEntity.Doll.Items.ContainsKey(slotId))
                                        continue;

                                    var moveItemResult = await characterEntity.MoveItem(fromAddress, slotItemClientFull.Key, toAddress,
                                        slotId, 1, fromItemGuid);
                                    equipped |= moveItemResult.Result == ContainerItemOperationResult.Success;
                                    if (moveItemResult.Result == ContainerItemOperationResult.Success)
                                        break;
                                }
                            }
                        }

                        return equipped; 
                    }
                }

                return false;
            }, repo);
            return result;
        }

        private static List<int> getItemDollSlots(SlotsListDef dollSlotsList, ItemTypeResource itemType)
        {
            var result = new List<int>();
            foreach (var dollSlot in dollSlotsList.Slots)
            {
                if (dollSlot.Target.AcceptsItems.Any(x => x == itemType))
                    result.Add(dollSlot.Target.SlotId);
                
            }
            return result;
        }

        public static Task<bool> BotPinTargetPosition(BotActionsStatesMachine2 sm, BotPinTargetPositionDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            if (!localKnowledge.TryGetValue("Target", out object targetObj))
                return Task.FromResult(false);

            var target = (GameObject)targetObj;
            var targetPosition = target.transform.position;

            localKnowledge["Point"] = targetPosition;
            return Task.FromResult(true);
        }

        public static async Task<bool> BotMoveToTarget(BotActionsStatesMachine2 sm, BotGoToTargetDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            if (!localKnowledge.TryGetValue("Target", out object targetObj))
                return false;

            var target = (GameObject)targetObj;
            if (target == null || sm.Transform == null)
                return false;

            var _targetPosition = target.transform.position;

            var lastCoord = sm.Transform.position;
            var startTime = Time.realtimeSinceStartup;
            var lastTime = startTime;

            try
            {
                while ((_targetPosition - sm.Transform.position).ToXZ().sqrMagnitude > 1)
                {
                    Vector2 camFlatDirection = sm._camera.ToXZ().normalized;

                    _targetPosition = target.transform.position;
                    Vector2 direction = (_targetPosition - sm.Transform.position).ToXZ().normalized;

                    var projModOnCamForwardDirection = Vector2.Dot(direction, camFlatDirection);
                    var projModOnCamLateralDirection = Vector2.Dot(direction, -Vector2.Perpendicular(camFlatDirection));
                    Vector2 projOnCamBasis = new Vector2(projModOnCamForwardDirection, projModOnCamLateralDirection).normalized;

                    sm.SetValue(MoveForward, Mathf.Max(projOnCamBasis.x, 0));
                    sm.SetValue(MoveBackward, Mathf.Max(-projOnCamBasis.x, 0));
                    sm.SetValue(MoveRight, Mathf.Max(projOnCamBasis.y, 0));
                    sm.SetValue(MoveLeft, Mathf.Max(-projOnCamBasis.y, 0));

                    await Task.Delay(TimeSpan.FromSeconds(def.UpdatePeriod), cancellation);

                    if (def.TimeoutSeconds > 0 && Time.realtimeSinceStartup - startTime > def.TimeoutSeconds) 
                        return false;

                    if (Time.realtimeSinceStartup - lastTime > 5)
                    {
                        var lastDist = (_targetPosition - lastCoord).ToXZ().sqrMagnitude;
                        var dist = (_targetPosition - sm.Transform.position).ToXZ().sqrMagnitude;
                        if (lastDist <= dist)
                        {
                            // Logger.IfError()?.Message("Bot {0}: I cannot reach target at point {1}",  sm.EntityId, _targetPosition).Write();
                            return false;
                        }

                        lastCoord = sm.Transform.position;
                        lastTime = Time.realtimeSinceStartup;
                    }
                }
            }
            finally
            {
                sm.SetValue(MoveForward, 0);
                sm.SetValue(MoveBackward, 0);
                sm.SetValue(MoveLeft, 0);
                sm.SetValue(MoveRight, 0);
            }
            return true;
        }

        public static async Task<bool> BotGoToPoint(BotActionsStatesMachine2 sm, BotGoToPointDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            Vector3 _targetPosition;
            if (def.FromKnowledge)
            {
                if (!localKnowledge.TryGetValue("Point", out object targetPointObj))
                    return false;
                _targetPosition = (Vector3)targetPointObj;
            }
            else
            {
                _targetPosition = (Vector3) def.Point;
            }

            var lastCoord = sm.Transform.position;
            var startTime = Time.realtimeSinceStartup;
            var lastTime = startTime;

            try
            {
                while ((_targetPosition - sm.Transform.position).ToXZ().sqrMagnitude > 1)
                {
                    Vector2 camFlatDirection = sm._camera.ToXZ().normalized;

                    Vector2 direction = (_targetPosition - sm.Transform.position).ToXZ().normalized;

                    var projModOnCamForwardDirection = Vector2.Dot(direction, camFlatDirection);
                    var projModOnCamLateralDirection = Vector2.Dot(direction, -Vector2.Perpendicular(camFlatDirection));
                    Vector2 projOnCamBasis = new Vector2(projModOnCamForwardDirection, projModOnCamLateralDirection).normalized;

                    sm.SetValue(MoveForward, Mathf.Max(projOnCamBasis.x, 0));
                    sm.SetValue(MoveBackward, Mathf.Max(-projOnCamBasis.x, 0));
                    sm.SetValue(MoveRight, Mathf.Max(projOnCamBasis.y, 0));
                    sm.SetValue(MoveLeft, Mathf.Max(-projOnCamBasis.y, 0));

                    await Task.Delay(TimeSpan.FromSeconds(def.UpdatePeriod), cancellation);

                    if (def.TimeoutSeconds > 0 && Time.realtimeSinceStartup - startTime > def.TimeoutSeconds)
                        return false;

                    if (Time.realtimeSinceStartup - lastTime > 5)
                    {
                        var lastDist = (_targetPosition - lastCoord).ToXZ().sqrMagnitude;
                        var dist = (_targetPosition - sm.Transform.position).ToXZ().sqrMagnitude;
                        if (lastDist <= dist)
                        {
                            // Logger.IfError()?.Message("Bot {0}: I cannot reach target at point {1}",  sm.EntityId, _targetPosition).Write();
                            return false;
                        }

                        lastCoord = sm.Transform.position;
                        lastTime = Time.realtimeSinceStartup;
                    }
                }
            }
            finally
            {
                sm.SetValue(MoveForward, 0);
                sm.SetValue(MoveBackward, 0);
                sm.SetValue(MoveLeft, 0);
                sm.SetValue(MoveRight, 0);
            }
            return true;
        }

        public static async Task<bool> BotDoInputActions(BotActionsStatesMachine2 sm, BotDoInputActionsDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var causer = new object();
            foreach (var defInputAction in def.InputActions)
                sm.PushTrigger(causer, defInputAction);

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(def.DurationSeconds), cancellation);
            }
            finally
            {
                foreach (var defInputAction in def.InputActions)
                    sm.PopTrigger(causer, defInputAction);
            }
            return true;
        }

        public static async Task<bool> BotDoActionRepeated(BotActionsStatesMachine2 sm, BotDoRepeatedDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            for (int i = 0; i < def.Times; ++i)
            {
                if (!await InvokeAction(sm, def.Action, localKnowledge, cancellation))
                    return false;

                await Task.Delay(TimeSpan.FromSeconds(def.IntervalSeconds), cancellation);
            }

            return true;
        }

        public static async Task<bool> BotDoIf(BotActionsStatesMachine2 sm, BotDoIf def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var ifCheck = await InvokeAction(sm, def.If, localKnowledge, cancellation);
            if (ifCheck)
            {
                if (def.Then != default)
                    return await InvokeAction(sm, def.Then, localKnowledge, cancellation);
            }
            else
            {
                if (def.Else != default)
                    return await InvokeAction(sm, def.Else, localKnowledge, cancellation);
            }

            return true;
        }

        public static async Task<bool> BotDropItem(BotActionsStatesMachine2 sm, BotDropItemDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var repo = sm.Repository;
            var fromToRef = new OuterRef<IEntity>(sm.EntityRef.Guid, sm.EntityRef.TypeId);
            if (!fromToRef.IsValid)
                return false;

            var result = await AsyncUtils.RunAsyncTask<bool>(async () =>
            {
                var fromAddress = await BotActions2Support.GetPropertyAddress(fromToRef, def.Container, repo);
                if (fromAddress != null)
                {
                    using (var wrapper = await repo.Get(fromToRef.TypeId, fromToRef.Guid))
                    {
                        var characterEntity = wrapper.Get<IWorldCharacterClientFull>(sm.EntityId);
                        if (characterEntity == null)
                            return false;

                        var container = def.Container == ContainerType.Doll ? (IItemsContainerClientFull)characterEntity.Doll : (IItemsContainerClientFull)characterEntity.Inventory;
                        var items = container.Items;

                        if (!items.ContainsKey(def.Slot))
                            return false;

                        var moveItemResult = await characterEntity.ContainerApi.Drop(fromAddress, def.Slot, def.Count);

                        return moveItemResult.IsSuccess;
                    }
                }

                return false;
            }, repo);
            return result;
        }

        public static async Task<bool> BotDoInParallel(BotActionsStatesMachine2 sm, BotDoInParallelDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            if (def.WaitForAll)
            {
                var tasks = def.Actions.Select(v => InvokeAction(sm, v, localKnowledge, cancellation)).ToArray();
                var results = await Task.WhenAll(tasks);
                return results.All(v => v);
            }
            else
            {
                bool result = false;
                var newCts = new CancellationTokenSource();
                cancellation.Register(() => newCts.Cancel());

                var tasks = def.Actions.Select(v => InvokeAction(sm, v, localKnowledge, newCts.Token)).ToArray();
                foreach (var bucket in tasks.Interleaved())
                {
                    var task = await bucket;
                    try
                    {
                        var taskResult = await task;
                        if (taskResult)
                        {
                            result = taskResult;
                            newCts.Cancel();
                            await Task.WhenAll(tasks);
                            break;
                        }
                        else if (!def.DoNotStopOnFail)
                            newCts.Cancel();
                    }
                    catch (OperationCanceledException) { }
                }

                return result;
            }
        }

        public static async Task<bool> BotDoInSequence(BotActionsStatesMachine2 sm, BotDoInSequenceDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var result = !def.Or;
            foreach (var action in def.Actions)
            {
                var res = await InvokeAction(sm, action, localKnowledge, cancellation);
                if (def.Or)
                    result |= res;
                else
                    result &= res;
            }

            return result;
        }

        public static async Task<bool> BotDoInLoop(BotActionsStatesMachine2 sm, BotDoInLoopDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var loopLeft = def.LoopCount;
            while (true)
            {
                foreach (var action in def.Actions)
                {
                    if (!await InvokeAction(sm, action, localKnowledge, cancellation) && !def.DoNotStopOnFail)
                        return false;
                    cancellation.ThrowIfCancellationRequested();
                }
                
                if (loopLeft > 0)
                {
                    loopLeft--;
                    if (loopLeft == 0)
                        break;
                }
            }

            return true;
        }

        public static async Task<bool> BotDoWhile(BotActionsStatesMachine2 sm, BotDoWhileDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            while (await InvokeAction(sm, def.While, localKnowledge, cancellation))
            {
                await InvokeAction(sm, def.Action, localKnowledge, cancellation);
                cancellation.ThrowIfCancellationRequested();
            }

            return true;
        }

        public static Task<bool> BotDoRandom(BotActionsStatesMachine2 sm, BotDoRandomDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var random = new System.Random();
            var idx = random.Next(def.Actions.Length);

            return InvokeAction(sm, def.Actions[idx], localKnowledge, cancellation);
        }

        public static async Task<bool> BotDoRandomWeigth(BotActionsStatesMachine2 sm, BotDoRandomWeigthDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var random = new System.Random();
            var weigthSum = (double)def.Actions.Sum(x => x.Target.Weigth);

            var selectedWeigth = random.NextDouble();
            double currentWeigth = 0f;
            foreach (var resourceRef in def.Actions)
            {
                currentWeigth += resourceRef.Target.Weigth;
                if (currentWeigth / weigthSum >= selectedWeigth)
                {
                    return await InvokeAction(sm, resourceRef.Target.Action, localKnowledge, cancellation);
                }
            }

            return false;
        }

        public static async Task<bool> BotCheckInteractiveAction(BotActionsStatesMachine2 sm, BotCheckInteractiveDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            var target = (GameObject)localKnowledge["Target"];
            var spellDoer = sm.SpellDoer;
            var interactiveTarget = target.Kind<Interactive>();
            if (!interactiveTarget)
                return false;

            var repo = sm.Repository;
            var spell = await AsyncUtils.RunAsyncTask(() => interactiveTarget.ChooseSpell(spellDoer, SpellDescription.AttackAction), repo);

            return spell != default(SpellDef);
        }

        public static async Task<bool> BotPing(BotActionsStatesMachine2 sm, BotPingDef def, IDictionary<string, object> localKnowledge, CancellationToken cancellation)
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return false;

            var pingTasks = new List<SuspendingAwaitable>();

            var remoteRepoId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;

            var characterId = sm.EntityId;
            if (Guid.Empty == characterId)
            {
                Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
                return false;
            }

            pingTasks.Add(AsyncUtils.RunAsyncTask(async () =>
            {
                var startTime = DateTime.UtcNow;
                using (var wrapper = await repo.Get<IRepositoryCommunicationEntityClientFull>(remoteRepoId))
                {
                    var entity = wrapper.Get<IRepositoryCommunicationEntityClientFull>(remoteRepoId);
                    var result = await entity.PingDiagnostics.PingRead();
                    if (result)
                    {
                        var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
                        if ((duration > def.WarningTime && Logger.IsWarnEnabled) || (duration > def.InfoTime && Logger.IsInfoEnabled))
                        {
                            var sb = new StringBuilder();
                            sb.AppendLine("PingReadUnityRepositoryEntity");
                            sb.AppendFormat("ping: {0} msec", duration).AppendLine();
                            if (duration > def.WarningTime)
                                Logger.IfWarn()?.Message(sb.ToString()).Write();
                            else
                                Logger.IfInfo()?.Message(sb.ToString()).Write();
                        }
                    }

                }
            }, repo));

            pingTasks.Add(AsyncUtils.RunAsyncTask(async () =>
            {
                var startTime = DateTime.UtcNow;
                using (var wrapper = await repo.Get<IRepositoryCommunicationEntityClientFull>(remoteRepoId))
                {
                    var entity = wrapper.Get<IRepositoryCommunicationEntityClientFull>(remoteRepoId);
                    var result = await entity.PingDiagnostics.PingWrite();
                    if (result)
                    {
                        var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
                        if ((duration > def.WarningTime && Logger.IsWarnEnabled) || (duration > def.InfoTime && Logger.IsInfoEnabled))
                        {
                            var sb = new StringBuilder();
                            sb.AppendLine("PingWriteUnityRepositoryEntity");
                            sb.AppendFormat("ping: {0} msec", duration).AppendLine();
                            if (duration > def.WarningTime)
                                Logger.IfWarn()?.Message(sb.ToString()).Write();
                            else
                                Logger.IfInfo()?.Message(sb.ToString()).Write();
                        }
                    }

                }
            }, repo));

            pingTasks.Add(AsyncUtils.RunAsyncTask(async () =>
            {
                var startTime = DateTime.UtcNow;
                using (var wrapper = await repo.Get<IWorldCharacterClientFull>(characterId))
                {
                    var entity = wrapper.Get<IWorldCharacterClientFull>(characterId);
                    var result = await entity.PingDiagnostics.PingRead();
                    if (result)
                    {
                        var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
                        if ((duration > def.WarningTime && Logger.IsWarnEnabled) || (duration > def.InfoTime && Logger.IsInfoEnabled))
                        {
                            var sb = new StringBuilder();
                            sb.AppendLine("PingReadCharacter");
                            sb.AppendFormat("ping: {0} msec", duration).AppendLine();
                            if (duration > def.WarningTime)
                                Logger.IfWarn()?.Message(sb.ToString()).Write();
                            else
                                Logger.IfInfo()?.Message(sb.ToString()).Write();
                        }
                    }
                }
            }, repo));

            pingTasks.Add(AsyncUtils.RunAsyncTask(async () =>
            {
                var startTime = DateTime.UtcNow;
                using (var wrapper = await repo.Get<IWorldCharacterClientFull>(characterId))
                {
                    var entity = wrapper.Get<IWorldCharacterClientFull>(characterId);
                    var result = await entity.PingDiagnostics.PingWrite();
                    if (result)
                    {
                        var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
                        if ((duration > def.WarningTime && Logger.IsWarnEnabled) || (duration > def.InfoTime && Logger.IsInfoEnabled))
                        {
                            var sb = new StringBuilder();
                            sb.AppendLine("PingWriteCharacter");
                            sb.AppendFormat("ping: {0} msec", duration).AppendLine();
                            if (duration > def.WarningTime)
                                Logger.IfWarn()?.Message(sb.ToString()).Write();
                            else
                                Logger.IfInfo()?.Message(sb.ToString()).Write();
                        }
                    }
                }
            }, repo));

            pingTasks.Add(AsyncUtils.RunAsyncTask(async () =>
            {
                var startTime = DateTime.UtcNow;
                using (var wrapper = await repo.Get<IWorldCharacterClientFull>(characterId))
                {
                    var characterEntity = wrapper.Get<IWorldCharacterClientFull>(characterId);
                    using (var wrapper2 = await repo.Get<IWizardEntityClientFull>(characterEntity.Wizard.Id))
                    {
                        var entity = wrapper2.Get<IWizardEntityClientFull>(characterEntity.Wizard.Id);
                        var result = await entity.PingDiagnostics.PingRead();
                        if (result)
                        {
                            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
                            if ((duration > def.WarningTime && Logger.IsWarnEnabled) || (duration > def.InfoTime && Logger.IsInfoEnabled))
                            {
                                var sb = new StringBuilder();
                                sb.AppendLine("PingReadWizard");
                                sb.AppendFormat("ping: {0} msec", duration).AppendLine();
                                if (duration > def.WarningTime)
                                    Logger.IfWarn()?.Message(sb.ToString()).Write();
                                else
                                    Logger.IfInfo()?.Message(sb.ToString()).Write();
                            }
                        }
                    }
                }
            }, repo));

            pingTasks.Add(AsyncUtils.RunAsyncTask(async () =>
            {
                var startTime = DateTime.UtcNow;
                using (var wrapper = await repo.Get<IWorldCharacterClientFull>(characterId))
                {
                    var characterEntity = wrapper.Get<IWorldCharacterClientFull>(characterId);
                    using (var wrapper2 = await repo.Get<IWizardEntityClientFull>(characterEntity.Wizard.Id))
                    {
                        var entity = wrapper2.Get<IWizardEntityClientFull>(characterEntity.Wizard.Id);
                        var result = await entity.PingDiagnostics.PingWrite();
                        if (result)
                        {
                            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
                            if ((duration > def.WarningTime && Logger.IsWarnEnabled) || (duration > def.InfoTime && Logger.IsInfoEnabled))
                            {
                                var sb = new StringBuilder();
                                sb.AppendLine("PingWriteWizard");
                                sb.AppendFormat("ping: {0} msec", duration).AppendLine();
                                if (duration > def.WarningTime)
                                    Logger.IfWarn()?.Message(sb.ToString()).Write();
                                else
                                    Logger.IfInfo()?.Message(sb.ToString()).Write();
                            }
                        }
                    }
                }
            }, repo));

            await SuspendingAwaitable.WhenAll(pingTasks);
            return true;
        }
    }
}
