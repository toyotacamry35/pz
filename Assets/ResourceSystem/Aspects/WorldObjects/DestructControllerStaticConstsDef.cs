using Assets.Src.ResourcesSystem.Base;

namespace Assets.ColonyShared.SharedCode.Aspects.WorldObjects
{
    public class DestructControllerStaticConstsDef : BaseResource
    {
        public float ExplosionForceFactor { get; set; }
        public float ExplosionR           { get; set; }
        public float TorqueMagnitude      { get; set; }
        public float HitSphereCastR       { get; set; }
        public float HitSphereCastDist    { get; set; }
        public float SmallGap             { get; set; }
    }
}
