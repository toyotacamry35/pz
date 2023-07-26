using System;
using Assets.ColonyShared.SharedCode.Utils;

namespace Src.Locomotion
{
    public class MobStateMachineContext : CommonStateMachineContext, ILocomotionUpdateable, ILocomotionDebugable
    {
        private const int InputHistoryCapacity = (int)(0.2 * 100);  // 0.2 sec при 100fps 
        
        private InputHistory<MobInputs> _inputProvider; // It's same as "(InputHistory<MobInputs>)base.Input", but lets avoid cast
        private ILocomotionInputSource<MobInputs> _inputSource;
        
        internal new readonly IMobStatsProvider Stats;
       
        public override bool IsReady => true;

        public static MobStateMachineContext Create(
            IMobStatsProvider stats,
            ILocomotionInputSource<MobInputs> input,
            ILocomotionBody body,
            ILocomotionEnvironment environment,
            ILocomotionHistory history,
            ILocomotionConstants constants,
            ILocomotionClock clock
        )
        {
            var inputProvider = new InputHistory<MobInputs>(InputHistoryCapacity);
            return new MobStateMachineContext(stats, input, inputProvider, body, environment, history, constants, clock);
        }

        public override void Reset()
        {
            ///#PZ-17474: #Dbg:
            if (DbgLog.Enabled) DbgLog.Log($"MobStateMachineContext.Reset 1of2");

            _inputProvider.Clean();
            base.Reset();
        }

        private MobStateMachineContext(   
            IMobStatsProvider stats,
            ILocomotionInputSource<MobInputs> inputSource,
            InputHistory<MobInputs> inputProvider,
            ILocomotionBody body,
            ILocomotionEnvironment environment,
            ILocomotionHistory history,
            ILocomotionConstants constants,
            ILocomotionClock clock
        ) : 
            base(stats, inputProvider, body, environment, history, constants, clock)
        {
            Stats = stats ?? throw new ArgumentNullException(nameof(stats));
            _inputProvider = inputProvider ?? throw new ArgumentNullException(nameof(inputProvider));
            _inputSource = inputSource ?? throw new ArgumentNullException(nameof(inputSource));
            AddDisposables(Stats, _inputProvider, _inputSource);
        }

        void ILocomotionUpdateable.Update(float dt)
        {
            _inputProvider.Push(_inputSource.GetInput(), dt);
        }
    }
}
