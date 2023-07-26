namespace Src.Locomotion
{
    public interface ILocomotionPipelineCommitNode
    {
        bool IsReady { get; }

        //#Important!: @param `inVars`: 'ref' === 'in' here - i.e. "const-ref" (only to avoid struct copying)
        // So DO NOT change `inVars`!
        void Commit(ref LocomotionVariables inVars, float dt); 
    }
}