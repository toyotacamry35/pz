using System;
using static Src.Locomotion.DebugTag;

namespace Src.Locomotion
{
    public abstract class CommonStateMachineContext : StateMachineContext
    {
        internal readonly ICommonStatsProvider Stats;
        internal readonly ILocomotionInputProvider Input;
        //#TODO: LocoVars should be used instead of it. But many refactoring needed to del it from here.
        internal readonly ILocomotionBody Body_Deprecated;
        internal readonly ILocomotionEnvironment Environment;
        internal readonly ILocomotionHistory History;
        internal readonly ILocomotionConstants Constants;

        protected CommonStateMachineContext(
            ICommonStatsProvider stats,
            ILocomotionInputProvider input,
            ILocomotionBody body,
            ILocomotionEnvironment environment,
            ILocomotionHistory history,
            ILocomotionConstants constants,
            ILocomotionClock clock
        )
            : base(clock)
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            Stats = stats ?? throw new ArgumentNullException(nameof(stats));
            Body_Deprecated = body ?? throw new ArgumentNullException(nameof(body));
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            History = history ?? throw new ArgumentNullException(nameof(history));
            Constants = constants ?? throw new ArgumentNullException(nameof(constants));
            AddDisposables(Input, Stats, Body_Deprecated, Environment, History, Constants);
        }
        
        public void GatherDebug(ILocomotionDebugAgent agent)
        {
            if (agent == null)
                return;
//            agent.Set("Position", this.Body.Position);
//            agent.Set("Velocity", this.Body.Velocity);
//            agent.Set("Orientation", this.Body.Orientation);
            agent.Set(DistanceToGround, this.Environment.DistanceToGround);
            agent.Set(Airborne, this.Environment.Airborne);
            agent.Set(SlopeFactor, this.Environment.SlopeFactor());
            agent.Set(SlopeFactorAlongVelocity, this.Environment.SlopeFactorAlongDirection(this.Body_Deprecated.Velocity.Horizontal));
            agent.Set(MoveAxes, this.Input[CommonInputs.Move]);
            agent.Set(GuideAxes, this.Input[CommonInputs.Guide]);
            agent.Set(FallHeight, this.History.LastFallHeight);
        }
    }
}