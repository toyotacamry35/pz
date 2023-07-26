using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Assets.ColonyShared.SharedCode.Aspects.Damage.Templates
{
    public abstract class AttackActionDef : BaseResource
    {
        [UsedImplicitly][JsonProperty(Required = Required.Always)] public ApplyWhen When { get; set; }
        
        public enum ApplyWhen { Always, Damage, Stagger, Recoil, Block, BlockAndStagger, BlockAndNoStagger }
        
        public abstract BaseResource IdResource { get; } 
    }
}