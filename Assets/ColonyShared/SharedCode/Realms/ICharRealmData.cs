using System;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratorAnnotations;
using ProtoBuf;
using SharedCode.Aspects.Sessions;
using SharedCode.Entities;
using SharedCode.Entities.Cloud;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;

namespace SharedCode.Entities
{
	[GenerateDeltaObjectCode]
	public interface ICharRealmData : IDeltaObject
	{
		[ReplicationLevel(ReplicationLevel.ClientFull)]
		Task<RealmOperationResult> FindRealm(RealmRulesQueryDef settings);

		[ReplicationLevel(ReplicationLevel.ClientFull)]
		Task<RealmOperationResult> EnterCurrentRealm(bool autoPlay);

		//IRealmEntity
		[ReplicationLevel(ReplicationLevel.ClientFull)]
		OuterRef<IEntity> CurrentRealm { get; set; }

		[ReplicationLevel(ReplicationLevel.ClientFull)]
		RealmCharStateEnum CurrentRealmCharState { get; set; }
		[ReplicationLevel(ReplicationLevel.ClientFull)]
		RealmRulesDef CurrentRealmRulesCached { get; set; }

		[ReplicationLevel(ReplicationLevel.ClientFull)]
		Task<bool> LeaveCurrentRealm();

		[ReplicationLevel(ReplicationLevel.ClientFull)]
		Task<bool> GiveUpCurrentRealm();

		//IRealmEntity
		[ReplicationLevel(ReplicationLevel.Server)]
		Task<FindRealmRequestResult> GetRealm(RealmRulesQueryDef settings);

		#region Cheats

		[CheatRpc(AccountType.GameMaster)]
		[ReplicationLevel(ReplicationLevel.ClientBroadcast)]
		Task ChangeCurrentRealmActivity(Guid accountId, bool active);

		[CheatRpc(AccountType.GameMaster)]
		[ReplicationLevel(ReplicationLevel.ClientBroadcast)]
		Task DestroyCurrentRealm(Guid accountId);

		#endregion
	}

	public enum RealmCharStateEnum
	{
		Inactive = 0,
		Active = 1,
		Surrendered = 2,
		Success = 3
	}

	[ProtoContract]
	public class RealmOperationResult
	{
		[ProtoMember(1)] public bool ReconnectRequired { get; set; }

		[ProtoMember(2)] public OuterRef<IEntity> RealmEntity { get; set; }
		[ProtoMember(3)] public bool CantGetRealm { get; set; }
	}
}

namespace GeneratedCode.DeltaObjects
{
	public partial class CharRealmData
	{
		// private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public async Task ChangeCurrentRealmActivityImpl(Guid accountId, bool active)
		{
			using (var wrapper = await EntitiesRepository.Get(CurrentRealm.TypeId, CurrentRealm.Guid))
			{
				var entity =
					wrapper.Get<IRealmEntityAlways>(CurrentRealm.TypeId, CurrentRealm.Guid, ReplicationLevel.Always);
				if (entity != null)
					await entity.SetActive(active);
			}
		}

		public async Task DestroyCurrentRealmImpl(Guid accountId)
		{
			await EntitiesRepository.Destroy(CurrentRealm.TypeId, CurrentRealm.Guid);
		}

		public async Task<RealmOperationResult> FindRealmImpl(RealmRulesQueryDef settings)
		{
			var realm = await GetRealm(settings);
			if(realm.Realm == default)
			{
				if(realm.Booked)
					return new RealmOperationResult
					{
						ReconnectRequired = true
					};
				else
					return new RealmOperationResult
					{
						CantGetRealm = true
					};
			}
			await Task.Delay(1000);
			using (var wrapper = await EntitiesRepository.Get(realm.Realm.TypeId, realm.Realm.Guid))
			{
				var entity = wrapper.Get<IRealmEntityServer>(realm.Realm.TypeId, realm.Realm.Guid, ReplicationLevel.Server);
				if (entity != null && entity.Active)
				{
					CurrentRealmRulesCached = entity.Def;
					CurrentRealm = realm.Realm;
					CurrentRealmCharState = RealmCharStateEnum.Active;
					using (var repoComW =
						await EntitiesRepository.Get<IRepositoryCommunicationEntityServer>(entity.OwnerRepositoryId))
					{
						var repoCom = repoComW.Get<IRepositoryCommunicationEntityServer>(entity.OwnerRepositoryId);
						await repoCom.SubscribeReplication(
							realm.Realm.TypeId,
							realm.Realm.Guid,
							Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId,
							ReplicationLevel.ClientFull
						);
					}

					//TODO Vabavia Remove After Replication Fix
					//хак до тех пор пока не починят гарантию репликации энтити до возвращения метода ее отреплицировавшего
					await Task.Delay(3000);

					// TODO Vabavia Await Run Async Task With Using
					// AsyncUtils.RunAsyncTask();
					//var task = entity.Enter(new OuterRef<IEntity>(parentEntity));
					return new RealmOperationResult
					{
						ReconnectRequired = false, RealmEntity = realm.Realm
					};
				}
			}

			return new RealmOperationResult
			{
				ReconnectRequired = true
			};
		}

		public async Task<RealmOperationResult> EnterCurrentRealmImpl(bool autoPlay)
		{
			if(autoPlay)
			{
				var res = await FindRealm(null);
				if(res.CantGetRealm || res.ReconnectRequired)
					return new RealmOperationResult { ReconnectRequired = true };
			}
			if (!CurrentRealm.IsValid)
				return new RealmOperationResult {ReconnectRequired = false, CantGetRealm = true};
			using (var wrapper = await EntitiesRepository.Get(CurrentRealm.TypeId, CurrentRealm.Guid))
			{
				var entity =
					wrapper.Get<IRealmEntityServer>(CurrentRealm.TypeId, CurrentRealm.Guid, ReplicationLevel.Server);
				if (entity != null && entity.Active)
				{
					using(var w = await EntitiesRepository.GetFirstService<ILoginServiceEntityServer>())
					{
						if(!await w.GetFirstService<ILoginServiceEntityServer>().NotifyPlatformOfJoining(ParentEntityId, CurrentRealm.Guid))
						{
							await GiveUpCurrentRealm();
							return new RealmOperationResult { ReconnectRequired = true };
						}
					}
					await entity.Enter(new OuterRef<IEntity>(parentEntity));
					return new RealmOperationResult
						{ReconnectRequired = false, RealmEntity = CurrentRealm};
				}
			}

			return new RealmOperationResult {ReconnectRequired = true};
		}

		public async Task<bool> LeaveCurrentRealmImpl()
		{
			using (var loginW = await EntitiesRepository.GetFirstService<ILoginServiceEntityServer>())
			{
				var loginS = loginW.GetFirstService<ILoginServiceEntityServer>();
				var realmRef = await loginS.NotifyPlatformOfLeaving(parentEntity.Id, CurrentRealm.Guid);
			}
			return true;
		}

		public async Task<bool> GiveUpCurrentRealmImpl()
		{
			CurrentRealmCharState = RealmCharStateEnum.Surrendered;
			var curRealm = CurrentRealm;
			CurrentRealm = default;
			using (var wrapper = await EntitiesRepository.Get(curRealm.TypeId, curRealm.Guid))
			{
				var entity =
					wrapper.Get<IRealmEntityServer>(curRealm.TypeId, curRealm.Guid, ReplicationLevel.Server);
				if (entity != null && entity.Active)
				{
					await entity.Leave(new OuterRef<IEntity>(parentEntity));
				}
			}

			using (var wrapper = await EntitiesRepository.GetFirstService<IWorldCoordinatorNodeServiceEntityServer>())
			{
				var wc = wrapper.GetFirstService<IWorldCoordinatorNodeServiceEntityServer>();
					await wc.RequestLogoutFromMap(((IAccountEntity)parentEntity).CurrentUserId, true);
			}
			using (var loginW = await EntitiesRepository.GetFirstService<ILoginServiceEntityServer>())
			{
				var loginS = loginW.GetFirstService<ILoginServiceEntityServer>();
				await loginS.NotifyPlatformOfRealmGiveUp(parentEntity.Id, curRealm.Guid);
			}
			return true;
		}

		public async Task<FindRealmRequestResult> GetRealmImpl(RealmRulesQueryDef settings)
		{
//TODO Vabavia Create Realm

			using (var coordinatorW = await EntitiesRepository.GetFirstService<ILoginServiceEntityServer>())
			{
				var coord = coordinatorW.GetFirstService<ILoginServiceEntityServer>();
				var realmRef = await coord.FindRealm(settings, parentEntity.Id, CurrentRealm.Guid);
				return realmRef;
			}
		}
	}
}