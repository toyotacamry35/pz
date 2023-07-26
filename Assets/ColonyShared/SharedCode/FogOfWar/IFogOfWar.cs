using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.GeneratedCode.Regions;
using Assets.Src.SpatialSystem;
using Core.Environment.Logging.Extension;
using GeneratedCode.EntitySystem;
using GeneratorAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using ResourceSystem.Utils;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Aspects.Regions;
using SharedCode.Entities;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Serializers;

namespace SharedCode.FogOfWar
{
	[GenerateDeltaObjectCode]
	public interface IFogOfWar : IDeltaObject
	{
		[ReplicationLevel(ReplicationLevel.ClientFull)]
		IDeltaDictionary<IndexedRegionDef, bool> Regions { get; set; }

		[BsonIgnore]
		[ReplicationLevel(ReplicationLevel.ClientFull)]
		IDeltaList<IndexedRegionDef> CurrentRegions { get; set; }

		[ReplicationLevel(ReplicationLevel.ClientFull)]
		IDeltaDictionary<IndexedRegionGroupDef, bool> RegionGroups { get; set; }

		[BsonIgnore]
		[ReplicationLevel(ReplicationLevel.ClientFull)]
		IDeltaList<IndexedRegionGroupDef> CurrentGroups { get; set; }

		[ReplicationLevel(ReplicationLevel.Server)]
		Task<bool> DiscoverRegion(IndexedRegionDef region);

		[ReplicationLevel(ReplicationLevel.ClientFull)]
		Task<bool> SetRegionVisited(IndexedRegionDef region);

		[ReplicationLevel(ReplicationLevel.Server)]
		Task<bool> DiscoverGroup(IndexedRegionGroupDef group);

		[ReplicationLevel(ReplicationLevel.ClientFull)]
		Task<bool> SetGroupVisited(IndexedRegionGroupDef group);

		[ReplicationLevel(ReplicationLevel.ClientFull)]
		Task<float> GetGroupPercent(IndexedRegionGroupDef group);

		[CheatRpc(AccountType.GameMaster)]
		[ReplicationLevel(ReplicationLevel.ClientBroadcast)]
		Task ClearRegions();
	}

	public interface IHasFogOfWar
	{
		[ReplicationLevel(ReplicationLevel.ClientFull)]
		IFogOfWar FogOfWar { get; set; }
	}
}

namespace GeneratedCode.DeltaObjects
{
	public partial class FogOfWar : IHookOnStart, IHookOnDestroy, IHookOnUnload, IHookOnDatabaseLoad
	{
		private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

		private bool _destroyed;

		public async Task OnDatabaseLoad()
		{
			using (await this.GetThisExclusive())
			{
				CurrentRegions = CurrentRegions ?? new DeltaList<IndexedRegionDef>();
				CurrentGroups = CurrentGroups ?? new DeltaList<IndexedRegionGroupDef>();
			}
		}

		public Task OnStart()
		{
			AsyncUtils.RunAsyncTask(Watcher, EntitiesRepository);
			return Task.CompletedTask;
		}

		//Discover Region Watcher
		private async Task Watcher()
		{
			var parentRef = new OuterRef(ParentEntityId, ParentTypeId);
			var fogRegions = new List<IIndexRegion>();
			var regions = new List<IRegion>();
			var regionDefs = new List<ARegionDef>();

			var addRegions = new List<IndexedRegionDef>();
			var removeRegions = new List<IndexedRegionDef>();
			var addGroups = new List<IndexedRegionGroupDef>();
			var removeGroups = new List<IndexedRegionGroupDef>();

			var discoverRegions = new List<IndexedRegionDef>();
			var discoverGroups = new List<IndexedRegionGroupDef>();

			while (!_destroyed)
			{
				await Task.Delay(TimeSpan.FromSeconds(Constants.WorldConstants.FogOfWarRefreshPeriod));
				using (var entitiesContainer = await EntitiesRepository.Get(ParentTypeId, ParentEntityId))
				{
					var positionedEntity =
						PositionedObjectHelper.GetPositioned(entitiesContainer, parentRef.TypeId, parentRef.Guid);
					if (positionedEntity == null)
						continue;
					var ownerScene = EntitiesRepository.GetSceneForEntity(new OuterRef<IEntity>(parentRef));
					if (ownerScene == default)
						continue;
					var rootRegion = RegionsHolder.GetRegionByGuid(ownerScene);

					fogRegions.Clear();
					GetRegions(
						rootRegion,
						indexRegion =>
							indexRegion.RegionDef?.Data?
								.Any(resourceRef => resourceRef.Target is FogOfWarRegionDef) ?? false,
						ref fogRegions
					);

					regions.Clear();
					foreach (var fogRegion in fogRegions)
						fogRegion.GetAllContainingRegionsNonAlloc(regions, positionedEntity.Position);

					addRegions.Clear();
					addGroups.Clear();
					regionDefs.Clear();
					discoverRegions.Clear();
					discoverGroups.Clear();
					foreach (var def in regions.Select(region => region.RegionDef))
					{
						regionDefs.Add(def);
						switch (def)
						{
							case IndexedRegionGroupDef groupDef:
								if (!RegionGroups.ContainsKey(groupDef)) discoverGroups.Add(groupDef);
								if (!CurrentGroups.Contains(groupDef)) addGroups.Add(groupDef);
								break;
							case IndexedRegionDef regionDef:
								if (!Regions.ContainsKey(regionDef)) discoverRegions.Add(regionDef);
								if (!CurrentRegions.Contains(regionDef)) addRegions.Add(regionDef);
								break;
						}
					}

					removeRegions.Clear();
					removeRegions.AddRange(CurrentRegions.Where(regionDef => !regionDefs.Contains(regionDef)));
					removeGroups.Clear();
					removeGroups.AddRange(CurrentGroups.Where(regionDef => !regionDefs.Contains(regionDef)));

					if (addGroups.Count > 0 ||
					    addRegions.Count > 0 ||
					    removeGroups.Count > 0 ||
					    removeRegions.Count > 0 ||
					    discoverGroups.Count > 0 ||
					    discoverRegions.Count > 0
					)
						using (await this.GetThisExclusive())
						{
							foreach (var def in removeGroups) CurrentGroups.Remove(def);
							foreach (var def in addGroups) CurrentGroups.Add(def);
							foreach (var def in discoverGroups) await DiscoverGroup(def);

							foreach (var def in removeRegions) CurrentRegions.Remove(def);
							foreach (var def in addRegions) CurrentRegions.Add(def);
							foreach (var def in discoverRegions) await DiscoverRegion(def);
						}
				}
			}
		}

		public Task<float> GetGroupPercentImpl(IndexedRegionGroupDef groupDef)
		{
			var region = RegionsHolder.GetRegionByGuid(groupDef.Id);
			if (region is IndexedRegionGroup group)
			{
				var opened = 0;
				var count = 0;
				foreach (var indexedRegion in group.IndexedChildRegions)
				{
					count++;
					if (Regions.ContainsKey(indexedRegion.IndexRegionDef))
						opened++;
				}

				var percent = count > 0 ? (float) opened / count : 1f;
				return Task.FromResult(percent);
			}

			return Task.FromResult(0f);
		}


		public Task<bool> DiscoverRegionImpl(IndexedRegionDef region)
		{
			if (Regions.ContainsKey(region))
				return Task.FromResult(false);

			Regions.Add(region, false);
			Logger.IfInfo()?.Message($"FogOfWar Discovered Region {region}").Write();
			return Task.FromResult(true);
		}

		public Task<bool> DiscoverGroupImpl(IndexedRegionGroupDef group)
		{
			if (RegionGroups.ContainsKey(group))
				return Task.FromResult(false);

			RegionGroups.Add(group, false);
			Logger.IfInfo()?.Message($"FogOfWar Discovered Group {group}").Write();
			return Task.FromResult(true);
		}

		public Task<bool> SetRegionVisitedImpl(IndexedRegionDef region)
		{
			if (!Regions.ContainsKey(region))
				return Task.FromResult(false);

			Regions[region] = true;
			return Task.FromResult(true);
		}

		public Task<bool> SetGroupVisitedImpl(IndexedRegionGroupDef group)
		{
			if (!RegionGroups.ContainsKey(group))
				return Task.FromResult(false);

			RegionGroups[group] = true;
			return Task.FromResult(true);
		}

		private static void GetRegions<T>(IRegion root, Predicate<T> predicate, ref List<T> regions)
			where T : class, IRegion
		{
			if (root is T regionOfType && predicate(regionOfType)) regions.Add(regionOfType);
			foreach (var region in root.ChildRegions) GetRegions(region, predicate, ref regions);
		}

		public Task OnDestroy()
		{
			_destroyed = true;
			return Task.CompletedTask;
		}

		public Task OnUnload()
		{
			_destroyed = true;
			return Task.CompletedTask;
		}

		public Task ClearRegionsImpl()
		{
			Regions.Clear();
			return Task.CompletedTask;
		}
	}
}