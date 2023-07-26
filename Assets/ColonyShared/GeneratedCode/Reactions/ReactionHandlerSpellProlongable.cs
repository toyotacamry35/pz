using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ColonyShared.GeneratedCode.Shared.Aspects;
using ColonyShared.SharedCode.Entities.Reactions;
using ColonyShared.SharedCode.Utils;
using ColonyShared.SharedCode.Wizardry;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ResourceSystem.Reactions;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.Serializers;
using SharedCode.Wizardry;

namespace ColonyShared.SharedCode.Reactions
{
    public class ReactionHandlerSpellProlongable : IReactionHandler, IDisposable
    {
        private readonly IReactionHandlerSpellProlongableDescriptor _desc;
        private readonly OuterRef<IHasWizardEntity> _hasWizard;
        private readonly IEntitiesRepository _repository;
        private CancellationTokenSource _cancelKillSwitch;
        private SpellId _spellId;

        public ReactionHandlerSpellProlongable(IReactionHandlerSpellProlongableDescriptor desc, OuterRef<IHasWizardEntity> hasWizard,
            IEntitiesRepository repository)
        {
            _desc = desc ?? throw new ArgumentNullException(nameof(desc));
            if (_desc.Spell == null) throw new ArgumentNullException(nameof(_desc.Spell));
            _hasWizard = hasWizard.IsValid ? hasWizard : throw new ArgumentException(nameof(hasWizard));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public void Dispose()
        {
            _cancelKillSwitch?.Cancel();
            _cancelKillSwitch?.Dispose();
            if (_spellId.IsValid)
                AsyncUtils.RunAsyncTask(async () =>
                    { 
                        await ClusterHelpers.UseWizard(_hasWizard, _repository, async wizard => { await wizard.StopCastSpell(_spellId); });
                    });
        }

        public async Task Invoke(ArgTuple[] args)
        {
            await ClusterHelpers.UseWizard(_hasWizard, _repository, async wizard =>
            {
                if (!_spellId.IsValid || !wizard.Spells.Any(x => x.Value.Id == _spellId && x.Value.FinishReason == SpellFinishReason.None))
                {
                    var spell = new SpellCastWithParameters {Def = _desc.Spell, Parameters = MapReactionArgsToSpellParams.Map(_desc.Params, args).ToArray() };
                    _spellId = await wizard.CastSpell(spell);
                }
                EngageKillSwitch(_spellId, _desc.Timeout);
            });
        }

        private void EngageKillSwitch(SpellId spellId, float timeout)
        {
            _cancelKillSwitch?.Cancel();
            _cancelKillSwitch?.Dispose();
            _cancelKillSwitch = new CancellationTokenSource();
            var token = _cancelKillSwitch.Token;
            AsyncUtils.RunAsyncTask(async () =>
            {
                try
                {
                    await Task.Delay((int) SyncTime.FromSeconds(timeout), token);
                    if (!token.IsCancellationRequested)
                        await ClusterHelpers.UseWizard(_hasWizard, _repository, async wizard => { await wizard.StopCastSpell(spellId); });
                } catch (OperationCanceledException) {} 
            });
        }
    }
}