using System;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Interfaces;
using ColonyShared.SharedCode.Aspects.Locomotion;
using JetBrains.Annotations;
using SharedCode.Utils;
using Src.Locomotion.Delegates;
using Mathf = Assets.ColonyShared.SharedCode.Utils.SharedHelpers;

namespace Src.Locomotion
{
    public class LocomotionEnvironment : ILocomotionEnvironment, ILocomotionUpdateable, IResettable
    {
        private readonly ILocomotionGroundSensor _groundSensor;
        private readonly ILocomotionCollider _collider;
        private readonly ISettings _settings;
        private readonly Func<bool> _isLocomotionReady;
        private float _slopeFactor;
        private float _airborneTime;

        public LocomotionEnvironment(
            [NotNull] ISettings settings,
            [NotNull] ILocomotionGroundSensor groundSensor,
            [NotNull] ILocomotionCollider collider,
            [NotNull] Func<bool> isLocomotionReady)
        {
            _settings          = settings          ?? throw new ArgumentNullException(nameof(settings));
            _groundSensor      = groundSensor      ?? throw new ArgumentNullException(nameof(groundSensor));
            _isLocomotionReady = isLocomotionReady ?? throw new ArgumentNullException(nameof(isLocomotionReady));
            _collider          = collider          ?? throw new ArgumentNullException(nameof(collider));
            
            SlopeFactorAlongDirection = (dir) => LocomotionHelpers.SurfaceTangent(_groundSensor.GroundNormal, dir).Vertical;
        }

        public void Reset()
        {
            //#Note: when (& if) mobs can jump & fall (taking damage) this 'll be bad solution & we probably need whether to pass update this node no matter is serverloco works or not or pass this data from prev.mob serverLoco owner
            _airborneTime = 0f;
        }

        public bool Airborne => _airborneTime > _settings.MinAirborneTime;

        public float DistanceToGround => _groundSensor.DistanceToGround;

        public Vector2 SlopeDirection => _groundSensor.GroundNormal.Horizontal.normalized;

        public float SlopeFactor() => _slopeFactor;

        public SlopeByDirFn SlopeFactorAlongDirection { get; }

        public bool HasGroundContact => _groundSensor.HasGroundContact;

        public float Gravity => _settings.Gravity;

        public bool Valid => _isLocomotionReady();

        public float ColliderOffset => _collider.OriginOffset;

        public IReadOnlyList<ContactPoint> Contacts => _collider.Contacts;

        void ILocomotionUpdateable.Update(float dt)
        {
            _airborneTime = _groundSensor.OnGround ? 0 : _airborneTime + dt;
            _slopeFactor = Mathf.Sqrt(1 - _groundSensor.GroundNormal.Vertical.Sqr());
        }
        
        public interface ISettings
        {
            float MinAirborneTime { get; }
            float Gravity { get; }
        }
    }
}