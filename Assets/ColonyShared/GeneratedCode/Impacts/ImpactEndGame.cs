using System;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using ResourceSystem.Aspects.ManualDefsForSpells;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Serializers;

namespace ColonyShared.GeneratedCode.Impacts
{
    public class ImpactEndGame : IImpactBinding<ImpactEndGameDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactEndGameDef def)
        {
            Guid accId;
            using (var wc = await repo.Get(cast.Caster.TypeId, cast.Caster.Guid, ReplicationLevel.Server))
            {
                var ch = wc.Get<IWorldCharacterServer>(cast.Caster.TypeId, cast.Caster.Guid, ReplicationLevel.Server);
                accId = ch.AccountId;
            }
            using (var w = await repo.GetFirstService<ILoginServiceEntityServer>())
            {
                var loginS = w.GetFirstService<ILoginServiceEntityServer>();
                await loginS.GiveUpRealmOnDeath(accId);
                await repo.Destroy<IWorldCharacter>(cast.Caster.Guid, true); // Yeh, delete(this), I know... :(
            }
        }
    }
}