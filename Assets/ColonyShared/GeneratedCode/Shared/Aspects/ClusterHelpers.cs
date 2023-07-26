using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using Assets.Src.Arithmetic;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using SharedCode.Logging;
using EnumerableExtensions;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using ResourceSystem.Utils;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using SharedCode.MapSystem;
using SharedCode.Wizardry;

namespace ColonyShared.GeneratedCode.Shared.Aspects
{
    public static class ClusterHelpers
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public static KeyValuePair<BaseItemResource, PropertyAddress> GetActiveWeaponResource(IHasDollClientBroadcast hasDoll)
        {
            if (hasDoll.Doll.UsedSlots.Count > 0)
            {
                var slotResId = hasDoll.Doll.UsedSlots[0];
                var usedIndex = ItemHelper.TmpPlug_GetIndexBySlotResourceIdFull(slotResId);
                if (hasDoll.Doll.Items.ContainsKey(usedIndex))
                {
                    var usedItem = hasDoll.Doll.Items[usedIndex];
                    var propertyAddress = EntityPropertyResolver.GetPropertyAddress(hasDoll.Doll.Items[usedIndex].Item);
                    return new KeyValuePair<BaseItemResource, PropertyAddress>(usedItem.Item.ItemResource, propertyAddress);
                }
            }

            return default(KeyValuePair<BaseItemResource, PropertyAddress>);
        }

        public static async Task UseWizard(OuterRef<IHasWizardEntityServer> casterRef, IEntitiesRepository repo, Func<IWizardEntityServer,Task> action)
        {
            OuterRef wizardRef = default;
            using(var c1 = await repo.Get(casterRef.TypeId, casterRef.Guid))
            {
                var hasWizard = c1.Get<IHasWizardEntityServer>(casterRef.TypeId, casterRef.Guid, ReplicationLevel.Server);
                if (hasWizard != null)
                    wizardRef = hasWizard.Wizard.OuterRef;
                else
                    Log.Logger.IfError()?.Message($"{casterRef} is not a {nameof(IHasWizardEntityServer)}").Write();
            }

            if (wizardRef.IsValid)
                using (var c2 = await repo.Get(wizardRef.TypeId, wizardRef.Guid))
                {
                    var wizard = c2.Get<IWizardEntityServer>(wizardRef, ReplicationLevel.Server);
                    if (wizard == null) throw new NullReferenceException($"HasWizardEntity {casterRef} with invalid wizard");
                    await action(wizard);
                }
        }

        public static async Task UseWizard(OuterRef<IHasWizardEntityClientFull> casterRef, IEntitiesRepository repo, Func<IWizardEntityClientFull,Task> action)
        {
            OuterRef wizardRef = default;
            using(var c1 = await repo.Get(casterRef.TypeId, casterRef.Guid))
            {
                var hasWizard = c1.Get<IHasWizardEntityClientFull>(casterRef, ReplicationLevel.ClientFull);
                if (hasWizard != null)
                    wizardRef = hasWizard.Wizard.OuterRef;
                else
                    Log.Logger.IfError()?.Message($"{casterRef} is not a {nameof(IHasWizardEntityClientFull)}").Write();
            }

            if (wizardRef.IsValid)
                using (var c2 = await repo.Get(wizardRef.TypeId, wizardRef.Guid))
                {
                    var wizard = c2.Get<IWizardEntityClientFull>(wizardRef, ReplicationLevel.ClientFull);
                    if (wizard == null) throw new NullReferenceException($"{casterRef} with invalid wizard");
                    await action(wizard);
                }
        }
        
        public static Task UseWizard(OuterRef<IHasWizardEntity> casterRef, IEntitiesRepository repo, Func<IWizardEntityServer,Task> action)
        {
            return UseWizard(casterRef.To<IHasWizardEntityServer>(), repo, action);
        }
        
        public static async Task Use<T>(OuterRef<T> casterRef, IEntitiesRepository repo, ReplicationLevel level, Func<T,Task> action)
        {
            using(var c1 = await repo.Get(casterRef.TypeId, casterRef.Guid))
            {
                if(c1 != null)
                {
                    var entity = c1.Get(casterRef, level);
                    if(entity != null)
                        await action(entity);
                    else
                        Log.Logger.IfError()?.Message($"{casterRef} is not a {typeof(T)}").Write();
                }
            }
        }

        public static async Task Use<T>(OuterRef<IEntity> casterRef, IEntitiesRepository repo, ReplicationLevel level, Func<T,Task> action)
        {
            using(var c1 = await repo.Get(casterRef.TypeId, casterRef.Guid))
            {
                if(c1 != null)
                {
                    var entity = c1.Get<T>(casterRef, level);
                    if(entity != null)
                        await action(entity);
                    else
                        Log.Logger.IfError()?.Message($"{casterRef} is not a {typeof(T)}").Write();
                }
            }
        }
        
        public static async Task UseIf<T>(OuterRef<T> casterRef, IEntitiesRepository repo, ReplicationLevel level, Func<T,Task> action)
        {
            using(var c1 = await repo.Get(casterRef.TypeId, casterRef.Guid))
            {
                if(c1 != null)
                {
                    var entity = c1.Get(casterRef, level);
                    if(entity != null)
                        await action(entity);
                }
            }
        }

        public static async Task UseIf<T>(OuterRef casterRef, IEntitiesRepository repo, ReplicationLevel level, Func<T,Task> action)
        {
            using(var c1 = await repo.Get(casterRef.TypeId, casterRef.Guid))
            {
                if(c1 != null)
                {
                    var entity = c1.Get<T>(casterRef, level);
                    if(entity != null)
                        await action(entity);
                }
            }
        }

        public static async Task<Guid> GetUnityIdByChar(OuterRef<IEntity> charRef, IEntitiesRepository repo)
        {
            // 1. Get worldSpace by char:
            OuterRef<IWorldSpaceServiceEntity> ownWorldSpace;
            using (var charWrapper = await repo.Get(charRef))
            {
                var @char = charWrapper.Get<IWorldCharacterAlways>(charRef, ReplicationLevel.Always);
                if (@char == null)
                {
                    Logger.IfError()?.Message($"Can't get {nameof(IWorldCharacterAlways)} by {charRef}.").Write();
                    return Guid.Empty;
                }
                ownWorldSpace = @char.WorldSpaced.OwnWorldSpace;
            }

            // 2. Get ownMap by worldSpace
            OuterRef<IMapEntity> ownMap;
            using (var wrapper = await repo.Get(ownWorldSpace))
            {
                var worldSpace = wrapper.Get<IWorldSpaceServiceEntityAlways>(ownWorldSpace, ReplicationLevel.Always);
                if (worldSpace == null)
                {
                    Logger.IfError()?.Message($"Can't get {nameof(IWorldSpaceServiceEntityAlways)} by {ownWorldSpace}.").Write();
                    return Guid.Empty;
                }
                ownMap = worldSpace.OwnMap;
            }

            // 3. Get unityId by ownMap
            Guid unityId;
            using (var wrapper = await repo.Get(ownMap))
            {
                var map = wrapper.Get<IMapEntityServer>(ownMap, ReplicationLevel.Server);
                if (map == null)
                {
                    Logger.IfError()?.Message($"Can't get {nameof(IMapEntityServer)} by {ownMap}.").Write();
                    return Guid.Empty;
                }
                var worldSpaceDescr = map.WorldSpaces.FirstOrDefault(x => x.WorldSpaceGuid == ownWorldSpace.Guid);
                if (worldSpaceDescr == null)
                {
                    Logger.IfError()?.Message($"Can't find elem by {ownWorldSpace.Guid} in {nameof(map.WorldSpaces)}: {map.WorldSpaces.ItemsToStringByLines()}.").Write();
                    return Guid.Empty;
                }

                unityId = worldSpaceDescr.UnityRepositoryId;
            }

            return unityId;
        }


        public static async Task<List<ItemResourcePack>> ResolveDefaultItems(IReadOnlyList<DefaultItemsStack> items, CalcerContext calcerCtx)
        {
            var itemsPack = new List<ItemResourcePack>(items.Count);
            foreach (var entry in items)
            {
                if (entry != null)
                {
                    BaseItemResource item = null;
                    if (entry.ItemCalcer != null)
                    {
                        var res = await entry.ItemCalcer.Target.CalcAsync(calcerCtx);
                        if (res != null)
                        {
                            if (res is BaseItemResource i)
                                item = i;
                            else
                                Logger.IfError()?.Write(calcerCtx.EntityRef.Guid, $"DefaultItemsStack calcer returned not an Item! Calcer:{entry.ItemCalcer.Target} Result:{res}");
                        }
                    }
                    else
                        item = entry.Item.Target;

                    if (item != null)
                        itemsPack.Add(new ItemResourcePack(item, entry.Count, entry.Slot.Target?.SlotId ?? -1));
                }
            }
            return itemsPack;
        }

        /*        
        public static async Task<ResultContainer<IWizardEntityServer>> GetWizard(OuterRef<IEntity> casterRef, IEntitiesRepository repo)
        {
            IEntitiesContainer c1 = null, c2 = null;
            try
            {
                c1 = await repo.Get(casterRef.TypeId, casterRef.Guid);
                var hasWizard = c1.Get<IHasWizardEntityServer>(casterRef.TypeId, casterRef.Guid, ReplicationLevel.Server);
                c2 = await repo.Get<IWizardEntityServer>(hasWizard.Wizard.Id);
                var wizard = c2.Get<IWizardEntityServer>(hasWizard.Wizard.Id);
                return new ResultContainer<IWizardEntityServer>(wizard, c2, c1);
            }
            catch (Exception)
            {
                c2?.Dispose();
                c1?.Dispose();
                throw;
            }
        }
        
        public class ResultContainer<TEntity> : IDisposable
        {
            private IEntitiesContainer[] _containers;

            public TEntity Entity { get; }

            public ResultContainer(TEntity entity, params IEntitiesContainer[] containers)
            {
                Entity = entity;
                _containers = containers;
            }

            public void Dispose()
            {
                foreach (var container in _containers)
                    container.Dispose();
            }
        }
    */
    }
}