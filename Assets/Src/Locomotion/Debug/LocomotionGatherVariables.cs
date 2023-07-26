using static Src.Locomotion.DebugTag;

namespace Src.Locomotion
{
    public static partial class LocomotionDebug
    {
        public static IGatherBody GatherBody(ILocomotionBody body) => new LocomotionGatherBody(body);

        public static readonly ILocomotionPipelineCommitNode GatherVariables = new LocomotionGatherVariables();

        public static readonly ILocomotionPipelineCommitNode GatherPredicted = new LocomotionGatherPredicted();
        
        public interface IGatherBody : ILocomotionPipelineCommitNode, ILocomotionDebugable {}
        
        private class LocomotionGatherBody : LocomotionGatherNode, IGatherBody
        {
            private readonly ILocomotionBody _body;
            
            public LocomotionGatherBody(ILocomotionBody body)
            {
                _body = body;
            }
            
            public void GatherDebug(ILocomotionDebugAgent agent)
            {
                if (_body != null && agent != null)
                {
                    agent.Set(BodyPosition, _body.Position);
                    agent.Set(BodyVelocity, _body.Velocity);
                    agent.Set(BodyOrientation, _body.Orientation);
                }
            }

            protected override void Gather(ref LocomotionVariables inVars) => GatherDebug(DebugAgent);
        }
        
        private class LocomotionGatherVariables : LocomotionGatherNode
        {
            protected override void Gather(ref LocomotionVariables inVars)
            {
                DebugAgent.Set(VarsPosition, inVars.Position);
                DebugAgent.Set(VarsVelocity, inVars.Velocity);
                DebugAgent.Set(VarsOrientation, inVars.Orientation);
                DebugAgent.Set(VarsAngularVelocity, inVars.AngularVelocity);
                DebugAgent.Set(Shift, inVars.ExtraPosition);
                DebugAgent.Set(MovementFlags, (int) inVars.Flags);
                DebugAgent.Set(TimeStamp, inVars.Timestamp);
            }
        }
        
        private class LocomotionGatherPredicted : LocomotionGatherNode
        {
            protected override void Gather(ref LocomotionVariables inVars)
            {
                DebugAgent.Set(NetworkPredictedPosition, inVars.Position);
                DebugAgent.Set(NetworkPredictedVelocity, inVars.Velocity);
                DebugAgent.Set(NetworkPredictedOrientation, inVars.Orientation);
            }
        }
        
        private abstract class LocomotionGatherNode : ILocomotionPipelineCommitNode
        {
            bool ILocomotionPipelineCommitNode.IsReady => true;

            void ILocomotionPipelineCommitNode.Commit(ref LocomotionVariables inVars, float dt)
            {
                if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## Loco Commit: b)LocoGatherVariables");
                if (DebugAgent != null && DebugAgent.IsActive)
                    Gather(ref inVars);
                LocomotionProfiler.EndSample();
            }

            protected abstract void Gather(ref LocomotionVariables inVars);
        }
    }
}