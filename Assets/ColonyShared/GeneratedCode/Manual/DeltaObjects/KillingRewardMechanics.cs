using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.Src.Arithmetic;
using Assets.Src.Impacts;
using ColonyShared.GeneratedCode.Manual.DeltaObjects;
using ColonyShared.SharedCode.Entities;
using ColonyShared.SharedCode.Entities.Reactions;
using GeneratedCode.EntitySystem;
using ResourceSystem.Aspects.Rewards;
using ResourceSystem.Utils;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using static SharedCode.Repositories.ReplicaTypeRegistry;

namespace GeneratedCode.DeltaObjects
{
    public partial class KillingRewardMechanics : IHookOnInit, IHookOnDatabaseLoad, IKillingRewardMechanics
    {
        public Task OnInit()
        {
            Subscribe();
            return Task.CompletedTask;
        }

        public Task OnDatabaseLoad()
        {
            Subscribe();
            return Task.CompletedTask;
        }

        public ValueTask<OuterRef> ReplaceKillingDamageDealerImpl(OuterRef entityRef)
        {
            var old = KillingDamageDealer;
            KillingDamageDealer = entityRef;
            return new ValueTask<OuterRef>(old);
        }

        private void Subscribe()
        {
            var mortal = (IHasMortal) parentEntity;
            mortal.Mortal.DieEvent += OnDie;
            mortal.Mortal.ResurrectEvent += OnResurrect;
            mortal.Mortal.ReviveFromKnockdown += OnReviveFromKnockdown;
            var health = (IHasHealth) parentEntity;
            health.Health.DamageReceivedExtEvent += OnDamage;
        }

        private async Task OnDamage(float prevhealthval, float newhealthval, float maxhealth, MortalState prevState, MortalState newState, Damage damage)
        {
            if (newState != MortalState.Alive && prevState != MortalState.Dead)
            {
                if (damage.Aggressor.Guid != ParentEntityId) // урон от самого себя (Kill self) не должен сбрасывать дилера, но любой другой урон, в том числе от никого (например урон от падения), сбрасывает
                    await ReplaceKillingDamageDealer(damage.Aggressor);
            }
        }

        private async Task OnDie(Guid arg1, int arg2, PositionRotation arg3)
        {
            var nomineeRef = await ReplaceKillingDamageDealer(OuterRef.Invalid);
            if (!nomineeRef.IsValid)
                return;
            
            var nomineeType = GetMasterTypeByReplicationLevelType(GetTypeById(nomineeRef.TypeId));
            if (!typeof(IHasFaction).IsAssignableFrom(nomineeType))
                return;

            RewardDef reward;
            using (var cnt = await EntitiesRepository.Get(EntityBatch.Create().Add(nomineeRef.TypeId, nomineeRef.Guid).Add(ParentTypeId, ParentEntityId)))
            {
                if (!cnt.TryGet<IHasFaction>(nomineeRef.TypeId, nomineeRef.Guid, ReplicationLevel.Master, out var hasFaction))
                    return;

                var rewardCalcer = hasFaction?.Faction?.RelationshipRules.Target?.KillingReward.Target;
                if (rewardCalcer == null)
                    return;

                var relationshipContext = new Relationship.Context(thisEntity: nomineeRef, otherEntity: new OuterRef(ParentEntityId, ParentTypeId));
                var args = new List<ArgTuple>();
                Relationship.MapRelationshipArgs(Constants.RelationshipConstants.ArgsMapping, relationshipContext, args);
                var ctx = new CalcerContext(cnt, nomineeRef, EntitiesRepository, args: args.ToCalcerArgs());
                reward = await rewardCalcer.CalcAsync(ctx) as RewardDef;
            }
            
            if (reward == null)
                return;

            await ImpactGrantAccountReward.GrantReward(reward, nomineeRef, EntitiesRepository);
        }
        
        private Task OnResurrect(Guid arg1, int arg2, PositionRotation arg3)
        {
            return ReplaceKillingDamageDealer(OuterRef.Invalid).AsTask();
        }
        
        private Task OnReviveFromKnockdown()
        {
            return ReplaceKillingDamageDealer(OuterRef.Invalid).AsTask();
        }
    }
}