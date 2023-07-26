using System;
using System.Linq;
using System.Threading.Tasks;
using ColonyShared.SharedCode.Modifiers;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using ResourcesSystem.Loader;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;

namespace ColonyShared.GeneratedCode.Effects
{
    [UsedImplicitly]
    public class EffectAddSpellModifier : IEffectBinding<EffectAddSpellModifierDef>
    {
        public async ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectAddSpellModifierDef def)
        {
            if (cast.OnServerMaster())
                using (var cnt = await repo.Get(cast.Caster.TypeId, cast.Caster.Guid))
                {
                    var hasSpellModifiers = cnt.Get<IHasSpellModifiers>(cast.Caster);
                    await hasSpellModifiers.SpellModifiersCollector.AddModifiers(GenerateCauser(cast, def), def.Condition, def.Modifiers.ToArray());
                }
        }

        public async ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectAddSpellModifierDef def)
        {
            if (cast.OnServerMaster())
                using (var cnt = await repo.Get(cast.Caster.TypeId, cast.Caster.Guid))
                {
                    var hasSpellModifiers = cnt.Get<IHasSpellModifiers>(cast.Caster);
                    await hasSpellModifiers.SpellModifiersCollector.RemoveModifiers(GenerateCauser(cast, def));
                }
        }

        private SpellModifiersCauser GenerateCauser(SpellWordCastData cast, EffectAddSpellModifierDef def)
        {
            var rcid = GameResourcesHolder.Instance.GetIDWithCrc(def);
            var guid = new Guid(
                (int) ((rcid.NetCrc64Id >> 32) & 0xFFFFFFFF),
                (short) ((rcid.NetCrc64Id >> 16) & 0xFFFF),
                (short) (rcid.NetCrc64Id & 0xFFFF),
                (byte) ((rcid.ResId.Line >> 24) & 0xFF),
                (byte) ((rcid.ResId.Line >> 16) & 0xFF),
                (byte) ((rcid.ResId.Line >> 8) & 0xFF),
                (byte) ((rcid.ResId.Line) & 0xFF),
                (byte) ((rcid.ResId.Col >> 8) & 0xFF),
                (byte) ((rcid.ResId.Col) & 0xFF),
                (byte) ((rcid.ResId.ProtoIndex >> 8) & 0xFF),
                (byte) ((rcid.ResId.ProtoIndex) & 0xFF)
            );
            return new SpellModifiersCauser(guid, cast.SpellId);
        }
    }
}