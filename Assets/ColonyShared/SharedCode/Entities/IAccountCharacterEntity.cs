using System;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using ResourceSystem.Aspects.Rewards;

namespace SharedCode.Entities
{
	/// <summary>
	/// Mostly - light-weight subset of character data, needed on account screen (at lobby mode before player enter a game world)
	/// But could contain some unique data (so is not totally namely "subset" of character data)
	/// </summary>
	[GeneratorAnnotations.GenerateDeltaObjectCode]
	public interface IAccountCharacter : IDeltaObject, IHasId
	{
		[ReplicationLevel(ReplicationLevel.ClientFull)]
		string CharacterName { get; set; }

		[ReplicationLevel(ReplicationLevel.ClientFull)]
		Guid MapId { get; set; }

		// Can't use IDeltaList, 'cos UI bindings can't work with it - it assumes list is hashset
		[ReplicationLevel(ReplicationLevel.ClientFull)]
		IDeltaDictionary<RewardDef, int> CurrentSessionRewards { get; set; }

		[ReplicationLevel(ReplicationLevel.Server)]
		ValueTask<bool> AssignData(Guid mapId);

		[ReplicationLevel(ReplicationLevel.Server)]
		ValueTask<bool> GrantReward(RewardDef value);
		
		#region Cheats

		[CheatRpc(AccountType.GameMaster)]
		[ReplicationLevel(ReplicationLevel.ClientBroadcast)]
		Task AddReward(RewardDef value);
		
		[CheatRpc(AccountType.GameMaster)]
		[ReplicationLevel(ReplicationLevel.ClientBroadcast)]
		Task ClearRewards();
		#endregion
	}
}

namespace GeneratedCode.DeltaObjects
{
	public partial class AccountCharacter
	{
		private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
		
		public ValueTask<bool> AssignDataImpl(Guid mapId)
		{
			MapId = mapId;

			return new ValueTask<bool>(true);
		}

		public ValueTask<bool> GrantRewardImpl(RewardDef value)
		{
			if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Grant Reward | AccountCharacter:{Id} Reward:{value?.____GetDebugAddress()}").Write();
			var count = 1;
			if (!CurrentSessionRewards.TryAdd(in value, in count))
				CurrentSessionRewards[value] += count;
			return new ValueTask<bool>(true);
		}

		public async Task AddRewardImpl(RewardDef value)
		{
			await GrantReward(value);
		}
		
		public Task ClearRewardsImpl()
		{
			CurrentSessionRewards.Clear();
			return Task.CompletedTask;
		}
	}
}