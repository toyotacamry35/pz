using System;

namespace Src.Locomotion
{
    public class LocomotionTimestampNode : ILocomotionPipelinePassNode
    {
        private readonly ILocomotionClock _clock;
        private readonly LocomotionTimestamp _offset;

        public LocomotionTimestampNode(ILocomotionClock clock, LocomotionTimestamp offset
            , Func<bool> shouldSaveLocoVars = null, Action<LocomotionVariables, Type> saveLocoVarsCallback = null)
        {
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _offset = offset;
            ShouldSaveLocoVars = shouldSaveLocoVars;
            SaveLocoVarsCallback = saveLocoVarsCallback;
        }

        ///#PZ-13568: #Dbg:
        protected Func<bool> ShouldSaveLocoVars;
        protected Action<LocomotionVariables, Type> SaveLocoVarsCallback;

        bool ILocomotionPipelinePassNode.IsReady => true;

        LocomotionVariables ILocomotionPipelinePassNode.Pass(LocomotionVariables vars, float dt)
        {
            if (vars.Timestamp.Valid) throw new ArgumentException("Frame already timestamped", "vars.Timestamp");
            vars.Timestamp = _clock.Timestamp + _offset;

            if (ShouldSaveLocoVars?.Invoke() ?? false)
                SaveLocoVarsCallback(vars, this.GetType());

            return vars;
        }
    }
}