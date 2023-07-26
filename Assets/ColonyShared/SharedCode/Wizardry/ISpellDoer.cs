using System;
using System.Threading.Tasks;
using Assets.ResourceSystem.Arithmetic.Templates.Predicates;
using ColonyShared.SharedCode.Wizardry;
using JetBrains.Annotations;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;

namespace SharedCode.Wizardry
{
    public class AwaitableSpellDoerCast
    {
        public TaskCompletionSource<SpellId> CastResultAwaiter = new TaskCompletionSource<SpellId>();
        public Action<SpellId, SpellFinishReason> Finished;
        public SpellId SpellId;
        public SpellFinishReason FinishReason;
        public bool ShouldReactToBindings = false;
    }
    public interface ISpellDoer
    {
        ISpellDoerCastPipeline DoCast(SpellCast order, AwaitableSpellDoerCast awaiter = default);
        ISpellDoerCastPipeline DoCast(SpellCastBuilder spellBuilder, AwaitableSpellDoerCast awaiter = default);
        ISpellDoerCastPipeline DoCastChain(SpellCast order, SpellId afterSpell, AwaitableSpellDoerCast awaiter = default);
        ISpellDoerCastPipeline DoCastChain(SpellCastBuilder spellBuilder, SpellId afterSpell, AwaitableSpellDoerCast awaiter = default);
        ISpellDoerCastPipeline GetPipeline(SpellId spellId);
        Task<bool> CanStartCast(SpellCast cast, PredicateIgnoreGroupDef predicateIgnoreGroupDef = null, bool checkPredicatesOnly = false);
        bool StopCast(SpellId id, FinishReasonType reason);
        void StopAllSpellsOfGroup([NotNull] SpellGroupDef group, SpellId orderId, FinishReasonType reason);
        event SpellDoerFinsihDelegate OnOrderFinished;
        IEntitiesRepository Repository { get; }
    }
    
    public delegate void SpellDoerStartDelegate(ISpellDoerCastPipeline spellCast);
    public delegate void SpellDoerFinsihDelegate(ISpellDoerCastPipeline spellCast);
}