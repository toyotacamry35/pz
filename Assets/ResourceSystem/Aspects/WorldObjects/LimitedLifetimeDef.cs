using Assets.Src.ResourcesSystem.Base;

namespace Assets.ColonyShared.SharedCode.Aspects.WorldObjects
{
    public class LimitedLifetimeDef : BaseResource
    {
        public ulong DefaultLifetime { get; set; }
        public bool DestroyIfBecomeEmpty { get; set; }
    }
}