using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Entities.GameMapData;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;

namespace Assets.Src.Effects
{
    public class EffectWorldObjectSet : IEffectBinding<EffectWorldObjectSetDef>
    {
        public async ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectWorldObjectSetDef def)
        {
            if (cast.IsSlave)
                return;

            using (var wrapper = await repo.Get<IWorldCharacterServer>(cast.Caster.Guid))
            {
                var worldCharacter = wrapper.Get<IWorldCharacterServer>(cast.Caster.Guid);
                await worldCharacter.WorldObjectInformationSetsEngine.AddWorldObjectInformationSubSet(def.Set.Target);
            }
        }

        public async ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectWorldObjectSetDef def)
        {
            if (cast.IsSlave)
                return;

            using (var wrapper = await repo.Get<IWorldCharacterServer>(cast.Caster.Guid))
            {
                var worldCharacter = wrapper.Get<IWorldCharacterServer>(cast.Caster.Guid);
                await worldCharacter.WorldObjectInformationSetsEngine.RemoveWorldObjectInformationSubSet(def.Set.Target);
            }
        }
    }
}
