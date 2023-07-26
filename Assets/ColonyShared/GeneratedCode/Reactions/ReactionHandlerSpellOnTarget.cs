using System;
using System.Linq;
using System.Threading.Tasks;
using ColonyShared.GeneratedCode.Shared.Aspects;
using ColonyShared.SharedCode.Entities.Reactions;
using ResourceSystem.Reactions;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace ColonyShared.SharedCode.Reactions
{
    public class ReactionHandlerSpellOnTarget : IReactionHandler
    {
        private readonly IReactionHandlerSpellOnTargetDescriptor _desc;
        private readonly IEntitiesRepository _repository;

        public ReactionHandlerSpellOnTarget(IReactionHandlerSpellOnTargetDescriptor desc, IEntitiesRepository repository)
        {
            _desc = desc ?? throw new ArgumentNullException(nameof(desc));
            if (_desc.Spell == null) throw new ArgumentNullException(nameof(_desc.Spell));
            if (_desc.Spell.IsInfinite)
                throw new InvalidOperationException(
                    $"Нельзя использовать бесконечный спел {_desc.Spell.____GetDebugAddress()} совместно с {nameof(ReactionHandlerSpell)}");
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task Invoke(ArgTuple[] args)
        {
            var spell = new SpellCastWithParameters {Def = _desc.Spell, Parameters = MapReactionArgsToSpellParams.Map(_desc.Params, args).ToArray() };
            var target = _desc.Target.GetValue(args);
            await ClusterHelpers.UseWizard(new OuterRef<IHasWizardEntity>(target), _repository, async wizard =>
            {
                await wizard.CastSpell(spell);
            });
        }
    }
}