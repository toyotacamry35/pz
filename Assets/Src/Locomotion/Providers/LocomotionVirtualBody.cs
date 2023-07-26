using System;
using ColonyShared.SharedCode.Aspects.Locomotion;
using SharedCode.Utils;

namespace Src.Locomotion
{
    public class LocomotionVirtualBody : LocomotionVirtualBodyBase, ILocomotionBody, ILocomotionPipelineFetchNode, ILocomotionPipelineCommitNode
    {
        LocomotionVariables ILocomotionPipelineFetchNode.Fetch(float dt)
        {
            ///#PZ-13568: #Dbg: #Tmp replaced by next lines:
            // new LocomotionVariables(0, Position, Velocity, Orientation);
            var result = new LocomotionVariables(0, Position, Velocity, Orientation);
            if (ShouldSaveLocoVars?.Invoke() ?? false)
                SaveLocoVarsCallback(result, this.GetType());
            return result;
        }

        void ILocomotionPipelineCommitNode.Commit(ref LocomotionVariables inVars, float dt)
        {
            if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## Loco Commit: i)LocoVirtualBody");
            base.Commit(ref inVars);
            LocomotionProfiler.EndSample();
        }
    }
    
    public class LocomotionVirtualBodyWithFlags : LocomotionVirtualBodyBase, ILocomotionBody, ILocomotionPipelineFetchNode, ILocomotionPipelineCommitNode
    {
        public LocomotionFlags Flags { get; private set; }
        
        LocomotionVariables ILocomotionPipelineFetchNode.Fetch(float dt) => new LocomotionVariables(Flags, Position, Velocity, Orientation);

        void ILocomotionPipelineCommitNode.Commit(ref LocomotionVariables inVars, float dt)
        {
            if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## Loco Commit: i)LocoVirtualBody");
            Flags = inVars.Flags;
            base.Commit(ref inVars);
            LocomotionProfiler.EndSample();
        }
    }
    
    public class LocomotionVirtualBodyBase
    {
        ///#PZ-13568: #Dbg:
        protected Func<bool> ShouldSaveLocoVars;
        protected Action<LocomotionVariables, Type> SaveLocoVarsCallback;
        public void Setup(LocomotionVector position, LocomotionVector velocity, float orientation, Func<bool> shouldSaveLocoVars, Action<LocomotionVariables, Type> saveLocoVarsCallback)
        {
            Velocity = velocity;
            Position = position;
            Orientation = orientation;
            Forward = LocomotionHelpers.LocomotionOrientationToForwardVector(orientation);
            ShouldSaveLocoVars = shouldSaveLocoVars;
            SaveLocoVarsCallback = saveLocoVarsCallback;
        }
        // Reset is done by Setup

        public LocomotionVector Velocity { get; private set; }

        public LocomotionVector Position  { get; private set; }

        public float Orientation  { get; private set; }

        public Vector2 Forward  { get; private set; }

        public bool IsReady => true;
        
        protected void Commit(ref LocomotionVariables inVars)
        {
            Position = inVars.Position;
            Velocity = inVars.Velocity;
            Orientation = inVars.Orientation;
            Forward = LocomotionHelpers.LocomotionOrientationToForwardVector(inVars.Orientation);
        }
    }
}