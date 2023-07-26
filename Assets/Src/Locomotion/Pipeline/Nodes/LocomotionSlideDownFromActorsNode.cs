using ColonyShared.SharedCode.Aspects.Locomotion;

namespace Src.Locomotion
{
    public class LocomotionSlideDownFromActorsNode : ILocomotionPipelinePassNode
    {
        private readonly ILocomotionCollider _collider;
        private readonly ISettings _settings;

        public LocomotionSlideDownFromActorsNode(ISettings settings, ILocomotionCollider collider)
        {
            _collider = collider;
            _settings = settings;
        }

        public bool IsReady => true;
        
        public LocomotionVariables Pass(LocomotionVariables vars, float dt)
        {
            if (vars.Flags.Any(LocomotionFlags.NoCollideWithActors))
                return vars;
            
            foreach (var cnt in _collider.Contacts)
            {
                if (cnt.Location == ContactPointLocation.Bottom && cnt.ObjectType == ContactPointObjectType.Actor)
                {
                    var dir = (vars.Position - cnt.ObjectPosition).Horizontal.normalized;
                    vars.Velocity = new LocomotionVector(LocomotionPhysics.ApplyAccel(vars.Velocity.Horizontal, dir, _settings.Accel, _settings.Speed, dt), vars.Velocity.Vertical);
                }
            }
            return vars;
        }
        
        public interface ISettings
        {
            float Accel { get; }
            float Speed { get; }
        }
    }
}