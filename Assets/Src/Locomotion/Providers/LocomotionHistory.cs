using Assets.ColonyShared.SharedCode.Interfaces;
using Mathf = Assets.ColonyShared.SharedCode.Utils.SharedHelpers; 

namespace Src.Locomotion
{
    public class LocomotionHistory : ILocomotionHistory, ILocomotionUpdateable, IResettable
    {
        private readonly ILocomotionBody _body;
        private readonly ILocomotionEnvironment _environment;
        private float _airborneTime;
        private LocomotionVector _airborneApex;
        private float _lastFallDistance;

        public LocomotionHistory(ILocomotionBody body, ILocomotionEnvironment environment)
        {
            _body = body;
            _environment = environment;
        }

        public void Reset()
        {
            ResetAirborne();
        }

        public float AirborneTime => _airborneTime;

        public LocomotionVector AirborneApex => _airborneApex;

        public float FallingDistance => _airborneApex.Vertical - _body.Position.Vertical;

        public float LastFallHeight => _lastFallDistance;

        public void ResetAirborne()
        {
            _airborneTime = 0;
            _lastFallDistance = 0;
            _airborneApex = _body.Position;
        }
        
        void ILocomotionUpdateable.Update(float dt)
        {
            if (_environment.Airborne)
            {
                if(_airborneTime == 0 || _airborneApex.Vertical < _body.Position.Vertical)
                    _airborneApex = _body.Position;
                _lastFallDistance = _airborneApex.Vertical - _body.Position.Vertical; 
                _airborneTime += dt;
            }
            else
            {
                _airborneApex = _body.Position;
                _airborneTime = 0;
            }
        }
    }
}
