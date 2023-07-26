using Assets.Src.ResourcesSystem.Base;

namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    public class RaycastGroundSensorDef : BaseResource
    {
        public float RaycastDistance { get; set; }
        public float RaycastDistanceLong { get; set; }
        public float RaycastOffset { get; set; }
        public float RaycastGroundTolerance { get; set; }
        
        /// При определении нормали к поверхности земли, собираются все нормали попавшие в SphereCast, начиная от ближайшей точки земли
        /// до точки отстоящей от неё вниз на расстоянии NormalSmoothingDistance, и усредняются с весом обратно пропорциональным расстоянию от ближайшей точки.     
        public float NormalSmoothingDistance { get; set; }
    }
}