using Assets.ColonyShared.SharedCode.Interfaces;
using ColonyShared.SharedCode.Aspects.Locomotion;
using SharedCode.Entities.Engine;

namespace Src.Locomotion
{
    public class LocomotionEngineAgent : ILocomotionEngineAgent, ILocomotionPipelineCommitNode, IResettable
    {
        private LocomotionFlags _flags;
        private bool _ready;

        public LocomotionFlags Flags => _flags;

        bool ILocomotionPipelineCommitNode.IsReady => _ready;

        //#Important!: @param `inVars`: 'ref' === 'in' here - i.e. "const-ref" (only to avoid struct copying)
        // So DO NOT change `inVars`!
        void ILocomotionPipelineCommitNode.Commit(ref LocomotionVariables inVars, float dt)
        {
            _flags = inVars.Flags;
        }

        public void SetReady(bool val)
        {
            _ready = val;
        }

        public void Reset()
        {
            //SetReady is called from LocomotionAgentExtensions.SetLocomotionToEntity & .CleanLocomotionToEntity
            _flags = default;
        }
    }
}