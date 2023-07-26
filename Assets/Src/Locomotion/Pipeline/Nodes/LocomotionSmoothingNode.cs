using System;
using Assets.ColonyShared.SharedCode.Utils;
using ColonyShared.SharedCode.Aspects.Locomotion;
using SharedCode.Utils;
using static Src.Locomotion.DebugTag;
using static Src.Locomotion.LocomotionDebug;

namespace Src.Locomotion
{
    public class LocomotionSmoothingNode : ILocomotionPipelinePassNode
    {
        private readonly ISettings _settings;
        private bool _firsttime = true;
        private readonly ILocomotionBody _body;
        private LocomotionVector _correctionLinearVelocity;
        private float _correctionAngularSpeed;

        public LocomotionSmoothingNode(ISettings settings, ILocomotionBody body)
        {
            _settings = settings;
            _body = body ?? throw new ArgumentNullException(nameof(body));
        }

        public bool IsReady => true;
        
        public LocomotionVariables Pass(LocomotionVariables vars, float dt)
        {
            if (_firsttime || vars.Flags.Any(LocomotionFlags.Teleport))
            {
                _firsttime = false;
                _correctionLinearVelocity = LocomotionVector.Zero;
                _correctionAngularSpeed = 0;
            }
            else
            {
                var bodyPosition = _body.Position + vars.Velocity * dt;
                var positionDelta = bodyPosition - vars.Position;
                positionDelta = LocomotionVector.SmoothDamp(positionDelta, LocomotionVector.Zero, ref _correctionLinearVelocity, _settings.SmoothFactor, _settings.MaxSpeed, dt);
                vars.Position = vars.Position + positionDelta;
                vars.Velocity += _correctionLinearVelocity;
                DebugAgent.Set(DamperPositionDelta, positionDelta);
                DebugAgent.Set(DamperLinearVelocity, _correctionLinearVelocity);

                var bodyOrientation = _body.Orientation;
                var orientationDelta = SharedHelpers.DeltaAngleRad(vars.Orientation, bodyOrientation);
                orientationDelta = SharedHelpers.SmoothDampAngleRad(orientationDelta, 0, ref _correctionAngularSpeed, _settings.SmoothFactor, float.PositiveInfinity, dt);
                vars.Orientation = vars.Orientation + orientationDelta;
                DebugAgent.Set(DamperOrientationDelta, orientationDelta);
                DebugAgent.Set(DamperAngularSpeed, _correctionAngularSpeed);
            }

            return vars;
        }
        
         public interface ISettings
        {
            /// <summary>
            /// Приблизительное время требующееся для достижения цели. Наименьшее значение достигнет цели быстрее. (не приводит к "черепахе Ахиллеса")
            /// </summary>
            float SmoothFactor { get; }
            
            float MaxSpeed { get; }
        }
    }
}