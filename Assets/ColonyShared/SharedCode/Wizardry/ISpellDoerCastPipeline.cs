using System.Threading.Tasks;
using GeneratedDefsForSpells;

namespace SharedCode.Wizardry
{
    public enum CastPipelineState
    {
        None,
        Casting,
        FailedToCastLocally,
        FailedToCastRemote,
        Casted,
        Stopping,
        Stopped,
        Failed,
        Succeded
    }
    
    public interface ISpellDoerCastPipeline
    {
        CastPipelineState State { get; }
        FinishReasonType FinishReason { get; }
        SpellId Id { get; }
        Task<SpellId> SpellGotIdTask { get; }
        Task<SpellId> SpellCastedTask { get; }
        Task<(SpellId Id, SpellFinishReason Reason)> SpellFinishedTask { get; }
        SpellDef SpellDef { get; }
        void FinishCast(SpellFinishMethod method, FinishReasonType reason = FinishReasonType.Success);
        void StopCast(FinishReasonType reason = FinishReasonType.Success);
        void CancelCast();
    }
}