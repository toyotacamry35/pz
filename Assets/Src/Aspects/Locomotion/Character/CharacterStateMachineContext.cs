using System;

namespace Src.Locomotion
{
    public sealed class CharacterStateMachineContext : CommonStateMachineContext, ILocomotionUpdateable, ILocomotionDebugable
    {
        private const int InputHistoryCapacity = (int)(0.2 * 200);  // 0.2 sec при 200fps 
 
        private readonly ILocomotionInputSource<CharacterInputs> _inputSource;
        private readonly InputHistory<CharacterInputs> _inputProvider;
        private readonly CalcersCache _calcersCache;
        
        internal new readonly ICharacterStatsProvider Stats;

        public override bool IsReady => _calcersCache.IsReady;

        public static CharacterStateMachineContext Create(
            CalcersCache calcersCache,
            ICharacterStatsProvider stats,
            ILocomotionInputSource<CharacterInputs> input,
            ILocomotionBody body,
            ILocomotionEnvironment environment,
            ILocomotionHistory history,
            ILocomotionConstants constants,
            ILocomotionClock clock
        )
        {
            var inputProvider = new InputHistory<CharacterInputs>(InputHistoryCapacity);
            return new CharacterStateMachineContext(calcersCache, input, inputProvider, stats, body, environment, history, constants, clock);
        }
        
        private CharacterStateMachineContext(   
            CalcersCache calcersCache,
            ILocomotionInputSource<CharacterInputs> inputSource,
            InputHistory<CharacterInputs> inputProvider,
            ICharacterStatsProvider stats,
            ILocomotionBody body,
            ILocomotionEnvironment environment,
            ILocomotionHistory history,
            ILocomotionConstants constants,
            ILocomotionClock clock
        ) : 
            base(stats, inputProvider, body, environment, history, constants, clock)
        {
            Stats = stats ?? throw new ArgumentNullException(nameof(stats));
            _calcersCache = calcersCache ?? throw new ArgumentNullException(nameof(calcersCache));
            _inputSource = inputSource ?? throw new ArgumentNullException(nameof(inputSource));
            _inputProvider = inputProvider ?? throw new ArgumentNullException(nameof(inputProvider));
            AddDisposables(Stats, _calcersCache, _inputSource, _inputProvider);
        }
        
        void ILocomotionUpdateable.Update(float dt)
        {
            _inputProvider.Push(_inputSource.GetInput(), dt);
        }
    }
}
