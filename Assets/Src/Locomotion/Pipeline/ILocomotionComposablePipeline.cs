using System;

namespace Src.Locomotion
{
    public interface ILocomotionComposablePipeline : IDisposable
    {
        // Using to compose a pipeline. Add modifying vars only node (not consuming) 
        ILocomotionComposablePipeline AddPass(ILocomotionPipelinePassNode node, Action<ILocomotionComposablePipeline> subPipeline);

        ILocomotionComposablePipeline AddPassIf(bool predicate, ILocomotionPipelinePassNode node, Action<ILocomotionComposablePipeline> subPipeline);

        // Using to compose a pipeline. Add consumer-only node (not modifying vars)
        ILocomotionComposablePipeline AddCommit(ILocomotionPipelineCommitNode node);
        
        ILocomotionComposablePipeline AddCommitIf(bool predicate, ILocomotionPipelineCommitNode node);
    }
}