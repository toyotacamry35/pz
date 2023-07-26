using System;
using System.Linq;
using System.Threading.Tasks;
using SharedCode.Logging;
using ColonyShared.GeneratedCode.Shared.Aspects;
using ColonyShared.SharedCode.Entities.Reactions;
using ColonyShared.SharedCode.Wizardry;
using ResourceSystem.Reactions;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using NLog;

namespace ColonyShared.SharedCode.Reactions
{
    // Fire And Forget
    public class ReactionHandlerSpell : IReactionHandler
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IReactionHandlerSpellDescriptor _desc;
        private readonly OuterRef<IHasWizardEntity> _hasWizard;
        private readonly IEntitiesRepository _repository;

        public ReactionHandlerSpell(IReactionHandlerSpellDescriptor desc, OuterRef<IHasWizardEntity> hasWizard,
            IEntitiesRepository repository)
        {
            _desc = desc ?? throw new ArgumentNullException(nameof(desc));
            if (_desc.Spell == null) throw new ArgumentNullException(nameof(_desc.Spell));
            if (_desc.Spell.IsInfinite)
                throw new InvalidOperationException(
                    $"Нельзя использовать бесконечный спел {_desc.Spell.____GetDebugAddress()} совместно с {nameof(ReactionHandlerSpell)}");
            _hasWizard = hasWizard.IsValid ? hasWizard : throw new ArgumentException(nameof(hasWizard));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            
        }

        public async Task Invoke(ArgTuple[] args)
        {
            var spell = new SpellCastWithParameters {Def = _desc.Spell, Parameters = MapReactionArgsToSpellParams.Map(_desc.Params, args).ToArray() };
            await ClusterHelpers.UseWizard(_hasWizard, _repository, async wizard =>
            {
                Log.AttackStopwatch.Milestone("Cast");
                await wizard.CastSpell(spell);
            });
        }
    }
}