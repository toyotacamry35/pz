using System.Collections.Generic;
using System.Linq;

namespace Src.Locomotion.Unity
{
    public class LocomotionNullCollider : ILocomotionCollider 
    {
        private readonly List<ContactPoint> _contacts =  new List<ContactPoint>();
        
        public LocomotionVector OriginPoint => LocomotionVector.Zero;
        public float OriginOffset => 0;
        public float Radius => 0;
        public int GroundLayerMask => 0;
        public List<ContactPoint> Contacts => _contacts;
        public bool IsSame(object collider) => false;
    }
}