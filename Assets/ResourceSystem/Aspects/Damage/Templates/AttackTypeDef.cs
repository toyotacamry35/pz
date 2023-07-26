using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;

namespace Assets.ColonyShared.SharedCode.Aspects.Damage.Templates
{
    public class AttackTypeDef : BaseResource, IResourceWithSoundSwitch
    {
        public string SoundSwitch { get; [UsedImplicitly] set; }
    }
}